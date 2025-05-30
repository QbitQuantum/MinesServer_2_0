﻿using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.SysMarket;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using MinesServer.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinesServer.GameShit.Buildings
{
    public class Market : Pack, IDamagable
    {
        #region fields
        [NotMapped]
        public override float charge { get; set; }
        public override PackType type => PackType.Market;
        public int maxhp { get; set; }
        public int hp { get; set; }
        public long moneyinside { get; set; }
        public DateTime brokentimer { get; set; }
        #endregion;
        private Market() {}
        public Market(int ownerid, int x, int y) : base(ownerid, x, y)
        {
            using var db = new DataBase();
            hp = 100;
            db.markets.Add(this);
            db.SaveChanges();
        }
        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            for (int xx = -2; xx < 3; xx++)
            {
                for (int yy = -2; yy < 3; yy++)
                {
                    int px = x + xx, py = y + yy;
                    if (px == x || py == y)
                    {
                        World.SetCell(px, py, 32, false);
                        continue;
                    }
                    World.SetCell(px, py, 32, false);
                }
            }
            World.SetCell(x + 2, y + 2, 32, false);
            World.SetCell(x - 2, y + 2, 32, false);
            World.SetCell(x + 2, y - 2, 32, false);
            World.SetCell(x - 2, y - 2, 32, false);
        }
        public void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.markets.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[3]++;
            }
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
        public Action<Player, Market> onadmn => (p, m) =>
        {
            if (p.id == m.ownerid)
            {
                p.win.ShowTabs = false;
                p.win?.CurrentTab.Open(new Page()
                {
                    Text = " ",
                    RichList = new RichListConfig()
                    {
                        Entries = [RichListEntry.Text($"hp {m.hp}"),
                            RichListEntry.Button($"прибыль {m.moneyinside}$", m.moneyinside == 0 ? new MButton() : new MButton("Получить", "getprofit", (args) => { using var db = new DataBase(); p.money += m.moneyinside; m.moneyinside = 0; p.SendMoney(); db.SaveChanges(); m.onadmn(p, m); p.SendWindow(); })),
                        ]
                    },
                    Buttons = []

                });
                p.SendWindow();
            }
        };
        private Tab BuildSelltab(Player p)
        {
            Action adminaction = (p.id != ownerid ? null : () => onadmn(p, this));
            return new Tab()
            {
                Label = "ПРОДАЖА",
                Action = "sellcrys",
                InitialPage = new Page()
                {
                    OnAdmin = adminaction,
                    CrystalConfig = new CrystalConfig(" ", "цена",
                            [new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0)}$</color>", 0, 0, p.crys[CrystalType.Green], 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1)}$</color>", 0, 0, p.crys[CrystalType.Blue], 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2)}$</color>", 0, 0, p.crys[CrystalType.Red], 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3)}$</color>", 0, 0, p.crys[CrystalType.Violet], 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4)}$</color>", 0, 0, p.crys[CrystalType.White], 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5)}$</color>", 0, 0, p.crys[CrystalType.Cyan], 0)]
                                ),
                    Text = "Продажа кри",
                    Buttons = [new MButton("sellall", $"sellall", (args) => MarketSystem.Sell(p.crys.cry, p, this)),
                            new MButton("sell", $"sell:{ActionMacros.CrystalSliders}", (args) => MarketSystem.Sell(args.CrystalSliders, p, this))]
                }
            };
        }
        private Tab BuildBuytab(Player p)
        {
            Action adminaction = (p.id != ownerid ? null : () => onadmn(p, this));
            return new Tab()
            {
                Label = "Покупка",
                Action = "buycrys",
                InitialPage = new Page()
                {
                    OnAdmin = adminaction,
                    CrystalConfig = new CrystalConfig(" ", "цена", [
                            new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(0) * 10)), 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(1) * 10)), 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(2) * 10)), 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(3) * 10)), 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(4) * 10)), 0),
                                new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5) * 10}$</color>", 0, 0, (int)(p.money / (World.GetCrysCost(5) * 10)), 0)

                            ], true),
                    Text = "Покупка",
                    Buttons = [new MButton("buy", $"buy:{ActionMacros.CrystalSliders}", (args) => MarketSystem.Buy(args.CrystalSliders, p, this))
                        ]
                }
            };
        }
        private Tab AucTab(Player p)
        {
            return new Tab()
            {
                InitialPage = MarketSystem.GlobalFirstPage(p)!,
                Action = "auc",
                Label = "Auc"
            };
        }
        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "Market",
                Tabs = [BuildSelltab(p),BuildBuytab(p),AucTab(p)]
            };
        }
    }
}
