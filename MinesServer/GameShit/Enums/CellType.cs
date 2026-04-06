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
        GraySand = 64,
        DarkGraySand = 65,
        GrayAcid = 66,
        PurpleAcid = 67,
        Pearl = 68,
        BlueAcid = 69,
        LavaBoulder = 70,
        XGreen = 71,
        XBlue = 72,
        XRed = 73,
        XCyan = 74,
        XViolet = 75,
        Obsidian = 76,
        Coralite = 77,
        EtherealRock = 78,
        Ultralit = 79,
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
        BlueRock = 113,
        BlackRock = 114,
        AliveBlackRock = 115,
        AliveBlue = 116,
        RedRock = 117,
        AcidRock = 118,
        HypnoRock = 119,
        GoldenRock = 120,
        GreenRock = 121,
        GRock = 122
    }
    // Методы расширения для перечисления CellType
    public static class CellTypeExtensions
    {
        /// <summary>
        /// Является ли квадро блоком
        /// </summary>
        public static bool IsQuadBlock(this CellType cell)
        {
            return cell switch
            {
                CellType.QuadBlock => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли простым блоком
        /// </summary>
        public static bool IsLightBlock(this CellType cell)
        {
            return cell switch
            {
                CellType.GreenBlock or CellType.YellowBlock or CellType.RedBlock or
                CellType.MilitaryBlockFrame or CellType.MilitaryBlock or
                CellType.Support => true,
                _ => false
            };
        }

        public static bool IsBuildingBlock(this CellType cell)
            => cell.IsLightBlock() || cell.IsQuadBlock();

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
                CellType.LavaBoulder or CellType.MetalBoulder2 or CellType.MetalBoulder3 or
                CellType.LavaBoulder => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка обычным песком
        /// </summary>
        public static bool IsLightSand(this CellType cell)
        {
            return cell switch
            {
                CellType.WhiteSand or CellType.DarkWhiteSand or
                CellType.BlueSand or CellType.DarkBlueSand or
                CellType.YellowSand or CellType.DarkYellowSand => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка металическим песком
        /// </summary>
        public static bool IsMetalicSand(this CellType cell)
        {
            return cell switch
            {
                CellType.RustySand or CellType.DarkRustySand or
                CellType.GraySand or CellType.DarkGraySand => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка песком
        /// </summary>
        public static bool IsSand(this CellType cell)
        {
            return cell.IsLightSand() || cell.IsMetalicSand();
        }

        /// <summary>
        /// Является ли ячейка кислотой
        /// </summary>
        public static bool IsAcid(this CellType cell)
        {
            return cell switch
            {
                CellType.GrayAcid or CellType.PurpleAcid or CellType.BlueAcid or
                CellType.PassiveAcid or CellType.LivingActiveAcid or
                CellType.CorrosiveActiveAcid or CellType.Pearl => true,
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
        /// Является ли ячейка легкой породой
        /// </summary>
        public static bool IsLightRock(this CellType cell)
        {
            return cell switch
            {
                CellType.Rock or CellType.BlueRock or 
                CellType.GoldenRock or CellType.GreenRock or 
                CellType.GRock => true,
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
                CellType.Obsidian or CellType.Coralite or
                CellType.EtherealRock or CellType.Ultralit => true,
                _ => false
            };
        }

        /// <summary>
        /// Является ли ячейка породой
        /// </summary>
        public static bool IsRock(this CellType cell)
        {
            return cell.IsLightRock() || cell.IsHeavyRock();
        }

        // Перегрузки для работы с byte, если нужна обратная совместимость
        public static bool IsBuildingBlock(byte cell) => ((CellType)cell).IsBuildingBlock();
        public static bool IsAlive(byte cell) => ((CellType)cell).IsAlive();
        public static bool IsRoad(byte cell) => ((CellType)cell).IsRoad();
        public static bool IsCry(byte cell) => ((CellType)cell).IsCry();
    }
}
