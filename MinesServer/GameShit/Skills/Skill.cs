using System.Text;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;

namespace MinesServer.GameShit.Skills
{
    public class Skill
    {
        public Skill()
        {
        }

        public Skill(int level, float expiriense, SkillType type)
        {
            _level = level;
            _expiriense = expiriense;
            _type = type;
        }

        private int _level = 1;
        private float _expiriense = 0;
        private readonly SkillType _type = SkillType.Unknown;

        public int Level => _level;
        public float Expiriense => _expiriense;
        public SkillType Type => _type;

        public bool IsUp => IsUpReady();

        public Dictionary<string, int> Up()
        {
            Dictionary<string, int> skillProgress = [];
            if (!IsUp) return skillProgress;
            _expiriense -= MaxExpiriense;
            _level += 1;
            skillProgress.Add(Type.GetCode(), (int)((Expiriense * 100f) / MaxExpiriense));
            return skillProgress;
        }

        public Dictionary<string, int> AddExp(float expv = 1, float UpgradeEffect = 0)
        {
            Dictionary<string, int> skillProgress = [];
            if (UpgradeEffect != 0)
                expv *= 1f + (UpgradeEffect / 100f);
            _expiriense += expv;
            skillProgress.Add(Type.GetCode(), (int)((Expiriense * 100f) / MaxExpiriense));
            return skillProgress;
        }

        //  Mетод проверки уровня
        public bool IsLevelSatisfied(int requiredLevel)
        {
            return Level >= requiredLevel;
        }

        // Метод для получения отставания по уровню
        public int GetLevelDeficit(int requiredLevel)
        {
            return requiredLevel - Level;
        }

        public bool IsRequiered(SkillType RequiredSkill, int RequiredLevel)
        {
            return Type == RequiredSkill && Level >= RequiredLevel;
        }

        private bool IsUpReady()
        {
            return Expiriense >= MaxExpiriense;
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
                
                if (upgradeSkill != null && upgradeSkill.Type != SkillUpgrade)
                    upgradeEffect = upgradeSkill.Effect;

                // Находим навык Discount
                var SkillDiscount = SkillType.Discount;
                var discountSkill = p.skillslist.GetSkill(SkillDiscount);

                if (discountSkill != null && discountSkill.Type != SkillDiscount)
                    discountEffect = discountSkill.Effect;
            }

            return TemplateDescription(
                Type,
                Level,
                Expiriense,
                MaxExpiriense,
                discountEffect,
                upgradeEffect
            );
        }

        public float MaxExpiriense
        {
            get
            {
                var info = Type.GetInfo();
                return info?.ExpFunc?.Invoke(Level) ?? 100f;
            }
        }

        public string Description
        {
            get
            {
                var info = Type.GetInfo();
                return info?.Description ?? "";
            }
        }

        public float Effect
        {
            get
            {
                var info = Type.GetInfo();
                return info?.EffectFunc?.Invoke(Level) ?? 0f;
            }
        }
        public float DurabilityEffect
        {
            get
            {
                var info = Type.GetInfo();
                return info?.DurabilityFunc?.Invoke(Level) ?? 0f;
            }
        }

        public float Cost
        {
            get
            {
                var info = Type.GetInfo();
                return info?.PriceFunc?.Invoke(Level) ?? 0f;
            }
        }

        /// <summary>
        /// Унифицированный шаблон описания навыка, основанный на его идентификаторе.
        /// </summary>
        private static string TemplateDescription(
            SkillType Type,
            int Level,
            float Expiriense,
            float MaxExpiriense,
            float discountEffect = 0,
            float upgradeEffect = 0)
        {
            var info = Type.GetInfo();

            if (info == null) return "Информация об умении отсутствует.";

            var name = info.Name ?? Type.ToString();
            var hint = info.LevelingHint ?? "Неизвестно";
            var effect = info.EffectFunc(Level);

            var sb = new StringBuilder();

            sb.AppendLine($"<color=white>{name}</color>. Уровень: <color=white>{Level}</color>");
            sb.AppendLine($"Опыт <color=white>{Expiriense}/{MaxExpiriense}</color> " + FormatUpgrade(upgradeEffect));
            sb.AppendLine($"Как качать: <color=white>{hint}</color>");
            sb.AppendLine(Type.GetDisplayInfo(effect));
            sb.AppendLine(FormatCost(Type.GetPrice(Level), discountEffect));
            sb.AppendLine($"<color=white>Баллы перепрошивки</color>: {Type.GetOpp(Level)}");

            return sb.ToString();
        }

        private static string FormatUpgrade(float upgradeEffect) =>
            upgradeEffect > 0
                ? $"<color=yellow>[Экспертное обучение x{1 + upgradeEffect / 100f:F1}]</color>"
                : "<color=red>[Экспертное обучение отсутствует]</color>";

        private static string FormatCost(float price, float discountEffect) =>
            $"Стоимость: {price} " + (discountEffect > 0
                ? $"<color=yellow>[Оптимизация стоимости {discountEffect:F1}%]</color>"
                : "<color=red>[Оптимизация отсутствует]</color>");
    }
}