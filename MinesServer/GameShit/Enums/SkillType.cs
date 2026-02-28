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
        public Func<int, int> PriceFunc { get; set; }  // Функция цены от уровня
        public Func<int, int> OppFunc { get; set; }    // Функция ОПП от уровня
        public SkillEffectType EffectType { get; set; }
        public Func<int, float> EffectFunc { get; set; }
        public Func<int, float> CostFunc { get; set; }
        public Func<int, float> ExpFunc { get; set; }
        public Func<int, float> DopFunc { get; set; }
        public List<SkillRequirement> Requirements { get; set; }

        // Вспомогательные методы для получения значений на текущем уровне
        public int GetPrice(int level) => PriceFunc?.Invoke(level) ?? 0;
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 0.08f + (float)(Math.Log10(lvl) * (Math.Pow(lvl, 0.5) / 4)),
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Movement] = new SkillInfo
            {
                Name = "Передвижение",
                Description = "Увеличивает передвижение робота",
                LevelingHint = "Передвигаться",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnMove,
                EffectFunc = (lvl) => 70f - lvl * 0.05f > 30f ? 70f - lvl * 0.05f : 30f,
                CostFunc = (lvl) => 0f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Health] = new SkillInfo
            {
                Name = "Защита",
                Description = "Увеличивает прочность робота",
                LevelingHint = "Получать урон любого вида (от пушек, ударами, С-190)",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHealth,
                EffectFunc = (lvl) => 100 + lvl * 3f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 100 + 20 * lvl,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
            [SkillType.Induction] = new SkillInfo
            {
                Name = "Индукция",
                Description = "Увеличивает расход пушек",
                LevelingHint = "Получать урон от пушки",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => 100f + lvl * 0.2f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.AntiGun] = new SkillInfo
            {
                Name = "Защита от пушек",
                Description = "Защита от пушек",
                LevelingHint = "Получать урон от пушки",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => 
                (float)Math.Round(1f + (lvl - (float)Math.Log10(lvl) * (float)Math.Pow(lvl, 0.9) / 2f - lvl * 0.098f)) >= 92 ? 
                92 : (float)Math.Round(1f + (lvl - (float)Math.Log10(lvl) * (float)Math.Pow(lvl, 0.9) / 2f - lvl * 0.098f)),
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 0f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHealth,
                EffectFunc = (lvl) => lvl * 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnMove,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Fridge,
                        RequiredLevel = 4
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Detection,
                        RequiredLevel = 5
                    }
                }
            },

            [SkillType.AntiSlime] = new SkillInfo
            {
                Name = "Защита от слизи",
                Description = "Снижает влияние слизи",
                LevelingHint = "Копать разные виды слизи",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.MineGeneral,
                        RequiredLevel = 10
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Detection,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.MineRed] = new SkillInfo
            {
                Name = "Добыча красных",
                Description = "Увеличивает добычу красных кристаллов",
                LevelingHint = "Копать красные кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.MineWhite] = new SkillInfo
            {
                Name = "Добыча белых",
                Description = "Увеличивает добычу белых кристаллов",
                LevelingHint = "Копать белые кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                Description = "Позволяет добывать дополнительные зеленые и синие породы",
                LevelingHint = "Копать зеленые и синие кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Crystallography] = new SkillInfo
            {
                Name = "Кристаллография",
                Description = "Увеличивает скорость добычи кристаллов",
                LevelingHint = "Работать с кристаллами",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Compression,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.Discount] = new SkillInfo
            {
                Name = "Оптимизация",
                Description = "Снижает затраты на прокачку умений",
                LevelingHint = "Выполнять различные действия",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnUp,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Sort] = new SkillInfo
            {
                Name = "Сортировка",
                Description = "Позволяет извлекать дополнительные красные/фиолетовые/белые кристаллы",
                LevelingHint = "Копать красные/фиолетовые/белые кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Fridge,
                        RequiredLevel = 10
                    },
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Detection,
                        RequiredLevel = 10
                    }
                }
            },

            [SkillType.DeMagnetizing] = new SkillInfo
            {
                Name = "Размагничивание",
                Description = "Увеличивает скорость разрушение песка",
                LevelingHint = "Разрушать песок",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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

            [SkillType.Upgrade] = new SkillInfo
            {
                Name = "Экспертное обучение",
                Description = "Увелчичивает скорость прокачивания навыков",
                LevelingHint = "Прокачивать навыки",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnUp,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.ExpertMining] = new SkillInfo
            {
                Name = "Экспертная добыча",
                Description = "Сильно повышает добычу ресурсов",
                LevelingHint = "Копать кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Washing] = new SkillInfo
            {
                Name = "Промывание",
                Description = "Позволяет добывать кристаллы из песка",
                LevelingHint = "Копать песок",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
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

            [SkillType.BuildUniversal] = new SkillInfo
            {
                Name = "Универсальная стройка",
                Description = "Позволяет строить любые блоки",
                LevelingHint = "Строить различные блоки",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.BuildWar] = new SkillInfo
            {
                Name = "Военный блок",
                Description = "Позволяет строить боевые структуры",
                LevelingHint = "Строить боевые блоки",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Architecture] = new SkillInfo
            {
                Name = "Архитектура",
                Description = "Позволяет строить опоры/дорогу и квадроблоки",
                LevelingHint = "Строить опоры/дорогу и квадроблоки",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.TotalDestruction] = new SkillInfo
            {
                Name = "Тотальное разрушение",
                Description = "Увеличивает скорость разрушения пород",
                LevelingHint = "Разрушения породы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.UltraWhite] = new SkillInfo
            {
                Name = "Ультра-добыча белых",
                Description = "Сильно повышает добычу белых кристаллов",
                LevelingHint = "Копать белые кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Jewlery] = new SkillInfo
            {
                Name = "Ювелирная добыча фиолетовых",
                Description = "Сильно повышает добычу фиолетовых кристаллов",
                LevelingHint = "Копать фиолетовые кристаллы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl,
                Requirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        RequiredSkill = SkillType.Digging,
                        RequiredLevel = 21
                    }
                }
            },

            [SkillType.MineSlime] = new SkillInfo
            {
                Name = "Слизевая добыча",
                Description = "Позволяет добывать крситаллы из слизи",
                LevelingHint = "Добывать слизь",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.MineDeep] = new SkillInfo
            {
                Name = "Глубинная добыча",
                Description = "Позволяет добывать крсталлы с глубинных пород",
                LevelingHint = "Копать глубинные породы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.GluonPacking] = new SkillInfo
            {
                Name = "Глюонная упаковка",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Ходить с перегрузом выше 50%",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
            },

            [SkillType.Detection] = new SkillInfo
            {
                Name = "Обнаружение",
                Description = "Позволяет добывать кристаллы c пород",
                LevelingHint = "Копать породы",
                PriceFunc = (lvl) => lvl * 10000000,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => 1f,
                CostFunc = (lvl) => 1f,
                ExpFunc = (lvl) => 1f,
                DopFunc = (lvl) => lvl
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

        public static int GetPrice(this SkillType skill, int level)
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

        public static (string Name, string Description, string LevelingHint) GetFullInfo(this SkillType skill)
        {
            var info = skill.GetInfo();
            if (info != null)
                return (info.Name, info.Description, info.LevelingHint);
            return (skill.ToString(), "Описание отсутствует", "Неизвестно");
        }
    }
}