using MinesServer.GameShit.Skills;

namespace MinesServer.Enums
{
    public enum SkillType
    {
        Unknown = -1,
        /// <summary>a | aacd | Защита от слизи</summary>
        AntiSlime,
        /// <summary>k | ablk | Анти-блок</summary>
        AntiBlock,
        /// <summary>j | adja | Смежное извлечение</summary>
        AdjacentExtraction,
        /// <summary>U | geol | Геология</summary>
        Geology,
        /// <summary>B | minb | Добыча синих</summary>
        MineBlue,
        /// <summary>G | ming | Добыча зеленых</summary>
        MineGreen,
        /// <summary>D | dest | Разрушение</summary>
        Destruction,
        /// <summary>x | anig | Аннигиляция</summary>
        Annihilation,
        /// <summary>y | crys | Кристаллография</summary>
        Crystallography,
        /// <summary>z | decn | Деконструкция</summary>
        Deconstruction,
        /// <summary>u | agun | Защита от пушек</summary>
        AntiGun,
        /// <summary>E | bldr | Стройка красных</summary>
        BuildRed,
        /// <summary>d | digg | Копание</summary>
        Digging,
        /// <summary>l | live | Защита</summary>
        Health,
        /// <summary>m | mine | Добыча</summary>
        MineGeneral,
        /// <summary>R | minr | Добыча красных</summary>
        MineRed,
        /// <summary>L | bldg | Стройка</summary>
        BuildGreen,
        /// <summary>Q | bldq | Стройка квадроблоков</summary>
        BuildQuadro,
        /// <summary>q | dete | Обнаружение</summary>
        Detection,
        /// <summary>M | moto | Передвижение</summary>
        Movement,
        /// <summary>Y | bldy | Стройка желтых</summary>
        BuildYellow,
        /// <summary>P | comp | Компрессия</summary>
        Compression,
        /// <summary>F | frig | Охлаждение</summary>
        Fridge,
        /// <summary>C | minc | Добыча голубых</summary>
        MineCyan,
        /// <summary>t | moro | Передвижение по дорогам</summary>
        RoadMovement,
        /// <summary>*U | upgr | Экспертное обучение</summary>
        Upgrade,
        /// <summary>Z | deac | Деактивация</summary>
        Deactivation,
        /// <summary>h | hcmp | Гиперкомпрессия</summary>
        HyperPacking,
        /// <summary>V | minv | Добыча фиолетовых</summary>
        MineViolet,
        /// <summary>p | pack | Вместимость</summary>
        Packing,
        /// <summary>b | pakb | Упаковка синих</summary>
        PackingBlue,
        /// <summary>c | pakc | Упаковка голубых</summary>
        PackingCyan,
        /// <summary>v | pakv | Упаковка фиолетовых</summary>
        PackingViolet,
        /// <summary>*M | mony | Оптимизация</summary>
        Discount,
        /// <summary>J | sort | Сортировка</summary>
        Sort,
        /// <summary>S | subl | Турбо-охлаждение</summary>
        Turbo,
        /// <summary>X | magn | Размагничивание</summary>
        DeMagnetizing,
        /// <summary>W | minw | Добыча белых</summary>
        MineWhite,
        /// <summary>r | pakr | Упаковка красных</summary>
        PackingRed,
        /// <summary>w | pakw | Упаковка белых</summary>
        PackingWhite,
        /// <summary>g | pakg | Упаковка зеленых</summary>
        PackingGreen,
        /// <summary>o | reco | Извлечение</summary>
        Extraction,
        /// <summary>e | repa | Ремонт</summary>
        Repair,
        /// <summary>*D | emin | Экспертная добыча</summary>
        ExpertMining,
        /// <summary>i | wash | Промывание</summary>
        Washing,
        /// <summary>f | frac | Дробление</summary>
        Fracturing,
        /// <summary>H | nano | Наноупаковка</summary>
        NanoPacking,
        /// <summary>O | opor | Стройка опор</summary>
        BuildStructure,
        /// <summary>A | road | Стройка дорог</summary>
        BuildRoad,
        /// <summary>*B | bldu | Универсальная стройка</summary>
        BuildUniversal,
        /// <summary>*L | warb | Военный блок</summary>
        BuildWar,
        /// <summary>*A | arch | Архитектура</summary>
        Architecture,
        /// <summary>*T | tods | Тотальное разрушение</summary>
        TotalDestruction,
        /// <summary>*u | ultr | Ультра-добыча белых</summary>
        UltraWhite,
        /// <summary>*J | jewl | Ювелирная добыча фиолетовых</summary>
        Jewlery,
        /// <summary>*I | indu | Индукция</summary>
        Induction,
        /// <summary>*a | acid | Слизевая добыча</summary>
        MineSlime,
        /// <summary>*d | deep | Глубинная добыча</summary>
        MineDeep,
        /// <summary>*g | gluo | Глюонная упаковка</summary>
        GluonPacking
    }
    public class SkillInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string LevelingHint { get; set; }
        public int BasePriceOPP { get; set; }
        public int BasePriceMoney { get; set; }
        public bool IsExpertSkill { get; set; }
        public Func<int, long> PriceFunc { get; set; }  // Функция цены от уровня
        public Func<int, int> OppFunc { get; set; }    // Функция ОПП от уровня
        public SkillEffectType EffectType { get; set; }
        public Func<int, float> EffectFunc { get; set; }
        public Func<int, float> ExpFunc { get; set; }
        public Func<int, float> DurabilityFunc { get; set; }
        public List<SkillRequirement> Requirements { get; set; }

        // Вспомогательные методы для получения значений на текущем уровне
        public long GetPrice(int level) => PriceFunc?.Invoke(level) ?? 0;
        public int GetOpp(int level) => OppFunc?.Invoke(level) ?? 0;
    }

    public class SkillRequirement
    {
        public SkillType RequiredSkill { get; set; }
        public int RequiredLevel { get; set; }
    }

    public static class SkillTypeExtensions
    {
        private static readonly Dictionary<SkillType, SkillInfo> _skillInfos = new()
        {
            [SkillType.MineGreen] = new SkillInfo
            {
                Name = "Добыча зеленых кристаллов",
                Description = "Увеличивает добычу зеленых кристаллов",
                LevelingHint = "Копать кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 998)
                        return (int)(2506.51955867603 * lvl - 1506.51955867603);
                    else
                        return 2500000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.0209f * lvl + 0.179f : 0.02f * lvl + 1.125f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 11
                    }
                }
            },

            [SkillType.MineBlue] = new SkillInfo
            {
                Name = "Добыча синих кристаллов",
                Description = "Увеличивает добычу синих кристаллов",
                LevelingHint = "Копать кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(2859 * lvl - 1859);
                    else
                        return 2860000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.016f * lvl + 2.101f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 13
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Extraction,
                        RequiredLevel = 3
                    }
                }
            },

            [SkillType.MineGeneral] = new SkillInfo
            {
                Name = "Добыча",
                Description = "Увеличивает добычу синих и зеленых кристаллов",
                LevelingHint = "Копать кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 176)
                        return (int)(45855.7142857143 * lvl - 45605.7142857143);
                    else
                        return (int)(25000 * lvl + 3625000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 250 ? 0.0337f * lvl + 0.166f : 0.015f * lvl + 4.85f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 9
                    }
                }
            },

            [SkillType.Digging] = new SkillInfo
            {
                Name = "Копание",
                Description = "Позволяет быстрее разрушать кристаллы и разную породу",
                LevelingHint = "Копать породу",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(9589.33933933934 * lvl - 8839.33933933934);
                    else
                        return (int)(1000 * lvl + 8580500);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 1 ? 1f : lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl
            },

            [SkillType.Movement] = new SkillInfo
            {
                Name = "Передвижение",
                Description = "Увеличивает передвижение робота",
                LevelingHint = "Передвигаться",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 500)
                        return (int)(46511.0220440882 * lvl - 43511.0220440882);
                    else
                        return (int)(288000 * lvl - 120788000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnMove,
                EffectFunc = (lvl) => lvl <= 400 ? -0.386f * lvl + 200.386f : 46f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl
            },

            [SkillType.Health] = new SkillInfo
            {
                Name = "Защита",
                Description = "Увеличивает прочность робота",
                LevelingHint = "Получать урон любого вида (от пушек, ударами, С-190)",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(8149.6 * lvl - 7749.6);
                    else
                        return 8150000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHealth,
                EffectFunc = (lvl) => lvl <= 500 ? 2.18f * lvl + 7.82f : lvl + 598f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    { 
                        RequiredSkill = SkillType.Digging, 
                        RequiredLevel = 4 
                    }
                }
            },

            // Стройка
            [SkillType.BuildGreen] = new SkillInfo
            {
                Name = "Стройка",
                Description = "Позволяет строить зеленые постройки",
                LevelingHint = "Устанавливать зеленые блоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(8149.6 * lvl - 7749.6);
                    else
                        return 8150000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => { if (lvl <= 1) { return 1; } else { return lvl; } },
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement { 
                        RequiredSkill = SkillType.Digging, 
                        RequiredLevel = 7 
                    }
                }
            },

            [SkillType.BuildYellow] = new SkillInfo
            {
                Name = "Стройка желтых",
                Description = "Позволяет строить желтые постройки",
                LevelingHint = "Устанавливать желтые блоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(10999.6 * lvl - 10599.6);
                    else
                        return 11000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => { if (lvl <= 1) { return 1; } else { return lvl; } },
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 6
                    }
                }
            },

            [SkillType.BuildRed] = new SkillInfo
            {
                Name = "Стройка красных",
                Description = "Позволяет строить красные постройки",
                LevelingHint = "Устанавливать красные блоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(14999.6 * lvl - 14599.6);
                    else
                        return 15000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,              
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => { if (lvl <= 1) { return 1; } else { return lvl; } },
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 15
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.BuildYellow,
                        RequiredLevel = 6
                    }
                }
            },

            // Упаковка
            [SkillType.Packing] = new SkillInfo
            {
                Name = "Вместимость",
                Description = "В хранилище влезает больше ресурсов",
                LevelingHint = "Передвигаться с грузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 501)
                        return (int)(509.8 * lvl - 409.8);
                    else
                        return 255000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1 ? 50f : 50f * lvl,               
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 3
                    }
                }
            },

            [SkillType.PackingBlue] = new SkillInfo
            {
                Name = "Упаковка синих",
                Description = "Синие кристаллы занимают меньше места",
                LevelingHint = "Копать синие кристаллы с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(7851.95195195195 * lvl - 6651.95195195195);
                    else
                        return (int)(3000 * lvl + 4845300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.976f * lvl + 83.524f : 0.875f * lvl + 3185f,               
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.PackingCyan] = new SkillInfo
            {
                Name = "Упаковка голубых",
                Description = "Голубые кристаллы занимают меньше места",
                LevelingHint = "Копать голубые кристаллы с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(7851.95195195195 * lvl - 6651.95195195195);
                    else
                        return (int)(3000 * lvl + 4845300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 2.272f * lvl + 47.728f : 0.5f * lvl + 1820f,               
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 13
                    }
                }
            },

            [SkillType.PackingGreen] = new SkillInfo
            {
                Name = "Упаковка зеленых",
                Description = "Зеленые кристаллы занимают меньше места",
                LevelingHint = "Копать зеленые кристаллы с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(7851.95195195195 * lvl - 6651.95195195195);
                    else
                        return (int)(3000 * lvl + 4845300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.408f * lvl + 71.592f : 0.75f * lvl + 2730f,               
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 3
                    }
                }
            },

            [SkillType.PackingRed] = new SkillInfo
            {
                Name = "Упаковка красных",
                Description = "Красные кристаллы занимают меньше места",
                LevelingHint = "Копать красные кристаллы с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(7851.95195195195 * lvl - 6651.95195195195);
                    else
                        return (int)(3000 * lvl + 4845300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.408f * lvl + 71.592f : 0.75f * lvl + 2730f,              
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 7
                    }
                }
            },

            [SkillType.PackingViolet] = new SkillInfo
            {
                Name = "Упаковка фиолетовых",
                Description = "Фиолетовые кристаллы занимают меньше места",
                LevelingHint = "Копать фиолетовые кристаллы с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(7851.95195195195 * lvl - 6651.95195195195);
                    else
                        return (int)(3000 * lvl + 4845300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.408f * lvl + 71.592f : 0.75f * lvl + 2730f,               
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 11
                    }
                }
            },

            [SkillType.PackingWhite] = new SkillInfo
            {
                Name = "Упаковка белых",
                Description = "Белые кристаллы занимают меньше места",
                LevelingHint = "Копать белые кристаллы с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(7851.95195195195 * lvl - 6651.95195195195);
                    else
                        return (int)(3000 * lvl + 4845300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 2.84f * lvl + 59.66f : 0.625f * lvl + 2275f,                
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 9
                    }
                }   
            },

            // Боевые навыки
            [SkillType.AntiGun] = new SkillInfo
            {
                Name = "Защита от пушек",
                Description = "Защита от пушек",
                LevelingHint = "Получать урон от пушки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(9765.51551551552 * lvl - 9515.51551551552);
                    else
                        return 9756000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => lvl <= 520 ? 0.1753f * lvl + 0.825f : 92f,               
                ExpFunc = (lvl) => 0f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Health,
                        RequiredLevel = 12
                    }
                }
            },

            [SkillType.Repair] = new SkillInfo
            {
                Name = "Ремонт",
                Description = "Позволяет чинить робота",
                LevelingHint = "Чинить робота",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 2000)
                        return (int)(5002.37618809405 * lvl - 4752.37618809405);
                    else
                        return 10000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHealth,
                EffectFunc = (lvl) => lvl <= 2000 ? 0.00825f * lvl + 0.992f : 0.0025f * lvl + 12.5f,               
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Health,
                        RequiredLevel = 12
                    }
                }
            },

            [SkillType.RoadMovement] = new SkillInfo
            {
                Name = "Передвижение по дорогам",
                Description = "По дорогам робот бегает быстрее",
                LevelingHint = "Передвигаться по дорогам",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(8007.75775775776 * lvl - 7757.75775775776);
                    else
                        return (int)(2000 * lvl + 6000000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnMove,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.000933f * lvl + 0.303f : 0.00015f * lvl + 1.087f,                
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Fridge,
                        RequiredLevel = 4
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Movement,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.AntiSlime] = new SkillInfo
            {
                Name = "Защита от слизи",
                Description = "Снижает влияние слизи",
                LevelingHint = "Копать разные виды слизи",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1112)
                        return (int)(4500.40504050405 * lvl - 4450.40504050405);
                    else
                        return 5000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => lvl <= 167 ? 0.59f * lvl + 0.41f : 99f,          
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Health,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.AntiBlock] = new SkillInfo
            {
                Name = "Анти-блок",
                Description = "Помогает быстрее копать квадроблоки",
                LevelingHint = "Ломать квадроблоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return (int)(715.151515151515 * lvl - 315.151515151515);
                    else
                        return (int)(2500 * lvl - 178800);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 183 ? 0.533f * lvl + 1.467f : 99f,              
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Deconstruction,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.AdjacentExtraction] = new SkillInfo
            {
                Name = "Смежное извлечение",
                Description = "Позволяет извлекать зеленые крсталлы из синих и наоборот",
                LevelingHint = "Извлекать ресурсы рядом с собой",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 150)
                        return (int)(29765.1006711409 * lvl - 4765.10067114094);
                    else
                        return (int)(5000 * lvl + 3710000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.00951f * lvl + 0.49f : 0.0025f * lvl + 7.5f,                
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Extraction,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Geology] = new SkillInfo
            {
                Name = "Геология",
                Description = "Увеличивает количество переносимых пород/кристаллов",
                LevelingHint = "Переность породы/кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return (int)(3558.21917808219 * lvl - 3308.21917808219);
                    else
                        return 260000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => lvl <= 1 ? 1f : lvl,          
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.MineGeneral,
                        RequiredLevel = 10
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.MineRed] = new SkillInfo
            {
                Name = "Добыча красных",
                Description = "Увеличивает добычу красных кристаллов",
                LevelingHint = "Копать красные кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3069.56956956957 * lvl - 2069.56956956957);
                    else
                        return 3067500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.013f * lvl + 5.1f,            
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 15
                    }
                }
            },

            [SkillType.MineCyan] = new SkillInfo
            {
                Name = "Добыча голубых",
                Description = "Увеличивает добычу голубых кристаллов",
                LevelingHint = "Копать голубые кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(4686.43643643644 * lvl - 3686.43643643644);
                    else
                        return 4682750;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.008f * lvl + 10.099f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 25
                    }
                }
            },

            [SkillType.MineViolet] = new SkillInfo
            {
                Name = "Добыча фиолетовых",
                Description = "Увеличивает добычу фиолетовых кристаллов",
                LevelingHint = "Копать фиолетовые кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3531.03103103103 * lvl - 2531.03103103103);
                    else
                        return 3528500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.013f * lvl + 5.1f,           
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 21
                    }
                }
            },

            [SkillType.MineWhite] = new SkillInfo
            {
                Name = "Добыча белых",
                Description = "Увеличивает добычу белых кристаллов",
                LevelingHint = "Копать белые кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3911.16116116116 * lvl - 2911.16116116116);
                    else
                        return 3908250;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.008f * lvl + 10.099f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 18
                    }
                }
            },

            [SkillType.Extraction] = new SkillInfo
            {
                Name = "Извлечение",
                Description = "Позволяет добывать дополнительные зеленые и синие кристаллы",
                LevelingHint = "Копать зеленые и синие кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(5716.71671671672 * lvl - 3216.71671671672);
                    else
                        return (int)(3000 * lvl + 2713500);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.01316f * lvl + 0.387f : 0.005f * lvl + 8.55f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Detection,
                        RequiredLevel = 6
                    }
                }
            },

            [SkillType.Crystallography] = new SkillInfo
            {
                Name = "Кристаллография",
                Description = "Увеличивает скорость добычи кристаллов",
                LevelingHint = "Работать с кристаллами",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return (int)(3558.21917808219 * lvl - 3308.21917808219);
                    else
                        return 260000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 73 ? 1.306f * lvl + 3.694f : 99f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Deconstruction] = new SkillInfo
            {
                Name = "Деконструкция",
                Description = "Ускоряет разрушение блоков",
                LevelingHint = "Разбирать конструкции",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return (int)(198.737373737374 * lvl + 226.262626262626);
                    else
                        return (int)(375 * lvl - 17400);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 121 ? 0.817f * lvl + 0.183f : 99f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 7
                    }
                }
            },

            [SkillType.Destruction] = new SkillInfo
            {
                Name = "Разрушение",
                Description = "Увеличивает эффективность разрушения пород",
                LevelingHint = "Разрушать объекты",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 89)
                        return (int)(3011.36363636364 * lvl - 2511.36363636364);
                    else
                        return (int)(5000 * lvl - 179500);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 89 ? lvl : 89f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.Annihilation] = new SkillInfo
            {
                Name = "Аннигиляция",
                Description = "Увеличивает скорость разрушение лавы",
                LevelingHint = "Разрушать лаву",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 61)
                        return (int)(641.666666666667 * lvl - 391.666666666667);
                    else
                        return (int)(1250 * lvl - 37500);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 30 ? 1.621f * lvl + 3.379f : 52f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 2
                    }
                }
            },

            [SkillType.Deactivation] = new SkillInfo
            {
                Name = "Деактивация",
                Description = "Увеличивает скорость разрушение слизи",
                LevelingHint = "Деактивировать объекты",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return (int)(129.191919191919 * lvl + 190.808080808081);
                    else
                        return (int)(220 * lvl - 8890);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 94 ? 1.054f * lvl - 0.054f : 99f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Annihilation,
                        RequiredLevel = 8
                    }
                }
            },

            [SkillType.HyperPacking] = new SkillInfo
            {
                Name = "Гиперкомпрессия",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Ходить с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 501)
                        return (int)(3999 * lvl - 3499);
                    else
                        return 2000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1 ? 250f : 250f * lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Compression,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Sort] = new SkillInfo
            {
                Name = "Сортировка",
                Description = "Позволяет извлекать дополнительные красные/фиолетовые/белые кристаллы",
                LevelingHint = "Копать красные/фиолетовые/белые кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 151)
                        return (int)(386766.666666667 * lvl - 276766.666666667);
                    else
                        return (int)(25000 * lvl + 54350000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.00951f * lvl + 0.49f : 0.0025f * lvl + 7.5f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Extraction,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Turbo] = new SkillInfo
            {
                Name = "Турбо-охлаждение",
                Description = "Увеличивает глубину перемещения",
                LevelingHint = "Спускаться на глубину",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1)
                        return 160000;
                    else
                        return 160000 * lvl;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => lvl <= 1 ? 750f : 750f * lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Fridge,
                        RequiredLevel = 10
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Movement,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.DeMagnetizing] = new SkillInfo
            {
                Name = "Размагничивание",
                Description = "Увеличивает скорость разрушение песка",
                LevelingHint = "Разрушать песок",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return (int)(99.7979797979798 * lvl + 220.20202020202);
                    else
                        return (int)(175 * lvl - 7300);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 80 ? 1.241f * lvl - 0.241f : 99f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Annihilation,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.Compression] = new SkillInfo
            {
                Name = "Компрессия",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Ходить с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 501)
                        return (int)(1010.51 * lvl - 760.51);
                    else
                        return 505505;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1 ? 100f : 100f * lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Fridge] = new SkillInfo
            {
                Name = "Охлаждение",
                Description = "Увеличивает глубину перемещения",
                LevelingHint = "Спускаться на глубину",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1)
                        return 5000;
                    else
                        return 5000 * lvl;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => lvl <= 1 ? 100f : 100f * lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 4
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 2
                    }
                }
            },

            [SkillType.Washing] = new SkillInfo
            {
                Name = "Промывание",
                Description = "Позволяет добывать кристаллы из песка",
                LevelingHint = "Копать песок",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3627.12712712713 * lvl - 2877.12712712713);
                    else
                        return (int)(10000 * lvl - 6375750);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.DeMagnetizing,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Fracturing] = new SkillInfo
            {
                Name = "Дробление",
                Description = "Улучшает дробление валунов",
                LevelingHint = "Копать валуны",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return (int)(198.737373737374 * lvl + 226.262626262626);
                    else
                        return (int)(375 * lvl - 17400);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 67 ? 1.485f * lvl - 0.485f : 99f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Destruction,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.NanoPacking] = new SkillInfo
            {
                Name = "Наноупаковка",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Ходить с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 250)
                        return (int)(30120.4819277108 * lvl - 27620.4819277108);
                    else
                        return (int)(30000 * lvl + 2500);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1 ? 500f : 500f * lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.HyperPacking,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.BuildStructure] = new SkillInfo
            {
                Name = "Стройка опор",
                Description = "Позволяет строить опоры",
                LevelingHint = "Строить опорные конструкции",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 250)
                        return (int)(1856.42570281124 * lvl - 1606.42570281124);
                    else
                        return 462500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 4
                    }
                }
            },

            [SkillType.BuildRoad] = new SkillInfo
            {
                Name = "Стройка дорог",
                Description = "Позволяет строить дороги",
                LevelingHint = "Строить дороги",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(2920.64564564565 * lvl - 2870.64564564565);
                    else
                        return 2917775;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Packing,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.BuildQuadro] = new SkillInfo
            {
                Name = "Стройка квадроблоков",
                Description = "Позволяет строить квадроблоки",
                LevelingHint = "Строить квадроблоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return (int)(3558.21917808219 * lvl - 3308.21917808219);
                    else
                        return 260000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.BuildStructure,
                        RequiredLevel = 6
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.BuildRed,
                        RequiredLevel = 6
                    }
                }
            },

            [SkillType.Detection] = new SkillInfo
            {
                Name = "Обнаружение",
                Description = "Позволяет добывать кристаллы c пород",
                LevelingHint = "Копать породы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3944.69469469469 * lvl - 3444.69469469469);
                    else
                        return (int)(2500 * lvl + 1441250);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.MineGeneral,
                        RequiredLevel = 3
                    }
                }
            },

            [SkillType.Induction] = new SkillInfo
            {
                Name = "Индукция",
                Description = "Пассивная перегрузка пушки.\n" +
                "Увеличивает интенсивность и боевой расход зарядов,\n" +
                "пропорционально уровню умения.\n" +
                "Чем выше уровень, тем быстрее истощаются заряд пушки.",
                LevelingHint = "Получать урон от пушки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return (int)(1067465.75342466 * lvl - 992465.753424658);
                    else
                        return 78000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => 13f * (float)Math.Sqrt(lvl),
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 900
            },

            [SkillType.Discount] = new SkillInfo
            {
                Name = "Оптимизация",
                Description = "Применение алгоритмов эффективности к процессу обучения.\n" +
                "Каждое действие, приносящее опыт, пересматривается с целью \n" +
                "снижения ресурсных затрат на последующее совершенствование навыков.",
                LevelingHint = "Выполнять различные действия",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 200)
                        return 5000000 * lvl;
                    else
                        return 1000000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnUp,
                EffectFunc = (lvl) => lvl <= 200 ? 0.201f * lvl + 9.799f : 50f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 600
            },

            [SkillType.Upgrade] = new SkillInfo
            {
                Name = "Экспертное обучение",
                Description = "Позволяет извлекать больше опыта из развития навыков,\n" +
                "ускоряя общий прогресс прокачки.",
                LevelingHint = "Прокачивать навыки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1270)
                        return (int)(1574074.07407407 * lvl + 925925.925925926);
                    else
                        return 2000000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnUp,
                EffectFunc = (lvl) => lvl <= 500 ? 0.782f * lvl + 9.218f : 0.4f * lvl + 200f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 300
            },

            [SkillType.ExpertMining] = new SkillInfo
            {
                Name = "Экспертная добыча",
                Description = "Профессиональное добыча из кристаллических структур.\n" +
                "Значительно увеличивает выход ценных пород при добыче, позволяя эффективнее использовать кристаллы.",
                LevelingHint = "Копать кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1500)
                        return (int)(666971.314209473 * lvl - 456971.314209473);
                    else
                        return (int)(500000 * lvl + 250000000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 2000 ? 0.0324f * lvl + 0.178f : 0.01f * lvl + 45f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 700
            },

            [SkillType.BuildUniversal] = new SkillInfo
            {
                Name = "Универсальная стройка",
                Description = "Объеденяет архитектурныие навыки возведения построек, \n" +
                "уменьшая количества занимаемых слотов для отдельного навыка.",
                LevelingHint = "Строить зеленые/желтые/красные блоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(149996 * lvl - 145996);
                    else
                        return 150000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => { if (lvl <= 1) { return 1; } else { return lvl; } },
                IsExpertSkill = true,
                BasePriceOPP = 200
            },

            [SkillType.BuildWar] = new SkillInfo
            {
                Name = "Военный блок",
                Description = "Доступ к закрытым разделам оборонной тактики.\n" +
                "Открывает возможность возведения военных блоков для укрепления территории.",
                LevelingHint = "Строить боевые блоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(30500 * lvl + 969500);
                    else
                        return 31500000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1000 ? -0.801f * lvl + 1000.801f : 200f, 
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => { if (lvl <= 667) { return (float)(0.375375375375375 * lvl + (-0.375375375375375)); } else { return 250; } },
                IsExpertSkill = true,
                BasePriceOPP = 100
            },

            [SkillType.Architecture] = new SkillInfo
            {
                Name = "Архитектура",
                Description = "Освоение несущих конструкций и модульного проектирования.\n" +
                "Дает возможность возводить опорные элементы, дорожные покрытия и квадроблоки.",
                LevelingHint = "Строить опоры/дорогу и квадроблоки",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return (int)(149996 * lvl - 145996);
                    else
                        return 150000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f, 
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => { if (lvl <= 1) { return 1; } else { return lvl; } },
                IsExpertSkill = true,
                BasePriceOPP = 50
            },

            [SkillType.UltraWhite] = new SkillInfo
            {
                Name = "Ультра-добыча белых",
                Description = "Мгновенно разрушает кристаллическу оболочку кристалла,\n" +
                "позволяя ускорить добычу вырожденных кристаллов.",
                LevelingHint = "Копать белые кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 984)
                        return (int)(5080111.90233978 * lvl + 1169888.09766022);
                    else
                        return 5000000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1743 ? 0.02186f * lvl + 0.363f : 38.462f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 1500
            },

            [SkillType.TotalDestruction] = new SkillInfo
            {
                Name = "Тотальное разрушение",
                Description = "Применение тактики контролируемого обрушения.\n" +
                "Значительно повышает эффективность воздействия на скальные породы.",
                LevelingHint = "Разрушения породы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 175)
                        return (int)(34195.4022988506 * lvl + 15804.5977011494);
                    else
                        return 6000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 174 ? 0.566f * lvl + 0.434f : 99f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 800
            },

            [SkillType.Jewlery] = new SkillInfo
            {
                Name = "Ювелирная добыча фиолетовых",
                Description = "Тонкая настройка инструментов для работы с фиолетовыми кристаллами.\n" +
                "Минимизирует потери при добыче, кратно увеличивая выход ресурса с одного кристалла",
                LevelingHint = "Копать фиолетовые кристаллы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(98098.0980980981 * lvl + 401901.901901902);
                    else
                        return (int)(50000 * lvl + 48500000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1088 ? 0.0819f * lvl + 0.918f : 90f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 1000
            },

            [SkillType.MineSlime] = new SkillInfo
            {
                Name = "Слизевая добыча",
                Description = "Освоение специфики вязких сред.\n" +
                "Позволяет эффективно фильтровать и извлекать кристаллические\n" +
                "включения из осадочных пород и слизистых отложений.",
                LevelingHint = "Добывать слизь",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3944694.69469469 * lvl - 3444694.69469469);
                    else
                        return (int)(2500000 * lvl + 1441250000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 1000
            },

            [SkillType.MineDeep] = new SkillInfo
            {
                Name = "Глубинная добыча",
                Description = "Адаптация к экстремальному давлению и составу коренных пород.\n" +
                "Открывает возможность извлечения кристаллов из глубинных слоев пород скал",
                LevelingHint = "Копать глубинные породы",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return (int)(3944694.69469469 * lvl - 3444694.69469469);
                    else
                        return (int)(2500000 * lvl + 1441250000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 3000
            },

            [SkillType.GluonPacking] = new SkillInfo
            {
                Name = "Глюонная упаковка",
                Description = "Позволяет упаковывать кристаллы в инвентаре с большей плотностью,\n" +
                "эффективно увеличивая свободный объем при переноске перегруженного контейнера",
                LevelingHint = "Ходить с перегрузом выше 50%",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return (int)(59343.4343434343 * lvl + 240656.565656566);
                    else
                        return (int)(50000 * lvl + 1175000);
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1 ? 5000f : 5000f * lvl,
                ExpFunc = (lvl) => 1f,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 5000
            } 
        };

        public static SkillInfo GetInfo(this SkillType skill)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info : null;
        }

        public static string GetName(this SkillType skill)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.Name : skill.ToString();
        }

        public static string GetDescription(this SkillType skill)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.Description : "Описание отсутствует";
        }

        public static string GetLevelingHint(this SkillType skill)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.LevelingHint : "Неизвестно";
        }

        public static long GetPrice(this SkillType skill, int level)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.GetPrice(level) : 0;
        }

        public static int GetOpp(this SkillType skill, int level)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.GetOpp(level) : 0;
        }
        public static Dictionary<SkillType, SkillInfo> GetAllInfos()
        {
            return _skillInfos;
        }
        public static int GetBasePriceMoney(this SkillType skill)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.BasePriceMoney : 0;
        }
        public static int GetBasePriceOPP(this SkillType skill)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.BasePriceOPP : 0;
        }

        public static (string Name, string Description, string LevelingHint) GetFullInfo(this SkillType skill)
        {
            var info = skill.GetInfo();
            if (info != null)
                return (info.Name, info.Description, info.LevelingHint);
            return (skill.ToString(), "Описание отсутствует", "Неизвестно");
        }
    }
}