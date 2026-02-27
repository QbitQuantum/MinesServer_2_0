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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                    new SkillRequirement { RequiredSkill = SkillType.MineGeneral, RequiredLevel = 5 }
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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
                DopFunc = (lvl) => lvl
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