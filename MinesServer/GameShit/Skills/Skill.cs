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
                lvl += 1;
                exp -= Expiriense;
                v.Add(type.GetCode(), (int)((exp * 100f) / Expiriense));
                p.connection?.SendU(new SkillsPacket(v));
                p.SendLvl();
                p.SendHealth();
                p.skillslist.Save();

                if (EffectType() == SkillEffectType.OnMove)
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

        public bool Visible(Player p, out bool meet)
        {
            bool visible = true;
            meet = true;

            var reqs = GetReqs;
            if (reqs is not null)
            {
                foreach (var req in reqs)
                {
                    var skill = p.skillslist.skills.FirstOrDefault(skill => skill.Value?.type == req.Key).Value;
                    if (skill == default)
                    {
                        visible = false;
                    }
                    else if (skill.lvl < req.Value)  // Изменил условие: убрал -3, так как теперь требования явные
                    {
                        meet = false;
                    }
                }
            }
            return visible;
        }

        public void AddExp(Player p, float expv = 1)
        {
            Dictionary<string, int> v = new();
            // Проверяем навык Upgrade для множителя опыта
            foreach (var i in p.skillslist.skills.Values)
            {
                if (i != null && UseSkill(SkillEffectType.OnExp, p))
                {
                    if (i.type == SkillType.Upgrade)
                    {
                        expv *= i.Effect;
                    }
                }
            }
            exp += expv;
            v.Add(type.GetCode(), (int)((exp * 100f) / Expiriense));
            p.connection?.SendU(new SkillsPacket(v));
            p.skillslist.Save();
        }
        public bool UseSkill(SkillEffectType e, Player p)
        {
            return e == EffectType();
        }

        public bool isUpReady()
        {
            return exp >= Expiriense;
        }
        public SkillEffectType EffectType()
        {
            return type.GetInfo()?.EffectType ?? SkillEffectType.OnExp;
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
                // Используем новый TemplateDescription, который принимает SkillType
                return TemplateDescription.Description(
                    type,
                    lvl,
                    exp,
                    Expiriense
                );
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
        public float AdditionalEffect
        {
            get
            {
                var info = type.GetInfo();
                return info?.DopFunc?.Invoke(lvl) ?? 0f;
            }
        }

        public float Cost
        {
            get
            {
                var info = type.GetInfo();
                return info?.CostFunc?.Invoke(lvl) ?? 0f;
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