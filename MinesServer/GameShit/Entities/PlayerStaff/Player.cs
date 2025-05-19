using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Identity.Client;
using MinesServer.Enums;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.ClanSystem;
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
using System.Data.Entity;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class Player : PEntity
    {
        #region forprogs
        public void RunProgramm(Program p = null)
        {
            win = null;
            SendWindow();
            if (p == null)
            {
                programsData.Run();
                return;
            }

            programsData.Run(p);
        }
        #endregion
        #region fields
        [NotMapped]
        private (int, int)? lastchunk { get; set; }
        [NotMapped]
        public Chat? currentchat { get; set; }
        [NotMapped]
        public Session? connection { get; set; }
        [NotMapped]
        public bool online
        {
            get => connection != null;
        }
        public Player() => Delay = ServerTime.Now;
        private DateTime lBotsUpdate = ServerTime.Now;
        public DateTime afkstarttime = ServerTime.Now;
        private DateTime lastSync = ServerTime.Now;
        public string name { get; set; }
        public Clan? clan { get; set; }
        public Rank? clanrank { get; set; }
        public override int pause //= 3500;
        {
            get
            {
                var retval = 10000;
                foreach (var c in skillslist.skills.Values)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnMove, this))
                    {
                        if (c.type == SkillType.Movement)
                        {
                            retval = (int)(c.Effect * 100);
                        }
                    }
                }
                return retval;
            }
        }
        public List<Program> programs { get; set; }
        public Resp? resp
        {
            get
            {
                if (_resp is null)
                {
                    using var db = new DataBase();
                    db.Attach(this);
                    var re = db.resps.Where(i => i.ownerid == 0);
                    var rp = Physics.r.Next(0, re.Count());
                    _resp = re.ElementAt(rp);
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
        private Resp? _resp;
        [NotMapped]
        public override int cid { get => clan == null ? 0 : clan.id; }
        public long money { get; set; }
        public long creds { get; set; }
        public string hash { get; set; }
        public string passwd { get; set; }
        [NotMapped]
        public int tail { get => programsData.ProgRunning ? 1 : 0; }
        public int skin
        {
            get
            {
                if (online)
                    return _skin;
                return 1;
            }
            set => _skin = value;
        }
        private int _skin;
        public bool autoDig { get; set; }
        public bool agression { get; set; }
        public int c190stacks = 1;
        public DateTime lastc190hit = ServerTime.Now;
        public override Basket crys { get; set; }
        public Inventory inventory { get; set; }
        public Settings settings { get; set; }
        public PlayerSkills skillslist { get; set; }
        public Queue<Line> console = new Queue<Line>();
        [NotMapped]
        public Window? win;
        [NotMapped]
        private float cb;
        public DateTime Delay = ServerTime.Now;
        public bool CanAct { get => !(Delay.AddMilliseconds(ServerTime.offset) > ServerTime.Now); }
        public bool OnRoad { get => World.isRoad(World.GetCell(x, y)); }
        public override double ServerPause
        {
            get => (OnRoad ? (pause * 5) * 0.80 : pause * 5) * 1.4 / 1000;
        }
        private void Sync()
        {
            using var db = new DataBase();
            db.players.Update(this);
            db.SaveChanges();
        }
        public void ProgrammatorUpdate()
        {
            if (programsData.ProgRunning)
                programsData.Step();
        }
        #endregion
        #region actions
        public override void Update()
        {
            var now = ServerTime.Now;
            if (now - lastSync >= TimeSpan.FromSeconds(10))
            {
                Sync();
                lastSync = now;
            }
            if (now - lastc190hit >= TimeSpan.FromMinutes(1))
            {
                c190stacks = 1;
                lastc190hit = now;
            }
            if (!online)
            {
                if (now - afkstarttime > TimeSpan.FromMinutes(5))
                {
                    DataBase.activeplayers.Remove(this);
                    Death();
                }
                return;
            }
            if (now - lBotsUpdate > TimeSpan.FromSeconds(4))
            {
                BotsRender();
                lBotsUpdate = ServerTime.Now;
            }
            var cell = World.GetCell(x, y);
            var cellprop = World.GetProp(cell);
            if (!cellprop.isEmpty)
            {
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
        }
        public void SetResp(Resp r) => resp = r;
        public void TryAct(Action a, double delay)
        {
            if (programsData.ProgRunning) return;
            if (Delay < ServerTime.Now)
            {
                a();
                Delay = ServerTime.Now + TimeSpan.FromMilliseconds(delay);
            }
        }
        #region mybuildings
        private Window MyBuildings => new Window()
        {
            Title = "мои здания да",
            Tabs = [new Tab()
            {
                Action = "amy",
                Label = "amyl",
                InitialPage = new Page()
                {
                    List = MyBuildingsList(),
                    Buttons = [new MButton("собратьб","четатам")]
                }
            }]
        };
        public void OpenMyBuildings()
        {
            win = MyBuildings;SendWindow();
        }
        private ListEntry[] MyBuildingsList()
        {
            using var db = new DataBase();
            var l = new List<ListEntry>();
            foreach (var i in db.teleports.Where(r => r.ownerid == id))
            {
                l.Add(new ListEntry($"tp {i.x}:{i.y}", null));
            }
            foreach (var i in db.resps.Where(r => r.ownerid == id))
            {
                l.Add(new ListEntry($"resp {i.x}:{i.y}", null));
            }
            foreach (var i in db.ups.Where(r => r.ownerid == id))
            {
                l.Add(new ListEntry($"up {i.x}:{i.y}", null));
            }
            foreach (var i in db.guns.Where(r => r.ownerid == id))
            {
                l.Add(new ListEntry($"gun {i.x}:{i.y}", null));
            }
            return l.ToArray();
        }
        #endregion
        private int ParseCryType(CellType cell)
        {
            return cell switch
            {
                CellType.XGreen or CellType.Green => 0,
                CellType.XBlue or CellType.Blue => 1,
                CellType.XRed or CellType.Red => 2,
                CellType.XViolet or CellType.Violet => 3,
                CellType.White => 4,
                CellType.XCyan or CellType.Cyan => 5,
                _ => 0
            };
        }
        public override void Geo()
        {
            base.Geo();
            this.SendGeo();
        }
        public void BBox(long[]? c)
        {
            var boxc = GetDirCord();
            if (!World.W.ValidCoord(boxc.x, boxc.y) || c == null)
            {
                return;
            }
            Box.BuildBox(boxc.x, boxc.y, c, this);
            connection?.CloseWindow();

        }
        private void Mine(byte cell, int x, int y)
        {
            float dob = 1 + (float)Math.Truncate(cb);
            foreach (var c in skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDigCrys, this))
                {
                    if (c.type == SkillType.MineGeneral)
                    {
                        dob += c.Effect;
                        c.AddExp(this, (float)Math.Truncate(dob));
                    }
                }
            }
            dob *= (CellType)cell switch
            {
                CellType.XGreen => 4,
                CellType.XBlue => 3,
                CellType.XRed => 2,
                CellType.XViolet => 2,
                CellType.XCyan => 2,
                _ => 1
            };
            cb -= (float)Math.Truncate(cb);
            long odob = (long)Math.Truncate(dob);
            var type = ParseCryType((CellType)cell);
            cb += dob - odob;
            crys.AddCrys(type, odob);
            World.AddDob(type, odob);
            SendDFToBots(2, x, y, id, (int)(odob < 255 ? odob : 255), type == 1 ? 3 : type == 2 ? 1 : type == 3 ? 2 : type);
        }
        public void GetBox(int x, int y)
        {
            var result = base.GetBox(x, y);
            connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "+ " + result)]));
        }
        private void OnDestroy(byte type)
        {
            foreach (var c in skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                {
                    c.AddExp(this);
                }
            }
        }
        public override void Bz()
        {
            var cord = GetDirCord();
            int x = cord.x, y = cord.y;
            if (!World.W.ValidCoord(x, y))
            {
                return;
            }
            SendDFToBots(0, this.x, this.y, id, dir);
            var cell = World.GetCell(x, y);
            if (World.GetProp(cell).damage > 0)
            {
                Hurt(World.GetProp(cell).damage);
            }
            if (!World.GetProp(cell).is_diggable)
            {
                return;
            }
            if (cell == 90)
            {
                GetBox(x, y);
                World.DamageCell(x, y, 1);
                return;
            }
            if (cell == (byte)CellType.MilitaryBlock)
            {
                World.DamageCell(x, y, 1);
                return;
            }
            float hitdmg = 0.2f;
            if (World.isCry(cell))
            {
                hitdmg = 1f;
                Mine(cell, x, y);
            }
            else
            {
                foreach (var c in skillslist.skills.Values)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                    {
                        hitdmg = c.type switch
                        {
                            SkillType.Digging => hitdmg * (c.Effect / 100f),
                            _ => 1f
                        };
                    }
                }
            }
            if (World.DamageCell(x, y, hitdmg)) OnDestroy(cell);
            if (World.GetProp(cell).isBoulder)
            {
                var plusy = dir == 2 ? -1 : dir == 0 ? 1 : 0;
                var plusx = dir == 3 ? 1 : dir == 1 ? -1 : 0;
                if (World.GetProp(World.GetCell(x + plusx, y + plusy)).isEmpty)
                {
                    World.MoveCell(x, y, plusx, plusy);
                    foreach (var c in skillslist.skills.Values)
                    {
                        if (c != null && c.UseSkill(SkillEffectType.OnDig, this))
                        {
                            c.AddExp(this);
                        }
                    }
                }
            }
        }
        public override bool Move(int x, int y, int dir = -1, bool prog = false)
        {
            if (!World.W.ValidCoord(x, y) || (win != null && !prog))
            {
                tp(this.x, this.y);
                return false;
            }
            if (dir > 9)
                dir -= 10;
            if (dir == -1 || this.x != x || this.y != y)
                this.dir = this.x > x ? 1 : this.x < x ? 3 : this.y > y ? 2 : 0;
            else
                this.dir = dir;
            var packhere = World.ContainsPack(x, y, out var pack);
            if (packhere && pack is Gate && pack.cid != cid)
            {
                tp(this.x, this.y);
                return false;
            }
            var cell = World.GetCell(x, y);
            if (!World.GetProp(cell).isEmpty)
            {
                if (dir == -1)
                {
                    tp(this.x, this.y);
                    if (autoDig)
                    {
                        Bz();
                    }
                    return true;
                }
                tp(this.x, this.y);
                return false;
            }
            if (Vector2.Distance(new Vector2(this.x,this.y), new Vector2(x, y)) < 1.2f)
            {
                foreach (var c in skillslist.skills.Values)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnMove, this))
                    {
                        if (c.type == SkillType.Movement)
                        {
                            c.AddExp(this);
                        }
                    }
                }
                this.x = x;this.y = y;
            }
            else
            {
                tp(this.x, this.y);
                return false;
            }
            SendMyMove();
            CheckChunkChanged();
            if (packhere && (pack.cid == cid || pack.cid == 0) && !programsData.ProgRunning)
            {
                win = pack.GUIWin(this)!;
                SendWindow();
            }
            return false;
        }
        public override void Build(string type)
        {
            int x = (int)GetDirCord().x, y = (int)GetDirCord().y;
            if (!World.W.ValidCoord(x, y) || !World.AccessGun(x, y, cid).access || World.PackPart(x, y))
            {
                return;
            }
            var prop = World.GetProp(x, y);
            var buildskills = skillslist.skills.Values.Where(c => c.EffectType() == SkillEffectType.OnBld);
            switch (type)
            {
                case "G":
                    foreach (var c in buildskills)
                    {
                        if (c.type == SkillType.BuildGreen && (World.TrueEmpty(x, y) || prop.isSand))
                        {
                            if (crys.RemoveCrys(0, (long)c.Effect))
                            {
                                c.AddExp(this);
                                World.SetCell(x, y, CellType.GreenBlock);
                                World.SetDurability(x, y, c.AdditionalEffect);
                            }
                            return;
                        }
                        else if (c.type == SkillType.BuildYellow && World.GetCell(x, y) == (byte)CellType.GreenBlock)
                        {
                            if (crys.RemoveCrys(4, (long)c.Effect))
                            {
                                c.AddExp(this);
                                World.SetCell(x, y, CellType.YellowBlock);
                                World.SetDurability(x, y, World.GetDurability(x, y) + c.AdditionalEffect);
                            }
                            return;
                        }
                        else if (c.type == SkillType.BuildRed && World.GetCell(x, y) == (byte)CellType.YellowBlock)
                        {
                            if (crys.RemoveCrys(2, (long)c.Effect))
                            {
                                c.AddExp(this);
                                World.SetCell(x, y, CellType.RedBlock);
                                World.SetDurability(x, y, World.GetDurability(x, y) + c.AdditionalEffect);
                            }
                            return;
                        }
                    }
                    break;
                case "V":
                    foreach (var c in buildskills)
                    {
                        if (c.type == SkillType.BuildWar)
                        {
                            if (crys.RemoveCrys(5, (long)c.Effect) && World.TrueEmpty(x, y))
                            {
                                c.AddExp(this);
                                World.SetCell(x, y, CellType.MilitaryBlockFrame);
                                World.W.StupidAction(10, x, y, () =>
                                {
                                    if (World.GetCell(x, y) == (byte)CellType.MilitaryBlockFrame)
                                    {
                                        c.AddExp(this);
                                        World.SetCell(x, y, CellType.MilitaryBlock);
                                        World.SetDurability(x, y, c.AdditionalEffect);
                                    }
                                });
                            }
                            return;
                        }
                    }
                    break;
                case "R":
                    foreach (var c in buildskills)
                    {
                        if (c.type == SkillType.BuildRoad)
                        {
                            if (crys.RemoveCrys(0, (long)c.Effect) && World.TrueEmpty(x,y))
                            {
                                c.AddExp(this);
                                World.SetCell(x, y, CellType.Road);
                            }
                            return;
                        }
                    }
                    break;
                case "O":
                    foreach (var c in buildskills)
                    {
                        if (c.type == SkillType.BuildStructure)
                        {
                            if (crys.RemoveCrys(0, (long)c.Effect) && (World.TrueEmpty(x, y) || prop.isSand))
                            {
                                c.AddExp(this);
                                World.SetCell(x, y, CellType.Support);
                            }
                            return;
                        }
                    }
                    break;
            }
        }
        #endregion
        #region creating
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
            skillslist = new PlayerSkills(this);
            x = 0;y = 0;
            dir = 0;
            clan = null;
            skin = 0;
        }
        public void dOnDisconnect()
        {
            using var db = new DataBase();
            db.players.Update(this);
            db.SaveChanges();
            afkstarttime = ServerTime.Now;
            connection = null;
            alreadyvisible.Clear();
        }
        public void Init()
        {
            connection.auth = null;
            if (DataBase.activeplayers.FirstOrDefault(p => p.id == id) == default)
            {
                DataBase.activeplayers.Add(this);
            }
            skillslist.LoadSkills();
            MaxHealth = 100;
            foreach (var c in skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, this))
                {
                    if (c.type == SkillType.Health)
                    {
                        MaxHealth = (int)c.Effect;
                    }
                }
            }
            MoveToChunk(ChunkX, ChunkY);
            Health = Health <= 0 ? MaxHealth : Health;
            this.SendAutoDigg();
            this.SendGeo();
            this.SendHealth();
            this.SendBotInfo();
            this.SendSpeed();
            if (crys.shouldsubscribe)
            crys.Changed += this.SendCrys;
            this.SendCrys();
            this.SendMoney();
            this.SendLvl();
            this.SendInventory();
            CheckChunkChanged(true);
            tp(x, y);
            console.Enqueue(new Line { text = "@@> Добро пожаловать в консоль!" });
            for (var i = 0; i < 4; i++)
            {
                MConsole.AddConsoleLine(this);
            }

            MConsole.AddConsoleLine(this, "Если вы не понимаете, что происходит,");
            MConsole.AddConsoleLine(this, "или вас попросили выполнить команду,");
            MConsole.AddConsoleLine(this, "сосите хуй глотайте сперму");
            for (var i = 0; i < 8; i++)
            {
                MConsole.AddConsoleLine(this);
            }
            settings.SendSettings(this);
            this.SendClan();
            SendChat();
            connection?.SendU(new ConfigPacket("oldprogramformat+"));
            if (programsData.selected is not null)
                this.UpdateProg(programsData.selected);
            this.ProgStatus();
            win = null;
        }

        #endregion
        #region senders

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
        public void SendWindow()
        {
            if (win != null)
            {
                connection?.SendU(new GUIPacket(win.ToString()));
                return;
            }
            connection?.SendU(new GuPacket());
        }
        public void OpenClan()
        {
            if (clan != null)
            {
                using var db = new DataBase();
                db.clans.Where(i => i.id == clan.id).Include(p => p.members).Include(p => p.reqs).FirstOrDefault()?.OpenClanWin(this);
            }
        }
        public void tp(int x, int y)
        {
            connection?.SendU(new TPPacket(x, y));
            SendMyMove();
        }
        #endregion
        #region renders
        public void BotsRender()
        {
            IEnumerable<IHubPacket> bots = new List<IHubPacket>();
            foreach (var chunk in vChunksAround())
                bots = bots.Concat(GetBotsInChunk(chunk.x, chunk.y));
            connection?.SendB(new HBPacket(bots.ToArray()));
        }
        private IHubPacket[] GetBotsInChunk(int chunky,int chunkx)
        {
            List<IHubPacket> bots = new();
            var ch = World.W.chunks[chunkx, chunky];
            foreach (var id in ch.bots)
            {
                var player = DataBase.GetPlayer(id.Key);
                if (player is not null)
                {
                    bots.Add(new HBBotPacket(player.id, player.x, player.y, player.dir, player.skin, player.cid, player.tail));
                }
            }
            return bots.ToArray();
        }
        private IHubPacket[] ChunkInfo(int chunkx,int chunky)
        {
            List<IHubPacket> result = new();
            var chunk = World.W.chunks[chunkx, chunky];
            result.Add(new HBMapPacket(chunk.WorldX, chunk.WorldY, 32, 32, chunk.cells));
            if (!alreadyvisible.Contains((chunkx, chunky))) return result.Concat(chunk.pPakcs(this)).ToArray();
            return result.ToArray();
        }
        private IHubPacket[] fChunkInfo(int chunkx, int chunky) => ChunkInfo(chunkx, chunky).Concat(GetBotsInChunk(chunkx, chunky)).ToArray();
        private void StupidVisabilityUpdate()
        {
            List<IHubPacket> packets = new();
            List<(int x,int y)> old = new List<(int x, int y)>(alreadyvisible);
            foreach (var chunk in vChunksAround())
            {
                var pos = chunk.x + chunk.y * World.ChunksH;
                var turple = (chunk.x, chunk.y);
                if (old.Contains(turple)) old.Remove(turple);
                else
                {
                    packets = packets.Concat(fChunkInfo(chunk.x,chunk.y)).ToList();
                }
                if (!alreadyvisible.Contains(turple))
                    alreadyvisible.Add(turple);
            }
            foreach(var abandoned in old)
            {
                alreadyvisible.Remove(abandoned);
                var chunk = World.W.chunks[abandoned.x, abandoned.y];
                foreach (var pack in chunk.packs.Values)
                {
                    packets.Add(new HBPacksPacket(chunk.PACKPOS(pack.x,pack.y), []));
                }
            }
            connection?.SendB(new HBPacket(packets.ToArray()));
        }
        public void SendMyMove()
        {
            if (connection is null) return;
            var valid = bool (int x, int y) => x >= 0 && y >= 0 && x < World.ChunksW && y < World.ChunksH;
            foreach (var ch in vChunksAround())
            {
                var chunk = World.W.chunks[ch.x,ch.y];
                foreach (var id in chunk.bots)
                    DataBase.GetPlayer(id.Key)?.connection?.SendB(new HBPacket([new HBBotPacket(this.id, x, y, dir, skin, cid, tail)]));
            }
        }
        List<(int,int)> alreadyvisible = new();
        public void CheckChunkChanged(bool force = false)
        {
            var valid = bool (int x, int y) => x >= 0 && y >= 0 && x < World.ChunksW && y < World.ChunksH;
            if (!valid(ChunkX, ChunkY)) return;
            if (lastchunk != (ChunkX, ChunkY) || force) MoveToChunk(ChunkX, ChunkY);
        }
        void MoveToChunk(int x, int y)
        {
            StupidVisabilityUpdate();
            if (lastchunk != null && World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2] != null)
            {
                var chtoremove = World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2];
                if (chtoremove.bots.ContainsKey(id))
                {
                    chtoremove.bots.Remove(id, out var p);
                }
            }
            var chtoadd = World.W.chunks[x, y];
            lastchunk = (x, y);
            if (World.W.chunks[lastchunk.Value.Item1, lastchunk.Value.Item2] != null)
            {
                if (!chtoadd.bots.ContainsKey(id))
                {
                    chtoadd.AddBot(this);
                }
            }
        }
        #endregion
        public string GenerateHash()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
        #region health
        public override bool Heal(int num = -1)
        {
            var heal = skillslist.skills.Values.FirstOrDefault(i => i.type == SkillType.Repair);
            if (Health == MaxHealth || heal == default)
                return false;
            num = (int)heal.Effect;
            if (num == -1)
                return false;
            if (crys.RemoveCrys(2, 1))
            {
                heal.AddExp(this);
                Health += num;
                if (Health > MaxHealth)
                    Health = MaxHealth;
                SendDFToBots(5, 0, 0, id, 0);
                this.SendHealth();
                return true;
            }
            return false;
        }

        public override void Hurt(int num, DamageType t = DamageType.Pure)
        {
            foreach (var c in skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, this))
                {
                    if (c.type == SkillType.Health)
                    {
                        c.AddExp(this);
                    }
                }
                if (c != null && c.UseSkill(SkillEffectType.OnHurt, this) && t == DamageType.Gun)
                {
                    if (c.type == SkillType.Induction)
                    {
                        c.AddExp(this);
                    }
                    if (c.type == SkillType.AntiGun)
                    {
                        c.AddExp(this);
                        var eff = (int)(num * (c.Effect / 100));
                        if (num - eff >= 0)
                        {
                            num -= eff;
                        }
                        else
                        {
                            num = 0;
                        }
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
        private async Task<(int x, int y)> FindEmptyForBox(int x, int y)
        {
            return await Task.Run(() =>
            {
                var dirs = new (int, int)[] { (0, 1), (1, 0), (-1, 0), (0, -1) };
                var q = new Queue<(int, int)>();
                var valid = bool (int x, int y) => World.GetProp(x, y).isEmpty && !World.PackPart(x, y) && World.W.ValidCoord(x, y);
                var a = World.PackPart(x, y);
                if (!valid(x, y))
                {
                    q.Enqueue((x, y));
                }
                while (q.Count > 0)
                {
                    var b = q.Dequeue();
                    foreach (var dir in dirs)
                    {
                        if (!valid(b.Item1 + dir.Item1, b.Item2 + dir.Item2))
                        {
                            q.Enqueue((b.Item1 + dir.Item1, b.Item2 + dir.Item2));
                            continue;
                        }
                        return (b.Item1 + dir.Item1, b.Item2 + dir.Item2);
                    }
                }
                return (x, y);
            });
        }

        public override void Death()
        {
            if (crys.AllCry > 0)
            {
                var c = FindEmptyForBox(x, y);
                c.ContinueWith((f) =>
                {
                    Box.BuildBox(f.Result.x, f.Result.y, crys.cry, this, true);
                    crys.ClearCrys();
                });
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
            }
            resp.OnRespawn(this);
            var newpos = resp.GetRandompoint();
            x = newpos.Item1; y = newpos.Item2;
            tp(x, y);
            BotsRender();
            CheckChunkChanged();
            this.SendHealth();
            if (programsData.ProgRunning)
            {
                if (programsData.RespawnOnProg)
                {
                    programsData.OnDeath();
                    return;
                }
                RunProgramm();
                connection?.SendU(new ProgrammatorPacket(false));
            }
        }
        #endregion
    }
}
