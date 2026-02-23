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
    public enum Item
    {
        Teleport = 0,
        Respawn = 1,
        Up = 2,
        Market = 3,
        ClanBuilding = 4,
        PlasmaBomb = 5,
        ProtonBomb = 6,
        DischargeBomb = 7,
        Credits = 8,
        RepairBot = 9,
        Geopack = 10,
        GeopackBlue = 11,
        GeopackRed = 12,
        GeopackPurple = 13,
        GeopackBlack = 14,
        GeopackWhite = 15,
        GeopackCyan = 16,
        VolcanoRadar = 17,
        GeodeRadar = 18,
        BotRadar = 19,
        Teleporter = 20,
        ConstructorBot = 21,
        CombatGenerator = 22,
        DefenseCharge = 23,
        Crafter = 24,
        BombShop = 25,
        Gun = 26,
        ClanGate = 27,
        Disassembler = 28,
        Storage = 29,
        ScanRadar = 30,
        LevelUpgradeX3 = 31,
        FreeUpgrade = 32,
        DiggingSpeedX4 = 33,
        Hypnoskal = 34,
        Polymer = 35,
        NanoBot = 36,
        Accumulator = 37,
        Translator = 38,
        Compressor = 39,
        C190 = 40,
        FederalBuilding = 41,
        GeopackBlackSkal = 42,
        GeopackRedSkal = 43,
        Automator = 44,
        EMIBomb = 45,
        GeopackRainbow = 46,
        SpotBot = 47,
        ScienceCenter = 48,
        Money = 49,
        RespecPoints = 50
    }
    public static class MarketSystem
    {
        private static readonly Dictionary<Item, string> _names = new()
        {
            [Item.Teleport] = "Телепортер",
            [Item.Respawn] = "Респаун",
            [Item.Up] = "UP",
            [Item.Market] = "Маркет",
            [Item.ClanBuilding] = "Здание кланов",
            [Item.PlasmaBomb] = "Плазменная бомба",
            [Item.ProtonBomb] = "Протонная бомба",
            [Item.DischargeBomb] = "Разрядная бомба",
            [Item.Credits] = "Кредиты",
            [Item.RepairBot] = "Ремонтный бот",
            [Item.Geopack] = "Геопак",
            [Item.GeopackBlue] = "Геопак с голубой живкой",
            [Item.GeopackRed] = "Геопак с красной живкой",
            [Item.GeopackPurple] = "Геопак с фиолетовой живкой",
            [Item.GeopackBlack] = "Геопак с чёрной живкой",
            [Item.GeopackWhite] = "Геопак с белой живкой",
            [Item.GeopackCyan] = "Геопак с синей живкой",
            [Item.VolcanoRadar] = "Радар вулканов",
            [Item.GeodeRadar] = "Радар живок",
            [Item.BotRadar] = "Радар ботов",
            [Item.Teleporter] = "Портативный телепортер",
            [Item.ConstructorBot] = "Конструкционный бот",
            [Item.CombatGenerator] = "Боевой генератор",
            [Item.DefenseCharge] = "Заряд защиты",
            [Item.Crafter] = "Крафтер",
            [Item.BombShop] = "Магазин бомб",
            [Item.Gun] = "Пушка",
            [Item.ClanGate] = "Клановые ворота",
            [Item.Disassembler] = "Дизассемблер",
            [Item.Storage] = "Склад",
            [Item.ScanRadar] = "Сканер зданий",
            [Item.LevelUpgradeX3] = "Прокачка уровня x3",
            [Item.FreeUpgrade] = "Бесплатная прокачка",
            [Item.DiggingSpeedX4] = "Ускорение копания x4",
            [Item.Hypnoskal] = "Гипноскал",
            [Item.Polymer] = "Полимер",
            [Item.NanoBot] = "Нано-бот",
            [Item.Accumulator] = "Аккумулятор",
            [Item.Translator] = "Транслятор",
            [Item.Compressor] = "Компрессор",
            [Item.C190] = "C-190",
            [Item.FederalBuilding] = "База федерации",
            [Item.GeopackBlackSkal] = "Геопак с черноскалом",
            [Item.GeopackRedSkal] = "Геопак с красноскалом",
            [Item.Automator] = "Автоматизатор",
            [Item.EMIBomb] = "EMI-бомба",
            [Item.GeopackRainbow] = "Геопак с радужной живкой",
            [Item.SpotBot] = "Спот-бот",
            [Item.ScienceCenter] = "Здание научного центра (НЦ)",
            [Item.Money] = "Деньги",
            [Item.RespecPoints] = "Очки перепрошивки"
        };
        public static string GetName(Item type) => _names[type];

        public static Item GetItemById(int id)
        {
            if (!Enum.IsDefined(typeof(Item), id))
            {
                throw new ArgumentException($"Item with ID {id} not found");
            }
            return (Item)id;
        }
        public static string PackName(int i)
        {
            return GetName(GetItemById(i));
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
        public static void OpenItemAuc(Player p, int item)
        {
            p.win?.CurrentTab.Open(new Page()
            {
                Title = PackName(item),
                Buttons = [new MButton("Создать Ордер", "createorder", (args) => { OpenOrderCreation(p, item); p.SendWindow(); })],
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
