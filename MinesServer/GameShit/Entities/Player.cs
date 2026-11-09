using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GChat;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.List;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Sys_Clan;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.Movement;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Entities
{
    public class Player : PEntity
    {
        public Player()
        {

        }

        private const int BaseMoveDelay = 10000;
        private const int AfkTimeoutMinutes = 5;
        private const int BotUpdateIntervalSeconds = 4;
        private const int C190StackResetMinutes = 1;
        private const int InventoryUseCooldownMs = 400;
        private const int MinimumServerPause = 20;
        private const int PauseBotDivisor = 1000;
        private const int PauseBotMultiplier = 7;

        private (int X, int Y)? lastchunk;

        private DateTime _lastBotsUpdate = ServerTime.Now;
        private DateTime _lastc190hit = ServerTime.Now;
        private DateTime _lastDelay = ServerTime.Now;
        private DateTime _laststarttime = ServerTime.Now;
        private DateTime _lastuseinventory = ServerTime.Now;

        private readonly HashSet<(int X, int Y)> _alreadyVisible = [];

        private int _c190stacks = 1;

        [NotMapped] public Session? connection { get; set; }
        [NotMapped] public Chat? currentchat { get; set; }
        [NotMapped] public Window? win { get; set; }
        [NotMapped] public bool online => connection != null;
        [NotMapped] public override int cid => clan?.id ?? 0;
        [NotMapped] public override int tail => HasActiveProgram ? 1 : 0;
        [NotMapped] public bool HasActiveProgram => programsData.ProgRunning;
        [NotMapped] public bool dailyRewardClaimed { get; set; }

        public string name { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string passwd { get; set; } = string.Empty;
        public long money { get; set; }
        public long creds { get; set; }
        public long opp { get; set; }
        public Clan? clan { get; set; }
        public Rank? clanrank { get; set; }
        public Resp? resp { get; set; }

        public bool autoRem { get; set; }
        public bool isAdmin { get; set; }
        public List<Program> programs { get; set; } = new();
        public override Basket crys { get; set; } = new();
        public Inventory inventory { get; set; } = new();
        public Settings settings { get; set; } = new();
        public PlayerSkills skillslist { get; set; } = new();
        public Queue<Line> console = new Queue<Line>();

        public override double ServerPause
        {
            get
            {
                var moveSkill = skillslist.GetSkill(SkillType.Movement);
                long pauseBot = moveSkill != null ? (int)(moveSkill.Effect * 100) : BaseMoveDelay;
                long pauseServer = pauseBot * PauseBotMultiplier / PauseBotDivisor;
                return Math.Max(pauseServer, MinimumServerPause);
            }
        }

        public string geology
        {
            get
            {
                if (geo == null || geo.Count == 0)
                    return "[]";

                return Newtonsoft.Json.JsonConvert.SerializeObject(geo.ToList());
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value == "[]")
                {
                    geo = new Stack<byte>();
                    return;
                }

                try
                {
                    var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<byte>>(value);
                    geo = new Stack<byte>(list ?? new List<byte>());
                }
                catch
                {
                    geo = new Stack<byte>();
                }
            }
        }

        #region Lifecycle

        public void Init()
        {
            skillslist.Init();
            settings.Init();
        }

        public void UpdateState()
        {
            if (!DataBase.activeplayers.Contains(this))
                DataBase.activeplayers.Add(this);
            win = null;
            skillslist.LoadStandartSkills();
            settings.LoadSettings();
            MaxHealth = CalculateMaxHealth();
            Health = MaxHealth;
            MoveToChunk();
            SendInitialData();
            SubscribeToEvents();
        }

        public void CreatePlayer()
        {
            money = 1000;
            creds = 0;
            Health = 100;
            MaxHealth = 100;
            x = 0;
            y = 0;
            dir = 0;
            skin = 0;
        }

        private int CalculateMaxHealth()
        {
            var healthSkill = skillslist.GetSkill(SkillType.Health);
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
            this.SendChat();
            this.SendSettings();
            this.SendConfig();
            this.UpdateProg(programsData.Selected);
            this.ProgStatus();
            this.SendCrys();
            this.SendWindow();
            this.SendNickList();
            this.SendOnlineCount();
            this.SendAgrStatus();
            this.SendAutoRemStatus();
            this.SendAdminStatus();
            this.SendDailyReward();
            this.SendMissionStatus();
        }

        private void SubscribeToEvents()
        {
            if (crys.ShouldSubscribe)
                crys.Changed += this.SendCrys;
        }

        public override void Update()
        {
            var now = ServerTime.Now;

            if (HadleProgramm())
            {
                UpdateBots(now);
                return;
            }

            UpdateC190Stacks(now);

            if (!online)
            {
                HandleOffline(now);
                return;
            }

            UpdateBots(now);
            HandleCurrentCell();
        }

        private void UpdateC190Stacks(DateTime now)
        {
            if (now - _lastc190hit < TimeSpan.FromMinutes(C190StackResetMinutes))
                return;

            _c190stacks = 1;
            _lastc190hit = now;
        }

        public void HurtC190Stacks()
        {
            Hurt(20 + 60 * _c190stacks);
            _c190stacks++;
            _lastc190hit = ServerTime.Now;
        }

        public bool HandleUseInventory()
        {
            var now = ServerTime.Now;
            bool canUse = now - _lastuseinventory >= TimeSpan.FromMilliseconds(InventoryUseCooldownMs)
                          || programsData.ProgRunning;
            if (canUse)
                _lastuseinventory = now;
            return canUse;
        }

        private void HandleOffline(DateTime now)
        {
            if (now - _laststarttime >= TimeSpan.FromMinutes(AfkTimeoutMinutes))
            {
                Death();
                DataBase.activeplayers.Remove(this);
            }
        }

        private void UpdateBots(DateTime now)
        {
            if (now - _lastBotsUpdate < TimeSpan.FromSeconds(BotUpdateIntervalSeconds))
                return;

            BotsRender();
            _lastBotsUpdate = now;
        }

        private void HandleCurrentCell()
        {
            var cell = World.GetCell(x, y);
            var cellProp = World.GetProp(cell);

            if (cellProp.isEmpty)
                return;

            Hurt(cellProp.fall_damage);

            if (cell == 90)
            {
                GetBox(x, y);
                World.DamageCell(x, y, 1, Operator.Unknown);
            }
            else if (cellProp.is_destructible)
            {
                World.Destroy(x, y);
            }
        }

        public void dOnDisconnect()
        {
            DateTime disconnectTime = ServerTime.Now;

            using var db = new DataBase();
            var player = db.players.FirstOrDefault(p => p.id == id);
            if (player is not null)
            {
                player._laststarttime = disconnectTime;
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    // Log if needed, but don't block disconnect
                }
            }

            _laststarttime = disconnectTime;
            connection = null;
            _alreadyVisible.Clear();
        }

        #endregion

        #region Movement

        public override bool Move(int x, int y, DirectionType direction = DirectionType.Unknown)
        {
            if (!World.ValidCoord(x, y) || win != null)
            {
                tp(this.x, this.y);
                return false;
            }

            UpdateDirection(x, y, direction);

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

            UpdatePosition(x, y);

            var factory = World.GetGuiFactory(x, y);
            if (factory != null)
            {
                var window = factory(this);
                if (window != null)
                {
                    win = window;
                    this.SendWindow();
                }
            }

            return false;
        }

        private bool IsGateBlocking(int x, int y)
        {
            return World.ContainsPack(x, y, out var pack)
                   && pack is Gate gate
                   && gate.cid != cid;
        }

        private bool HandleObstacle()
        {
            if (autoDig)
            {
                Bz();
                return true;
            }

            tp(x, y);
            return false;
        }

        private void UpdatePosition(int x, int y)
        {
            int dx = this.x - x;
            int dy = this.y - y;
            int distSq = dx * dx + dy * dy;

            if (distSq >= 1)
            {
                skillslist.HandleExperience(this, SkillType.Movement, 1);
                if (World.isRoad(x, y))
                    skillslist.HandleExperience(this, SkillType.RoadMovement, 1);
            }

            this.x = x;
            this.y = y;
            tp(x, y);
        }

        #endregion

        #region Building

        public override void Build(string type)
        {
            var (targetX, targetY) = GetDirCord();
            int x = targetX, y = targetY;

            if (!World.CanBuildAt(x, y, cid))
                return;

            bool success = type switch
            {
                "G" => BuildBlock(x, y),
                "V" => BuildMilitary(x, y),
                "R" => BuildRoad(x, y),
                "O" => BuildSupport(x, y),
                _ => false
            };

            if (success)
                skillslist.HandleBuildingExperience(this, type, 1);
        }

        private bool BuildBlock(int x, int y)
        {
            var cell = World.GetCell(x, y);
            var cellProp = World.GetProp(cell);

            var greenSkill = skillslist.GetSkill(SkillType.BuildGreen);
            var yellowSkill = skillslist.GetSkill(SkillType.BuildYellow);
            var redSkill = skillslist.GetSkill(SkillType.BuildRed);

            if (greenSkill != null && (World.TrueEmpty(x, y) || cellProp.isSand))
            {
                if (crys.RemoveCrys(CrystalType.Green, (long)greenSkill.Cost))
                {
                    World.SetCell(x, y, CellType.GreenBlock);
                    World.DamageCell(x, y, (int)greenSkill.DurabilityEffect, Operator.Unknown);
                    return true;
                }
            }
            else if (yellowSkill != null && cell == (byte)CellType.GreenBlock)
            {
                if (crys.RemoveCrys(CrystalType.Violet, (long)yellowSkill.Cost))
                {
                    World.SetCell(x, y, CellType.YellowBlock);
                    World.DamageCell(x, y, -(int)yellowSkill.DurabilityEffect, Operator.Minus);
                    return true;
                }
            }
            else if (redSkill != null && cell == (byte)CellType.YellowBlock)
            {
                if (crys.RemoveCrys(CrystalType.Red, (long)redSkill.Cost))
                {
                    World.SetCell(x, y, CellType.RedBlock);
                    World.DamageCell(x, y, -(int)redSkill.DurabilityEffect, Operator.Minus);
                    return true;
                }
            }

            return false;
        }

        private bool BuildMilitary(int x, int y)
        {
            var warSkill = skillslist.GetSkill(SkillType.BuildWar);
            if (warSkill == null)
                return false;

            long cost = (long)warSkill.Cost;
            if (!crys.RemoveCrys(CrystalType.Cyan, cost) || !World.TrueEmpty(x, y))
                return false;

            World.SetCell(x, y, CellType.MilitaryBlockFrame);

            _ = Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(_ =>
            {
                if (World.GetCell(x, y) != (byte)CellType.MilitaryBlockFrame)
                    return;

                World.SetCell(x, y, CellType.MilitaryBlock);
                World.DamageCell(x, y, (int)warSkill.DurabilityEffect, Operator.Unknown);
            }, TaskScheduler.Default);

            return true;
        }

        private bool BuildRoad(int x, int y)
        {
            var roadSkill = skillslist.GetSkill(SkillType.BuildRoad);
            if (roadSkill == null)
                return false;

            long cost = (long)roadSkill.Cost;
            if (crys.RemoveCrys(0, cost) && World.TrueEmpty(x, y))
            {
                World.SetCell(x, y, CellType.Road);
                return true;
            }
            return false;
        }

        private bool BuildSupport(int x, int y)
        {
            var structureSkill = skillslist.GetSkill(SkillType.BuildStructure);
            if (structureSkill == null)
                return false;

            var cellProp = World.GetProp(World.GetCell(x, y));
            bool isBuildable = World.TrueEmpty(x, y) || cellProp.isSand;

            if (!isBuildable || !crys.RemoveCrys(CrystalType.Green, (long)structureSkill.Cost))
                return false;

            World.SetCell(x, y, CellType.Support);
            return true;
        }

        #endregion

        #region Actions

        public void TryAct(Action a, double delay)
        {
            if (HasActiveProgram)
                return;

            if (ServerTime.Now < _lastDelay)
                return;

            a();
            _lastDelay = ServerTime.Now + TimeSpan.FromMilliseconds(delay);
        }

        public override void Bz()
        {
            ResourceExtraction.PerformDig(this, this, crys);
        }

        public override void Beep()
        {
            this.SendBeep();
        }

        public override long GetBox(int x, int y)
        {
            var result = base.GetBox(x, y);
            connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "+ " + result)]));
            return result;
        }

        public void tp(int x, int y)
        {
            connection?.SendU(new SmoothTPPacket(x, y));
            BotsRender();
            CheckChunkChanged();
        }

        public void teleport_tp(int x, int y)
        {
            connection?.SendU(new TPPacket(x, y));
            BotsRender();
            CheckChunkChanged();
        }

        public override void Geo()
        {
            ResourceExtraction.PerformGeo(this, this);
        }

        #endregion

        #region Health

        public override bool Heal()
        {
            return ResourceExtraction.PerformRepair(this, this);
        }

        public override void Hurt(int damage, DamageTypePlayer type = DamageTypePlayer.Pure)
        {
            skillslist.HandleDamageExperience(this, type, 1);
            int modifiedDamage = skillslist.HandleDamageReceived(damage);

            if (Health - modifiedDamage <= 0)
            {
                Health = 0;
                Death();
            }
            else
            {
                Health -= modifiedDamage;
                SendDFToBots(6, 0, 0, id, 0);
            }

            this.SendHealth();
        }

        public override void Death()
        {
            if (crys.AllCry > 0)
            {
                Box.BuildBox(x, y, crys.crystall, this, true);
            }

            ResetHp();

            if (resp == null)
            {
                var respawns = DataBase.GetResp(0);
                resp = respawns[Random.Shared.Next(respawns.Count)];
            }

            resp?.OnRespawn(this);
            var (newX, newY) = resp != null ? resp.GetRandompoint() : (0, 0);
            x = newX;
            y = newY;

            tp(x, y);

            World.W.SendFx(x, y, 2);
            World.W.SendLeaveBot(id, x, y);

            if (HasActiveProgram && programsData.RespawnOnProg)
                programsData.OnDeath();

            win = null;
            this.SendWindow();
            this.SendHealth();
            this.ProgStatus();
        }

        #endregion

        #region Program Management

        private void RunProgramm(Program programm)
        {
            win = null;
            this.SendWindow();
            programsData.Start(programm);
        }

        public void UpdateUIProgramm()
        {
            var p = programsData.Selected;
            if (p != null && !programsData.ProgRunning)
                this.OpenProg(p);
            if (programsData.ProgRunning)
                programsData.Stop();
            this.ProgStatus();
        }

        public void StartedProg((int id, string source) data)
        {
            Program? programm = DataBase.SaveAndGetProg(data);
            if (programm == default) return;
            RunProgramm(programm);
            this.UpdateProg(programm);
            this.ProgStatus();
        }

        public void OpenGuiProgram()
        {
            ProgrammatorUI.OpenGui(this);
        }

        public void RenameProgram(int id)
        {
            ProgrammatorUI.Rename(this, id);
        }

        public void DeleteProgram(int id)
        {
            ProgrammatorUI.DeleteProgram(this, id);
        }

        #endregion

        #region Rendering

        public override void BotsRender()
        {
            if (!online) return;
            base.BotsRender();
        }

        public void CheckChunkChanged(bool force = false)
        {
            var chunkPos = Chunk.GetChunkPosByCoords(x, y);
            if (!World.ValidChunk(chunkPos.x, chunkPos.y))
                return;

            if (lastchunk != (chunkPos.x, chunkPos.y) || force)
                MoveToChunk();
        }

        private void MoveToChunk()
        {
            UpdateChunkRegistration();
            UpdateVisibility();
        }

        private void UpdateChunkRegistration()
        {
            if (lastchunk != null)
            {
                var oldChunk = World.W.GetPosChunk(lastchunk.Value.X, lastchunk.Value.Y);
                oldChunk.bots.Remove(id, out _);
            }

            var newChunk = World.W.GetChunk(x, y);
            lastchunk = newChunk.pos;
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
            var result = new List<(int x, int y)>();
            foreach (var chunk in currentChunks)
            {
                if (!_alreadyVisible.Contains(chunk))
                    result.Add(chunk);
            }
            return result;
        }

        private List<(int x, int y)> GetObsoleteChunks(List<(int x, int y)> currentChunks)
        {
            var currentSet = new HashSet<(int x, int y)>(currentChunks);
            var result = new List<(int x, int y)>();
            foreach (var seen in _alreadyVisible)
            {
                if (!currentSet.Contains(seen))
                    result.Add(seen);
            }
            return result;
        }

        private void SendPackets(IEnumerable<IHubPacket> packets)
        {
            if (connection == null || packets == null) return;

            if (packets is ICollection<IHubPacket> collection)
            {
                if (collection.Count == 0) return;
                connection.SendB(new HBPacket(collection.ToArray()));
                return;
            }

            var list = packets.ToList();
            if (list.Count == 0) return;
            connection.SendB(new HBPacket(list.ToArray()));
        }

        private void UpdateTrackedChunks(
            List<(int x, int y)> chunksToAdd,
            List<(int x, int y)> chunksToRemove)
        {
            foreach (var chunk in chunksToAdd)
                _alreadyVisible.Add(chunk);

            foreach (var chunk in chunksToRemove)
                _alreadyVisible.Remove(chunk);
        }

        #endregion

        #region UI

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

            Clan.OpenWinAsync(this);
        }

        public void OpenMyBuildings()
        {
            win = MyBuildings();
            this.SendWindow();
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

            var entries = db.teleports
                .Where(t => t.ownerid == id)
                .Select(t => new ListEntry($"tp {t.x}:{t.y}", null))
                .ToList();

            var respawns = db.resps
                .Where(r => r.ownerid == id)
                .Select(r => new ListEntry($"resp {r.x}:{r.y}", null))
                .ToList();

            var ups = db.ups
                .Where(u => u.ownerid == id)
                .Select(u => new ListEntry($"up {u.x}:{u.y}", null))
                .ToList();

            var guns = db.guns
                .Where(g => g.ownerid == id)
                .Select(g => new ListEntry($"gun {g.x}:{g.y}", null))
                .ToList();

            entries.AddRange(respawns);
            entries.AddRange(ups);
            entries.AddRange(guns);

            return entries.ToArray();
        }

        #endregion

        public override void SpecialAction(ActionType action)
        {
            switch (action)
            {
                case ActionType.BOOM:
                    inventory.selected = (int)Item.PlasmaBomb;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.DISCHARGE:
                    inventory.selected = (int)Item.DischargeBomb;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.PROTON:
                    inventory.selected = (int)Item.ProtonBomb;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.VB:
                    Build("V");
                    break;
                case ActionType.Geopack:
                    inventory.selected = (int)Item.Geopack;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.ZZ:
                    inventory.selected = (int)Item.DefenseCharge;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.C190:
                    inventory.selected = (int)Item.C190;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.Up:
                    // TODO: Прокачка всех скилов
                    break;
                case ActionType.Craft:
                    // TODO: Создание крафтов
                    break;
                case ActionType.Nano:
                    inventory.selected = (int)Item.NanoBot;
                    inventory.UseSelectedItem(this);
                    break;
                case ActionType.Rembot:
                    inventory.selected = (int)Item.RepairBot;
                    inventory.UseSelectedItem(this);
                    break;
                default:
                    break;
            }
        }
    }
}