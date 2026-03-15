using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using MinesServer.Enums;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.Server;

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

        public PlayerSkills(bool initializeDefaultSkills)
        {
            if (initializeDefaultSkills)
            {
                // Установка базовых навыков без Player
                InstallSkill(SkillType.MineGeneral.GetCode(), 0);
                InstallSkill(SkillType.Digging.GetCode(), 1);
                InstallSkill(SkillType.Movement.GetCode(), 2);
                InstallSkill(SkillType.Health.GetCode(), 3);
            }
            slots = 20;

            // Инициализируем пустой список купленных экспертных скиллов
            _purchasedExpertSkills = new List<string>();
            expertSkill = "[]";

            Save(); // Сохраняем после инициализации
        }

        /// <summary>
        /// Сериализованное представление словаря слот -> (код навыка, уровень, опыт).
        /// </summary>
        public string ser { get; set; } = "";

        /// <summary>
        /// JSON поле для хранения купленных экспертных скиллов (храним строковые коды)
        /// </summary>
        public string expertSkill { get; set; } = "[]";

        [NotMapped]
        private List<string> _purchasedExpertSkills;

        [NotMapped]
        public List<string> PurchasedExpertSkills
        {
            get
            {
                if (_purchasedExpertSkills == null)
                {
                    try
                    {
                        _purchasedExpertSkills = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(expertSkill ?? "[]") ?? new List<string>();
                    }
                    catch
                    {
                        _purchasedExpertSkills = new List<string>();
                    }
                }
                return _purchasedExpertSkills;
            }
            set
            {
                _purchasedExpertSkills = value;
                expertSkill = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }

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

        public bool DeleteSkill()
        {
            if (!skills.ContainsKey(selectedslot) || skills[selectedslot] == null)
                return false;
            skills.Remove(selectedslot);
            Save();
            return true;
        }

        /// <summary>
        /// Проверяет, выполнены ли требования для указанного навыка
        /// </summary>
        private bool MeetsRequirements(SkillType skillType)
        {
            var info = skillType.GetInfo();
            if (info?.Requirements == null || !info.Requirements.Any())
                return true; // Нет требований - можно устанавливать

            foreach (var req in info.Requirements)
            {
                var hasReq = skills.Values.Any(s =>
                    s?.type == req.RequiredSkill &&
                    s?.lvl >= req.RequiredLevel);

                if (!hasReq)
                    return false; // Не выполнено хотя бы одно требование
            }

            return true; // Все требования выполнены
        }

        /// <summary>
        /// Проверяет, куплен ли экспертный навык игроком
        /// </summary>
        private bool IsExpertSkillPurchased(SkillType skillType)
        {
            var info = skillType.GetInfo();

            // Если это не экспертный навык - считаем что доступен всегда
            if (info == null || !info.IsExpertSkill)
                return true;

            // Получаем строковый код навыка
            string skillCode = skillType.GetCode();

            // Проверяем наличие кода навыка в списке купленных
            return PurchasedExpertSkills.Contains(skillCode);
        }

        public bool CanInstallSkill(string typeCode, int slot)
        {
            if (slot > slots || slot < 0)
                return false;

            if (skills.ContainsKey(slot) && skills[slot] != null)
                return false;

            var skillType = Mines3Enums.SkillFromCode(typeCode);
            if (skillType == SkillType.Unknown)
                return false;

            var info = skillType.GetInfo();
            if (info == null)
                return false;

            // Для экспертных навыков проверяем, куплен ли он
            if (info.IsExpertSkill && !IsExpertSkillPurchased(skillType))
                return false;

            return MeetsRequirements(skillType);
        }

        public bool InstallSkill(string typeCode, int slot)
        {
            if (!CanInstallSkill(typeCode, slot))
                return false;

            var skillType = Mines3Enums.SkillFromCode(typeCode);
            
            skills[slot] = new Skill
            {
                type = skillType,
                lvl = 1,
                exp = 0
            };

            Save();
            return true;
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

            // Сохраняем из поля _purchasedExpertSkills, а не создаем новый список
            if (_purchasedExpertSkills != null)
            {
                expertSkill = Newtonsoft.Json.JsonConvert.SerializeObject(_purchasedExpertSkills);
            }

            db.SaveChanges();
        }

        public Dictionary<SkillType, bool> SkillToInstall()
        {
            var result = new Dictionary<SkillType, bool>();
            var allSkills = SkillTypeExtensions.GetAllInfos();

            foreach (var kvp in allSkills)
            {
                var skillType = kvp.Key;
                var info = kvp.Value;

                // Пропускаем если навык уже есть
                if (skills.Values.Any(s => s?.type == skillType))
                    continue;

                // Для экспертных навыков проверяем, куплен ли он
                if (info.IsExpertSkill && !IsExpertSkillPurchased(skillType))
                    continue;

                // Если нет требований - сразу доступен
                if (info?.Requirements == null || !info.Requirements.Any())
                {
                    result.Add(skillType, true);
                    continue;
                }

                // Проверяем требования
                bool allRequirementsMet = true;
                bool allRequirementsExist = true;
                int maxMissingLevels = 0;

                foreach (var req in info.Requirements)
                {
                    var playerSkill = skills.Values.FirstOrDefault(s => s?.type == req.RequiredSkill);

                    // Если требуемого навыка НЕТ У ИГРОКА - навык полностью недоступен
                    if (playerSkill == null)
                    {
                        allRequirementsExist = false;
                        break;
                    }

                    int playerLevel = playerSkill.lvl;

                    // Проверяем уровень
                    if (playerLevel < req.RequiredLevel)
                    {
                        allRequirementsMet = false;
                        int missing = req.RequiredLevel - playerLevel;
                        maxMissingLevels = Math.Max(maxMissingLevels, missing);
                    }
                }

                // Если нет какого-то требуемого навыка - пропускаем (не показываем)
                if (!allRequirementsExist)
                    continue;

                // Добавляем если все требования выполнены ИЛИ отстают максимум на 3 уровня
                if (allRequirementsMet || maxMissingLevels <= 3)
                {
                    result.Add(skillType, allRequirementsMet);
                }
            }

            return result;
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

        /// <summary>
        /// Обрабатывает получение опыта для навыков по типу эффекта
        /// </summary>
        public void HandleExperience(Player player, SkillEffectType effectType, float baseExp = 1f)
        {
            LoadSkills();

            foreach (var skill in skills.Values.Where(s => s != null))
            {
                if (skill != null && skill.UseSkill(effectType, player))
                {
                    skill.AddExp(player, baseExp);
                }
            }
        }

        /// <summary>
        /// Обрабатывает получение опыта для конкретного типа навыка
        /// </summary>
        public void HandleExperience(Player player, SkillType skillType, float baseExp = 1f)
        {
            LoadSkills();

            var skill = skills.Values.FirstOrDefault(s => s?.type == skillType);
            skill?.AddExp(player, baseExp);
        }


        /// <summary>
        /// Обрабатывает получение опыта при добыче ресурсов
        /// </summary>
        public void HandleMiningExperience(Player player, float baseExp = 1f)
        {
            HandleExperience(player, SkillType.MineGeneral, baseExp);
        }

        /// <summary>
        /// Обрабатывает получение опыта при ударах по кристаллам
        /// </summary>
        public void HandleDiggingExperience(Player player, float baseExp = 1f)
        {
            HandleExperience(player, SkillType.Digging, baseExp);
        }

        /// <summary>
        /// Обрабатывает получение опыта при строительстве
        /// </summary>
        public void HandleBuildingExperience(Player player, string buildType, float baseExp = 1f)
        {
            LoadSkills();

            // Определяем, какой навык строительства использовать
            SkillType skillType = buildType switch
            {
                "G" => SkillType.BuildGreen,
                "Y" => SkillType.BuildYellow,
                "R" => SkillType.BuildRed,
                "V" => SkillType.BuildWar,
                "O" => SkillType.BuildStructure,
                _ => SkillType.Unknown
            };

            if (skillType != SkillType.Unknown)
                HandleExperience(player, skillType, baseExp);

            /*
            // Также обрабатываем общий навык OnBld для всех строительных навыков
            foreach (var skill in skills.Values.Where(s => s != null && s.EffectType() == SkillEffectType.OnBld))
            {
                if (skill != null && skill.type != skillType) // Чтобы не дублировать для конкретного навыка
                {
                    skill.AddExp(player, Exp); // Меньше опыта за смежные навыки
                }
            }
            */

        }

        /// <summary>
        /// Обрабатывает получение опыта при получении урона
        /// </summary>
        public void HandleDamageExperience(Player player, DamageTypePlayer damageType, float baseExp = 1f)
        {
            LoadSkills();

            // Навык здоровья получает опыт при любом уроне
            HandleExperience(player, SkillType.Health, baseExp);

            // Специфические навыки для урона
            if (damageType == DamageTypePlayer.Gun)
            {
                HandleExperience(player, SkillType.Induction, baseExp);

                HandleExperience(player, SkillType.AntiGun, baseExp);
            }
        }

        /// <summary>
        /// Возвращает модифицированный урон
        /// </summary>
        public int HandleDamageReceived(int damage)
        {
            LoadSkills();
            int modifiedDamage = damage;

            foreach (var skill in skills.Values.Where(s => s != null))
            {
                if (skill != null && skill.type == SkillType.AntiGun)
                {
                    // Уменьшаем урон
                    int reduction = (int)(damage * (skill.Effect / 100));
                    modifiedDamage = Math.Max(0, modifiedDamage - reduction);
                    break;
                }
            }
            return modifiedDamage;
        }

        /// <summary>
        /// Получает множитель добычи от навыков
        /// </summary>
        public float GetMiningMultiplier(ref float cb)
        {
            LoadSkills();

            float multiplier = 1 + (float)Math.Truncate(cb);

            // TODO: Может стоять ExpertMining
            var miningSkill = skills.Values.FirstOrDefault(s => s?.type == SkillType.MineGeneral);
            if (miningSkill != null)
            {
                multiplier += miningSkill.Effect;
            }

            // Обновляем дробную часть cb
            float floorMult = (float)Math.Truncate(multiplier);
            cb -= (float)Math.Truncate(cb);
            cb += multiplier - floorMult;

            return floorMult;
        }

        /// <summary>
        /// Обрабатывает получение опыта при перемещении валуна
        /// </summary>
        public void HandleBoulderMoveExperience(Player player)
        {
            HandleExperience(player, SkillEffectType.OnDig, 10); // Больше опыта за перемещение
        }

        /// <summary>
        /// Обрабатывает получение опыта при разрушении блока (без кристаллов)
        /// </summary>
        public void HandleDestructionExperience(Player player)
        {
            HandleExperience(player, SkillEffectType.OnDig);
        }

        /// <summary>
        /// Получает множитель урона при копании
        /// </summary>
        public float GetDiggingDamageMultiplier(float baseDamage)
        {
            LoadSkills();

            float damage = baseDamage;

            var diggingSkill = skills.Values.FirstOrDefault(s => s?.type == SkillType.Digging);
            if (diggingSkill != null)
            {
                damage = baseDamage * (diggingSkill.Effect / 100f);
            }

            return damage;
        }


        /// <summary>
        /// Возвращает коэффициент потребление пушки
        /// </summary>
        public float HandleInductionReceived()
        {
            LoadSkills();
            float modifiedinductionMultiplier = 1f;

            foreach (var skill in skills.Values.Where(s => s != null))
            {
                if (skill != null && skill.type == SkillType.Induction)
                {
                    // Увеличиваем потребление
                    modifiedinductionMultiplier = (skill.Effect / 100);
                    break;
                }
            }
            return modifiedinductionMultiplier;
        }

        public int slots { get; set; }
        [NotMapped]
        public Dictionary<int, Skill?> skills = new();
    }
}