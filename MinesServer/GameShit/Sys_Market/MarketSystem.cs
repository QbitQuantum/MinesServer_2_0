using MinesServer.GameShit.Buildings;
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
                "Телепорт", // 0
                "Респаун", // 1
                "UP", // 2
                "Маркет", // 3
                "Здание кланов", // 4
                "Плазменная бомба", // 5
                "Протонная бомба", // 6
                "Разрядная бомба", // 7
                "Кредиты", // 8
                "Ремонтный бот", // 9
                "Геопак", // 10
                "Геопак с голубой живкой", // 11
                "Геопак с красной живкой", // 12
                "Геопак с фиолетовой живкой", // 13
                "Геопак с чёрной живкой", // 14
                "Геопак с белой живкой", // 15
                "Геопак с синей живкой", // 16
                "Радар вулканов", // 17
                "Радар живок", // 18
                "Радар ботов", // 19
                "Телепортер", // 20
                "Конструкторный бот", // 21
                "Боевой генератор", // 22
                "Заряд защиты", // 23
                "Крафтер", // 24
                "Магазин бомб", // 25
                "Пушка", // 26
                "Клановые ворота", // 27
                "Дизассемблер", // 28
                "Склад", // 29
                "Магазин радаров", // 30
                "Прокачка уровня x3", // 31
                "Бесплатная прокачка", // 32
                "Ускорение копание x4", // 33
                "Гипноскал", // 34
                "Полимер", // 35
                "Нано-бот", // 36
                "Аккумулятор", // 37
                "Транслятор", // 38
                "Компрессор", // 39
                "C-190", // 40
                "Федеральное здание", // 41
                "Геопак с черноскалом", // 42
                "Геопак с красноскалом", // 43
                "Автоматизатор", // 44
                "EMI-бомба", // 45
                "Радужная живка", // 46
                "Спот-бот", // 47
                "Здание научного центра (НЦ)", // 48
                "Деньги", // 49
                "Очки перепрошивки" // 50
            };
            if (i >= 0 && names.Length > i)
            {
                return names[i];
            }
            return "";
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
