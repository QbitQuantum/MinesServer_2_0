namespace MinesServer.GameShit.Enums
{
    public enum CellType : byte
    {
        Nothing = 0,
        Gate = 30,
        VolcanoBackground = 31,
        Empty = 32,
        BackgroundWithLightTraces = 33,
        BackgroundWithHeavyTraces = 34,
        Road = 35,
        GoldenRoad = 36,
        BuildingDoor = 37,
        BuildingBorder = 38,
        PolymerRoad = 39,
        BlackBoulder1 = 40,
        BlackBoulder2 = 41,
        BlackBoulder3 = 42,
        MetalBoulder1 = 43,
        MetalBoulder2 = 44,
        MetalBoulder3 = 45,
        QuadBlock = 48,
        Support = 49,
        AliveCyan = 50,
        AliveRed = 51,
        AliveViol = 52,
        AliveBlack = 53,
        AliveWhite = 54,
        AliveRainbow = 55,
        WhiteSand = 60,
        DarkWhiteSand = 61,
        RustySand = 62,
        DarkRustySand = 63,
        BlackSand = 64,
        DarkBlackSand = 65,
        GrayAcid = 66,
        PurpleAcid = 67,
        Pearl = 68,
        UnknownRock1 = 69,
        UnknownRock2 = 70,
        XGreen = 71,
        XBlue = 72,
        XRed = 73,
        XCyan = 74,
        XViolet = 75,
        UnknownRock3 = 76,
        UnknownRock4 = 77,
        UnknownRock5 = 78,
        UnknownRock6 = 79,
        MilitaryBlockFrame = 80,
        MilitaryBlock = 81,
        MilitaryBlockSand = 82,
        TeleportBlock = 83,
        PassiveAcid = 86,
        SuperRainbow = 87,
        Skull = 88,
        Box = 90,
        Lava = 91,
        Boulder1 = 92,
        Boulder2 = 93,
        Boulder3 = 94,
        LivingActiveAcid = 95,
        CorrosiveActiveAcid = 96,
        BlueSand = 97,
        DarkBlueSand = 98,
        YellowSand = 99,
        DarkYellowSand = 100,
        GreenBlock = 101,
        YellowBlock = 102,
        Rock = 103,
        Border = 104,
        RedBlock = 105,
        InvisibleBlock = 106,
        Green = 107,
        Red = 108,
        Blue = 109,
        Violet = 110,
        White = 111,
        Cyan = 112,
        HeavyRock = 113,
        BlackRock = 114,
        AliveBlackRock = 115,
        AliveBlue = 116,
        RedRock = 117,
        AcidRock = 118,
        HypnoRock = 119,
        GoldenRock = 120,
        DeepRock = 121,
        GRock = 122
    }
    // Методы расширения для перечисления CellType
    public static class CellTypeExtensions
    {
        public static bool IsBuildingBlock(this CellType cell)
        {
            return cell switch
            {
                CellType.GreenBlock or CellType.YellowBlock or CellType.RedBlock or
                CellType.MilitaryBlockFrame or CellType.MilitaryBlock or
                CellType.Support or CellType.QuadBlock => true,
                _ => false
            };
        }

        public static bool IsAlive(this CellType cell)
        {
            return cell switch
            {
                CellType.AliveBlue or CellType.AliveCyan or CellType.AliveRed or
                CellType.AliveBlack or CellType.AliveViol or CellType.AliveWhite or
                CellType.AliveRainbow or CellType.AliveBlackRock => true,
                _ => false
            };
        }

        public static bool IsRoad(this CellType cell)
        {
            return cell switch
            {
                CellType.Road or CellType.GoldenRoad or CellType.PolymerRoad or
                CellType.BuildingDoor => true,
                _ => false
            };
        }

        public static bool IsCry(this CellType cell)
        {
            return cell switch
            {
                CellType.XGreen or CellType.Green => true,
                CellType.XBlue or CellType.Blue => true,
                CellType.XRed or CellType.Red => true,
                CellType.XViolet or CellType.Violet => true,
                CellType.White => true,
                CellType.XCyan or CellType.Cyan => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка валуном/камнем
        /// </summary>
        public static bool IsBoulder(this CellType cell)
        {
            return cell switch
            {
                CellType.Boulder1 or CellType.Boulder2 or CellType.Boulder3 or
                CellType.BlackBoulder1 or CellType.BlackBoulder2 or CellType.BlackBoulder3 or
                CellType.MetalBoulder1 or CellType.MetalBoulder2 or CellType.MetalBoulder3 => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка песком
        /// </summary>
        public static bool IsSand(this CellType cell)
        {
            return cell switch
            {
                CellType.WhiteSand or CellType.DarkWhiteSand or
                CellType.RustySand or CellType.DarkRustySand or
                CellType.BlackSand or CellType.DarkBlackSand or
                CellType.BlueSand or CellType.DarkBlueSand or
                CellType.YellowSand or CellType.DarkYellowSand => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка кислотой
        /// </summary>
        public static bool IsAcid(this CellType cell)
        {
            return cell switch
            {
                CellType.GrayAcid or CellType.PurpleAcid or
                CellType.PassiveAcid or CellType.LivingActiveAcid or
                CellType.CorrosiveActiveAcid => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка активной кислотой (живой/коррозивной)
        /// </summary>
        public static bool IsActiveAcid(this CellType cell)
        {
            return cell switch
            {
                CellType.LivingActiveAcid or CellType.CorrosiveActiveAcid => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка обычной породой/камнем
        /// </summary>
        public static bool IsRock(this CellType cell)
        {
            return cell switch
            {
                CellType.Rock or CellType.BlackRock or CellType.RedRock or
                CellType.AcidRock or CellType.HypnoRock or CellType.GoldenRock or
                CellType.GRock or CellType.UnknownRock1 or CellType.UnknownRock2 or
                CellType.UnknownRock3 or CellType.UnknownRock4 or CellType.UnknownRock5 or
                CellType.UnknownRock6 => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка тяжелой породой
        /// </summary>
        public static bool IsHeavyRock(this CellType cell)
        {
            return cell switch
            {
                CellType.HeavyRock or CellType.DeepRock => true,
                _ => false
            };
        }

        // Перегрузки для работы с byte, если нужна обратная совместимость
        public static bool IsBuildingBlock(byte cell) => ((CellType)cell).IsBuildingBlock();
        public static bool IsAlive(byte cell) => ((CellType)cell).IsAlive();
        public static bool IsRoad(byte cell) => ((CellType)cell).IsRoad();
        public static bool IsCry(byte cell) => ((CellType)cell).IsCry();
    }
}
