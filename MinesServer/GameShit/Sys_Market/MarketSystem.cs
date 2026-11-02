using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Canvas;
using MinesServer.GameShit.GUI.Cards;
using MinesServer.GameShit.GUI.Inputs;
using MinesServer.GameShit.GUI.Inventory;
using MinesServer.GameShit.GUI.List;
using MinesServer.GameShit.GUI.Styles;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using MoreLinq;


namespace MinesServer.GameShit.SysMarket
{
    public static class L
    {
        public const string CreateOrderTitle = "Создание ордера";
        public const string Step1Title = "Шаг 1 из 2 — цена";
        public const string Step2Title = "Шаг 2 из 2 — количество";
        public const string EnterCost = "Введите цену за 1 шт. (целое число, $):";
        public const string EnterNum = "Сколько выставить на продажу?";
        public const string InInventory = "В инвентаре";
        public const string InvalidCost = "<color=#ff6666>Цена должна быть целым числом больше 0.</color>";
        public const string InvalidNum = "<color=#ff6666>Количество должно быть целым числом от 1 до {0}.</color>";
        public const string NotTradable = "<color=#ff6666>Этот предмет нельзя выставить на аукцион.</color>";
        public const string CreatedOk = "Ордер создан.\nЕго можно отменить в течение 5 минут после первой ставки.";
    }

    public static class MarketSystem
    {
        private static readonly HashSet<int> NonTradable = new() { 31, 32, 33, 49, 50 };

        public static void OpenOrderCreation(Player p, int itemtype)
        {
            if (NonTradable.Contains(itemtype))
            {
                p.win?.CurrentTab.Replace(new Page
                {
                    Title = L.CreateOrderTitle,
                    Text = L.NotTradable,
                    Buttons = []
                });
                p.SendWindow();
                return;
            }

            var amount = p.inventory[itemtype];
            if (amount <= 0)
            {
                p.win?.CurrentTab.Replace(new Page
                {
                    Title = L.CreateOrderTitle,
                    Text = $"<color=#ff6666>У вас нет {ItemTypeExt.PackName(itemtype)} в инвентаре.</color>",
                    Buttons = []
                });
                p.SendWindow();
                return;
            }

            p.win?.CurrentTab.Open(CostPage(p, itemtype, amount, error: null));
            p.SendWindow();
        }

        private static Page CostPage(Player p, int itemtype, int amount, string? error)
        {
            var name = ItemTypeExt.PackName(itemtype);
            return new Page
            {
                Title = $"{L.CreateOrderTitle}: {name}",
                Text = $"{L.Step1Title}\n\n{L.EnterCost}\n" +
                       $"{L.InInventory}: <color=white>{amount:N0}</color>" +
                       (error is null ? "" : $"\n{error}"),
                Input = new InputConfig("цена за 1 шт.", MaxLength: 12),
                Card = new Card(CardImageType.Item, itemtype.ToString(), name),
                Buttons = new[]
                {
                    new MButton("<color=yellow>Далее</color>", $"setcost:{ActionMacros.Input}",
                        args => OnCostEntered(p, itemtype, amount, args.Input))
                }
            };
        }

        private static void OnCostEntered(Player p, int itemtype, int amount, string? raw)
        {
            if (!int.TryParse(raw, out var cost) || cost <= 0)
            {
                p.win?.CurrentTab.Replace(CostPage(p, itemtype, amount, L.InvalidCost));
                p.SendWindow();
                return;
            }
            p.win?.CurrentTab.Open(NumPage(p, itemtype, cost, amount, error: null));
            p.SendWindow();
        }

        private static Page NumPage(Player p, int itemtype, int cost, int amount, string? error)
        {
            var name = ItemTypeExt.PackName(itemtype);
            return new Page
            {
                Title = $"{L.CreateOrderTitle}: {name}",
                Text = $"{L.Step2Title}\n\n{L.EnterNum}\n" +
                       $"Цена: <color=#aaeeaa>{cost:N0}$</color> за 1 шт.\n" +
                       $"{L.InInventory}: <color=white>{amount:N0}</color>" +
                       (error is null ? "" : $"\n{error}"),
                Input = new InputConfig("количество", MaxLength: 10),
                Card = new Card(CardImageType.Item, itemtype.ToString(), name),
                Buttons = new[]
                {
                    new MButton("<color=yellow>Создать</color>", $"setnum:{ActionMacros.Input}",
                        args => OnNumEntered(p, itemtype, cost, amount, args.Input))
                }
            };
        }

        private static void OnNumEntered(Player p, int itemtype, int cost, int amount, string? raw)
        {
            if (!int.TryParse(raw, out var num) || num <= 0 || num > amount)
            {
                p.win?.CurrentTab.Replace(NumPage(p, itemtype, cost, amount,
                    string.Format(L.InvalidNum, amount)));
                p.SendWindow();
                return;
            }
            CreateOrder(p, itemtype, num, cost);
        }

        public static void CreateOrder(Player p, int type, int num, int cost)
        {
            if (NonTradable.Contains(type) ||
                num <= 0 || cost <= 0 ||
                p.inventory[type] < num)
            {
                p.win = null;
                return;
            }

            p.inventory[type] -= num;

            using var db = new DataBase();
            db.orders.Add(new Order
            {
                initiatorid = p.id,
                cost = cost,
                num = num,
                itemid = type
            });
            db.SaveChanges();

            p.win?.CurrentTab.Replace(new Page
            {
                Title = $"{L.CreateOrderTitle}: {ItemTypeExt.PackName(type)}",
                Text = $"{L.CreatedOk}\n\n" +
                       $"Выставлено: <color=white>{num:N0}</color> x " +
                       $"<color=#aaeeaa>{cost:N0}$</color>",
                Buttons = new[]
                {
                    new MButton("К аукциону", "backtoauc",
                        _ => { OpenItemAuc(p, type); p.SendWindow(); })
                }
            });
            p.SendWindow();
        }

        public static void OpenOrder(Player p, int orderid)
        {
            using var db = new DataBase();
            var order = db.orders.First(i => i.id == orderid);
            Player? buyer = null;
            if (order.buyerid > 0)
            {
                buyer = db.players.First(p => p.id == order.buyerid);
            }
            var cost = buyer == null ? order.cost : order.cost + (order.cost * 0.01f);
            var timer = order.buyerid > 0
                ? $" (до конца {TimeSpan.FromMinutes(5) - (DateTime.Now - order.bettime):mm\\:ss})"
                : "";
            var sellerName = db.players.FirstOrDefault(x => x.id == order.initiatorid)?.name;

            p.win?.CurrentTab.Open(new Page()
            {
                Title = $"Ордер: {ItemTypeExt.PackName(order.itemid)} x{order.num}{timer}",
                Text = (buyer == null
                            ? "Ставок пока нет."
                            : $"Последняя ставка: <color=white>{buyer.name}</color>")
                       + (sellerName == null
                            ? ""
                            : $"\nПродавец: <color=white>{sellerName}</color>"),
                Input = new InputConfig(
                    $"минимальная ставка — <color=#aaeeaa>{(int)Math.Ceiling(cost)}$</color>",
                    null, false),
                Buttons = [
                    new MButton($"<color=yellow>Поставить {(int)Math.Ceiling(cost)}$</color>", "minimalbet",
                (args) =>
                {
                    var db = new DataBase();
                    db.Attach(order);
                    order.Bet(p, (long)cost);
                    db.SaveChanges();
                    OpenOrder(p, orderid);
                    p.SendWindow();
                }),

            new MButton("Ставка", $"bet:{ActionMacros.Input}",
                (args) =>
                {
                    if (int.TryParse(args.Input, out var bet))
                    {
                        var db = new DataBase();
                        db.Attach(order);
                        order.Bet(p, bet);
                        db.SaveChanges();
                    }
                    OpenOrder(p, orderid);
                    p.SendWindow();
                })
                ],
                Card = new Card(CardImageType.Item, order.itemid.ToString(),
                    $"{ItemTypeExt.PackName(order.itemid)} x{order.num}\n" +
                    $"Цена: <color=#aaeeaa>{order.cost:N0}$</color>"),
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