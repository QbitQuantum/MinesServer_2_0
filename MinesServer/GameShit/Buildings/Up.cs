using System.ComponentModel.DataAnnotations.Schema;
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

        private IPage AdminPage(Player p) => new Page()
        {
            Title = "UP",
            RichList = new RichListConfig()
            {
                Entries = [
                    RichListEntry.Text("hp", $"Прочность здания {hp}/{maxhp}"),
                    RichListEntry.Button("Доход со здания составляет " + moneyinside, new MButton("Собрать", "collect", _ =>
                    {
                        using var db = new DataBase();
                        p.money += moneyinside;
                        moneyinside = 0;

                        db.Save(this, t => t.moneyinside);

                        p.SendMoney();
                        p.win?.CurrentTab.Refresh();
                        p.SendWindow();
                    }))
                ]
            },
            Buttons = [],
            Rebuild = () => AdminPage(p),
        };

        private static bool TryPurchaseSlot(Player p, int nextSlot)
        {
            if (nextSlot <= 10)
            {
                if (PriceSlots.TryGetValue(nextSlot, out long requiredMoney) && p.money >= requiredMoney)
                {
                    p.money -= requiredMoney;
                    p.skillslist.slots++;
                    return true;
                }
            }
            else if (nextSlot <= 34)
            {
                if (PriceSlots.TryGetValue(nextSlot, out long requiredCreds) && p.creds >= requiredCreds)
                {
                    p.creds -= requiredCreds;
                    p.skillslist.slots++;
                    return true;
                }
            }
            return false;
        }

        private MButton? CreateBuySlotButton(Player p)
        {
            int nextSlot = p.skillslist.slots + 1;
            string buttonText = "Купить слот за ";

            if (nextSlot <= 10)
                buttonText += $"<color=green>{PriceSlots[nextSlot]:N0} $</color>";
            else
                buttonText += $"<color=yellow>{PriceSlots[nextSlot]:N0} C</color>";

            return new MButton(buttonText, "buyslot", (args) =>
            {
                if (p.skillslist.slots < 34)
                {
                    if (TryPurchaseSlot(p, nextSlot))
                    {
                        using var db = new DataBase();
                        db.Save(p.skillslist, sk => sk.slots);
                        db.Save(p, p => p.money, p => p.creds);

                        p.win?.CurrentTab.Refresh();
                        p.SendWindow();
                    }
                }
            });
        }

        private UpPage BuildSkillPage(Player p)
        {
            void onskill(int arg)
            {
                p.skillslist.InstallSlot(arg);
                p.win?.CurrentTab.Refresh();
                p.SendWindow();
            }

            // Создаем базовый объект с общими свойствами
            var basePage = new UpPage
            {
                OnAdmin = p.id == ownerid ? () => { p.win?.CurrentTab.Open(AdminPage(p)); p.SendWindow(); } : null,
                Skills = p.skillslist.GetSkills(),
                SlotAmount = p.skillslist.slots,
                OnSkill = onskill,
            };

            var oninstall = (int slot, SkillType skilltype) =>
            {
                // Получаем текущие навыки игрока для проверки конфликтов
                var currentSkills = p.skillslist.GetSkillTypes();

                // Проверяем конфликты
                var conflictWith = SkillConflicts.CanLearn(skilltype, currentSkills);

                // Создаем описание навыка
                string description = "";

                // Если есть конфликт - добавляем сообщение о конфликте в описание и не показываем кнопку установки
                if (conflictWith.HasValue)
                {
                    description = $"НЕЛЬЗЯ УСТАНОВИТЬ ⚠️\n\n" +
                                  $"Навык \"{skilltype.GetName()}\" конфликтует с навыком \"{conflictWith?.GetName()}\".\n" +
                                  $"Для установки этого навыка необходимо сначала удалить конфликтующий навык.\n\n";
                }
                else
                {
                    var playerSkill = p.skillslist.GetSkill(skilltype);
                    description = playerSkill != null ? playerSkill.GetDescription(p) : SkillTypeExtensions.GetDescription(skilltype);
                }

                var installPage = basePage with
                {
                    SkillIcon = skilltype,
                    Text = description,
                    // Показываем кнопку установки только если нет конфликта
                    Button = !conflictWith.HasValue ? new MButton("Установить", "confirm", (args) =>
                    {
                        if (p.skillslist.InstallSkill(skilltype, p.skillslist.GetCurrentSlot()))
                        {
                            p.SendLvl();
                            // Установили навык — данные изменились, возвращаемся к корню вкладки.
                            p.win?.CurrentTab.Refresh();
                            p.SendWindow();
                        }
                    }) : null
                };

                p.win?.CurrentTab.Replace(installPage);
                p.SendWindow();
            };

            var skillfromslot = p.skillslist.GetSelectedSkill();

            UpPage uppage;

            if (p.skillslist.GetCurrentSlot() == -1)
            {
                uppage = basePage with
                {
                    SkillsToInstall = null,
                    Text = "Выберите скилл или пустой слот",
                    Button = p.skillslist.slots < 34 ? CreateBuySlotButton(p) : null,
                    SkillIcon = SkillType.Unknown
                };
            }
            else
            {
                // Случай: слот выбран
                uppage = basePage with
                {
                    SelectedSlot = p.skillslist.GetCurrentSlot(),
                    SkillsToInstall = skillfromslot == null ? p.skillslist.SkillToInstall() : null,
                    OnInstall = skillfromslot == null ? oninstall : null,
                    Text = skillfromslot?.GetDescription(p),
                    Button = skillfromslot != null && skillfromslot.isUpReady() && p.money > skillfromslot.Cost ?
                        new MButton("Прокачать", "upgrade", (args) =>
                        {
                            long money = (long)(skillfromslot.Cost);
                            p.money -= money;
                            moneyinside += (long)(money * 0.1);

                            using var db = new DataBase();
                            db.Save(this, t => t.moneyinside);
                            db.Save(p.skillslist, skill => skill.ser, skill => skill.expertSkill);

                            p.SendMoney();
                            p.SendSkills(skillfromslot.Up());
                            p.SendLvl();

                            if (skillfromslot.type == SkillType.Movement || skillfromslot.type == SkillType.RoadMovement)
                            {
                                p.SendSpeed();
                            }

                            if (skillfromslot.type == SkillType.Health)
                            {
                                p.MaxHealth = (int)skillfromslot.Effect;
                                p.SendHealth();
                            }

                            p.win?.CurrentTab.Refresh();
                            p.SendWindow();
                        }) :
                        null,
                    OnDelete = skillfromslot != null ?
                        (slot) =>
                        {
                            if (p.skillslist.DeleteSkill())
                            {
                                p.SendLvl();
                                p.win?.CurrentTab.Refresh();
                                p.SendWindow();
                            }
                        }
                    :
                    null,
                    SkillIcon = skillfromslot?.type
                };
            }
            return uppage with { Rebuild = () => BuildSkillPage(p) };
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
        private Card MainTitle(Player p, SkillType skilltype = SkillType.Unknown)
        {
            if (skilltype == SkillType.Unknown)
            {
                string InfoPlayerOpp = "Баллов перепрошивки: <color=yellow>" + p.opp + "</color>";
                return new Card(CardImageType.Skill, SkillType.Architecture.GetCode(),
                    "Удаляя умения вы получаете баллы перепрошивки которые открывают доступ в экспертным умениям.\n" +
                    "Эти умения не доступны в стандратной прошивке робота.\n" +
                    "(Рис.1 - Пример экспертного умения: Архитектура)\n" + InfoPlayerOpp);
            }
            else
            {
                string InfoPlayerOpp = "Стоимость перепрошивки: <color=yellow>" + skilltype.GetBasePriceOPP() + "</color>";
                return new Card(CardImageType.Skill, skilltype.GetCode(),
                    skilltype.GetDescription() + "\n" +
                    InfoPlayerOpp);
            }
        }

        // Открывает отдельную страницу с подробной информацией о выбранном экспертном умении
        // и кнопкой покупки (или "Куплено"/"Назад").
        private Page BuildSkillDetailPage(Player p, SkillType currentDisplaySkill)
        {
            bool isPurchased = p.skillslist.IsExpertSkillPurchased(currentDisplaySkill);
            int price = currentDisplaySkill.GetBasePriceOPP();

            MButton actionButton;

            if (isPurchased)
            {
                // Уже куплено — зелёная надпись, кнопка неактивна
                actionButton = new MButton("<color=lime>Вам уже доступно это умение</color>", "purchased", _ =>
                {
                    // Ничего не делаем — умение уже куплено
                });
            }
            else if (p.opp < price)
            {
                // Не хватает OPP — красная кнопка с указанием недостающего количества
                long missing = price - p.opp;
                actionButton = new MButton(
                    $"<color=red>Не хватает {missing:N0} OPP</color>",
                    "notenough",
                    _ =>
                    {
                        // Ничего не делаем — недостаточно OPP.
                        // Можно перерисовать, чтобы обновить значение (на случай изменения)
                    });
            }
            else
            {
                // Достаточно OPP — обычная кнопка покупки
                actionButton = new MButton($"Купить за <color=yellow>{price}</color> OPP", "buy", _ =>
                {
                    p.opp -= price;
                    p.skillslist.AddExpertSkillPurchased(currentDisplaySkill);

                    using var db = new DataBase();
                    db.Save(p, t => t.opp);

                    // Перерисовываем страницу, чтобы отобразить "Куплено"
                    var refreshed = BuildSkillDetailPage(p, currentDisplaySkill);
                    p.win?.CurrentTab.Replace(refreshed);
                    p.SendWindow();
                });
            }

            return new Page
            {
                OnAdmin = p.id == ownerid
                    ? () => { p.win?.CurrentTab.Open(AdminPage(p)); p.SendWindow(); }
                : null,
                Card = MainTitle(p, currentDisplaySkill),
                Buttons = new[] { actionButton },
            };
        }

        private void OpenSkillPage(Player p, SkillType currentDisplaySkill)
        {
            var detailPage = BuildSkillDetailPage(p, currentDisplaySkill);
            p.win?.CurrentTab.Open(detailPage);
            p.SendWindow();
        }

        // Корневая страница вкладки экспертных умений. Rebuild — да, чтобы Refresh
        // пересобирал её с актуальными ✓/цветами при внешних изменениях.
        private Page TabFlashingPage(Player p)
        {
            var allSkills = SkillTypeExtensions.GetAllInfos();
            var rich = new List<RichListEntry> { };

            foreach (var kvp in allSkills)
            {
                var skillType = kvp.Key;
                var info = kvp.Value;

                if (!info.IsExpertSkill)
                    continue;

                bool isPurchased = p.skillslist.IsExpertSkillPurchased(skillType);

                // Название умения — зелёное, если уже куплено
                string color = isPurchased ? "lime" : "white";

                string skillName = $"Умение <color={color}>{info.Name}</color>";
                if (isPurchased)
                {
                    skillName = $"✓ {skillName}";
                }

                rich.Add(RichListEntry.Button(skillName, new MButton("Подробнее", skillType.ToString(), _ =>
                {
                    // Открываем отдельную страницу с подробностями и кнопкой покупки
                    OpenSkillPage(p, skillType);
                })));
            }

            return new Page
            {
                OnAdmin = p.id == ownerid ? () => { p.win?.CurrentTab.Open(AdminPage(p)); p.SendWindow(); } : null,
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
