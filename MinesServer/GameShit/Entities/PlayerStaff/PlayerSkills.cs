using Microsoft.IdentityModel.Tokens;
using MinesServer.Enums;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class PlayerSkills
    {
        public int id { get; set; }
        private PlayerSkills() {
        }
        public PlayerSkills(Player p)
        {//base skills
            InstallSkill(SkillType.MineGeneral.GetCode(), 0, p);
            InstallSkill(SkillType.Digging.GetCode(), 1, p);
            InstallSkill(SkillType.Movement.GetCode(), 2, p);
            InstallSkill(SkillType.Health.GetCode(), 3, p);
            slots = 20;
            Save();
        }
        public string ser { get; set; } = "";
        public void LoadSkills()
        {
            if (skills.Count < 1)
                skills = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Skill?>>(ser);
        }
        [NotMapped]
        public int selectedslot = -1;
        public void DeleteSkill(Player p)
        {
            if (!skills.ContainsKey(selectedslot))
            {
                return;
            }
            skills.Remove(selectedslot);
            p.SendLvl();
            Save();
        }
        public void InstallSkill(string type, int slot, Player p)
        {
            if (skills.ContainsKey(slot) && skills[slot] != null || slot > slots || slot < 0 || !skillz.First(i => i.type.GetCode() == type).Visible(p,out var meet) || !meet)
            {
                return;
            }
            var s = new Skill();
            skills[slot] = skillz.First(i => i.type.GetCode() == type).Clone();
            p.SendLvl();
            Save();
        }
        public void Save()
        {
            using var db = new DataBase();
            db.skills.Attach(this);
            ser = Newtonsoft.Json.JsonConvert.SerializeObject(skills, Newtonsoft.Json.Formatting.None);
            db.SaveChanges();
        }
        public Dictionary<SkillType, bool> SkillToInstall(Player p)
        {
            Dictionary<SkillType, bool> d = new();
            foreach (var sk in skillz)
            {
                if (skills.FirstOrDefault(skill => skill.Value?.type == sk.type).Value == null && sk.Visible(p,out var meet))
                {
                    d.Add(sk.type, meet);
                }
            }
            return d;
        }
        public int lvlsummary() => skills.Sum(i => i.Value?.lvl ?? 0);
        public UpSkill[] GetSkills()
        {
            List<UpSkill> ski = new();
            LoadSkills();
            for(int i = 0;i < slots;i++)
            {
                if (skills.ContainsKey(i) && skills[i] is not null)
                ski.Add(new UpSkill(i, skills[i].lvl, skills[i].isUpReady(), skills[i].type));
            }
            return ski.ToArray();
        }
        public int slots { get; set; }
        [NotMapped]
        public Dictionary<int, Skill?> skills = new();
        [NotMapped]
        public static List<Skill> skillz = new List<Skill>()
        {
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 100f + x * 10,
                    expfunc = (x) => 1,
                    type = SkillType.Digging, // dick
                    effecttype = SkillEffectType.OnDig
                },
                new Skill()
                {
                    requirements = new() { {SkillType.Digging,5} },
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 5f - x * 0.2f < 0 ? 1f : 5f - x * 0.2f,
                    expfunc = (x) => 1f,
                    type = SkillType.BuildRoad,
                    effecttype = SkillEffectType.OnBld
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1,
                    dopfunc = (x) => x,
                    expfunc = (x) => 1f,
                    type = SkillType.BuildGreen,
                    effecttype = SkillEffectType.OnBld,
                    description = (lvl,effect,dopeffect,cost,expcurrent,expneed) =>
                    {
                        return $"Стройка зеленых Уровень:{lvl}\nExp - {expcurrent}/{expneed}\nСтоимость блока: {effect}\nПрочность блока: {dopeffect}";
                    }
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1,
                    dopfunc = (x) => x,
                    expfunc = (x) => 1f,
                    type = SkillType.BuildYellow,
                    effecttype = SkillEffectType.OnBld,
                    description = (lvl,effect,dopeffect,cost,expcurrent,expneed) =>
                    {
                        return $"Стройка желтых Уровень:{lvl}\nExp - {expcurrent}/{expneed}\nСтоимость блока: {effect}\nПрочность блока: {dopeffect}";
                    }
                },
                 new Skill()
                {
                     requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1,
                    dopfunc = (x) => x,
                    expfunc = (x) => 1f,
                    type = SkillType.BuildRed,
                    effecttype = SkillEffectType.OnBld,
                     description = (lvl,effect,dopeffect,cost,expcurrent,expneed) =>
                    {
                        return $"Стройка красных Уровень:{lvl}\nExp - {expcurrent}/{expneed}\nСтоимость блока: {effect}\nПрочность блока: {dopeffect}";
                    }
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.BuildStructure,
                    effecttype = SkillEffectType.OnBld
                },
                 new Skill()
                {
                     requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    dopfunc = (x) => x,
                    expfunc = (x) => 1f,
                    type = SkillType.BuildWar,
                    effecttype = SkillEffectType.OnBld,
                    description = (lvl,effect,dopeffect,cost,expcurrent,expneed) =>
                    {
                        return $"Стройка ВБ:{lvl}\nExp - {expcurrent}/{expneed}\nСтоимость блока: {effect}\nПрочность блока: {dopeffect}";
                    }
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.Fridge, // охлад
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 0f,
                    effectfunc = (x) => 70f - x * 0.05f > 30f ? 70f - x * 0.05f : 30f,
                    expfunc = (x) => 1f,
                    type = SkillType.Movement, // движение,
                    effecttype = SkillEffectType.OnMove,
                    description = (lvl,effect,dopeffect,cost,expcurrent,expneed) =>
                    {
                        return $"Передвижение Уровень:{lvl}\nExp - {expcurrent}/{expneed}\nСкорость передвижения {Math.Round(1 / (effect * 1.2f * 0.001f) * 0.3f * 3.6f,2)} км/ч";
                    }
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.RoadMovement, // по дорогам
                    effecttype = SkillEffectType.OnMove
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 100 + 20 * x,
                    expfunc = (x) => 1f,
                    type = SkillType.Packing, // упаковка
                    effecttype = SkillEffectType.OnPackCrys
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 100 + x * 3f,
                    expfunc = (x) => 1f,
                    type = SkillType.Health, // хп
                    effecttype = SkillEffectType.OnHealth
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.PackingBlue, // упаковка синь
                    effecttype = SkillEffectType.OnPackCrys
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.PackingCyan, // упаковка голь
                    effecttype = SkillEffectType.OnPackCrys
                },
                 new Skill()
                {
                     requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.PackingGreen, // упаковка зель
                    effecttype = SkillEffectType.OnPackCrys
                },
                  new Skill()
                {
                      requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.PackingRed, // упаковка крась
                    effecttype = SkillEffectType.OnPackCrys
                },
                    new Skill()
                {
                        requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.PackingViolet, // упаковка фиоль
                    effecttype = SkillEffectType.OnPackCrys
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.PackingWhite, // упаковка бель
                    effecttype = SkillEffectType.OnPackCrys
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 0.08f + (float)(Math.Log10(x) * (Math.Pow(x, 0.5) / 4)),
                    expfunc = (x) => 1f,
                    type = SkillType.MineGeneral, // доба
                    effecttype = SkillEffectType.OnDigCrys
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) => 1f,
                    effectfunc = (x) => 100f + x * 0.2f,
                    expfunc = (x) => 1f,
                    type = SkillType.Induction, // инда
                    effecttype = SkillEffectType.OnHurt
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) =>1f,
                    effectfunc = (x) =>  (float)Math.Round(1f+(x-(float)Math.Log10(x)*(float)Math.Pow(x,0.9)/2f-x*0.098f)) >= 92 ? 92 : (float)Math.Round(1f+(x-(float)Math.Log10(x)*(float)Math.Pow(x,0.9)/2f-x*0.098f)),
                    expfunc = (x) => 0,
                    type = SkillType.AntiGun, // антипуфка
                    effecttype = SkillEffectType.OnHurt
                },
                new Skill()
                {
                    requirements = null,
                    costfunc = (x) =>1f,
                    effectfunc = (x) =>  x * 1f,
                    expfunc = (x) => 1f,
                    type = SkillType.Repair, // хил
                    effecttype = SkillEffectType.OnHealth
                }

        };
    }
}
