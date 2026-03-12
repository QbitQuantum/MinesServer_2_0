
namespace MinesServer.Enums
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

    public static class ItemTypeExt
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
    }
}