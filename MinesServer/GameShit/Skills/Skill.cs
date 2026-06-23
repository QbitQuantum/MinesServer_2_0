using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;

namespace MinesServer.GameShit.Skills
{
    class TemplateDescription
    {
        /// <summary>
        /// Унифицированный шаблон описания навыка, основанный на его идентификаторе.
        /// </summary>
        public static string Description(
            SkillType skillType,
            int level,
            float currentExp,
            float maxExp,
            float discountEffect = 0,
            float upgradeEffect = 0)
        {
            var info = skillType.GetInfo();
            return
                $"<color=white>{info?.Name ?? skillType.ToString()}</color>. Уровень:<color=white>{level}</color>\n" +
                $"Опыт {currentExp}/{maxExp} " +
                (upgradeEffect > 0
                    ? $"<color=yellow>[Экспертное обучение x{1 + (upgradeEffect / 100f):F1}]</color>\n"
                    : $"<color=red>[Экспертное обучение отсутствует]</color>\n") +
                $"Как качать: {info?.LevelingHint ?? "Неизвестно"}\n" +
                skillType.GetDisplayInfo(skillType.GetInfo().EffectFunc(level)) + "\n" +
                $"Стоимость: {skillType.GetPrice(level)} " +
                (discountEffect > 0
                    ? $"<color=yellow>[Оптимизация стоимости {discountEffect:F1}%]</color>\n"
                    : $"<color=red>[Оптимизация отсутствует]</color>\n") +
                $"ОПП: {skillType.GetOpp(level)}\n";
        }
    }
    public class Skill
    {
        public Skill()
        {
        }

        public Skill(int lvl, float exp, SkillType type)
        {
            this.lvl = lvl;
            this.exp = exp;
            this.type = type;
        }

        public int lvl = 1;
        public float exp = 0;
        public SkillType type;

        public SaledSkill saledSkill => new(lvl, isUpReady(), type);

        public Dictionary<string, int> Up()
        {
            Dictionary<string, int> skillProgress = [];
            if (!isUpReady()) return skillProgress;
            exp -= Expiriense;
            lvl += 1;
            skillProgress.Add(type.GetCode(), (int)((exp * 100f) / Expiriense));
            return skillProgress;
        }

        public Dictionary<string, int> AddExp(float expv = 1, float UpgradeEffect = 0)
        {
            Dictionary<string, int> skillProgress = [];
            if (UpgradeEffect != 0)
                expv *= 1f + (UpgradeEffect / 100f);
            exp += expv;
            skillProgress.Add(type.GetCode(), (int)((exp * 100f) / Expiriense));
            return skillProgress;
        }

        //  Mетод проверки уровня
        public bool IsLevelSatisfied(int requiredLevel)
        {
            return lvl >= requiredLevel;
        }

        // Метод для получения отставания по уровню
        public int GetLevelDeficit(int requiredLevel)
        {
            return requiredLevel - lvl;
        }

        public bool IsRequiered(SkillType RequiredSkill, int RequiredLevel)
        {
            return type == RequiredSkill && lvl >= RequiredLevel;
        }

        public bool isUpReady()
        {
            return exp >= Expiriense;
        }

        public string GetDescription(Player p)
        {
            float upgradeEffect = 0f;
            float discountEffect = 0f;

            if (p?.skillslist != null)
            {
                // Находим навык Upgrade
                var SkillUpgrade = SkillType.Upgrade;
                var upgradeSkill = p.skillslist.GetSkill(SkillUpgrade);
                
                if (upgradeSkill != null && upgradeSkill.type != SkillUpgrade)
                    upgradeEffect = upgradeSkill.Effect;

                // Находим навык Discount
                var SkillDiscount = SkillType.Discount;
                var discountSkill = p.skillslist.GetSkill(SkillDiscount);

                if (discountSkill != null && discountSkill.type != SkillDiscount)
                    discountEffect = discountSkill.Effect;
            }

            return TemplateDescription.Description(
                type,
                lvl,
                exp,
                Expiriense,
                discountEffect,
                upgradeEffect
            );
        }

        public float Expiriense
        {
            get
            {
                var info = type.GetInfo();
                return info?.ExpFunc?.Invoke(lvl) ?? 100f;
            }
        }

        public string Description
        {
            get
            {
                var info = type.GetInfo();
                return info.Description;
            }
        }

        public float Effect
        {
            get
            {
                var info = type.GetInfo();
                return info?.EffectFunc?.Invoke(lvl) ?? 0f;
            }
        }
        public float DurabilityEffect
        {
            get
            {
                var info = type.GetInfo();
                return info?.DurabilityFunc?.Invoke(lvl) ?? 0f;
            }
        }

        public float Cost
        {
            get
            {
                var info = type.GetInfo();
                return info?.PriceFunc?.Invoke(lvl) ?? 0f;
            }
        }

        public Dictionary<SkillType, int>? requirements
        {
            get;
            set;
        }

        public Dictionary<SkillType, int>? GetReqs
        {
            get
            {
                // Сначала проверяем, есть ли локальные требования
                if (requirements != null)
                    return requirements;

                // Иначе берем из централизованного хранилища
                var info = type.GetInfo();
                if (info?.Requirements != null)
                {
                    // Конвертируем List<SkillRequirement> в Dictionary<SkillType, int>
                    return info.Requirements.ToDictionary(r => r.RequiredSkill, r => r.RequiredLevel);
                }

                return null;
            }
        }
    }
}