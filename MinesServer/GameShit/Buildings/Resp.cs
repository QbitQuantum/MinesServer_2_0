using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using MinesServer.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace MinesServer.GameShit.Buildings
{
    public sealed class Resp : PackCharge
    {
        private Resp() { }
        public Resp(int x, int y, int ownerid) : base(x, y, ownerid, 1000, 1000)
        {
            charge = 100;
            using var db = new DataBase();
            db.resps.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Resp;
        [NotMapped] public override int PackId => 1;
        [NotMapped] public int clanzone { get; set; }
        [NotMapped] public override int off
        {
            get => (charge > 0 ? 1 : 0);
        }
        public int cost { get; set; }
        public long moneyinside { get; set; }

        #endregion

        #region affectworld
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x + 1, y, 37, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x + 1, y + 2, 106, true);
            World.SetCell(x - 1, y + 2, 106, true);
            World.SetCell(x, y + 2, 37, true);
            for (int xx = x + 2; xx < x + 6; xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, true);
                }
            }
            base.Build();
        }
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x + 1, y + 2, 32, false);
            World.SetCell(x - 1, y + 2, 32, false);
            World.SetCell(x, y + 2, 32, false);
            for (int xx = x + 2; xx < x + 6; xx++)
            {
                for (int yy = y - 1; yy < y + 3; yy++)
                {
                    World.SetCell(xx, yy, 35, false);
                }
            }
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            foreach (var i in db.players.Include(p => p.resp))
            {
                var player = DataBase.GetPlayer(i.id);
                if (player != null) player.resp = null;
            }
            db.resps.Remove(this);
            db.SaveChanges();

            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[1]++;
            }
        }

        #endregion

        public void OnRespawn(Player p)
        {
            if (ownerid == 0) return;

            bool ExistMoney = p.money > cost;
            bool ExistCharge = charge > 0;

            if (ExistMoney && ExistCharge)
            {
                using var db = new DataBase();
                db.Attach(this);
                p.money -= cost;
                moneyinside += cost;
                charge--;
                p.SendMoney();
                base.Update();
                db.SaveChanges();
            }
        }

        public (int x, int y) GetRandompoint()
        {
            var r = new Random();
            return (r.Next(x + 2, x + 5), r.Next(y - 1, y + 3));
        }

        public void Fill(Player p, long num)
        {
            using var db = new DataBase();
            if (p.crys[CrystalType.Blue] < num)
            {
                num = p.crys[CrystalType.Blue];
            }
            db.Attach(this);
            if (p.crys.RemoveCrys(CrystalType.Blue, num))
            {
                charge += (int)num;
            }
            p.win?.CurrentTab.Replace(AdmnPage(p));
            p.SendWindow();
            db.SaveChanges();
        }

        public void AdminSaveChanges(Player p, Dictionary<string, string> d)
        {
            cid = 0;
            if (bool.TryParse(d["clan"], out var clan))
            {
                if (clan)
                {
                    var Player = DataBase.GetPlayer(ownerid);
                    if (Player != null) cid = Player.cid;
                }                
            }
            if (int.TryParse(d["cost"], out var costs) && costs >= 0 && costs <= 5000)
            {
                cost = costs;
            }
            if (int.TryParse(d["clanzone"], out var clanz))
            {
                clanzone = clanz;
            }
            p.win?.CurrentTab.Replace(AdmnPage(p));
        }

        private Page AdmnPage(Player p)
        {
            var Crystall = p.crys[CrystalType.Blue];
            MButton[] fillbuttons = [
                Crystall >= 100 ? new MButton("+100", "fill:100", (args) => Fill(p, 100)) : new MButton("+100", "fill:100"),
                Crystall >= 1000 ? new MButton("+1000", "fill:1000", (args) => Fill(p, 1000)) : new MButton("+1000", "fill:1000"),
                Crystall >= 0 ? new MButton("max", "fill:max", (args) => Fill(p, (long)(maxcharge - charge))) : new MButton("max", "fill:max")
               ];

            return new Page()
            {
                Text = " ",
                RichList = new RichListConfig()
                {
                    Entries = 
                    [
                        RichListEntry.Fill("заряд", (int)charge, (int)maxcharge, CrystalType.Blue, fillbuttons[0], fillbuttons[1], fillbuttons[2]),
                        RichListEntry.Text("hp", "hp"),
                        RichListEntry.UInt32("cost", "cost", (uint)cost),
                        RichListEntry.Button($"прибыль {moneyinside}$", moneyinside == 0 ? default : new MButton("Получить", "getprofit", (args) => { 
                            using var db = new DataBase(); 
                            p.money += moneyinside; moneyinside = 0; 
                            p.SendMoney(); 
                            db.SaveChanges(); 
                            p.win?.CurrentTab.Replace(AdmnPage(p)); 
                            p.SendWindow(); 
                        })),
                        RichListEntry.Bool("Клановый респ", "clan", cid > 0),
                        RichListEntry.UInt32("clanzone", "clanzone", (uint)clanzone)
                    ]
                },
                Buttons = [new MButton("СОХРАНИТЬ", $"save:{ActionMacros.RichList}", (args) => { AdminSaveChanges(p, args.RichList); })]
            };
        }

        private Tab RespTab(Player p)
        {
            Action? adminaction = (p.id == ownerid) ? () =>
            {
                p.win?.CurrentTab.Open(AdmnPage(p));
            }
            : null;

            bool InvalidPlayerResp = p.resp == null || (p.resp != null && p.resp.x != x && p.resp.y != y);

            MButton[] buttons = InvalidPlayerResp ? [new MButton("ПРИВЯЗАТЬ", "bind", (args) =>
            {
                p.resp = this;
                p.win  = GUIWin(p)!;
            })]
            : [];

            string viewtext = $"@@Респ - это место, где будет появляться ваш робот после уничтожения\n\nЦена восстановления: <color=green>${cost}</color>\n\n" +
                "<color=green>" + (InvalidPlayerResp ? "Привязать робота к респу?" : "Вы привязаны к этому респу") + "</color>";

            Page page = new Page()
            {
                OnAdmin = adminaction,
                Text = viewtext,
                Buttons = buttons
            };

            return new Tab()
            {
                Label = "Респаун",
                Action = "respawn",
                InitialPage = page
            };
        }

        private CanvasElement[] CanvasSkinsTab()
        {
            List<CanvasElement> n = new();

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=#00ff00>НАДЕТО</color>", "skin0"),
                    originDX: -200, originDY: -100));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=yellow>100</color>", "skin1"),
                    originDX: 125));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=yellow>500</color>", "skin2"),
                    originDX: 125));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=yellow>1000</color>", "skin3"),
                    originDX: 125));

            n.Add(CanvasElement.TextField("<color=yellow>   ПЧЕЛА</color>\n\n\n", originDX: -385, originDY: 87));
            n.Add(CanvasElement.TextField("<color=yellow>   ТОЛСТЯК</color>\n\n<size=11>При перевесе\nурон в 4 раза\n    меньше</size>", originDX: 135));
            n.Add(CanvasElement.TextField("<color=yellow>   КОМАР</color>\n\n<size=11>Вас не видят\n бот-радары</size>", originDX: 120));
            n.Add(CanvasElement.TextField("<color=yellow>   ВДОВА</color>\n\n<size=11>   +100 к\n потреблению\nпушек</size>", originDX: 120));

            n.Add(CanvasElement.Image("inner:SKIN:0", originDX: -365, originDY: 90, width: 2, height: 2));
            n.Add(CanvasElement.Image("inner:SKIN:4", originDX: 125, width: 2, height: 2));
            n.Add(CanvasElement.Image("inner:SKIN:5", originDX: 125, width: 2, height: 2));
            n.Add(CanvasElement.Image("inner:SKIN:6", originDX: 125, width: 2, height: 2));

            n.Add(CanvasElement.MicroButton(
                new MButton($"->", "right"),
                    originDX: 100, originDY: -75));

            return n.ToArray();
        }

        private Tab SkinsTab(Player p)
        {
            return new Tab()
            {
                Action = "SkinsTab",
                Label = "Cкины",
                InitialPage = new Page()
                {
                    Canvas = CanvasSkinsTab(),
                    Style = new Style()
                    {
                        Space = 0,
                        Canvas = new GridStyle()
                        {
                            Height = 275
                        }
                    },
                    Buttons = [],
                }
            };
        }

        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "Респаун",
                Tabs = [
                    RespTab(p),
                    SkinsTab(p)
                ]
            };
        }
    }
}
