﻿using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using MinesServer.Enums;
using MoreLinq;
using System;
using System.ComponentModel.DataAnnotations;


namespace MinesServer.GameShit.SysMarket
{
    public static class MarketSystem
    {
        public static string PackName(int i)
        {
            string[] names =
            {
                "TP","Resp","UP","Market","Clans","boom","prot","raz","Cred","Rembot","geopack","CyanAlive","RedAlive","VioletAlive","BlackNigger","WhiteAlive",
                "BlueAlive","VulcRadar","AliveRadar","BotRadar","TPR","Konstr Bot","Boy gay","Zalupa Zalupa","Crafter","BoomShop","Gun","Gate","Dizz","Storage",
                "PackRadar","x3 up","freeup","mine x4","Gypno","poli","nano bot","accum","transgender","Comp","c190","Fed","NiggerRock","RedRock","AntiMage","EMO",
                "RainbowAlive","spot","NC","Money","Оперативные Порно Покемоны."
            };
            if (i >= 0 && names.Length > i)
            {
                return names[i];
            }
            return "";
        }
        public static void Buy(long[] sliders, Player p, Market m)
        {
            if (sliders == null)
            {
                return;
            }
            long money = 0;
            using var db = new DataBase();
            db.players.Attach(p);
            for (int i = 0; i < 6; i++)
            {
                if (sliders[i] <= 0 || p.money - (sliders[i] * World.GetCrysCost(i) * 10) < 0)
                    continue;
                money -= sliders[i] * (World.GetCrysCost(i) * 10);
                p.crys.AddCrys(i, sliders[i]);
            }
            p.money += money;
            db.SaveChanges();
            p.SendMoney();
            var page = new Page()
            {
                OnAdmin = (p.id != m.ownerid ? null : () => m.onadmn(p, m)),
                CrystalConfig = new CrystalConfig(" ", "цена", [
                            new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0) * 10}$</color>", 0, 0, (p.money / (World.GetCrysCost(0) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1) * 10}$</color>", 0, 0, (p.money / (World.GetCrysCost(1) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2) * 10}$</color>", 0, 0, (p.money / (World.GetCrysCost(2) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3) * 10}$</color>", 0, 0, (p.money / (World.GetCrysCost(3) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4) * 10}$</color>", 0, 0, (p.money / (World.GetCrysCost(4) * 10)), 0),
                    new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5) * 10}$</color>", 0, 0, (p.money / (World.GetCrysCost(5) * 10)), 0)

                            ], true),
                Text = $"Покупка\nКупленно кристалов на <color=#aaeeaa>{-money}$</color>",
                Buttons = [new MButton("buy", $"buy:{ActionMacros.CrystalSliders}", (args) => Buy(args.CrystalSliders, p, m))]
            };
            p.win?.CurrentTab.SetInitialPage(page);
            p.SendWindow();
        }
        public static void Sell(long[] sliders, Player p, Market m)
        {
            long money = 0;
            if (sliders != null)
            {
                using var db = new DataBase();
                db.players.Attach(p);
                for (int i = 0; i < 6; i++)
                {
                    var value = sliders[i];
                    if (p.crys.RemoveCrys(i, sliders[i]))
                        money += value * World.GetCrysCost(i);
                }
                m.moneyinside += (long)(money * 0.1);
                p.money += money;
                db.SaveChanges();
                p.SendMoney();
                var page = new Page()
                {
                    OnAdmin = (p.id != m.ownerid ? null : () => m.onadmn(p, m)),
                    CrystalConfig = new CrystalConfig(" ", "цена", [new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(0)}$</color>", 0, 0, p.crys[CrystalType.Green], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(1)}$</color>", 0, 0, p.crys[CrystalType.Blue], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(2)}$</color>", 0, 0, p.crys[CrystalType.Red], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(3)}$</color>", 0, 0, p.crys[CrystalType.Violet], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(4)}$</color>", 0, 0, p.crys[CrystalType.White], 0),
                        new CrysLine($"<color=#aaeeaa>{World.GetCrysCost(5)}$</color>", 0, 0, p.crys[CrystalType.Cyan], 0)]),
                    Text = $"Продажа кри\nПродано кристалов на <color=#aaeeaa>{money}$</color>",
                    Buttons = [new MButton("sellall", $"sellall", (args) => Sell(p.crys.cry, p, m)),
                        new MButton("sell", $"sell:{ActionMacros.CrystalSliders}", (args) => Sell(args.CrystalSliders, p, m))]
                };
                p.win?.CurrentTab.SetInitialPage(page);
                p.SendWindow();
            }

        }
        public static void CreateOrder(Player p, int type, int num, int cost)
        {
            if (p.inventory[type] < num || num <= 0)
            {
                p.win = null;
                return;
            }
            p.inventory[type] -= num;
            using var db = new DataBase();
            var order = new Order()
            {
                initiatorid = p.id,
                cost = cost,
                num = num,
                itemid = type
            };
            db.orders.Add(order);
            db.SaveChanges();
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ok",
                Text = "u just created order u can cancel it within five mins after first bet",
                Buttons = []
            });
        }
        public static void OpenOrder(Player p, int orderid)
        {
            using var db = new DataBase();
            var o = db.orders.First(i => i.id == orderid);
            Player? buyer = null;
            if (o.buyerid > 0)
            {
                buyer = db.players.First(p => p.id == o.buyerid);
            }
            var cost = buyer == null ? o.cost : o.cost + (o.cost * 0.01f);
            var timer = o.buyerid > 0 ? $"(time till ends {TimeSpan.FromMinutes(5) - (DateTime.Now - o.bettime):mm\\:ss})" : "";
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order of player {p.name} {timer}",
                Text = buyer == null ? null : $"last bet by: {buyer.name}",
                Input = new InputConfig($"minimal bet is <color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", null, false),
                Buttons = [new MButton("minimalbet","minimalbet", (args) => { var db = new DataBase(); db.Attach(o); o.Bet(p, (long)cost); db.SaveChanges(); OpenOrder(p, orderid); p.SendWindow(); }),new MButton("bet", $"bet:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var bet)) { var db = new DataBase(); db.Attach(o); o.Bet(p, bet); db.SaveChanges(); } OpenOrder(p, orderid); p.SendWindow(); })],
                Card = new Card(CardImageType.Item, o.itemid.ToString(), $"{PackName(o.itemid)} x{o.num} costs <color=#aaeeaa>{o.cost}$</color>"),
            });

        }
        public static ListEntry[] GetItems(Player p, int type)
        {
            ListEntry[] re = [];
            using var db = new DataBase();
            var list = db.orders.Where(o => o.itemid == type);
            foreach (var i in list.OrderBy(it => it.cost))
            {
                var cost = i.buyerid == 0 ? i.cost : i.cost + (i.cost * 0.01f);
                re = re.Append(new ListEntry($"{PackName(i.itemid)} x{i.num}", new MButton($"<color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", $"openorder:{i.id}", (args) => { OpenOrder(p, i.id); p.SendWindow(); }))).ToArray();
            }
            return re;
        }
        public static void OpenOrdersGui(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ItemNameOrders",
                Buttons = [],
                Card = new Card(CardImageType.Item, itemtype.ToString(), PackName(itemtype)),
                List = GetItems(p, itemtype)
            });
        }
        public static void OpenOrderCreation(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order creation {PackName(itemtype)}",
                Text = "Enter cost",
                Input = new InputConfig("cost", null, false),
                Buttons = [new MButton("createorder", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res)) OrderCreationNum(p, itemtype, res); else p.win = null; p.SendWindow(); })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), PackName(itemtype)),
            });
        }
        public static void OrderCreationNum(Player p, int itemtype, int cost)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order creation {PackName(itemtype)}",
                Text = $"{PackName(itemtype)} to sell count",
                Input = new InputConfig("num", null, false),
                Buttons = [new MButton("createorder", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res)) CreateOrder(p, itemtype, res, cost); else p.win = null; p.SendWindow(); })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), PackName(itemtype)),
            });
        }
        private static InventoryItem[] Items()
        {
            using var db = new DataBase();
            InventoryItem[] items = [];
            for (int i = 0; i < 51; i++)
            {
                if (i == 49)
                    continue;
                var c = db.orders.Where(z => z.itemid == i).OrderBy(i => i.cost).FirstOrDefault()?.cost.ToString();
                var count = db.orders.Where(order => order.itemid == i).Count();
                items = items.Append(InventoryItem.Item(i, (count > 0 ? count.ToString() : ""), (string.IsNullOrWhiteSpace(c) ? "" : c + "$"), false, InventoryTextColor.Default, InventoryTextColor.Green)).ToArray();
            }
            return items;
        }
        public static void OpenItemAuc(Player p, int item)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "Auc " + PackName(item),
                Buttons = [new MButton("Создать Ордер", "createorder", (args) => { OpenOrderCreation(p, item); p.SendWindow(); })],
                List = GetItems(p, item)
            });
        }
        private static Random r = new Random();
        public static void GenerateRandomOrders()
        {
            foreach (var i in constypes)
            {
                var num = r.Next(0, 11);
                if (num == 0)
                    continue;
                using var db = new DataBase();
                var order = new Order()
                {
                    initiatorid = 0,
                    cost = RDes.ByResultId(i).Cost() * num,
                    num = num,
                    itemid = i
                };
                db.orders.Add(order);
                db.SaveChanges();
            }
        }
        public static long Cost(this Recipie r)
        {
            long ret = 0;
            ret += r.costres?.Select(i => RDes.ByResultId(i.id).Cost() * i.num).Sum() ?? 0;
            ret += r.costcrys?.Select(i => World.GetCrysCost(i.id) * 2L * i.num).Sum() ?? 0;
            return ret;
        }
        private static int[] constypes = [5];
        public static IPage? GlobalFirstPage(Player p)
        {
            var oninventory = (int type) => { OpenItemAuc(p, type); };
            return new Page()
            {
                OnInventory = oninventory,
                Inventory = Items(),
                Title = "МАРКЕТ",
                Buttons = [],
            };
        }
    }
}
