using Microsoft.EntityFrameworkCore;
using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Network.GUI;

namespace MinesServer.GameShit.Skills
{
    public class Skill
    {
        public Skill()
        {
        }

        public int lvl = 1;
        public float exp = 0;
        public SkillType type;

        public void Up(Player p)
        {
            if (isUpReady())
            {
                Dictionary<string, int> v = new();
                exp -= Expiriense;
                lvl += 1;
                v.Add(type.GetCode(), (int)((exp * 100f) / Expiriense));
                p.connection?.SendU(new SkillsPacket(v));
                p.SendLvl();
                p.SendHealth();
                p.skillslist.Save();

                if (type == SkillType.Movement || 
                    type == SkillType.RoadMovement)
                {
                    p.SendSpeed();
                }

                if (type == SkillType.Health)
                {
                    p.MaxHealth = (int)Effect;
                    p.SendHealth();
                }
            }
        }

        public void AddExp(Player p, float expv = 1)
        {
            Dictionary<string, int> v = new();

            var SkillUpgrade = p.skillslist.GetSkill(SkillType.Upgrade);

            // TODO: Добавить функцию MultiExp
            // И по возможности кэшировать все значения функций
            // Чтобы постоянно не перебирать
            if (SkillUpgrade != null)
                expv *= 1f + (SkillUpgrade.Effect / 100f);

            exp += expv;
            v.Add(type.GetCode(), (int)((exp * 100f) / Expiriense));
            p.connection?.SendU(new SkillsPacket(v));
            p.skillslist.Save();
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
                return info?.Description;
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