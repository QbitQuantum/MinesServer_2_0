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
    public static class SkillTypeExtensions
    {
        public static string GetName(this SkillType skill)
        {
            return skill switch
            {
                SkillType.AntiSlime => "Защита от слизи",
                SkillType.AntiBlock => "Анти-блок",
                SkillType.AdjacentExtraction => "Смежное извлечение",
                SkillType.Geology => "Геология",
                SkillType.MineBlue => "Добыча синих",
                SkillType.MineGreen => "Добыча зеленых",
                SkillType.Destruction => "Разрушение",
                SkillType.Annihilation => "Аннигиляция",
                SkillType.Crystallography => "Кристаллография",
                SkillType.Deconstruction => "Деконструкция",
                SkillType.AntiGun => "Защита от пушек",
                SkillType.BuildRed => "Стройка красных",
                SkillType.Digging => "Копание",
                SkillType.Health => "Защита",
                SkillType.MineGeneral => "Добыча",
                SkillType.MineRed => "Добыча красных",
                SkillType.BuildGreen => "Стройка",
                SkillType.BuildQuadro => "Стройка квадроблоков",
                SkillType.Detection => "Обнаружение",
                SkillType.Movement => "Передвижение",
                SkillType.BuildYellow => "Стройка желтых",
                SkillType.Compression => "Компрессия",
                SkillType.Fridge => "Охлаждение",
                SkillType.MineCyan => "Добыча голубых",
                SkillType.RoadMovement => "Передвижение по дорогам",
                SkillType.Upgrade => "Экспертное обучение",
                SkillType.Deactivation => "Деактивация",
                SkillType.HyperPacking => "Гиперкомпрессия",
                SkillType.MineViolet => "Добыча фиолетовых",
                SkillType.Packing => "Вместимость",
                SkillType.PackingBlue => "Упаковка синих",
                SkillType.PackingCyan => "Упаковка голубых",
                SkillType.PackingViolet => "Упаковка фиолетовых",
                SkillType.Discount => "Оптимизация",
                SkillType.Sort => "Сортировка",
                SkillType.Turbo => "Турбо-охлаждение",
                SkillType.DeMagnetizing => "Размагничивание",
                SkillType.MineWhite => "Добыча белых",
                SkillType.PackingRed => "Упаковка красных",
                SkillType.PackingWhite => "Упаковка белых",
                SkillType.PackingGreen => "Упаковка зеленых",
                SkillType.Extraction => "Извлечение",
                SkillType.Repair => "Ремонт",
                SkillType.ExpertMining => "Экспертная добыча",
                SkillType.Washing => "Промывание",
                SkillType.Fracturing => "Дробление",
                SkillType.NanoPacking => "Наноупаковка",
                SkillType.BuildStructure => "Стройка опор",
                SkillType.BuildRoad => "Стройка дорог",
                SkillType.BuildUniversal => "Универсальная стройка",
                SkillType.BuildWar => "Военный блок",
                SkillType.Architecture => "Архитектура",
                SkillType.TotalDestruction => "Тотальное разрушение",
                SkillType.UltraWhite => "Ультра-добыча белых",
                SkillType.Jewlery => "Ювелирная добыча фиолетовых",
                SkillType.Induction => "Индукция",
                SkillType.MineSlime => "Слизевая добыча",
                SkillType.MineDeep => "Глубинная добыча",
                SkillType.GluonPacking => "Глюонная упаковка",
                SkillType.Unknown => "Неизвестный навык",
                _ => skill.ToString()
            };
        }
        public static string GetDescription(this SkillType skill)
        {
            return skill switch
            {
                SkillType.MineBlue => "Увеличивет добычу синих кристаллов",
                SkillType.MineGreen => "Увеличивет добычу зеленых кристаллов",
                SkillType.MineRed => "Увеличивет добычу красных кристаллов",
                SkillType.MineViolet => "Увеличивет добычу фиолетовых кристаллов",
                SkillType.MineWhite => "Увеличивет добычу белых кристаллов",
                SkillType.MineCyan => "Увеличивет добычу голубых кристаллов",

                SkillType.PackingGreen => "Зеленые кристаллы занимают меньше места",
                SkillType.PackingBlue => "Синие кристаллы занимают меньше места",
                SkillType.PackingRed => "Красные кристаллы занимают меньше места",
                SkillType.PackingWhite => "Белые кристаллы занимают меньше места",
                SkillType.PackingViolet => "Фиолетовые кристаллы занимают меньше места",
                SkillType.PackingCyan => "Голубые кристаллы занимают меньше места",

                SkillType.Sort => "Позволяет добывать дополнительные кристаллы",

                SkillType.BuildGreen => "Позволяет строить зеленые постройки",
                SkillType.BuildYellow => "Позволяет строить желтые постройки",
                SkillType.BuildRed => "Позволяет строить красные постройки",
                SkillType.BuildQuadro => "Позволяет строить квадро-блоки",

                SkillType.Movement => "Увеличивает передвижение робота",
                SkillType.RoadMovement => "По дорогам робот бегает быстрее",

                SkillType.Packing => "В хранилище влезает больше ресурсов",
                SkillType.Compression => "Уплотняет руду для экономии места",
                SkillType.HyperPacking => "Ещё сильнее плотняет руду для экономии места",
                SkillType.NanoPacking => "Очень плотная упаковка ресурсов",

                SkillType.BuildStructure => "Строит опоры и перекрытия",
                SkillType.BuildRoad => "Строит дороги",
                SkillType.BuildWar => "Строит Военный блок",

               
                SkillType.AdjacentExtraction => "Позволяет добывать зеленые кристаллы из синих и наоборот",
                SkillType.Extraction => "Увеличивает добычу зеленых и синих кристаллов",
                SkillType.Geology => "Позволяет таскать с собой кристаллы",

                SkillType.Destruction => "Увеличивает скорость разрушение скал",
                SkillType.Annihilation => "Увеличивает скорость разрушение песка",
                SkillType.Crystallography => "Ускоряет разрушение кристаллов",

                SkillType.Deconstruction => "Ускоряет разрушение блоков",

                SkillType.AntiSlime => "Уменьшает урон слизи при поедание",
                SkillType.AntiBlock => "Позволяет быстрее разрушать квадро-блоков",
                SkillType.AntiGun => "Защита от пушек",

                SkillType.Digging => "Позволяет быстрее разрушать кристаллы и разную породу",
                SkillType.Health => "Увеличивает прочность робота",
                SkillType.MineGeneral => "Увеличивает добычу синих и зеленых кристаллов",
                SkillType.Detection => "Позволяет находить кристаллы из скал",

                SkillType.Fridge => "Охлаждает робота при погружение на глубину",
                SkillType.Turbo => "Охлаждение робота при погружение на глубину на полную мощность",

                SkillType.Deactivation => "Увеличивает скорость поедание слизи",
                SkillType.DeMagnetizing => "Ускоряет разрушение металического песка",
                SkillType.Repair => "Позволяет чинить робота",
                SkillType.Washing => "Извлекает чистые кристаллы из песка",
                SkillType.Fracturing => "Ускоряет дробление валунов",

                // =============== Скиллы за очки перепрошивки ===============
                SkillType.Architecture => "Строит зеленые/красные/желтые блоки",
                SkillType.BuildUniversal => "Строит опоры и дороги",
                SkillType.Upgrade => "Ускоряет получение опыта",
                SkillType.Discount => "Позволяет тратить меньше денег на прокачку скилов",
                SkillType.ExpertMining => "Позволяет добывать огромное количество кристаллов",
                SkillType.TotalDestruction => "Ломает более прочную и тяжелую породу скал",
                SkillType.Induction => "Увеличивает расход пушек",
                SkillType.Jewlery => "Аккуратная добыча фиолетовых без сколов",
                SkillType.UltraWhite => "Добывает белые кристаллы за один удар",
                SkillType.MineSlime => "Добыча кристаллов из слизи",
                SkillType.MineDeep => "Копает глбинные породы",
                SkillType.GluonPacking => "Увеличивает вместимость до огромнейшего размера",
                // ==========================================================

                SkillType.Unknown => "Неизвестный навык",
                _ => "Описание отсутствует"
            };
        }
    }
}