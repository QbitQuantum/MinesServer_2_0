using MinesServer.Enums;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace MinesServer.GameShit.Entities.PlayerStaff
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
            float maxExp)
        {
            var info = skillType.GetInfo();
            return
                $"{info?.Name ?? skillType.ToString()}. Уровень:{level}\n" +
                $"Опыт {currentExp}/{maxExp}\n" +
                $"Как качать: {info?.LevelingHint ?? "Неизвестно"}\n" +
                $"Стоимость: {skillType.GetPrice(level)}\n" +
                $"ОПП: {skillType.GetOpp(level)}\n";
        }
    }

    /// <summary>
    /// Лёгкая структура для сохранения навыков игрока в БД.
    /// Хранит только идентификатор навыка, уровень и опыт.
    /// </summary>
    class PlayerSkillData
    {
        public string Code { get; set; } = "";
        public int Level { get; set; }
        public float Exp { get; set; }
    }

    public class PlayerSkills
    {
        public int id { get; set; }

        private PlayerSkills() { }

        public PlayerSkills(Player p)
        {
            // базовые навыки
            InstallSkill(SkillType.MineGeneral.GetCode(), 0, p);
            InstallSkill(SkillType.Digging.GetCode(), 1, p);
            InstallSkill(SkillType.Movement.GetCode(), 2, p);
            InstallSkill(SkillType.Health.GetCode(), 3, p);
            slots = 20;
            Save();
        }

        /// <summary>
        /// Сериализованное представление словаря слот -> (код навыка, уровень, опыт).
        /// </summary>
        public string ser { get; set; } = "";

        public void LoadSkills()
        {
            if (skills.Count > 0 || string.IsNullOrEmpty(ser))
                return;

            var raw = Newtonsoft.Json.JsonConvert
                .DeserializeObject<Dictionary<int, PlayerSkillData?>>(ser)
                ?? new Dictionary<int, PlayerSkillData?>();

            foreach (var kvp in raw)
            {
                var slot = kvp.Key;
                var data = kvp.Value;

                if (data is null || string.IsNullOrEmpty(data.Code))
                {
                    skills[slot] = null;
                    continue;
                }

                var skillType = Mines3Enums.SkillFromCode(data.Code);
                if (skillType == SkillType.Unknown)
                {
                    continue;
                }

                skills[slot] = new Skill
                {
                    type = skillType,
                    lvl = data.Level,
                    exp = data.Exp
                };
            }
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

        /// <summary>
        /// Установка навыка по его коду (ID) в указанный слот.
        /// Навык создаётся как "тонкий" объект, связанный с шаблоном через тип.
        /// </summary>
        public void InstallSkill(string typeCode, int slot, Player p)
        {
            if (slot > slots || slot < 0)
            {
                return;
            }

            if (skills.ContainsKey(slot) && skills[slot] != null)
            {
                return;
            }

            var skillType = Mines3Enums.SkillFromCode(typeCode);
            if (skillType == SkillType.Unknown)
            {
                return;
            }

            var info = skillType.GetInfo();
            if (info == null)
            {
                return;
            }

            // Проверка требований (если есть)
            if (info.Requirements != null)
            {
                foreach (var req in info.Requirements)
                {
                    var hasReq = skills.Values.Any(s => s?.type == req.RequiredSkill && s?.lvl >= req.RequiredLevel);
                    if (!hasReq)
                        return;
                }
            }

            skills[slot] = new Skill
            {
                type = skillType,
                lvl = 1,
                exp = 0
            };

            p.SendLvl();
            Save();
        }

        public void Save()
        {
            using var db = new DataBase();
            db.skills.Attach(this);

            var raw = new Dictionary<int, PlayerSkillData?>(skills.Count);

            foreach (var kvp in skills)
            {
                var slot = kvp.Key;
                var skill = kvp.Value;

                if (skill is null)
                {
                    raw[slot] = null;
                    continue;
                }

                raw[slot] = new PlayerSkillData
                {
                    Code = skill.type.GetCode(),
                    Level = skill.lvl,
                    Exp = skill.exp
                };
            }

            ser = Newtonsoft.Json.JsonConvert.SerializeObject(raw, Newtonsoft.Json.Formatting.None);
            db.SaveChanges();
        }

        public Dictionary<SkillType, bool> SkillToInstall(Player p)
        {
            Dictionary<SkillType, bool> d = new();

            foreach (var kvp in SkillTypeExtensions.GetAllInfos())
            {
                var skillType = kvp.Key;
                var info = kvp.Value;

                if (skills.FirstOrDefault(skill => skill.Value?.type == skillType).Value == null)
                {
                    // Проверяем требования
                    bool meetsReqs = true;
                    if (info.Requirements != null)
                    {
                        foreach (var req in info.Requirements)
                        {
                            var hasReq = skills.Values.Any(s => s?.type == req.RequiredSkill && s?.lvl >= req.RequiredLevel);
                            if (!hasReq)
                            {
                                meetsReqs = false;
                                break;
                            }
                        }
                    }

                    d.Add(skillType, meetsReqs);
                }
            }

            return d;
        }

        public int lvlsummary() => skills.Sum(i => i.Value?.lvl ?? 0);

        public UpSkill[] GetSkills()
        {
            List<UpSkill> ski = new();
            LoadSkills();
            for (int i = 0; i < slots; i++)
            {
                if (skills.ContainsKey(i) && skills[i] is not null)
                    ski.Add(new UpSkill(i, skills[i].lvl, skills[i].isUpReady(), skills[i].type));
            }
            return ski.ToArray();
        }

        public int slots { get; set; }
        [NotMapped]
        public Dictionary<int, Skill?> skills = new();
    }
}