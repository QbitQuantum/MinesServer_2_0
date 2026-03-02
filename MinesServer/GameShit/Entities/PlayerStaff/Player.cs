using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using MinesServer.Enums;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GChat;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network;
using MinesServer.Network.BotInfo;
using MinesServer.Network.Chat;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.HubEvents.FX;
using MinesServer.Network.HubEvents.Packs;
using MinesServer.Network.Movement;
using MinesServer.Network.Programmator;
using MinesServer.Network.World;
using MinesServer.Server;
using MinesServer.Server.Network;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Player : PEntity
    {
        private const int BaseMoveDelay = 10000;
        private const int SyncIntervalSeconds = 10;
        private const int AfkTimeoutMinutes = 5;
        private const int BotUpdateIntervalSeconds = 4;
        private const int C190StackResetMinutes = 1;

        private (int X, int Y)? lastchunk;
        private DateTime lBotsUpdate = DateTime.UtcNow;
        private DateTime lastSync = DateTime.UtcNow;
        private float cb;
        private Resp? _resp;
        private readonly List<(int X, int Y)> alreadyvisible = new();

        public Player() => Delay = DateTime.UtcNow;

        [NotMapped] public Session? connection { get; set; }
        [NotMapped] public Chat? currentchat { get; set; }
        [NotMapped] public Window? win { get; set; }
        [NotMapped] public bool online => connection != null;
        [NotMapped] public override int cid => clan?.id ?? 0;
        [NotMapped] public bool HasActiveProgram => programsData.ProgRunning;
        [NotMapped] public int tail => HasActiveProgram ? 1 : 0;

        public string name { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string passwd { get; set; } = string.Empty;
        public int skin { get; set; }
        public long money { get; set; }
        public long creds { get; set; }
        public long opp { get; set; }
        public bool autoDig { get; set; }
        public bool agression { get; set; }
        public int c190stacks = 1;
        public DateTime lastc190hit = ServerTime.Now;
        public DateTime Delay = ServerTime.Now;
        public DateTime afkstarttime = ServerTime.Now;
        public Clan? clan { get; set; }
        public Rank? clanrank { get; set; }
        public List<Program> programs { get; set; } = new();
        public override Basket crys { get; set; } = null!;
        public Inventory inventory { get; set; } = null!;
        public Settings settings { get; set; } = null!;
        public PlayerSkills skillslist { get; set; } = null!;
        public Queue<Line> console = new Queue<Line>();
        public bool OnRoad => World.isRoad(World.GetCell(x, y));
        public override double ServerPause => (OnRoad ? (pause * 5) * 0.80 : pause * 5) * 1.4 / 1000;

        public override int pause
        {
            get
            {
                var moveSkill = skillslist.skills.Values.FirstOrDefault(s =>
                    s != null &&
                    s.UseSkill(SkillEffectType.OnMove, this) == true &&
                    s.type == SkillType.Movement);
                return moveSkill != null ? (int)(moveSkill.Effect * 100) : BaseMoveDelay;
            }
        }

        public Resp? resp
        {
            get
            {
                if (_resp == null)
                {
                    using var db = new DataBase();
                    db.Attach(this);
                    var respawns = db.resps.Where(r => r.ownerid == 0).ToList();
                    var randomRespawn = respawns[Random.Shared.Next(respawns.Count)];
                    _resp = randomRespawn;
                    db.SaveChanges();
                }
                return _resp;
            }
            set
            {
                using var db = new DataBase();
                db.Attach(this);
                _resp = value;
                db.SaveChanges();
            }
        }

        #region Lifecycle

        public void Init()
        {
            if (connection != null)
                connection.auth = null;

            if (!DataBase.activeplayers.Contains(this))
                DataBase.activeplayers.Add(this);

            skillslist.LoadSkills();
            MaxHealth = CalculateMaxHealth();
            Health = Health <= 0 ? MaxHealth : Health;

            MoveToChunk(ChunkX, ChunkY);

            SendInitialData();
            SubscribeToEvents();

            win = null;
        }
        public void CreatePlayer()
        {
            name = "";
            money = 1000;
            creds = 0;
            hash = GenerateHash();
            passwd = "";
            Health = 100;
            MaxHealth = 100;
            inventory = new Inventory();
            settings = new Settings(true);
            crys = new Basket(true);
            skillslist = new PlayerSkills(true); // Инициализация без передачи this
            x = 0; y = 0;
            dir = 0;
            clan = null;
            skin = 0;
        }
        public string GenerateHash()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private int CalculateMaxHealth()
        {
            var healthSkill = skillslist.skills.Values.FirstOrDefault(s =>
                s != null &&
                s.UseSkill(SkillEffectType.OnHealth, this) == true &&
                s.type == SkillType.Health);

            return healthSkill != null ? (int)healthSkill.Effect : 100;
        }

        private void SendInitialData()
        {
            this.SendAutoDigg();
            this.SendGeo();
            this.SendHealth();
            this.SendBotInfo();
            this.SendSpeed();
            this.SendMoney();
            this.SendLvl();
            this.SendInventory();
            this.SendClan();
            SendChat();

            settings.SendSettings(this);
            connection?.SendU(new ConfigPacket("oldprogramformat+"));

            if (programsData.selected != null)
                this.UpdateProg(programsData.selected);

            this.ProgStatus();
        }

        private void SubscribeToEvents()
        {
            if (crys.shouldsubscribe)
                crys.Changed += this.SendCrys;
            this.SendCrys();
        }

        public override void Update()
        {
            var now = DateTime.UtcNow;

            SyncIfNeeded(now);
            UpdateC190Stacks(now);

            if (!online)
            {
                HandleOffline(now);
                return;
            }

            UpdateBots(now);
            HandleCurrentCell();
        }

        private void SyncIfNeeded(DateTime now)
        {
            if (now - lastSync > TimeSpan.FromSeconds(SyncIntervalSeconds))
            {
                using var db = new DataBase();
                db.Entry(this).State = EntityState.Modified;
                db.Entry(this).Collection(p => p.programs).IsModified = false;
                db.SaveChanges();
                lastSync = now;
            }
        }

        private void UpdateC190Stacks(DateTime now)
        {
            if (now - lastc190hit > TimeSpan.FromMinutes(C190StackResetMinutes))
            {
                c190stacks = 1;
                lastc190hit = now;
            }
        }

        private void HandleOffline(DateTime now)
        {
            if (now - afkstarttime > TimeSpan.FromMinutes(AfkTimeoutMinutes))
            {
                DataBase.activeplayers.Remove(this);
                Death();
            }
        }

        private void UpdateBots(DateTime now)
        {
            if (now - lBotsUpdate > TimeSpan.FromSeconds(BotUpdateIntervalSeconds))
            {
                BotsRender();
                lBotsUpdate = now;
            }
        }

        private void HandleCurrentCell()
        {
            var cell = World.GetCell(x, y);
            var cellprop = World.GetProp(cell);

            if (cellprop.isEmpty)
                return;

            Hurt(cellprop.fall_damage);

            if (cell == 90)
            {
                GetBox(x, y);
                World.DamageCell(x, y, 1);
            }
            else if (cellprop.is_destructible)
            {
                World.Destroy(x, y);
            }
        }

        public void dOnDisconnect()
        {
            using var db = new DataBase();
            db.players.Update(this);
            db.SaveChanges();

            afkstarttime = DateTime.UtcNow;
            connection = null;
            alreadyvisible.Clear();
        }

        #endregion

        #region Movement

        public override bool Move(int x, int y, int dir = -1, bool prog = false)
        {
            if (!World.W.ValidCoord(x, y) || (win != null && !prog))
            {
                tp(this.x, this.y);
                return false;
            }

            UpdateDirection(x, y, ref dir);

            if (IsGateBlocking(x, y))
            {
                tp(this.x, this.y);
                return false;
            }

            var cell = World.GetCell(x, y);
            if (!World.GetProp(cell).isEmpty)
            {
                return HandleObstacle(dir);
            }

            UpdatePosition(x, y, dir);
            CheckChunkChanged();

            if (World.ContainsPack(x, y, out var pack) &&
                (pack.cid == cid || pack.cid == 0) &&
                !prog)
            {
                win = pack.GUIWin(this);
                SendWindow();
            }

            return false;
        }

        private void UpdateDirection(int x, int y, ref int dir)
        {
            if (dir > 9)
                dir -= 10;

            if (dir == -1 || this.x != x || this.y != y)
            {
                dir = this.x > x ? 1 : this.x < x ? 3 : this.y > y ? 2 : 0;
            }

            this.dir = dir;
        }

        private bool IsGateBlocking(int x, int y)
        {
            return World.ContainsPack(x, y, out var pack) &&
                   pack is Gate &&
                   pack.cid != cid;
        }

        private bool HandleObstacle(int dir)
        {
            if (dir == -1 && autoDig)
            {
                Bz();
                return true;
            }

            tp(this.x, this.y);
            return false;
        }

        private void UpdatePosition(int x, int y, int dir)
        {
            if (Vector2.Distance(new Vector2(this.x, this.y), new Vector2(x, y)) < 1.2f)
            {
                var moveSkill = skillslist.skills.Values.FirstOrDefault(s =>
                    s != null &&
                    s.UseSkill(SkillEffectType.OnMove, this) == true &&
                    s.type == SkillType.Movement);
                moveSkill?.AddExp(this);
            }

            this.x = x;
            this.y = y;
            SendMyMove();
            CheckChunkChanged();
        }

        #endregion

        #region Building

        public override void Build(string type)
        {
            var (targetX, targetY) = GetDirCord();
            int x = (int)targetX, y = (int)targetY;

            if (!CanBuildAt(x, y))
                return;

            var buildskills = skillslist.skills.Values.Where(s => s?.EffectType() == SkillEffectType.OnBld);

            switch (type)
            {
                case "G": BuildBlock(x, y, buildskills); break;
                case "V": BuildMilitary(x, y, buildskills); break;
                case "R": BuildRoad(x, y, buildskills); break;
                case "O": BuildSupport(x, y, buildskills); break;
            }
        }

        private bool CanBuildAt(int x, int y)
        {
            return World.W.ValidCoord(x, y) &&
                   World.AccessGun(x, y, cid).access &&
                   !World.PackPart(x, y);
        }

        private void BuildBlock(int x, int y, IEnumerable<Skill> buildskills)
        {
            var cell = World.GetCell(x, y);
            var cellprop = World.GetProp(cell);

            foreach (var c in buildskills)
            {
                if (c.type == SkillType.BuildGreen && (World.TrueEmpty(x, y) || cellprop.isSand))
                {
                    if (crys.RemoveCrys(0, (long)c.Effect))
                    {
                        c.AddExp(this);
                        World.SetCell(x, y, CellType.GreenBlock);
                        World.SetDurability(x, y, (int)c.AdditionalEffect);
                    }
                    return;
                }

                if (c.type == SkillType.BuildYellow && cell == (byte)CellType.GreenBlock)
                {
                    if (crys.RemoveCrys(4, (long)c.Effect))
                    {
                        c.AddExp(this);
                        World.SetCell(x, y, CellType.YellowBlock);
                        World.SetDurability(x, y, World.GetDurability(x, y) + (int)c.AdditionalEffect);
                    }
                    return;
                }

                if (c.type == SkillType.BuildRed && cell == (byte)CellType.YellowBlock)
                {
                    if (crys.RemoveCrys(2, (long)c.Effect))
                    {
                        c.AddExp(this);
                        World.SetCell(x, y, CellType.RedBlock);
                        World.SetDurability(x, y, World.GetDurability(x, y) + (int)c.AdditionalEffect);
                    }
                    return;
                }
            }
        }

        private void BuildMilitary(int x, int y, IEnumerable<Skill> buildskills)
        {
            foreach (var c in buildskills.Where(s => s.type == SkillType.BuildWar))
            {
                if (crys.RemoveCrys(5, (long)c.Effect) && World.TrueEmpty(x, y))
                {
                    c.AddExp(this);
                    World.SetCell(x, y, CellType.MilitaryBlockFrame);

                    var finalDurability = (int)c.AdditionalEffect;
                    _ = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ =>
                    {
                        if (World.GetCell(x, y) == (byte)CellType.MilitaryBlockFrame)
                        {
                            World.SetCell(x, y, CellType.MilitaryBlock);
                            World.SetDurability(x, y, finalDurability);
                        }
                    });
                }
                return;
            }
        }

        private void BuildRoad(int x, int y, IEnumerable<Skill> buildskills)
        {
            foreach (var c in buildskills.Where(s => s.type == SkillType.BuildRoad))
            {
                if (crys.RemoveCrys(0, (long)c.Effect) && World.TrueEmpty(x, y))
                {
                    c.AddExp(this);
                    World.SetCell(x, y, CellType.Road);
                }
                return;
            }
        }

        private void BuildSupport(int x, int y, IEnumerable<Skill> buildskills)
        {
            var cellprop = World.GetProp(World.GetCell(x, y));

            foreach (var c in buildskills.Where(s => s.type == SkillType.BuildStructure))
            {
                if (crys.RemoveCrys(0, (long)c.Effect) && (World.TrueEmpty(x, y) || cellprop.isSand))
                {
                    c.AddExp(this);
                    World.SetCell(x, y, CellType.Support);
                }
                return;
            }
        }

        #endregion

        #region Actions

        public void TryAct(Action a, double delay)
        {
            if (HasActiveProgram)
                return;

            if (Delay < DateTime.UtcNow)
            {
                a();
                Delay = DateTime.UtcNow + TimeSpan.FromMilliseconds(delay);
            }
        }

        public override void Bz()
        {
            ResourceExtractionService.PerformDig(this, this, skillslist.skills.Values, ref cb, crys);
        }

        public void BBox(long[]? c)
        {
            var (x, y) = GetDirCord();

            if (!World.W.ValidCoord((int)x, (int)y) || c == null)
                return;

            Box.BuildBox((int)x, (int)y, c, this);
            connection?.CloseWindow();
        }

        public void GetBox(int x, int y)
        {
            var result = base.GetBox(x, y);
            connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "+ " + result)]));
        }

        public void tp(int x, int y)
        {
            connection?.SendU(new TPPacket(x, y));
            SendMyMove();
        }

        public void SetResp(Resp r) => resp = r;

        #endregion

        #region Health

        public override bool Heal(int num = -1)
        {
            if (Health == MaxHealth)
                return false;

            var heal = skillslist.skills.Values.FirstOrDefault(s => s.type == SkillType.Repair);
            if (heal == default)
                return false;

            num = (int)heal.Effect;

            if (!crys.RemoveCrys(2, 1))
                return false;

            heal.AddExp(this);
            Health = Math.Min(Health + num, MaxHealth);

            SendDFToBots(5, 0, 0, id, 0);
            this.SendHealth();

            return true;
        }

        public override void Hurt(int num, DamageTypePlayer t = DamageTypePlayer.Pure)
        {
            foreach (var c in skillslist.skills.Values.Where(s => s != null))
            {
                if (c.UseSkill(SkillEffectType.OnHealth, this) && c.type == SkillType.Health)
                    c.AddExp(this);

                if (c.UseSkill(SkillEffectType.OnHurt, this) && t == DamageTypePlayer.Gun)
                {
                    if (c.type == SkillType.Induction)
                        c.AddExp(this);

                    if (c.type == SkillType.AntiGun)
                    {
                        c.AddExp(this);
                        num = (int)Math.Max(0, num - num * (c.Effect / 100));
                    }
                }
            }

            if (Health - num > 0)
            {
                Health -= num;
                SendDFToBots(6, 0, 0, id, 0);
            }
            else
            {
                Death();
            }

            this.SendHealth();
        }

        public override void Death()
        {
            if (crys.AllCry > 0)
            {
                var (boxX, boxY) = FindEmptyForBox(x, y);
                Box.BuildBox(boxX, boxY, crys.cry, this, true);
            }

            win = null;
            SendWindow();
            SendFXoBots(2, x, y);

            Health = MaxHealth;

            if (!online && !programsData.RespawnOnProg)
            {
                using var db = new DataBase();
                db.players.Attach(this);
                db.SaveChanges();
                DataBase.activeplayers.Remove(this);
                return;
            }

            resp?.OnRespawn(this);
            var (newX, newY) = resp.GetRandompoint();
            x = newX;
            y = newY;

            tp(x, y);
            BotsRender();
            CheckChunkChanged();
            this.SendHealth();

            if (!HasActiveProgram)
                return;

            if (programsData.RespawnOnProg)
            {
                programsData.OnDeath();
            }
            else
            {
                RunProgramm(null);
                connection?.SendU(new ProgrammatorPacket(false));
            }
        }

        private (int x, int y) FindEmptyForBox(int startX, int startY)
        {
            var dirs = new[] { (0, 1), (1, 0), (-1, 0), (0, -1) };
            var queue = new Queue<(int x, int y)>();
            var visited = new HashSet<(int, int)>();

            bool IsValid(int tx, int ty) =>
                World.W.ValidCoord(tx, ty) &&
                World.GetProp(tx, ty).isEmpty &&
                !World.PackPart(tx, ty);

            if (IsValid(startX, startY))
                return (startX, startY);

            queue.Enqueue((startX, startY));
            visited.Add((startX, startY));

            while (queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();

                foreach (var (dx, dy) in dirs)
                {
                    int nx = cx + dx, ny = cy + dy;

                    if (visited.Contains((nx, ny)))
                        continue;

                    if (IsValid(nx, ny))
                        return (nx, ny);

                    visited.Add((nx, ny));
                    queue.Enqueue((nx, ny));
                }
            }

            return (startX, startY);
        }

        #endregion

        #region Program Management

        public void RunProgramm(Program p = null)
        {
            win = null;
            SendWindow();

            if (p == null)
                programsData.Run();
            else
                programsData.Run(p);
        }

        public void ProgrammatorUpdate()
        {
            if (programsData.ProgRunning)
                programsData.Step();
        }

        #endregion

        #region Rendering

        public void BotsRender()
        {
            var packets = new List<IHubPacket>();

            foreach (var chunk in vChunksAround())
                packets.AddRange(GetBotsInChunk(chunk.x, chunk.y));

            connection?.SendB(new HBPacket(packets.ToArray()));
        }

        private IHubPacket[] GetBotsInChunk(int chunkX, int chunkY)
        {
            if (!World.W.ValidChunk(chunkX, chunkY))
                return Array.Empty<IHubPacket>();

            var packets = new List<IHubPacket>();
            var chunk = World.W.chunks[chunkX, chunkY];

            foreach (var (playerId, _) in chunk.bots)
            {
                var player = DataBase.GetPlayer(playerId);
                if (player != null)
                {
                    packets.Add(new HBBotPacket(
                        player.id,
                        player.x,
                        player.y,
                        player.dir,
                        player.skin,
                        player.cid,
                        player.tail));
                }
            }

            return packets.ToArray();
        }

        public void SendMyMove()
        {
            if (connection == null)
                return;

            foreach (var ch in vChunksAround())
            {
                var chunk = World.W.chunks[ch.x, ch.y];

                foreach (var (playerId, _) in chunk.bots)
                {
                    var player = DataBase.GetPlayer(playerId);
                    player?.connection?.SendB(new HBPacket([new HBBotPacket(id, x, y, dir, skin, cid, tail)]));
                }
            }
        }

        public void CheckChunkChanged(bool force = false)
        {
            if (!World.W.ValidChunk(ChunkX, ChunkY))
                return;

            if (lastchunk != (ChunkX, ChunkY) || force)
                MoveToChunk(ChunkX, ChunkY);
        }

        private void MoveToChunk(int x, int y)
        {
            StupidVisabilityUpdate();

            if (lastchunk != null)
            {
                var oldChunk = World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2];
                oldChunk.bots.Remove(id, out var p);
            }

            var newChunk = World.W.chunks[x, y];
            lastchunk = (x, y);

            if (!newChunk.bots.ContainsKey(id))
                newChunk.AddBot(this);
        }

        private void StupidVisabilityUpdate()
        {
            var packets = new List<IHubPacket>();
            var currentChunks = vChunksAround().ToList();
            var oldChunks = new List<(int x, int y)>(alreadyvisible);

            foreach (var chunk in currentChunks)
            {
                var tuple = (chunk.x, chunk.y);

                if (oldChunks.Contains(tuple))
                {
                    oldChunks.Remove(tuple);
                }
                else
                {
                    packets.AddRange(fChunkInfo(chunk.x, chunk.y));
                    alreadyvisible.Add(tuple);
                }
            }

            foreach (var abandoned in oldChunks)
            {
                alreadyvisible.Remove(abandoned);
                var chunk = World.W.chunks[abandoned.x, abandoned.y];
                foreach (var pack in chunk.packs.Values)
                {
                    packets.Add(new HBPacksPacket(
                        chunk.PACKPOS(pack.x, pack.y), []));
                }
            }
            if (packets.Any())
                connection?.SendB(new HBPacket(packets.ToArray()));
        }

        private IHubPacket[] ChunkInfo(int chunkx, int chunky)
        {
            var packets = new List<IHubPacket>();
            var chunk = World.W.chunks[chunkx, chunky];

            packets.Add(new HBMapPacket(chunk.WorldX, chunk.WorldY, World.ChunkWidth, World.ChunkHeight, chunk.cells));

            if (!alreadyvisible.Contains((chunkx, chunky)))
                packets.AddRange(chunk.pPakcs(this));

            return packets.ToArray();
        }

        private IHubPacket[] fChunkInfo(int chunkx, int chunky) =>
            ChunkInfo(chunkx, chunky).Concat(GetBotsInChunk(chunkx, chunky)).ToArray();

        private IEnumerable<(int x, int y)> vChunksAround()
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int chunkX = ChunkX + dx;
                    int chunkY = ChunkY + dy;

                    if (World.W.ValidChunk(chunkX, chunkY))
                        yield return (chunkX, chunkY);
                }
            }
        }
        #endregion

        #region UI

        public void SendWindow()
        {
            if (win != null)
            {
                connection?.SendU(new GUIPacket(win.ToString()));
            }
            else
            {
                connection?.SendU(new GuPacket());
            }
        }

        public void CallWinAction(string text)
        {
            if (win == null)
            {
                connection?.SendU(new GuPacket());
                return;
            }
            win.ProcessButton(text);
        }

        public void OpenClan()
        {
            if (clan == null)
                return;

            using var db = new DataBase();
            db.clans
                .Where(i => i.id == clan.id)
                .Include(p => p.members)
                .Include(p => p.reqs)
                .FirstOrDefault()
                ?.OpenClanWin(this);
        }

        public void OpenMyBuildings()
        {
            win = MyBuildings();
            SendWindow();
        }

        private Window MyBuildings()
        {
            return new Window()
            {
                Title = "мои здания да",
                Tabs = new[]
                {
                    new Tab()
                    {
                        Action = "amy",
                        Label = "amyl",
                        InitialPage = new Page()
                        {
                            List = MyBuildingsList(),
                            Buttons = new[] { new MButton("собратьб", "четатам") }
                        }
                    }
                }
            };
        }

        private ListEntry[] MyBuildingsList()
        {
            using var db = new DataBase();
            var entries = new List<ListEntry>();

            entries.AddRange(db.teleports
                .Where(t => t.ownerid == id)
                .Select(t => new ListEntry($"tp {t.x}:{t.y}", null)));

            entries.AddRange(db.resps
                .Where(r => r.ownerid == id)
                .Select(r => new ListEntry($"resp {r.x}:{r.y}", null)));

            entries.AddRange(db.ups
                .Where(u => u.ownerid == id)
                .Select(u => new ListEntry($"up {u.x}:{u.y}", null)));

            entries.AddRange(db.guns
                .Where(g => g.ownerid == id)
                .Select(g => new ListEntry($"gun {g.x}:{g.y}", null)));

            return entries.ToArray();
        }

        public void SendChat()
        {
            using var db = new DataBase();
            currentchat ??= db.chats.FirstOrDefault(i => i.tag == "FED");

            connection?.SendU(new CurrentChatPacket(currentchat.tag, currentchat.Name));

            var msg = currentchat.GetMessages();
            if (msg.Length > 0)
            {
                connection?.SendU(new ChatMessagesPacket("FED", currentchat.GetMessages()));
            }
        }

        #endregion

        #region Helpers

        private (float x, float y) GetDirCord()
        {
            return base.GetDirCord();
        }
        #endregion
    }
}