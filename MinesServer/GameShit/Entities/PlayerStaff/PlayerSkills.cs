using System.ComponentModel.DataAnnotations.Schema;
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
        public PlayerSkills() { }

        [NotMapped] private readonly HashSet<string> _purchasedExpertSkills = [];

        [NotMapped] private readonly Dictionary<int, Skill> _purchasedStandartSkills = [];

        [NotMapped] private int selectedslot = -1;

        public int id { get; set; }

        public int slots { get; set; }

        /// <summary>
        /// Сериализованное представление словаря слот -> (код навыка, уровень, опыт).
        /// </summary>
        public string standartSkills { get; set; } = "";

        /// <summary>
        /// JSON поле для хранения купленных экспертных скиллов (храним строковые коды)
        /// </summary>
        public string expertSkill { get; set; } = "[]";

        /// <summary>
        /// Установка умений по умолчанию
        /// </summary>
        public void Init()
        {
            var DefaultSkills = new List<Skill>
            {
                new(level: 1, expiriense: 0, type: SkillType.MineGeneral),
                new(level: 1, expiriense: 0, type: SkillType.Digging),
                new(level: 1, expiriense: 0, type: SkillType.Movement),
                new(level: 1, expiriense: 0, type: SkillType.Health)
            };

            for (int i = 0; i < DefaultSkills.Count; i++)
                _purchasedStandartSkills[i] = DefaultSkills[i];
            slots = _purchasedStandartSkills.Count;
            Save();
        }

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
            return _purchasedStandartSkills.Values.FirstOrDefault(s => s?.Type == Type);
        }

        /// <summary>
        /// Возвращает навык на указанном слоте
        /// </summary>
        private Skill? GetSkillAtSlot(int slot)
        {
            if (slot > -1 && _purchasedStandartSkills.TryGetValue(slot, out Skill? value))
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
            return _purchasedStandartSkills.Values
                    .Where(s => s != null)
                    .Select(s => s.Type)
                    .ToList();
        }

        public void LoadStandartSkills()
        {
            if (_purchasedStandartSkills.Count > 0 || string.IsNullOrEmpty(standartSkills))
                return;

            var loadStandartSkills = Newtonsoft.Json.JsonConvert
                .DeserializeObject<Dictionary<int, PlayerSkillData>>(standartSkills) ?? [];

            foreach (var kvp in loadStandartSkills)
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
            if (!_purchasedStandartSkills.ContainsKey(selectedslot))
                return false;

            _purchasedStandartSkills.Remove(selectedslot);
            Save();
            return true;
        }

        /// <summary>
        /// Проверяет, выполнены ли требования для указанного навыка
        /// </summary>
        private bool MeetsRequirements(SkillType skillType)
        {
            var info = skillType.GetInfo();
            if (info?.Requirements == null || info.Requirements.Count == 0)
                return true; // Нет требований - можно устанавливать

            foreach (var req in info.Requirements)
            {
                var hasReq = _purchasedStandartSkills.Values.Any(s => s?.IsRequiered(req.RequiredSkill, req.RequiredLevel) ?? false);
                if (!hasReq) return false; // Не выполнено хотя бы одно требование
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

            if (_purchasedStandartSkills.ContainsKey(slot))
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

        private void ForceInstallSkill(SkillType skillType, int slot, int lvl, float exp)
        {
            _purchasedStandartSkills[slot] = new Skill(lvl, exp, skillType);
        }

        private void Save()
        {
            var raw = new Dictionary<int, PlayerSkillData>(_purchasedStandartSkills.Count);

            foreach (var kvp in _purchasedStandartSkills)
            {
                var skill = kvp.Value;
                raw[kvp.Key] = new PlayerSkillData
                {
                    Code = skill.Type.GetCode(),
                    Level = skill.Level,
                    Exp = skill.Expiriense
                };
            }

            standartSkills = Newtonsoft.Json.JsonConvert.SerializeObject(raw, Newtonsoft.Json.Formatting.None);

            expertSkill = Newtonsoft.Json.JsonConvert.SerializeObject(_purchasedExpertSkills);

            using var db = new DataBase();
            db.Save(this, t => t.standartSkills, t => t.expertSkill, t => t.slots);
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
                if (_purchasedStandartSkills.Values.Any(s => s?.Type == skillType))
                    continue;

                // Для экспертных навыков проверяем, куплен ли он
                if (info.IsExpertSkill && !IsExpertSkillPurchased(skillType))
                    continue;

                // Если нет требований - сразу доступен
                if (info?.Requirements == null || info.Requirements.Count == 0)
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

        public int lvlsummary() => _purchasedStandartSkills.Sum(i => i.Value?.Level ?? 0);

        public UpSkill[] GetSkills()
        {
            LoadStandartSkills();

            List<UpSkill> ski = [];
            
            for (int i = 0; i < slots; i++)
            {
                if (_purchasedStandartSkills.TryGetValue(i, out Skill? value) && value is not null)
                {
                    ski.Add(new UpSkill(i, value.Level, value.IsUp, value.Type));
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
            LoadStandartSkills();
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
            LoadStandartSkills();

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
            LoadStandartSkills();

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
            LoadStandartSkills();
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
            LoadStandartSkills();
            var miningSkill = GetSkill(SkillType.MineGeneral);
            miningSkill ??= GetSkill(SkillType.ExpertMining);
            return miningSkill?.Effect ?? 0f;
        }

        public bool HasSkill(SkillType skillType)
        {
            LoadStandartSkills();
            var skill = GetSkill(skillType);
            return skill != null;
        }

        public float GetSkillEffect(SkillType skillType)
        {
            LoadStandartSkills();
            var skill = GetSkill(skillType);
            return skill?.Effect ?? 0f;
        }

        /// <summary>
        /// Возвращает коэффициент потребление пушки
        /// </summary>
        public float HandleInductionReceived()
        {
            LoadStandartSkills();
            float modifiedinductionMultiplier = 1f;

            var InductionSkill = GetSkill(SkillType.Induction);

            if (InductionSkill != null)
                modifiedinductionMultiplier = (InductionSkill.Effect / 100);

            return modifiedinductionMultiplier;
        }
    }
}