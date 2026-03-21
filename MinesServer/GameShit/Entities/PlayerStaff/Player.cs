using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.EntityFrameworkCore;
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

        public CrystalCBStorage CrystalCB = new();
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
        public bool OnRoad => World.isRoad(x, y);
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

            MoveToChunk();

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
            var healthSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.Health);
            return 100 + (healthSkill != null ? (int)healthSkill.Effect : 0);
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

        public override bool Move(int x, int y, int dir = -1)
        {
            if (!World.ValidCoord(x, y) || (win != null && HasActiveProgram))
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
                return HandleObstacle();
            }

            UpdatePosition(x, y, dir);

            if (World.ContainsPack(x, y, out var pack) &&
                (pack.cid == cid || pack.cid == 0) &&
                !HasActiveProgram)
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

        private bool HandleObstacle()
        {
            if (autoDig)
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
                skillslist.HandleExperience(this, SkillType.Movement, 1);

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
            int x = targetX, y = targetY;

            if (!World.CanBuildAt(x, y, cid))
                return;

            // Убираем buildskills IEnumerable, работаем напрямую
            bool success = false;
            switch (type)
            {
                case "G": success = BuildBlock(x, y); break;
                case "V": success = BuildMilitary(x, y); break;
                case "R": success = BuildRoad(x, y); break;
                case "O": success = BuildSupport(x, y); break;
            }

            if (success)
            {
                skillslist.HandleBuildingExperience(this, type, 1);
            }
        }

        private bool BuildBlock(int x, int y)
        {
            var cell = World.GetCell(x, y);
            var cellprop = World.GetProp(cell);

            // Проверяем наличие навыков
            var greenSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.BuildGreen);
            var yellowSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.BuildYellow);
            var redSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.BuildRed);

            if (greenSkill != null && (World.TrueEmpty(x, y) || cellprop.isSand))
            {
                if (crys.RemoveCrys(CrystalType.Green, (long)greenSkill.Cost))
                {
                    World.SetCell(x, y, CellType.GreenBlock);
                    World.SetDurability(x, y, (int)greenSkill.DurabilityEffect);
                    return true;
                }
            }
            else if (yellowSkill != null && cell == (byte)CellType.GreenBlock)
            {
                if (crys.RemoveCrys(CrystalType.Violet, (long)yellowSkill.Cost))
                {
                    World.SetCell(x, y, CellType.YellowBlock);
                    World.SetDurability(x, y, World.GetDurability(x, y) + (int)yellowSkill.DurabilityEffect);
                    return true;
                }
            }
            else if (redSkill != null && cell == (byte)CellType.YellowBlock)
            {
                if (crys.RemoveCrys(CrystalType.Red, (long)redSkill.Cost))
                {
                    World.SetCell(x, y, CellType.RedBlock);
                    World.SetDurability(x, y, World.GetDurability(x, y) + (int)redSkill.DurabilityEffect);
                    return true;
                }
            }

            return false;
        }

        private bool BuildMilitary(int x, int y)
        {
            long cost = 0;
            var warSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.BuildWar);

            if (warSkill != null)
            {
                cost = (long)warSkill.Cost;
                if (crys.RemoveCrys(CrystalType.Cyan, cost) && World.TrueEmpty(x, y))
                {
                    World.SetCell(x, y, CellType.MilitaryBlockFrame);
                    var finalDurability = (int)warSkill.DurabilityEffect;
                    _ = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ =>
                    {
                        if (World.GetCell(x, y) == (byte)CellType.MilitaryBlockFrame)
                        {
                            World.SetCell(x, y, CellType.MilitaryBlock);
                            World.SetDurability(x, y, finalDurability);
                        }
                    });
                    return true;
                }
            }
            return false;
        }

        private bool BuildRoad(int x, int y)
        {
            long cost = 0;
            var roadSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.BuildRoad);

            if (roadSkill != null)
            {
                cost = (long)roadSkill.Cost;
                if (crys.RemoveCrys(0, cost) && World.TrueEmpty(x, y))
                {
                    World.SetCell(x, y, CellType.Road);
                    return true;
                }
            }
            return false;
        }

        private bool BuildSupport(int x, int y)
        {
            var structureSkill = skillslist.skills.Values.FirstOrDefault(s => s?.type == SkillType.BuildStructure);
            var cellprop = World.GetProp(World.GetCell(x, y));

            if (structureSkill != null)
            {
                if (crys.RemoveCrys(CrystalType.Green, (long)structureSkill.Cost) && (World.TrueEmpty(x, y) || cellprop.isSand))
                {
                    World.SetCell(x, y, CellType.Support);
                    return true;
                }
            }
            return false;
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
            // TODO: Пофиксить. Проблема может возникать из-за серрилизации из бд
            if (CrystalCB == null)
            {
                CrystalCB = new CrystalCBStorage();
                Console.WriteLine("CrystalCB был null в Bz(), создан новый");
            }
            ResourceExtractionService.PerformDig(this, this, ref cb, ref CrystalCB, crys);
        }

        public void BBox(long[]? c)
        {
            var (x, y) = GetDirCord();

            if (!World.ValidCoord(x, y) || c == null)
                return;

            Box.BuildBox(x, y, c, this);
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

        public override bool Heal()
        {
            return ResourceExtractionService.PerformRepair(this, this);
        }

        public override void Hurt(int damage, DamageTypePlayer type = DamageTypePlayer.Pure)
        {
            // Обработка опыта
            skillslist.HandleDamageExperience(this, type, 1);

            // Получение модифицированного урона
            int modifiedDamage = skillslist.HandleDamageReceived(damage);

            if (Health - modifiedDamage > 0)
            {
                Health -= modifiedDamage;
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
                var (boxX, boxY) = World.FindEmptyForBox(x, y);
                Box.BuildBox(boxX, boxY, crys.cry, this, true);
            }

            win = null;
            SendWindow();

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
            if (connection == null) return;

            World.W.SendBotsInfo(id, x, y, dir, skin, cid, tail);
        }

        public void SendMyMove()
        {
            BotsRender();
        }

        public void CheckChunkChanged(bool force = false)
        {
            // TODO: 
            // Выпилить ссылку на GetChunkPosByCoords, чтобы в итоге сделать ее приватной
            // Так как внешне она больше нигде не вызывается
            // А может и не надо делать приватной
            var ChunkPos = Chunk.GetChunkPosByCoords(x, y);
            if (!World.ValidChunk(ChunkPos.x, ChunkPos.y))
                return;

            if (lastchunk != (ChunkPos.x, ChunkPos.y) || force)
                MoveToChunk();
        }

        private void MoveToChunk()
        {
            UpdateChunkRegistration();
            UpdateVisibility();
        }

        private void UpdateChunkRegistration()
        {
            // Удаляем игрока из старого чанка
            if (lastchunk != null)
            {
                var oldChunk = World.W.GetPosChunk(lastchunk.Value.X, lastchunk.Value.Y);
                oldChunk.bots.Remove(id, out var p);
            }

            // Добавляем игрока в новый чанк
            var newChunk = World.W.GetChunk(x, y);
            lastchunk = newChunk.pos;

            // AddBot уже проверяет на ContainsKey
            newChunk.AddBot(this);
        }

        private void UpdateVisibility()
        {
            var currentChunks = World.W.GetVisibleChunksPos(x, y).ToList();
            var chunksToAdd = GetNewChunks(currentChunks);
            var chunksToRemove = GetObsoleteChunks(currentChunks);

            SendPackets(World.W.GetChunksPacketsAdded(chunksToAdd));
            SendPackets(World.W.GetChunksPacketsRemoved(chunksToRemove));

            UpdateTrackedChunks(chunksToAdd, chunksToRemove);
        }

        private List<(int x, int y)> GetNewChunks(List<(int x, int y)> currentChunks)
        {
            return currentChunks.Where(chunk => !alreadyvisible.Contains(chunk)).ToList();
        }

        private List<(int x, int y)> GetObsoleteChunks(List<(int x, int y)> currentChunks)
        {
            return alreadyvisible.Where(chunk => !currentChunks.Contains(chunk)).ToList();
        }
        private void SendPackets(IEnumerable<IHubPacket> packets)
        {
            var packetArray = packets.ToArray();
            if (packetArray.Any())
                connection?.SendB(new HBPacket(packetArray));
        }

        private void UpdateTrackedChunks(
            List<(int x, int y)> chunksToAdd,
            List<(int x, int y)> chunksToRemove)
        {
            foreach (var chunk in chunksToAdd)
                alreadyvisible.Add(chunk);

            foreach (var chunk in chunksToRemove)
                alreadyvisible.Remove(chunk);
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
    }
}