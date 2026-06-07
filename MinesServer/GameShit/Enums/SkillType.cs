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
    public class SkillConflict
    {
        /// <summary>
        /// Навык, который конфликтует
        /// </summary>
        public SkillType Skill { get; set; }

        /// <summary>
        /// С кем конфликтует
        /// </summary>
        public SkillType ConflictsWith { get; set; }
    }

    public static class SkillConflicts
    {
        private static readonly List<SkillConflict> _conflicts = new()
        {
            // Разрушение
            new SkillConflict { Skill = SkillType.TotalDestruction, ConflictsWith = SkillType.Destruction},
            
            // Стройка
            new SkillConflict { Skill = SkillType.BuildUniversal, ConflictsWith = SkillType.BuildGreen},
            new SkillConflict { Skill = SkillType.BuildUniversal, ConflictsWith = SkillType.BuildYellow},
            new SkillConflict { Skill = SkillType.BuildUniversal, ConflictsWith = SkillType.BuildRed},

            new SkillConflict { Skill = SkillType.Architecture, ConflictsWith = SkillType.BuildStructure},
            new SkillConflict { Skill = SkillType.Architecture, ConflictsWith = SkillType.BuildRoad},
            new SkillConflict { Skill = SkillType.Architecture, ConflictsWith = SkillType.BuildQuadro},
            
            // Добыча
            new SkillConflict { Skill = SkillType.ExpertMining, ConflictsWith = SkillType.MineGeneral},

        };

        /// <summary>
        /// Проверяет, конфликтуют ли два навыка
        /// </summary>
        public static bool HasConflict(SkillType skill1, SkillType skill2)
        {
            return _conflicts.Any(c =>
                (c.Skill == skill1 && c.ConflictsWith == skill2) ||
                (c.Skill == skill2 && c.ConflictsWith == skill1));
        }

        /// <summary>
        /// Получает все конфликты для навыка
        /// </summary>
        public static IEnumerable<SkillType> GetConflictsFor(SkillType skill)
        {
            return _conflicts
                .Where(c => c.Skill == skill)
                .Select(c => c.ConflictsWith);
        }

        /// <summary>
        /// Проверяет, можно ли изучить навык при текущих навыках
        /// </summary>
        public static SkillType? CanLearn(
            SkillType newSkill,
            IEnumerable<SkillType> currentSkills)
        {
            foreach (var currentSkill in currentSkills)
            {
                if (HasConflict(newSkill, currentSkill))
                {
                    return currentSkill;
                }
            }
            return null;
        }
    }

    public class SkillInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LevelingHint { get; set; } = string.Empty;
        public int BasePriceOPP { get; set; }
        public int BasePriceMoney { get; set; }
        public bool IsExpertSkill { get; set; }
        public Func<int, long> PriceFunc { get; set; }  // Функция цены от уровня
        public Func<int, int> OppFunc { get; set; }    // Функция ОПП от уровня
        public SkillEffectType EffectType { get; set; }
        public Func<int, float> EffectFunc { get; set; }
        public Func<int, float> ExpFunc { get; set; }
        public Func<int, float> DurabilityFunc { get; set; }
        public Func<float, string> EffectDisplayFunc { get; set; }
        public List<SkillRequirement> Requirements { get; set; }

        // Вспомогательные методы для получения значений на текущем уровне
        public long GetPrice(int level) => PriceFunc?.Invoke(level) ?? 0;
        public int GetOpp(int level) => OppFunc?.Invoke(level) ?? 0;
        public string GetDisplayInfo(float value) => EffectDisplayFunc?.Invoke(value) ?? "";
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
                LevelingHint = "Добывать зеленые кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1} кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 998)
                        return 2507 * lvl - 1507;
                    else
                        return 2500000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.0209f * lvl + 0.179f : 0.02f * lvl + 1.125f,
                ExpFunc = (lvl) => lvl <= 1001 ? 69 * lvl - 19 : 69500,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 11 }]
            },

            [SkillType.MineBlue] = new SkillInfo
            {
                Name = "Добыча синих кристаллов",
                Description = "Увеличивает добычу синих кристаллов",
                LevelingHint = "Добывать синие кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1} кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 2859 * lvl - 1859;
                    else
                        return 2860000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.016f * lvl + 2.101f,
                ExpFunc = (lvl) => lvl <= 1001 ? 69 * lvl - 19 : 69500,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.Digging, RequiredLevel = 13 },
                    new() { RequiredSkill = SkillType.Extraction, RequiredLevel = 3 }
                ]
            },

            [SkillType.MineGeneral] = new SkillInfo
            {
                Name = "Добыча",
                Description = "Увеличивает добычу кристаллов",
                LevelingHint = "Добывать кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1} кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 176)
                        return 45856 * lvl - 45606;
                    else
                        return 25000 * lvl + 3625000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 250 ? 0.0337f * lvl + 0.166f : 0.015f * lvl + 4.85f,
                ExpFunc = (lvl) => lvl <= 500 ? 130 * lvl - 120 : 50 * lvl + 39910,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 9 }]
            },

            [SkillType.Digging] = new SkillInfo
            {
                Name = "Копание",
                Description = "Позволяет быстрее разрушать кристаллы и разную породу",
                LevelingHint = "Разрушать породу и кристаллы",
                EffectDisplayFunc = (effect) => $"Скорость разрушения: <color=yellow>{effect:F0}%</color> от базовой",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 9589 * lvl - 8839;
                    else
                        return 1000 * lvl + 8580500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl,
                ExpFunc = (lvl) => lvl <= 2501 ? 32 * lvl - 27 : 80250,
                DurabilityFunc = (lvl) => lvl
            },

            [SkillType.Movement] = new SkillInfo
            {
                Name = "Передвижение",
                Description = "Увеличивает передвижение робота",
                LevelingHint = "Передвигаться",
                EffectDisplayFunc = (effect) => $"Скорость передвижения: <color=yellow>{effect:F1} м/с</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 500)
                        return 46511 * lvl - 43511;
                    else
                        return 288000 * lvl - 120788000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnMove,
                EffectFunc = (lvl) => lvl <= 400 ? -0.386f * lvl + 200.386f : 46f,
                ExpFunc = (lvl) => lvl <= 500 ? 729 * lvl - 629 : 364000,
                DurabilityFunc = (lvl) => lvl
            },

            [SkillType.Health] = new SkillInfo
            {
                Name = "Защита",
                Description = "Увеличивает прочность робота",
                LevelingHint = "Получать урон любого вида",
                EffectDisplayFunc = (effect) => $"Прочность: <color=yellow>{effect:F0}</color> ед.",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 8150 * lvl - 7750;
                    else
                        return 8150000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHealth,
                EffectFunc = (lvl) => lvl <= 500 ? 2.18f * lvl + 7.82f : lvl + 598f,
                ExpFunc = (lvl) => lvl <= 1001 ? 70 * lvl - 45 : 69750,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 4 }]
            },

            [SkillType.BuildGreen] = new SkillInfo
            {
                Name = "Стройка зеленых блоков",
                Description = "Позволяет строить зеленые постройки",
                LevelingHint = "Устанавливать зеленые блоки",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки блока: <color=green>{effect:F1} зеленых кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 8150 * lvl - 7750;
                    else
                        return 8150000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.0861f * lvl + 0.9139f : 87f,
                ExpFunc = (lvl) => lvl <= 1000 ? 42 * lvl - 32 : 10 * lvl + 31625,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 7 }]
            },

            [SkillType.BuildYellow] = new SkillInfo
            {
                Name = "Стройка желтых блоков",
                Description = "Позволяет строить желтые постройки",
                LevelingHint = "Устанавливать желтые блоки",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки блока: <color=yellow>{effect:F1} фиолетовых кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 11000 * lvl - 10600;
                    else
                        return 11000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.113f * lvl + 0.887f : 114f,
                ExpFunc = (lvl) => lvl <= 1000 ? 42 * lvl - 32 : 10 * lvl + 31625,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 6 }]
            },

            [SkillType.BuildRed] = new SkillInfo
            {
                Name = "Стройка красных блоков",
                Description = "Позволяет строить красные постройки",
                LevelingHint = "Устанавливать красные блоки",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки блока: <color=red>{effect:F1} красных кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 15000 * lvl - 14600;
                    else
                        return 15000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.1892f * lvl + 0.8108f : 190f,
                ExpFunc = (lvl) => lvl <= 1000 ? 42 * lvl - 32 : 10 * lvl + 31625,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.Packing, RequiredLevel = 15 },
                    new() { RequiredSkill = SkillType.BuildYellow, RequiredLevel = 6 }
                ]
            },

            [SkillType.Packing] = new SkillInfo
            {
                Name = "Вместимость",
                Description = "В хранилище влезает больше ресурсов",
                LevelingHint = "Передвигаться с грузом выше 50%",
                EffectDisplayFunc = (effect) => $"Вместимость хранилища: <color=yellow>+{effect:F0}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 501)
                        return 510 * lvl - 410;
                    else
                        return 255000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1 ? 50f : 50f * lvl,
                ExpFunc = (lvl) => lvl <= 1001 ? 40 * lvl - 30 : 40000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 3 }]
            },

            [SkillType.PackingBlue] = new SkillInfo
            {
                Name = "Упаковка синих",
                Description = "Синие кристаллы занимают меньше места",
                LevelingHint = "Добывать синие кристаллы с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Эффективность упаковки: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 7852 * lvl - 6652;
                    else
                        return 3000 * lvl + 4845300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.976f * lvl + 83.524f : 0.875f * lvl + 3185f,
                ExpFunc = (lvl) => lvl <= 1000 ? 51 * lvl - 1 : 10 * lvl + 41150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 5 }]
            },

            [SkillType.PackingCyan] = new SkillInfo
            {
                Name = "Упаковка голубых",
                Description = "Голубые кристаллы занимают меньше места",
                LevelingHint = "Добывать голубые кристаллы с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Эффективность упаковки: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 7852 * lvl - 6652;
                    else
                        return 3000 * lvl + 4845300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 2.272f * lvl + 47.728f : 0.5f * lvl + 1820f,
                ExpFunc = (lvl) => lvl <= 1000 ? 51 * lvl - 1 : 10 * lvl + 41150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 13 }]
            },

            [SkillType.PackingGreen] = new SkillInfo
            {
                Name = "Упаковка зеленых",
                Description = "Зеленые кристаллы занимают меньше места",
                LevelingHint = "Добывать зеленые кристаллы с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Эффективность упаковки: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 7852 * lvl - 6652;
                    else
                        return 3000 * lvl + 4845300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.408f * lvl + 71.592f : 0.75f * lvl + 2730f,
                ExpFunc = (lvl) => lvl <= 1000 ? 51 * lvl - 1 : 10 * lvl + 41150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 3 }]
            },

            [SkillType.PackingRed] = new SkillInfo
            {
                Name = "Упаковка красных",
                Description = "Красные кристаллы занимают меньше места",
                LevelingHint = "Добывать красные кристаллы с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Эффективность упаковки: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 7852 * lvl - 6652;
                    else
                        return 3000 * lvl + 4845300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.408f * lvl + 71.592f : 0.75f * lvl + 2730f,
                ExpFunc = (lvl) => lvl <= 1000 ? 51 * lvl - 1 : 10 * lvl + 41150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 7 }]
            },

            [SkillType.PackingViolet] = new SkillInfo
            {
                Name = "Упаковка фиолетовых",
                Description = "Фиолетовые кристаллы занимают меньше места",
                LevelingHint = "Добывать фиолетовые кристаллы с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Эффективность упаковки: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 7852 * lvl - 6652;
                    else
                        return 3000 * lvl + 4845300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 3.408f * lvl + 71.592f : 0.75f * lvl + 2730f,
                ExpFunc = (lvl) => lvl <= 1000 ? 51 * lvl - 1 : 10 * lvl + 41150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 11 }]
            },

            [SkillType.PackingWhite] = new SkillInfo
            {
                Name = "Упаковка белых",
                Description = "Белые кристаллы занимают меньше места",
                LevelingHint = "Добывать белые кристаллы с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Эффективность упаковки: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 7852 * lvl - 6652;
                    else
                        return 3000 * lvl + 4845300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 2.84f * lvl + 59.66f : 0.625f * lvl + 2275f,
                ExpFunc = (lvl) => lvl <= 1000 ? 51 * lvl - 1 : 10 * lvl + 41150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 9 }]
            },

            [SkillType.AntiGun] = new SkillInfo
            {
                Name = "Защита от пушек",
                Description = "Уменьшает входящий урон от пушек",
                LevelingHint = "Получать урон от пушки",
                EffectDisplayFunc = (effect) => $"Снижение урона: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 9766 * lvl - 9516;
                    else
                        return 9756000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => lvl <= 520 ? 0.1753f * lvl + 0.825f : 92f,
                ExpFunc = (lvl) => lvl <= 521 ? 143 * lvl - 93 : 74650,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Health, RequiredLevel = 12 }]
            },

            [SkillType.Repair] = new SkillInfo
            {
                Name = "Ремонт",
                Description = "Восстанавливает прочность робота",
                LevelingHint = "Чинить робота [V]",
                EffectDisplayFunc = (effect) => $"Восстановление: <color=red>{effect:F2}</color> ед./кристалл",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 2000)
                        return 5002 * lvl - 4752;
                    else
                        return 10000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHealth,
                EffectFunc = (lvl) => lvl <= 2000 ? 0.00825f * lvl + 0.992f : 0.0025f * lvl + 12.5f,
                ExpFunc = (lvl) => lvl <= 2000 ? 10 * lvl + 5 : 20000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Health, RequiredLevel = 12 }]
            },

            [SkillType.RoadMovement] = new SkillInfo
            {
                Name = "Передвижение по дорогам",
                Description = "По дорогам робот бегает быстрее",
                LevelingHint = "Передвигаться по дорогам",
                EffectDisplayFunc = (effect) => $"Бонус скорости: <color=yellow>+{effect:F2}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 8008 * lvl - 7758;
                    else
                        return 2000 * lvl + 6000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnMove,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.000933f * lvl + 0.303f : 0.00015f * lvl + 1.087f,
                ExpFunc = (lvl) => lvl <= 1000 ? 78 * lvl - 28 : 25 * lvl + 52625,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.Fridge, RequiredLevel = 4 },
                    new() { RequiredSkill = SkillType.Movement, RequiredLevel = 5 }
                ]
            },

            [SkillType.AntiSlime] = new SkillInfo
            {
                Name = "Защита от слизи",
                Description = "Снижает входящий урон от копание слизи",
                LevelingHint = "Добывать разные виды слизи",
                EffectDisplayFunc = (effect) => $"Снижение урона: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1112)
                        return 4500 * lvl - 4450;
                    else
                        return 5000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => lvl <= 167 ? 0.59f * lvl + 0.41f : 99f,
                ExpFunc = (lvl) => lvl <= 1113 ? 36 * lvl - 26 : 40000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Health, RequiredLevel = 5 }]
            },

            [SkillType.AntiBlock] = new SkillInfo
            {
                Name = "Анти-блок",
                Description = "Помогает быстрее разрушать квадроблоки",
                LevelingHint = "Разрушать квадроблоки",
                EffectDisplayFunc = (effect) => $"Скорость разрушения: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return 715 * lvl - 315;
                    else
                        return 2500 * lvl - 178800;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 183 ? 0.533f * lvl + 1.467f : 99f,
                ExpFunc = (lvl) => lvl <= 183 ? (int)(0.533 * lvl + 1.467) : 99,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Deconstruction, RequiredLevel = 5 }]
            },

            [SkillType.AdjacentExtraction] = new SkillInfo
            {
                Name = "Смежное извлечение",
                Description = "Позволяет извлекать зеленые крсталлы из синих и наоборот",
                LevelingHint = "Добывать зеленые и синие кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>{effect:F2}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 150)
                        return 29765 * lvl - 4765;
                    else
                        return 5000 * lvl + 3710000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.00951f * lvl + 0.49f : 0.0025f * lvl + 7.5f,
                ExpFunc = (lvl) => lvl <= 70 ? 1154 * lvl - 754 : 80000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Extraction, RequiredLevel = 10 }]
            },

            [SkillType.Geology] = new SkillInfo
            {
                Name = "Геология",
                Description = "Увеличивает количество переносимых пород/кристаллов",
                LevelingHint = "Переносить породы/кристаллы/слизь [G]",
                EffectDisplayFunc = (effect) => $"Вместимость: <color=yellow>{effect:F0}</color> ед.",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return 3558 * lvl - 3308;
                    else
                        return 260000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => lvl <= 1 ? 1f : lvl,
                ExpFunc = (lvl) => lvl <= 74 ? 164 * lvl - 144 : 12000,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.MineGeneral, RequiredLevel = 10 },
                    new() { RequiredSkill = SkillType.Packing, RequiredLevel = 10 }
                ]
            },

            [SkillType.MineRed] = new SkillInfo
            {
                Name = "Добыча красных",
                Description = "Увеличивает добычу красных кристаллов",
                LevelingHint = "Добывать красные кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3070 * lvl - 2070;
                    else
                        return 3067500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.013f * lvl + 5.1f,
                ExpFunc = (lvl) => lvl <= 1001 ? 69 * lvl - 19 : 69500,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 15 }]
            },

            [SkillType.MineCyan] = new SkillInfo
            {
                Name = "Добыча голубых",
                Description = "Увеличивает добычу голубых кристаллов",
                LevelingHint = "Добывать голубые кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 4686 * lvl - 3686;
                    else
                        return 4682750;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.008f * lvl + 10.099f,
                ExpFunc = (lvl) => lvl <= 1001 ? 69 * lvl - 19 : 69500,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 25 }]
            },

            [SkillType.MineViolet] = new SkillInfo
            {
                Name = "Добыча фиолетовых",
                Description = "Увеличивает добычу фиолетовых кристаллов",
                LevelingHint = "Добывать фиолетовые кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3531 * lvl - 2531;
                    else
                        return 3528500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.013f * lvl + 5.1f,
                ExpFunc = (lvl) => lvl <= 1001 ? 69 * lvl - 19 : 69500,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 21 }]
            },

            [SkillType.MineWhite] = new SkillInfo
            {
                Name = "Добыча белых",
                Description = "Увеличивает добычу белых кристаллов",
                LevelingHint = "Добывать белые кристаллы",
                EffectDisplayFunc = (effect) => $"Количество дополнительных кристаллов: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3911 * lvl - 2911;
                    else
                        return 3908250;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1001 ? 0.0179f * lvl + 0.182f : 0.008f * lvl + 10.099f,
                ExpFunc = (lvl) => lvl <= 1001 ? 69 * lvl - 19 : 69500,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 18 }]
            },

            [SkillType.Extraction] = new SkillInfo
            {
                Name = "Извлечение",
                Description = "Позволяет добывать дополнительные зеленые и синие кристаллы",
                LevelingHint = "Добывать зеленые и синие кристаллы",
                EffectDisplayFunc = (effect) => $"Шанс извлечения: <color=yellow>{effect:F2}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 5717 * lvl - 3217;
                    else
                        return 3000 * lvl + 2713500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.01316f * lvl + 0.387f : 0.005f * lvl + 8.55f,
                ExpFunc = (lvl) => lvl <= 1000 ? 33 * lvl - 7 : 22 * lvl + 11434,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Detection, RequiredLevel = 6 }]
            },

            [SkillType.Crystallography] = new SkillInfo
            {
                Name = "Кристаллография",
                Description = "Увеличивает скорость добычи кристаллов",
                LevelingHint = "Разрушать кристаллы",
                EffectDisplayFunc = (effect) => $"Скорость разрушения кристалла: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return 3558 * lvl - 3308;
                    else
                        return 260000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 73 ? 1.306f * lvl + 3.694f : 99f,
                ExpFunc = (lvl) => lvl <= 74 ? 164 * lvl - 144 : 12000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 10 }]
            },

            [SkillType.Deconstruction] = new SkillInfo
            {
                Name = "Деконструкция",
                Description = "Ускоряет разрушение блоков",
                LevelingHint = "Разрушать блоки различного цвета",
                EffectDisplayFunc = (effect) => $"Скорость разрушения блоков: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return 199 * lvl + 226;
                    else
                        return 375 * lvl - 17400;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 121 ? 0.817f * lvl + 0.183f : 99f,
                ExpFunc = (lvl) => lvl <= 121 ? 40 * lvl - 25 : 10 * lvl + 3598,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 7 }]
            },

            [SkillType.Destruction] = new SkillInfo
            {
                Name = "Разрушение",
                Description = "Увеличивает эффективность разрушения пород",
                LevelingHint = "Разрушать объекты",
                EffectDisplayFunc = (effect) => $"Эффективность: <color=yellow>{effect:F0}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 89)
                        return 3011 * lvl - 2511;
                    else
                        return 5000 * lvl - 179500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 89 ? lvl : 89f,
                ExpFunc = (lvl) => lvl <= 89 ? 81 * lvl - 75 : 50 * lvl + 2640,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 5 }]
            },

            [SkillType.Annihilation] = new SkillInfo
            {
                Name = "Аннигиляция",
                Description = "Увеличивает скорость разрушение песка",
                LevelingHint = "Разрушать песок",
                EffectDisplayFunc = (effect) => $"Скорость разрушения песка: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 61)
                        return 642 * lvl - 392;
                    else
                        return 1250 * lvl - 37500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 30 ? 1.621f * lvl + 3.379f : 52f,
                ExpFunc = (lvl) => lvl <= 61 ? 11 * lvl - 9 : 20 * lvl - 546,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Digging, RequiredLevel = 2 }]
            },

            [SkillType.Deactivation] = new SkillInfo
            {
                Name = "Деактивация",
                Description = "Увеличивает скорость разрушение слизи",
                LevelingHint = "Деактивировать объекты",
                EffectDisplayFunc = (effect) => $"Скорость разрушения слизи: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return 129 * lvl + 191;
                    else
                        return 220 * lvl - 8890;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 94 ? 1.054f * lvl - 0.054f : 99f,
                ExpFunc = (lvl) => lvl <= 120 ? 22 * lvl - 7 : 12 * lvl + 1251,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Annihilation, RequiredLevel = 8 }]
            },

            [SkillType.HyperPacking] = new SkillInfo
            {
                Name = "Гиперкомпрессия",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Передвигаться с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Вместимость: <color=yellow>+{effect:F0}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 501)
                        return 3999 * lvl - 3499;
                    else
                        return 2000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 250f * lvl,
                ExpFunc = (lvl) => lvl <= 1001 ? 150 * lvl - 100 : 150150,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Compression, RequiredLevel = 10 }]
            },

            [SkillType.Sort] = new SkillInfo
            {
                Name = "Сортировка",
                Description = "Позволяет извлекать дополнительные красные/фиолетовые/белые кристаллы",
                LevelingHint = "Добывать красные/фиолетовые/белые кристаллы",
                EffectDisplayFunc = (effect) => $"Шанс извлечения: <color=yellow>{effect:F2}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 151)
                        return 386767 * lvl - 276767;
                    else
                        return 25000 * lvl + 54350000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.00951f * lvl + 0.49f : 0.0025f * lvl + 7.5f,
                ExpFunc = (lvl) => lvl <= 70 ? 1154 * lvl - 754 : 80000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Extraction, RequiredLevel = 10 }]
            },

            [SkillType.Turbo] = new SkillInfo
            {
                Name = "Турбо-охлаждение",
                Description = "Увеличивает глубину перемещения",
                LevelingHint = "Спускаться на глубину",
                EffectDisplayFunc = (effect) => $"Макс. глубина: <color=yellow>{effect:F0}</color>",
                PriceFunc = (lvl) => 160000 * lvl,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => 750f * lvl,
                ExpFunc = (lvl) => 2000 * lvl - 400,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.Fridge, RequiredLevel = 10 },
                    new() { RequiredSkill = SkillType.Movement, RequiredLevel = 10 }
                ]
            },

            [SkillType.DeMagnetizing] = new SkillInfo
            {
                Name = "Размагничивание",
                Description = "Увеличивает скорость разрушение песка",
                LevelingHint = "Разрушать песок",
                EffectDisplayFunc = (effect) => $"Скорость разрушения песка: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return 100 * lvl + 220;
                    else
                        return 175 * lvl - 7300;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 80 ? 1.241f * lvl - 0.241f : 99f,
                ExpFunc = (lvl) => lvl <= 120 ? 21 * lvl - 14 : 9 * lvl + 1414,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Annihilation, RequiredLevel = 5 }]
            },

            [SkillType.Compression] = new SkillInfo
            {
                Name = "Компрессия",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Передвигаться с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Вместимость: <color=yellow>+{effect:F0}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 501)
                        return 1011 * lvl - 761;
                    else
                        return 505505;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 100f * lvl,
                ExpFunc = (lvl) => lvl <= 1001 ? 50 * lvl - 30 : 50000,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 10 }]
            },

            [SkillType.Fridge] = new SkillInfo
            {
                Name = "Охлаждение",
                Description = "Увеличивает глубину перемещения",
                LevelingHint = "Спускаться на глубину",
                EffectDisplayFunc = (effect) => $"Макс. глубина: <color=yellow>{effect:F0}</color>",
                PriceFunc = (lvl) => 5000 * lvl,
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnExp,
                EffectFunc = (lvl) => 100f * lvl,
                ExpFunc = (lvl) => 150 * lvl - 50,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.Digging, RequiredLevel = 4 },
                    new() { RequiredSkill = SkillType.Packing, RequiredLevel = 2 }
                ]
            },

            [SkillType.Washing] = new SkillInfo
            {
                Name = "Промывание",
                Description = "Позволяет добывать кристаллы из песка",
                LevelingHint = "Разрушать песок различного цвета",
                EffectDisplayFunc = (effect) => $"Дополнительные кристаллы: <color=yellow>+{effect:F0}</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3627 * lvl - 2877;
                    else
                        return 10000 * lvl - 6375750;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 2000 ? 0.0099f * lvl + 0.1901f : 0.01f * lvl,
                ExpFunc = (lvl) => lvl <= 2000 ? 21 * lvl - 11 : 10 * lvl + 22610,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.DeMagnetizing, RequiredLevel = 10 }]
            },

            [SkillType.Fracturing] = new SkillInfo
            {
                Name = "Дробление",
                Description = "Ускоряет разрушение валунов",
                LevelingHint = "Разрушать валуны",
                EffectDisplayFunc = (effect) => $"Скорость разрушения валунов: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return 199 * lvl + 226;
                    else
                        return 375 * lvl - 17400;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 67 ? 1.485f * lvl - 0.485f : 99f,
                ExpFunc = (lvl) => lvl <= 120 ? 29 * lvl - 22 : 14 * lvl + 1800,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Destruction, RequiredLevel = 5 }]
            },

            [SkillType.NanoPacking] = new SkillInfo
            {
                Name = "Наноупаковка",
                Description = "Увеличиваем вместимость кристаллов",
                LevelingHint = "Передвигаться с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Вместимость: <color=yellow>+{effect:F0}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 250)
                        return 30120 * lvl - 27620;
                    else
                        return 30000 * lvl + 2500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 500f * lvl,
                ExpFunc = (lvl) => lvl <= 251 ? 221 * lvl + 29 : 55555,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.HyperPacking, RequiredLevel = 10 }]
            },

            [SkillType.BuildStructure] = new SkillInfo
            {
                Name = "Стройка опор",
                Description = "Позволяет строить опоры",
                LevelingHint = "Строить опорные конструкции",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки опоры: <color=green>{effect:F1} зеленых кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 250)
                        return 1856 * lvl - 1606;
                    else
                        return 462500;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 101 ? -0.0485f * lvl + 5.0485f : 0.15f,
                ExpFunc = (lvl) => lvl <= 1000 ? 42 * lvl - 32 : 10 * lvl + 31625,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 4 }]
            },

            [SkillType.BuildRoad] = new SkillInfo
            {
                Name = "Стройка дорог",
                Description = "Позволяет строить дороги",
                LevelingHint = "Строить дороги",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки дороги: <color=green>{effect:F1} зеленых кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 2921 * lvl - 2871;
                    else
                        return 2917775;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 76 ? -0.0653f * lvl + 5.0653f : 0.1f,
                ExpFunc = (lvl) => lvl <= 1000 ? 42 * lvl - 32 : 10 * lvl + 31625,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.Packing, RequiredLevel = 5 }]
            },

            [SkillType.BuildQuadro] = new SkillInfo
            {
                Name = "Стройка квадроблоков",
                Description = "Позволяет строить квадроблоки",
                LevelingHint = "Строить квадроблоки",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки квадроблока: <color=yellow>{effect:F1} белых кри.</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return 3558 * lvl - 3308;
                    else
                        return 260000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 73 ? 1.3056f * lvl + 3.6944f : 99f,
                ExpFunc = (lvl) => lvl <= 74 ? 164 * lvl - 144 : 12000,
                DurabilityFunc = (lvl) => lvl,
                Requirements =
                [
                    new() { RequiredSkill = SkillType.BuildStructure, RequiredLevel = 6 },
                    new() { RequiredSkill = SkillType.BuildRed, RequiredLevel = 6 }
                ]
            },

            [SkillType.Detection] = new SkillInfo
            {
                Name = "Обнаружение",
                Description = "Позволяет добывать кристаллы c пород",
                LevelingHint = "Разрушать породу",
                EffectDisplayFunc = (effect) => $"Дополнительные кристаллы: <color=yellow>+{effect:F0}</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3945 * lvl - 3445;
                    else
                        return 2500 * lvl + 1441250;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.0109f * lvl + 0.2891f : 0.0045f * lvl + 6.7135f,
                ExpFunc = (lvl) => lvl <= 1000 ? 24 * lvl - 17 : 7 * lvl + 16863,
                DurabilityFunc = (lvl) => lvl,
                Requirements = [new() { RequiredSkill = SkillType.MineGeneral, RequiredLevel = 3 }]
            },

            [SkillType.Induction] = new SkillInfo
            {
                Name = "Индукция",
                Description = "Пассивная перегрузка пушки.\n" +
                "Увеличивает интенсивность и боевой расход зарядов,\n" +
                "пропорционально уровню умения.\n" +
                "Чем выше уровень, тем быстрее истощаются заряд пушки.",
                LevelingHint = "Получать урон от пушки",
                EffectDisplayFunc = (effect) => $"Увеличение расхода: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 74)
                        return 1067466 * lvl - 992466;
                    else
                        return 78000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnHurt,
                EffectFunc = (lvl) => 13f * (float)Math.Sqrt(lvl),
                ExpFunc = (lvl) => lvl <= 74 ? 4595 * lvl - 4035 : 336000,
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
                LevelingHint = "Прокачивать навыки",
                EffectDisplayFunc = (effect) => $"Скидка на прокачку: <color=yellow>{effect:F1}%</color>",
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
                ExpFunc = (lvl) => lvl <= 9 ? lvl + 11 : 20,
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
                EffectDisplayFunc = (effect) => $"Бонус опыта: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1270)
                        return 1574074 * lvl + 925926;
                    else
                        return 2000000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnUp,
                EffectFunc = (lvl) => lvl <= 500 ? 0.782f * lvl + 9.218f : 0.4f * lvl + 200f,
                ExpFunc = (lvl) => lvl <= 50 ? 3 * lvl + 7 : 150,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 300
            },

            [SkillType.ExpertMining] = new SkillInfo
            {
                Name = "Экспертная добыча",
                Description = "Профессиональное добыча из кристаллических структур.\n" +
                "Значительно увеличивает выход ценных пород при добыче, позволяя эффективнее использовать кристаллы.",
                LevelingHint = "Добывать кристаллы",
                EffectDisplayFunc = (effect) => $"Бонус к добыче: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1500)
                        return 666971 * lvl - 456971;
                    else
                        return 500000 * lvl + 250000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 2000 ? 0.0324f * lvl + 0.178f : 0.01f * lvl + 45f,
                ExpFunc = (lvl) => lvl <= 190 ? 949 * lvl - 249 : 180000,
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
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки блока: <color=yellow>{effect:F1} серо-буро-малиновых кристаллов</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 149996 * lvl - 145996;
                    else
                        return 150000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.1892f * lvl + 0.8108f : 190f,
                ExpFunc = (lvl) => lvl <= 1000 ? 208 * lvl - 158 : 50 * lvl + 158125,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 200
            },

            [SkillType.BuildWar] = new SkillInfo
            {
                Name = "Военный блок",
                Description = "Доступ к закрытым разделам оборонной тактики.\n" +
                "Открывает возможность возведения военных блоков для укрепления территории.",
                LevelingHint = "Строить боевые блоки",
                EffectDisplayFunc = (effect) => $"Затраты на строительство постройки блока: <color=cyan>{effect:F1} голубых кристаллов</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 30500 * lvl + 969500;
                    else
                        return 31500000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1000 ? -0.801f * lvl + 1000.801f : 200f,
                ExpFunc = (lvl) => lvl <= 1000 ? 33 * lvl - 18 : 32750,
                DurabilityFunc = (lvl) => lvl <= 667 ? (float)(0.375375375375375 * lvl - 0.375375375375375) : 250,
                IsExpertSkill = true,
                BasePriceOPP = 100
            },

            [SkillType.Architecture] = new SkillInfo
            {
                Name = "Архитектура",
                Description = "Освоение несущих конструкций и модульного проектирования.\n" +
                "Дает возможность возводить опорные элементы, дорожные покрытия и квадроблоки.",
                LevelingHint = "Строить опоры/дорогу и квадроблоки",
                EffectDisplayFunc = (effect) => "Доступны: <color=gray>опоры</color>, <color=white>дороги</color>, <color=purple>квадроблоки</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1001)
                        return 149996 * lvl - 145996;
                    else
                        return 150000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnBld,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.1892f * lvl + 0.8108f : 190f,
                ExpFunc = (lvl) => lvl <= 1000 ? 208 * lvl - 158 : 50 * lvl + 158125,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 50
            },

            [SkillType.UltraWhite] = new SkillInfo
            {
                Name = "Ультра-добыча белых",
                Description = "Мгновенно разрушает кристаллическу оболочку кристалла,\n" +
                "позволяя ускорить добычу вырожденных кристаллов.",
                LevelingHint = "Добывать белые кристаллы",
                EffectDisplayFunc = (effect) => $"Шанс добычи полного кристалла за 1 удар: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 984)
                        return 5080112 * lvl + 1169888;
                    else
                        return 5000000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1743 ? 0.02186f * lvl + 0.363f : 38.462f,
                ExpFunc = (lvl) => lvl <= 249 ? 632 * lvl + 868 : 1000 * lvl - 90875,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 1500
            },

            [SkillType.TotalDestruction] = new SkillInfo
            {
                Name = "Тотальное разрушение",
                Description = "Применение тактики контролируемого обрушения.\n" +
                "Значительно повышает эффективность воздействия на скальные породы.",
                LevelingHint = "Разрушать породы",
                EffectDisplayFunc = (effect) => $"Эффективность разрушения: <color=yellow>{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 175)
                        return 34195 * lvl + 15805;
                    else
                        return 6000000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 174 ? 0.566f * lvl + 0.434f : 99f,
                ExpFunc = (lvl) => lvl <= 175 ? 193 * lvl - 168 : 33625,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 800
            },

            [SkillType.Jewlery] = new SkillInfo
            {
                Name = "Ювелирная добыча фиолетовых",
                Description = "Тонкая настройка инструментов для работы с фиолетовыми кристаллами.\n" +
                "Минимизирует потери при добыче, кратно увеличивая выход ресурса с одного кристалла",
                LevelingHint = "Добывать фиолетовые кристаллы",
                EffectDisplayFunc = (effect) => $"Шанс на ювелирную добычу: <color=yellow>+{effect:F1}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 98098 * lvl + 401902;
                    else
                        return 50000 * lvl + 48500000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDigCrys,
                EffectFunc = (lvl) => lvl <= 1088 ? 0.0819f * lvl + 0.918f : 90f,
                ExpFunc = (lvl) => lvl <= 500 ? 41 * lvl + 2459 : 20 * lvl + 12850,
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
                LevelingHint = "Разрушать слизь",
                EffectDisplayFunc = (effect) => $"Дополнительные кристаллы: <color=yellow>+{effect:F0}</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3944695 * lvl - 3444695;
                    else
                        return 2500000 * lvl + 1441250000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.1185f * lvl + 12.771f : 0.0825f * lvl + 48.8257f,
                ExpFunc = (lvl) => lvl <= 1000 ? 2388 * lvl - 1688 : 700 * lvl + 1686300,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 1000
            },

            [SkillType.MineDeep] = new SkillInfo
            {
                Name = "Глубинная добыча",
                Description = "Адаптация к экстремальному давлению и составу коренных пород.\n" +
                "Открывает возможность извлечения кристаллов из глубинных слоев пород скал",
                LevelingHint = "Разрушать глубинные породы",
                EffectDisplayFunc = (effect) => $"Дополнительные кристаллы: <color=yellow>+{effect:F0}</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 1000)
                        return 3944695 * lvl - 3444695;
                    else
                        return 2500000 * lvl + 1441250000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnDig,
                EffectFunc = (lvl) => lvl <= 1000 ? 0.2185f * lvl + 5.7815f : 0.09f * lvl + 134.27f,
                ExpFunc = (lvl) => lvl <= 1000 ? 2388 * lvl - 1688 : 700 * lvl + 1686300,
                DurabilityFunc = (lvl) => lvl,
                IsExpertSkill = true,
                BasePriceOPP = 3000
            },

            [SkillType.GluonPacking] = new SkillInfo
            {
                Name = "Глюонная упаковка",
                Description = "Позволяет упаковывать кристаллы в инвентаре с большей плотностью,\n" +
                "эффективно увеличивая свободный объем при переноске перегруженного контейнера",
                LevelingHint = "Передвигаться с перегрузом выше 50%",
                EffectDisplayFunc = (effect) => $"Вместимость: <color=yellow>+{effect:F0}%</color>",
                PriceFunc = (lvl) =>
                {
                    if (lvl <= 100)
                        return 59343 * lvl + 240657;
                    else
                        return 50000 * lvl + 1175000;
                },
                OppFunc = (lvl) => lvl * 68,
                EffectType = SkillEffectType.OnPackCrys,
                EffectFunc = (lvl) => 5000f * lvl,
                ExpFunc = (lvl) => lvl <= 101 ? 846 * lvl + 154 : 250 * lvl + 60375,
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
            return _skillInfos.TryGetValue(skill, out var info) ? $"<color=white>{info.Name}</color>" + "\n" + info.Description : "Описание отсутствует";
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
        public static string GetDisplayInfo(this SkillType skill, float value)
        {
            return _skillInfos.TryGetValue(skill, out var info) ? info.GetDisplayInfo(value) : "";
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