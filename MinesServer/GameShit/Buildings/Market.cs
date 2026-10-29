using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Crystal;
using MinesServer.GameShit.GUI.Rich;
using MinesServer.GameShit.GUI.Styles;
using MinesServer.GameShit.SysMarket;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public class Market : PackDamage
    {
        private Market() { }
        public Market(int ownerid, int x, int y) : base(ownerid, x, y, 1000)
        {
            using var db = new DataBase();
            hp = 100;
            db.markets.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Market;
        [NotMapped] public override int PackId => 3;
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }
        public long moneyinside { get; set; }

        #endregion;
        
        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            for (int xx = -2; xx < 3; xx++)
            {
                for (int yy = -2; yy < 3; yy++)
                {
                    int px = x + xx, py = y + yy;
                    World.SetCell(px, py, 32, false);
                }
            }
            World.SetCell(x + 2, y + 2, 32, false);
            World.SetCell(x - 2, y + 2, 32, false);
            World.SetCell(x + 2, y - 2, 32, false);
            World.SetCell(x - 2, y - 2, 32, false);
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.markets.Remove(this);
            db.SaveChanges();

            TryToReturnInventory(p);
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            for (int xx = -2; xx < 3; xx++)
            {
                for (int yy = -2; yy < 3; yy++)
                {
                    int px = x + xx, py = y + yy;
                    if (px == x || py == y)
                    {
                        World.SetCell(px, py, 37, true);
                        continue;
                    }
                    World.SetCell(px, py, 106, true);
                }
            }
            World.SetCell(x + 2, y + 2, 38, true);
            World.SetCell(x - 2, y + 2, 38, true);
            World.SetCell(x + 2, y - 2, 38, true);
            World.SetCell(x - 2, y - 2, 38, true);
            base.Build();
        }
        #endregion

        private static Action<Player, Market> AdminPanel => (p, m) =>
        {
            if (p.id != m.ownerid || p.win == null) return;

            p.win.ShowTabs = false;
            p.win.CurrentTab.Open(new Page()
            {
                Text = " ",
                RichList = new RichListConfig()
                {
                    Entries = [
                        RichListEntry.Text("hp", $"hp {m.hp}"),
                            RichListEntry.Button($"прибыль {m.moneyinside}$",
                            m.moneyinside == 0 ?
                                new MButton() :
                                new MButton("Получить", "getprofit", (args) =>
                                {
                                    using var db = new DataBase();
                                    p.money += m.moneyinside;
                                    m.moneyinside = 0;
                                    p.SendMoney();
                                    db.SaveChanges();
                                    AdminPanel(p, m);
                                    p.SendWindow();
                                })),
                        ]
                },
                Buttons = []
            });
            p.SendWindow();
        };

        private Page SellPage(Player p, long money = -1)
        {
            Action? adminaction = p.id != ownerid ? null : () => AdminPanel(p, this);

            var InitialPage = new Page()
            {
                OnAdmin = adminaction,
                CrystalConfig = new CrystalConfig(" ", "цена", [
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0)}$</color>", 0, 0, p.crys[CrystalType.Green], 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1)}$</color>", 0, 0, p.crys[CrystalType.Blue], 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2)}$</color>", 0, 0, p.crys[CrystalType.Red], 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3)}$</color>", 0, 0, p.crys[CrystalType.Violet], 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4)}$</color>", 0, 0, p.crys[CrystalType.White], 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5)}$</color>", 0, 0, p.crys[CrystalType.Cyan], 0)
                ]),
                Text = "Продажа кристаллов" + (money == -1 ? "" : $"\nПродано кристалов на <color=#aaeeaa>{money}$</color>"),
                Buttons = [new MButton("Продать всё", $"sellallcrys", (args) => SellAllCrys(p)),
                        new MButton("Продать", $"sellcrys:{ActionMacros.CrystalSliders}", (args) => Sell(args.CrystalSliders, p))]
            };
            return InitialPage;
        }

        private void SellAllCrys(Player p)
        {
            long[] allCrys = new long[6];
            for (int i = 0; i < 6; i++)
            {
                allCrys[i] = p.crys[CrystalTypeExt.CrysType[i]];
            }
            Sell(allCrys, p);
        }

        private void Sell(long[]? Sliders, Player p)
        {
            if (Sliders == null) return;

            long money = 0;

            // 1. Изменяем данные в памяти
            for (int i = 0; i < 6; i++)
            {
                var value = Sliders[i];
                if (value > 0)
                {
                    if (p.crys.RemoveCrys(CrystalTypeExt.CrysType[i], value))
                        money += value * World.GetCrysCost(i);
                }
            }

            moneyinside += (long)(money * 0.1);
            p.money += money;

            using var db = new DataBase();

            var playerEntry = db.players.Find(p.id);
            if (playerEntry != null)
                playerEntry.money = p.money;
            p.crys.SaveToDatabase();
            db.SaveChanges();

            p.SendMoney();
            var page = SellPage(p, money);
            p.win?.CurrentTab.SetInitialPage(page);
            p.SendWindow();
        }

        private Page BuyPage(Player p, long money = -1)
        {
            Action? adminaction = p.id != ownerid ? null : () => AdminPanel(p, this);

            var InitialPage = new Page()
            {
                OnAdmin = adminaction,
                CrystalConfig = new CrystalConfig(" ", "цена", [
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(0) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(1) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(2) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(3) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(4) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(5) * 10)), 0)
                ]),
                Text = "Покупка кристаллов" + (money == -1 ?  "": $"\nКуплено кристалов на <color=#aaeeaa>{money}$</color>"),
                Buttons = [new MButton("Покупка", $"buycrys:{ActionMacros.CrystalSliders}", (args) => Buy(args.CrystalSliders, p))]
            };
            return InitialPage;
        }

        private void Buy(long[]? Sliders, Player p)
        {
            if (Sliders == null) return;

            long money = 0;
            using var db = new DataBase();
            db.players.Attach(p);
            for (int i = 0; i < 6; i++)
            {
                if (Sliders[i] <= 0 || p.money - (Sliders[i] * World.GetCrysCost(i) * 10) < 0)
                    continue;
                money -= Sliders[i] * (World.GetCrysCost(i) * 10);
                p.crys.AddCrys(CrystalTypeExt.CrysType[i], Sliders[i]);
            }
            p.money += money;
            p.crys.SaveToDatabase();
            db.SaveChanges();
            p.SendMoney();
            var page = BuyPage(p, -money);
            p.win?.CurrentTab.SetInitialPage(page);
            p.SendWindow();
        }

        private Tab TabCellCry(Player p)
        {
            return new Tab()
            {
                Label = "Продажа",
                Action = "sellcrys",
                InitialPage = SellPage(p)
            };
        }

        private Tab TabBuyCry(Player p)
        {
            Action? adminaction = p.id != ownerid ? null : () => AdminPanel(p, this);
            return new Tab()
            {
                Label = "Покупка",
                Action = "buycrys",
                InitialPage = BuyPage(p)
            };
        }

        private Tab TabProducts(Player p)
        {
            Action? adminaction = p.id != ownerid ? null : () => AdminPanel(p, this);
            return new Tab()
            {
                Label = "Товары",
                Action = "buyproducts",
                InitialPage = new Page()
                {
                    OnInventory = (int type) => { MarketSystem.OpenItemAuc(p, type); },
                    Inventory = MarketSystem.Items(),
                    Buttons = [],
                    Style = new Style
                    {
                        Inventory = new GridStyle
                        {
                            CellHeight = 75,
                        },
                    }

                }
            };
        }
        private Tab TabAuction(Player p)
        {
            RichCard[] Card = [
                new RichCard(
                    new MButton("<color=green>Да</color>", $"yes_remont"), 
                    "1111111111111111111111111111111111111",
                    "inner:SKIN:0",
                    1, 1
                )
            ];

            return new Tab()
            {
                Action = "gun",
                Label = "хуй",
                Title = "Пушка",
                InitialPage = new Page()
                {
                    RichList = new RichListConfig()
                    {
                        Entries =
                            [
                                RichListEntry.CardLine(Card)
                            ]
                    },
                    Buttons = []
                }
            };
        }

        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "МАРКЕТ",
                Tabs = [TabCellCry(p), TabBuyCry(p), TabProducts(p), TabAuction(p)]
            };
        }
    }
}