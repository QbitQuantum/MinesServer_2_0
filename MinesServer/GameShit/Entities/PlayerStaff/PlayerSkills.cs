using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.Enums;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.Network.GUI;
using MinesServer.Server;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
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
        private PlayerSkills() { }

        public PlayerSkills(bool initializeDefaultSkills)
        {
            if (initializeDefaultSkills)
            {
                slots = 4;
                // Установка базовых навыков без Player
                ForceInstallSkill(SkillType.MineGeneral, 0, 1, 0);
                ForceInstallSkill(SkillType.Digging, 1, 1, 0);
                ForceInstallSkill(SkillType.Movement, 2, 1, 0);
                ForceInstallSkill(SkillType.Health, 3, 1, 0);
            }

            Save(); // Сохраняем после инициализации
        }

        [NotMapped] private HashSet<string> _purchasedExpertSkills = [];

        [NotMapped] private readonly Dictionary<int, Skill> skills = [];

        [NotMapped] private int selectedslot = -1;

        public int id { get; set; }

        public int slots { get; set; }

        /// <summary>
        /// Сериализованное представление словаря слот -> (код навыка, уровень, опыт).
        /// </summary>
        public string ser { get; set; } = "";

        /// <summary>
        /// JSON поле для хранения купленных экспертных скиллов (храним строковые коды)
        /// </summary>
        public string expertSkill { get; set; } = "[]";

        /// <summary>
        /// Устанавливает текущий слот
        /// </summary>
        public void InstallSlot(int SelectedSlot)
        {
            selectedslot = SelectedSlot;
        }

        /// <summary>
        /// Возвращает текущий слот
        /// </summary>
        public int GetCurrentSlot()
        {
            return selectedslot;
        }

        /// <summary>
        /// Возвращает навык по типу навыка
        /// </summary>
        public Skill? GetSkill(SkillType Type)
        {
            return skills.Values.FirstOrDefault(s => s?.type == Type);
        }

        /// <summary>
        /// Возвращает навык на указанном слоте
        /// </summary>
        private Skill? GetSkillAtSlot(int slot)
        {
            if (slot > -1 && skills.TryGetValue(slot, out Skill? value))
                return value;
            return null;
        }

        /// <summary>
        /// Возвращает выбранный навык
        /// </summary>
        public Skill? GetSelectedSkill()
        {
            return GetSkillAtSlot(selectedslot);
        }

        public Skill? GetDestructionRockSkill()
        {
            var Skill = GetSkill(SkillType.Destruction);
            Skill ??= GetSkill(SkillType.TotalDestruction);
            return Skill;
        }

        /// <summary>
        /// Получает все типы навыки
        /// </summary>
        public List<SkillType> GetSkillTypes()
        {
            return skills.Values
                    .Where(s => s != null)
                    .Select(s => s.type)
                    .ToList();
        }

        public void LoadSkills()
        {
            if (skills.Count > 0 || string.IsNullOrEmpty(ser))
                return;

            var raw = Newtonsoft.Json.JsonConvert
                .DeserializeObject<Dictionary<int, PlayerSkillData>>(ser) // Убрали ?
                ?? [];

            foreach (var kvp in raw)
            {
                var data = kvp.Value;
                if (data == null || string.IsNullOrEmpty(data.Code))
                    continue;

                var skillType = Mines3Enums.SkillFromCode(data.Code);
                if (skillType == SkillType.Unknown)
                    continue;

                ForceInstallSkill(skillType, kvp.Key, data.Level, data.Exp);
            }
        }

        public bool DeleteSkill()
        {
            if (!skills.ContainsKey(selectedslot))
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
                var hasReq = skills.Values.Any(s => s?.IsRequiered(req.RequiredSkill, req.RequiredLevel) ?? false);
                if (!hasReq)
                    return false; // Не выполнено хотя бы одно требование
            }

            return true; // Все требования выполнены
        }

        /// <summary>
        /// Проверяет, куплен ли экспертный навык игроком
        /// </summary>
        public bool IsExpertSkillPurchased(SkillType skillType)
        {
            var info = skillType.GetInfo();

            // Если это не экспертный навык - считаем что доступен всегда
            if (info == null || !info.IsExpertSkill)
                return true;

            // Получаем строковый код навыка
            string skillCode = skillType.GetCode();

            // Проверяем наличие кода навыка в списке купленных
            return _purchasedExpertSkills.Contains(skillCode);
        }

        public void AddExpertSkillPurchased(SkillType skillType)
        {
            string skillCode = skillType.GetCode();
            if (!_purchasedExpertSkills.Add(skillCode))  // Add сам возвращает bool
                return;
            Save();
        }

        public bool CanInstallSkill(SkillType skillType, int slot)
        {
            if (slot > slots || slot < 0)
                return false;

            if (skills.ContainsKey(slot) && skills[slot] != null)
                return false;

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

        public bool InstallSkill(SkillType skillType, int slot)
        {
            if (!CanInstallSkill(skillType, slot))
                return false;
            ForceInstallSkill(skillType, slot, 1, 0);
            Save();
            return true;
        }

        public void ForceInstallSkill(SkillType skillType, int slot, int lvl, float exp)
        {
            skills[slot] = new Skill(lvl, exp, skillType);
        }

        public void Save()
        {
            using var db = new DataBase();
            db.skills.Attach(this);

            var raw = new Dictionary<int, PlayerSkillData>(skills.Count);

            foreach (var kvp in skills)
            {
                var skill = kvp.Value;
                raw[kvp.Key] = new PlayerSkillData
                {
                    Code = skill.type.GetCode(),
                    Level = skill.lvl,
                    Exp = skill.exp
                };
            }

            ser = Newtonsoft.Json.JsonConvert.SerializeObject(raw, Newtonsoft.Json.Formatting.None);

            expertSkill = Newtonsoft.Json.JsonConvert.SerializeObject(_purchasedExpertSkills);

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
                    var playerSkill = GetSkill(req.RequiredSkill);

                    if (playerSkill == null)
                    {
                        allRequirementsExist = false;
                        break;
                    }

                    // Используем инкапсулированные методы
                    if (!playerSkill.IsLevelSatisfied(req.RequiredLevel))
                    {
                        allRequirementsMet = false;
                        maxMissingLevels = Math.Max(maxMissingLevels,
                            playerSkill.GetLevelDeficit(req.RequiredLevel));
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
            LoadSkills();

            List<UpSkill> ski = [];
            
            for (int i = 0; i < slots; i++)
            {
                if (skills.TryGetValue(i, out Skill? value) && value is not null)
                {
                    var saled = value.saledSkill;
                    ski.Add(new UpSkill(i, saled.Lvl, saled.IsUp, saled.Type));
                }
            }
            return ski.ToArray();
        }

        /// <summary>
        /// Получение множителя опыта от навыка
        /// </summary>
        private float UpgradeEffect()
        {
            return GetSkill(SkillType.Upgrade)?.Effect ?? 0f;
        }

        /// <summary>
        /// Обрабатывает получение опыта для конкретного типа навыка
        /// </summary>
        public void HandleExperience(Player player, SkillType skillType, float baseExp = 1f)
        {
            LoadSkills();
            var skill = GetSkill(skillType);
            if (skill == null) return;
            var skillProgress = skill.AddExp(baseExp, UpgradeEffect());
            player.connection?.SendU(new SkillsPacket(skillProgress));
            Save();
        }

        /// <summary>
        /// Обрабатывает получение опыта при добыче ресурсов
        /// </summary>
        public void HandleMiningExperience(Player player, float baseExp = 1f)
        {
            // Вообще не должно, надо явно указывать при наличие одного из скиллов
            // Но так один из них не будет установлен, то прокачается только один
            // Как минимум я на это надеюсь :D
            HandleExperience(player, SkillType.ExpertMining, baseExp);
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

            var AntiGunSkill = GetSkill(SkillType.AntiGun);

            if (AntiGunSkill == null)
                return modifiedDamage;

            // Уменьшаем урон
            int reduction = (int)(damage * (AntiGunSkill.Effect / 100));
            modifiedDamage = Math.Max(0, modifiedDamage - reduction);

            return modifiedDamage;
        }

        /// <summary>
        /// Получает множитель добычи
        /// </summary>
        public float GetMiningMultiplier()
        {
            LoadSkills();
            var miningSkill = GetSkill(SkillType.MineGeneral);
            miningSkill ??= GetSkill(SkillType.ExpertMining);
            return miningSkill?.Effect ?? 0f;
        }

        public bool HasSkill(SkillType skillType)
        {
            LoadSkills();
            var skill = GetSkill(skillType);
            return skill != null;
        }

        public float GetSkillEffect(SkillType skillType)
        {
            LoadSkills();
            var skill = GetSkill(skillType);
            return skill?.Effect ?? 0f;
        }

        /// <summary>
        /// Возвращает коэффициент потребление пушки
        /// </summary>
        public float HandleInductionReceived()
        {
            LoadSkills();
            float modifiedinductionMultiplier = 1f;

            var InductionSkill = GetSkill(SkillType.Induction);

            if (InductionSkill != null)
                modifiedinductionMultiplier = (InductionSkill.Effect / 100);

            return modifiedinductionMultiplier;
        }
    }
}