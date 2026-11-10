using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Cards;
using MinesServer.GameShit.GUI.Rich;
using MinesServer.GameShit.GUI.Styles;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public sealed class Up : PackDamage
    {
        private Up() { }

        public Up(int x, int y, int ownerid) : base(x, y, ownerid, 1000)
        {
            using var db = new DataBase();
            db.ups.Add(this);
            db.SaveChanges();
        }
        #region fields

        [NotMapped] public override PackType type => PackType.Up;
        [NotMapped] public override int PackId => 2;
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }
        public long moneyinside { get; set; }

        public static readonly Dictionary<int, long> PriceSlots = new()
        {
            { 1, 2_500_000 },
            { 2, 5_000_000 },
            { 3, 7_500_000 },
            { 4, 10_000_000 },
            { 5, 12_500_000 },
            { 6, 15_000_000 },
            { 7, 17_500_000 },
            { 8, 20_000_000 },
            { 9, 22_500_000 },
            { 10, 25_000_000 },
            { 11, 5 }, { 12, 10 },
            { 13, 25 }, { 14, 50 },
            { 15, 75 }, { 16, 100 },
            { 17, 250 }, { 18, 500 },
            { 19, 750 }, { 20, 1000 },
            { 21, 1500 }, { 22, 2000 },
            { 23, 2500 }, { 24, 3000 },
            { 25, 3500 }, { 26, 4000 },
            { 27, 4500 }, { 28, 5000 },
            { 29, 5100 }, { 30, 5200 },
            { 31, 5300 }, { 32, 5400 },
            { 33, 5500 }, { 34, 5600 },
        };

        #endregion

        #region helpers

        /// <summary>
        /// Пересобирает текущую страницу вкладки (через IPage.Rebuild) и отправляет окно клиенту.
        /// </summary>
        private static void RefreshAndSend(Player p)
        {
            p.win?.CurrentTab.Refresh();
            p.SendWindow();
        }

        /// <summary>
        /// Открывает админ-страницу текущего здания во вкладке игрока.
        /// </summary>
        private void OpenAdmin(Player p)
        {
            p.win?.CurrentTab.Open(AdminPage(p));
            p.SendWindow();
        }

        #endregion

        #region admin page

        private void CollectIncome(Player p)
        {
            using var db = new DataBase();
            p.money += moneyinside;
            moneyinside = 0;

            db.Save(this, t => t.moneyinside);

            p.SendMoney();
            RefreshAndSend(p);
        }

        private IPage AdminPage(Player p) => new Page()
        {
            Title = "UP",
            RichList = new RichListConfig()
            {
                Entries =
                [
                    RichListEntry.Text("hp", $"Прочность здания {hp}/{maxhp}"),
                    RichListEntry.Button(
                        $"Доход со здания составляет {moneyinside}",
                        new MButton("Собрать", "collect", _ => CollectIncome(p)))
                ]
            },
            Buttons = [],
            Rebuild = () => AdminPage(p),
        };

        #endregion

        #region slot purchase

        private static bool TryPurchaseSlot(Player p, int nextSlot)
        {
            if (nextSlot <= 10)
            {
                if (!PriceSlots.TryGetValue(nextSlot, out long requiredMoney))
                    return false;

                if (p.money < requiredMoney)
                    return false;

                p.money -= requiredMoney;
                p.skillslist.slots++;
                return true;
            }

            if (nextSlot > 34)
                return false;

            if (!PriceSlots.TryGetValue(nextSlot, out long requiredCreds))
                return false;

            if (p.creds < requiredCreds)
                return false;

            p.creds -= requiredCreds;
            p.skillslist.slots++;
            return true;
        }

        private static void BuySlot(Player p, int nextSlot)
        {
            if (p.skillslist.slots >= 34)
                return;

            if (!TryPurchaseSlot(p, nextSlot))
                return;

            using var db = new DataBase();
            db.Save(p.skillslist, sk => sk.slots);
            db.Save(p, pl => pl.money, pl => pl.creds);

            RefreshAndSend(p);
        }

        private static MButton? CreateBuySlotButton(Player p)
        {
            int nextSlot = p.skillslist.slots + 1;

            if (nextSlot > 34) return null;

            var sb = new StringBuilder("Купить слот за ");

            if (nextSlot <= 10)
                sb.Append($"<color=green>{PriceSlots[nextSlot]:N0} $</color>");
            else
                sb.Append($"<color=yellow>{PriceSlots[nextSlot]:N0} C</color>");

            string buttonText = sb.ToString();

            return new MButton(buttonText, "buyslot", _ => BuySlot(p, nextSlot));
        }

        #endregion

        #region skill page

        private static void InstallSlot(Player p, int arg)
        {
            p.skillslist.InstallSlot(arg);
            RefreshAndSend(p);
        }

        private static void UpdateStatePlayer(Player p, SkillType skilltype)
        {
            if (skilltype == SkillType.Movement || skilltype == SkillType.RoadMovement)
                p.SendSpeed();

            if (skilltype == SkillType.Health)
            {
                p.UpdateMaxHp();
                p.SendHealth();
            }
            p.SendLvl();
            RefreshAndSend(p);
        }

        private static void ConfirmSkillInstall(Player p, SkillType skilltype)
        {
            if (!p.skillslist.InstallSkill(skilltype, p.skillslist.GetCurrentSlot()))
                return;
            UpdateStatePlayer(p, skilltype);
        }

        private void UpgradeSkill(Player p, Skill skillfromslot)
        {
            long money = (long)skillfromslot.Cost;
            p.money -= money;
            moneyinside += (long)(money * 0.1);

            using var db = new DataBase();
            db.Save(this, t => t.moneyinside);
            db.Save(p.skillslist, skill => skill.standartSkills, skill => skill.expertSkill);

            p.SendMoney();
            p.SendSkills(skillfromslot.Up());
            UpdateStatePlayer(p, skillfromslot.Type);
        }

        private static void DeleteSkill(Player p)
        {
            var currentSkill = p.skillslist.GetSelectedSkill();
            if (currentSkill == null) return;

            var skilltype = currentSkill.Type;

            if (!p.skillslist.DeleteSkill())
                return;

            UpdateStatePlayer(p, skilltype);
        }

        private static string BuildInstallDescription(Player p, SkillType skilltype, SkillType? conflictWith)
        {
            if (!conflictWith.HasValue)
            {
                var playerSkill = p.skillslist.GetSkill(skilltype);
                return playerSkill != null
                    ? playerSkill.GetDescription(p)
                    : SkillTypeExtensions.GetDescription(skilltype);
            }

            var sb = new StringBuilder();
            sb.AppendLine("НЕЛЬЗЯ УСТАНОВИТЬ ⚠️");
            sb.AppendLine();
            sb.AppendLine($"Навык \"{skilltype.GetName()}\" конфликтует с навыком \"{conflictWith?.GetName()}\".");
            sb.AppendLine("Для установки этого навыка необходимо сначала удалить конфликтующий навык.");
            sb.AppendLine();
            return sb.ToString();
        }

        private void ShowInstallPage(Player p, SkillType skilltype, SkillType? conflictWith, UpPage basePage)
        {
            string description = BuildInstallDescription(p, skilltype, conflictWith);

            var installPage = basePage with
            {
                SkillIcon = skilltype,
                Text = description,
                Rebuild = () => BuildSkillPage(p),
                Button = !conflictWith.HasValue
                    ? new MButton("Установить", "confirm", _ => ConfirmSkillInstall(p, skilltype)) : null
            };

            p.win?.CurrentTab.Replace(installPage);
            p.SendWindow();
        }

        private UpPage BuildSkillPage(Player p)
        {
            void onskill(int arg) => InstallSlot(p, arg);

            Action<int, SkillType> oninstall = (slot, skilltype) =>
            {
                // Получаем текущие навыки игрока для проверки конфликтов
                var currentSkills = p.skillslist.GetSkillTypes();

                // Проверяем конфликты
                var conflictWith = SkillConflicts.CanLearn(skilltype, currentSkills);

                // Создаем базовый объект с общими свойствами
                var basePage = new UpPage
                {
                    OnAdmin = p.id == ownerid ? () => OpenAdmin(p) : null,
                    Skills = p.skillslist.GetSkills(),
                    SlotAmount = p.skillslist.slots,
                    OnSkill = onskill,
                };

                ShowInstallPage(p, skilltype, conflictWith, basePage);
            };

            var skillfromslot = p.skillslist.GetSelectedSkill();

            // Случай: слот не выбран
            if (p.skillslist.GetCurrentSlot() == -1)
            {
                var emptyPage = new UpPage
                {
                    OnAdmin = p.id == ownerid ? () => OpenAdmin(p) : null,
                    Skills = p.skillslist.GetSkills(),
                    SlotAmount = p.skillslist.slots,
                    OnSkill = onskill,
                    SkillsToInstall = null,
                    Text = "Выберите скилл или пустой слот",
                    Button = p.skillslist.slots < 34 ? CreateBuySlotButton(p) : null,
                    SkillIcon = SkillType.Unknown,
                };
                return emptyPage with { Rebuild = () => BuildSkillPage(p) };
            }

            // Случай: слот выбран
            var selectedPage = new UpPage
            {
                OnAdmin = p.id == ownerid ? () => OpenAdmin(p) : null,
                Skills = p.skillslist.GetSkills(),
                SlotAmount = p.skillslist.slots,
                OnSkill = onskill,
                SelectedSlot = p.skillslist.GetCurrentSlot(),
                SkillsToInstall = skillfromslot == null ? p.skillslist.SkillToInstall() : null,
                OnInstall = skillfromslot == null ? oninstall : null,
                Text = skillfromslot?.GetDescription(p),
                Button = skillfromslot != null && skillfromslot.IsUp && p.money > skillfromslot.Cost
                    ? new MButton("Прокачать", "upgrade", _ => UpgradeSkill(p, skillfromslot))
                    : null,
                OnDelete = skillfromslot != null ? _ => DeleteSkill(p) : null,
                SkillIcon = skillfromslot?.Type,
            };

            return selectedPage with { Rebuild = () => BuildSkillPage(p) };
        }

        private Tab TabSkillPage(Player p)
        {
            return new Tab()
            {
                Action = "Upgrade",
                Label = "Просмотр умений",
                InitialPage = BuildSkillPage(p)
            };
        }

        #endregion

        #region cards / titles

        private static Card MainTitle(Player p, SkillType skilltype = SkillType.Unknown)
        {
            if (skilltype != SkillType.Unknown)
            {
                var descriptionSb = new StringBuilder()
                    .AppendLine(skilltype.GetDescription())
                    .Append("Стоимость перепрошивки: <color=yellow>")
                    .Append(skilltype.GetBasePriceOPP())
                    .Append("</color>")
                    .ToString();

                return new Card(
                    CardImageType.Skill,
                    skilltype.GetCode(),
                    descriptionSb);
            }

            var description = new StringBuilder()
                .AppendLine("Удаляя умения вы получаете баллы перепрошивки которые открывают доступ в экспертным умениям.")
                .AppendLine("Эти умения не доступны в стандратной прошивке робота.")
                .AppendLine("(Рис.1 - Пример экспертного умения: Архитектура)")
                .Append("Баллов перепрошивки: <color=yellow>")
                .Append(p.opp)
                .Append("</color>")
                .ToString();

            return new Card(
                CardImageType.Skill,
                SkillType.Architecture.GetCode(),
                description);
        }

        #endregion

        #region expert skills

        private void BuyExpertSkill(Player p, SkillType skill, int price)
        {
            p.opp -= price;
            p.skillslist.AddExpertSkillPurchased(skill);

            using var db = new DataBase();
            db.Save(p, t => t.opp);

            // Перерисовываем страницу, чтобы отобразить "Куплено"
            var refreshed = BuildSkillDetailPage(p, skill);
            p.win?.CurrentTab.Replace(refreshed);
            p.SendWindow();
        }

        private MButton BuildExpertSkillButton(Player p, SkillType skill, int price)
        {
            if (p.skillslist.IsExpertSkillPurchased(skill))
                return new MButton("<color=lime>Вам уже доступно это умение</color>", "purchased", _ => { });

            if (p.opp < price)
            {
                long missing = price - p.opp;
                var sb = new StringBuilder()
                    .Append("<color=red>Не хватает ")
                    .Append(missing.ToString("N0"))
                    .Append(" OPP</color>");

                return new MButton(sb.ToString(), "notenough", _ => { });
            }

            var buySb = new StringBuilder()
                .Append("Купить за <color=yellow>")
                .Append(price)
                .Append("</color> OPP");

            return new MButton(buySb.ToString(), "buy", _ => BuyExpertSkill(p, skill, price));
        }

        // Открывает отдельную страницу с подробной информацией о выбранном экспертном умении
        private Page BuildSkillDetailPage(Player p, SkillType currentDisplaySkill)
        {
            int price = currentDisplaySkill.GetBasePriceOPP();
            MButton actionButton = BuildExpertSkillButton(p, currentDisplaySkill, price);

            return new Page
            {
                OnAdmin = p.id == ownerid ? () => OpenAdmin(p) : null,
                Card = MainTitle(p, currentDisplaySkill),
                Buttons = new[] { actionButton },
                Rebuild = () => BuildSkillDetailPage(p, currentDisplaySkill),
            };
        }

        private void OpenSkillPage(Player p, SkillType currentDisplaySkill)
        {
            var detailPage = BuildSkillDetailPage(p, currentDisplaySkill);
            p.win?.CurrentTab.Open(detailPage);
            p.SendWindow();
        }

        // Корневая страница вкладки экспертных умений
        private Page TabFlashingPage(Player p)
        {
            var allSkills = SkillTypeExtensions.GetAllInfos();
            var rich = new List<RichListEntry>();

            foreach (var kvp in allSkills)
            {
                var skillType = kvp.Key;
                var info = kvp.Value;

                if (!info.IsExpertSkill)
                    continue;

                bool isPurchased = p.skillslist.IsExpertSkillPurchased(skillType);

                // Название умения — зелёное, если уже куплено
                string color = isPurchased ? "lime" : "white";

                var sb = new StringBuilder();
                if (isPurchased)
                    sb.Append("✓ ");

                sb.Append("Умение <color=")
                  .Append(color)
                  .Append('>')
                  .Append(info.Name)
                  .Append("</color>");

                string skillName = sb.ToString();

                rich.Add(RichListEntry.Button(skillName, new MButton("Подробнее", skillType.ToString(), _ => OpenSkillPage(p, skillType))));
            }

            return new Page
            {
                OnAdmin = p.id == ownerid ? () => OpenAdmin(p) : null,
                Card = MainTitle(p),
                RichList = new RichListConfig(rich.ToArray(), NoScroll: false),
                Buttons = [],
                // Корневая страница вкладки — при Refresh пересобирается с актуальными данными
                Rebuild = () => TabFlashingPage(p),
                Style = new Style()
                {
                    ScrollHeight = 350
                }
            };
        }

        private Tab TabFlashing(Player p)
        {
            return new Tab()
            {
                Action = "Flashing",
                Label = "Эксперт. умения",
                InitialPage = TabFlashingPage(p),
            };
        }

        #endregion
        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "Здание прокачки умений",
                Tabs = [TabSkillPage(p), TabFlashing(p)]
            };
        }
        #region affectworld
        public override void Build()
        {
            World.SetCell(x - 1, y - 2, 38, true);
            World.SetCell(x + 1, y - 2, 38, true);
            World.SetCell(x, y - 2, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x, y, 37, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x, y + 1, 37, true);
            base.Build();
        }
        protected override void ClearBuilding()
        {
            World.SetCell(x - 1, y - 2, 32, false);
            World.SetCell(x + 1, y - 2, 32, false);
            World.SetCell(x, y - 2, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x, y + 1, 32, false);
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.ups.Remove(this);
            db.SaveChanges();

            TryToReturnInventory(p);
        }
        #endregion
    }
}
