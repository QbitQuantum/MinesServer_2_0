using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Skills
{
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
}
