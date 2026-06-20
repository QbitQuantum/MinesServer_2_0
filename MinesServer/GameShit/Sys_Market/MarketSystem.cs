using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using MinesServer.Enums;
using MoreLinq;


namespace MinesServer.GameShit.SysMarket
{
    public static class MarketSystem
    {
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
                Card = new Card(CardImageType.Item, o.itemid.ToString(), $"{ItemTypeExt.PackName(o.itemid)} x{o.num} costs <color=#aaeeaa>{o.cost}$</color>"),
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
                re = re.Append(new ListEntry($"{ItemTypeExt.PackName(i.itemid)} x{i.num}", new MButton($"<color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>", $"openorder:{i.id}", (args) => { OpenOrder(p, i.id); p.SendWindow(); }))).ToArray();
            }
            return re;
        }
        public static void OpenOrdersGui(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = "ItemNameOrders",
                Buttons = [],
                Card = new Card(CardImageType.Item, itemtype.ToString(), ItemTypeExt.PackName(itemtype)),
                List = GetItems(p, itemtype)
            });
        }
        public static void OpenOrderCreation(Player p, int itemtype)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order creation {ItemTypeExt.PackName(itemtype)}",
                Text = "Enter cost",
                Input = new InputConfig("cost", null, false),
                Buttons = [new MButton("createorder", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res)) OrderCreationNum(p, itemtype, res); else p.win = null; p.SendWindow(); })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), ItemTypeExt.PackName(itemtype)),
            });
        }
        public static void OrderCreationNum(Player p, int itemtype, int cost)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Order creation {ItemTypeExt.PackName(itemtype)}",
                Text = $"{ItemTypeExt.PackName(itemtype)} to sell count",
                Input = new InputConfig("num", null, false),
                Buttons = [new MButton("createorder", $"createorder:{ActionMacros.Input}", (args) => { if (int.TryParse(args.Input, out var res)) CreateOrder(p, itemtype, res, cost); else p.win = null; p.SendWindow(); })],
                Card = new Card(CardImageType.Item, itemtype.ToString(), ItemTypeExt.PackName(itemtype)),
            });
        }
        public static InventoryItem[] Items()
        {
            using var db = new DataBase();
            var items = new List<InventoryItem>();

            for (int i = 0; i < 51; i++)
            {
                if (i == 31) continue;  // X3
                if (i == 32) continue;  // FreeUP
                if (i == 33) continue;  // MineX4
                if (i == 49) continue;  // Деньги
                if (i == 50) continue;  // ОПП

                // Получаем минимальную цену из ордеров для этого предмета
                var minCostOrder = db.orders
                    .Where(z => z.itemid == i)
                    .OrderBy(i => i.cost)
                    .FirstOrDefault();

                string cost = minCostOrder?.cost.ToString() ?? "";

                // Получаем количество ордеров
                var ordersCount = db.orders
                    .Where(order => order.itemid == i)
                    .Count();

                string buy = "<b><color=yellow><size=9>C 134ККК</size></color></b>";
                string sell = "<color=red><size=7>Нет в продаже</size></color>";
                // Создаем предмет инвентаря
                var item = InventoryItem.Item(
                    code: i,
                    upText: buy,
                    downText: sell,
                    faint: false
                );

                items.Add(item);
            }

            return items.ToArray();
        }

        private static CanvasElement[] Canvass()
        {
            var n = new List<CanvasElement>();

            n.Add(CanvasElement.TextField("<size=20><color=white> ПОКУПКА </color></size>", originDX: -220, originDY: 250));
            n.Add(CanvasElement.Button(new MButton($"<color=yellow> Купить 1 </color>", "buy1"), originDX: 30, originDY: -85));
            n.Add(CanvasElement.Button(new MButton($"<color=yellow> Купить 10</color>", "buy10"), originDY: 28));
            n.Add(CanvasElement.Button(new MButton($"<color=#FF3333>----------</color>", "no100"), originDY: 28));
            n.Add(CanvasElement.TextField("<color=#00FF00><size=15>x 94 000 000$</size></color>", originDX: 113, originDY: -64));
            n.Add(CanvasElement.TextField("<color=#00FF00><size=15>x 943 535 350$</size></color>", originDY: 28));
            n.Add(CanvasElement.TextField("<color=#FF3333><size=15>---------</size></color>", originDY: 28));

            n.Add(CanvasElement.TextField("<size=20><color=white> ПРОДАЖА </color></size>", originDX: -143, originDY: -88));
            n.Add(CanvasElement.Button(new MButton($"<color=#FF3333>----------</color>", "no1s"), originDX: 30, originDY: -85));
            n.Add(CanvasElement.Button(new MButton($"<color=#FF3333>----------</color>", "no10s"), originDY: 28));
            n.Add(CanvasElement.Button(new MButton($"<color=#FF3333>----------</color>", "no100s"), originDY: 28));
            n.Add(CanvasElement.TextField("<color=lime><size=15>x 70 000 001$</size></color>", originDX: 113, originDY: -64));
            n.Add(CanvasElement.TextField("<color=lime><size=15>x 700 000 010$</size></color>", originDY: 28));
            n.Add(CanvasElement.TextField("<color=lime><size=15>x 980 086 011$</size></color>", originDY: 28));

            // TODO: Добавить событие, чтобы при нажатие на кнопку, именно выбранная кнопка была желтой
            n.Add(CanvasElement.Button(new MButton($"<color=yellow>[Продажа]</color>", "setsell"), originDX: 227, originDY: -38));
            n.Add(CanvasElement.Button(new MButton($"<color=white>Покупка</color>", "setbuy"), originDY: 28));
            n.Add(CanvasElement.TextField("Создание ордера:", originDX: -18, originDY: 16));

            return n.ToArray();
        }

        public static void OpenItemAuc(Player p, int item)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = ItemTypeExt.PackName(item),
                Text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n",
                Card = Card.Item((short)item, "Ордер на <color=yellow>" + ItemTypeExt.PackName(item) + "</color>\nВ инвентаре: <color=white>0</color>\n"),
                Canvas = Canvass(),
                Style = new Style()
                {
                    Canvas = new GridStyle()
                    {
                        Height = 0
                    }
                },
                Buttons = [new MButton("<color=yellow>Создать ордер</color>", "neworder",
                (args) =>
                {
                    OpenOrderCreation(p, item); 
                    p.SendWindow();
                })],
                //List = GetItems(p, item)
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
    }
}
