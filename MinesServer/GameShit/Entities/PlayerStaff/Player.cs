using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
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
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.Movement;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Entities.PlayerStaff
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

        private (int X, int Y)? lastchunk;
        private float cb;

        private DateTime lBotsUpdate = ServerTime.Now;
        private DateTime lastc190hit = ServerTime.Now;
        private DateTime lastDelay = ServerTime.Now;
        private DateTime laststarttime = ServerTime.Now;
        private DateTime lastuseinventory = ServerTime.Now;

        private readonly List<(int X, int Y)> alreadyvisible = [];

        private int c190stacks = 1;

        [NotMapped] public Session? connection { get; set; }
        [NotMapped] public Chat? currentchat { get; set; }
        [NotMapped] public Window? win { get; set; }
        [NotMapped] public bool online => connection != null;
        [NotMapped] public override int cid => clan?.id ?? 0;
        [NotMapped] public override int tail => HasActiveProgram ? 1 : 0;
        [NotMapped] public bool HasActiveProgram => programsData.ProgRunning;

        public CrystalCBStorage CrystalCB = new();
        public string name { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string passwd { get; set; } = string.Empty;
        public long money { get; set; }
        public long creds { get; set; }
        public long opp { get; set; }
        public Clan? clan { get; set; }
        public Rank? clanrank { get; set; }
        public Resp? resp { get; set; }
        public List<Program> programs { get; set; } = new();
        public override Basket crys { get; set; } = null!;
        public Inventory inventory { get; set; } = null!;
        public Settings settings { get; set; } = null!;
        public PlayerSkills skillslist { get; set; } = null!;
        public Queue<Line> console = new Queue<Line>();
        public override double ServerPause => (World.isRoad(x, y) ? (pause * 5) * 0.80 : pause * 5) * 1.4 / 1000;

        public override int pause
        {
            get
            {
                var moveSkill = skillslist.GetSkill(SkillType.Movement);
                return moveSkill != null ? (int)(moveSkill.Effect * 100) : BaseMoveDelay;
            }
        }

        public string geology
        {
            get
            {
                if (geo == null || geo.Count == 0)
                    return "[]";

                // Сериализуем стек в JSON массив
                return Newtonsoft.Json.JsonConvert.SerializeObject(geo.ToList());
            }
            set
            {
                if (value == null || string.IsNullOrEmpty(value) || value == "[]")
                {
                    geo = new Stack<byte>();
                    return;
                }

                try
                {
                    // Десериализуем JSON обратно в список и создаем стек
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
            if (!DataBase.activeplayers.Contains(this))
                DataBase.activeplayers.Add(this);

            skillslist.LoadSkills();
            MaxHealth = CalculateMaxHealth();
            Health = Health <= 0 ? MaxHealth : Health;
            win = null;

            MoveToChunk();

            SendInitialData();
            SubscribeToEvents();

            
        }
        public void CreatePlayer()
        {
            name = "";
            money = 1000;
            creds = 0;
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
        }

        private void SubscribeToEvents()
        {
            if (crys.shouldsubscribe)
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
            if (now - lastc190hit <= TimeSpan.FromMinutes(C190StackResetMinutes))
                return;

            c190stacks = 1;
            lastc190hit = now;
        }

        public void HurtC190Stacks()
        {
            Hurt(20 + 60 * c190stacks);
            c190stacks++;
            lastc190hit = ServerTime.Now;
        }

        public bool HandleUseInventory()
        {
            DateTime now = ServerTime.Now;
            bool canUse = (now - lastuseinventory) >= TimeSpan.FromMilliseconds(400) ||
                           programsData.ProgRunning;
            if (canUse)
                lastuseinventory = now;
            return canUse;
        }

        private void HandleOffline(DateTime now)
        {
            if (now - laststarttime <= TimeSpan.FromMinutes(AfkTimeoutMinutes))
                return;

            DataBase.activeplayers.Remove(this);
            Death();
        }

        private void UpdateBots(DateTime now)
        {
            if (now - lBotsUpdate <= TimeSpan.FromSeconds(BotUpdateIntervalSeconds))
                return;

            BotsRender();
            lBotsUpdate = now;
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

            laststarttime = ServerTime.Now;
            connection = null;
            alreadyvisible.Clear();
        }

        #endregion

        #region Movement

        public override bool Move(int x, int y, DirectionType Direction = DirectionType.Unknown)
        {
            if (!World.ValidCoord(x, y) || win != null)
            {
                tp(this.x, this.y);
                return false;
            }

            UpdateDirection(x, y, Direction);

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

            if (World.ContainsPack(x, y, out var pack) && 
                (pack.cid == cid ||
                pack.ownerid == id) &&
                !HasActiveProgram)
            {
                win = pack.GUIWin(this);
                this.SendWindow();
            }

            return false;
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

        private void UpdatePosition(int x, int y)
        {
            if (Vector2.Distance(new Vector2(this.x, this.y), new Vector2(x, y)) >= 1f)
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
            var greenSkill = skillslist.GetSkill(SkillType.BuildGreen);
            var yellowSkill = skillslist.GetSkill(SkillType.BuildYellow);
            var redSkill = skillslist.GetSkill(SkillType.BuildRed);

            if (greenSkill != null && (World.TrueEmpty(x, y) || cellprop.isSand))
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
            });
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

            var cellprop = World.GetProp(World.GetCell(x, y));

            if (!crys.RemoveCrys(CrystalType.Green, (long)structureSkill.Cost) || 
                !World.TrueEmpty(x, y) && !cellprop.isSand)
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

            if (lastDelay >= ServerTime.Now)
                return;

            a();
            lastDelay = ServerTime.Now + TimeSpan.FromMilliseconds(delay);
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

        public override void Beep() 
        {
            pSenders.Beep(this);
        }

        public void BBox(long[]? c)
        {
            var (x, y) = GetDirCord();

            if (!World.ValidCoord(x, y) || c == null)
                return;

            Box.BuildBox(x, y, c, this);
            connection?.CloseWindow();
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
            ResourceExtractionService.PerformGeo(this, this);
        }

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

        public void RunProgramm(Program? p = null)
        {
            win = null;
            this.SendWindow();

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

        public void UpdateUIProgramm()
        {
            var p = programsData.Selected;
            if (p != null && !programsData.ProgRunning)
                this.OpenProg(p);
            if (programsData.ProgRunning)
                RunProgramm();
            this.ProgStatus();
        }

        public void StartedProg((int id, string source) data)
        {
            Program? programm = DataBase.SaveAndGetProg(data);
            if (programm != default)
            {
                RunProgramm(programm);
                this.UpdateProg(programm);
            }
            this.ProgStatus();
        }

        public void OpenGuiProgramm()
        {
            StaticGUI.OpenGui(this);
        }

        public void Rename(int Id)
        {
            StaticGUI.Rename(this, Id);
        }

        #endregion

        #region Rendering

        public override void BotsRender()
        {
            if (connection == null) return;

            World.W.SendBotsInfo(id, x, y, dir, skin, cid, tail);
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
        #endregion

        public override void SpecialAction(ActionType Action)
        {
            switch (Action)
            {
                case ActionType.BOOM:
                    inventory.selected = (int)Item.PlasmaBomb;
                    inventory.Use(this);
                    break;
                case ActionType.DISCHARGE:
                    inventory.selected = (int)Item.DischargeBomb;
                    inventory.Use(this);
                    break;
                case ActionType.PROTON:
                    inventory.selected = (int)Item.ProtonBomb;
                    inventory.Use(this);
                    break;
                case ActionType.VB:
                    Build("V");
                    break;
                case ActionType.Geopack:
                    inventory.selected = (int)Item.Geopack;
                    inventory.Use(this);
                    break;
                case ActionType.ZZ:
                    inventory.selected = (int)Item.DefenseCharge;
                    inventory.Use(this);
                    break;
                case ActionType.C190:
                    inventory.selected = (int)Item.C190;
                    inventory.Use(this);
                    break;
                case ActionType.Up:
                    // TODO: Прокачка всех скилов
                    break;
                case ActionType.Craft:
                // TODO: Создание крафтов
                case ActionType.Nano:
                    inventory.selected = (int)Item.NanoBot;
                    inventory.Use(this);
                    break;
                case ActionType.Rembot:
                    inventory.selected = (int)Item.RepairBot;
                    inventory.Use(this);
                    break;
                default: break;
            }
        }
    }
}