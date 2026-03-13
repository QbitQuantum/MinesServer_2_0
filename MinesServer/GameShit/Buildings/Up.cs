using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.GUI.UP;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Buildings
{
    public class Up : PackDamage
    {
        #region fields
        public override int PackId => 2;
        public override PackType type => PackType.Up;
        public long moneyinside { get; set; }

        public static readonly Dictionary<int, long> PriceSlots =
            new()
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
                { 11, 5 },
                { 12, 10 },
                { 13, 25 },
                { 14, 50 },
                { 15, 75 },
                { 16, 100 },
                { 17, 250 },
                { 18, 500 },
                { 19, 750 },
                { 20, 1000 },
                { 21, 1500 },
                { 22, 2000 },
                { 23, 2500 },
                { 24, 3000 },
                { 25, 3500 },
                { 26, 4000 },
                { 27, 4500 },
                { 28, 5000 },
                { 29, 5100 },
                { 30, 5200 },
                { 31, 5300 },
                { 32, 5400 },
                { 33, 5500 },
                { 34, 5600 },
            };
        #endregion
        public Up(int x, int y, int ownerid) : base(x, y, ownerid, 1000)
        {
            using var db = new DataBase();
            db.ups.Add(this);
            db.SaveChanges();
        }
        private Up() {  }
        private IPage AdminPage => new Page()
        {
            Title ="UP",
            RichList = new RichListConfig() {
                Entries = [
                    RichListEntry.Text("hp", $"Прочность здания {hp}/{maxhp}"), 
                    RichListEntry.Button("Доход со здания составляет " + moneyinside, new MButton("Собрать", "collect", _ =>
                    {
                        using var db = new DataBase();
                        var player = DataBase.GetPlayer(ownerid);
                        if (player == null) return;

                        db.Attach(player);
                        db.Attach(this);
                        player.money += moneyinside;
                        moneyinside = 0;
                        db.SaveChanges();
                        player.SendMoney();
                        player.win = GUIWin(player);
                        player.SendWindow();

                    }))]
            },
            Buttons = []
        };
        private bool TryPurchaseSlot(Player p, int nextSlot)
        {
            using var db = new DataBase();
            db.Attach(p.skillslist);
            db.Attach(p);

            if (nextSlot <= 10)
            {
                if (PriceSlots.TryGetValue(nextSlot, out long requiredMoney) && p.money >= requiredMoney)
                {
                    p.money -= requiredMoney;
                    p.skillslist.slots++;
                    db.SaveChanges();
                    return true;
                }
            }
            else if (nextSlot <= 34)
            {
                if (PriceSlots.TryGetValue(nextSlot, out long requiredCreds) && p.creds >= requiredCreds)
                {
                    p.creds -= requiredCreds;
                    p.skillslist.slots++;
                    db.SaveChanges();
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
                        p.win = GUIWin(p);
                        p.SendWindow();
                    }
                }
            });
        }
        private Tab TabSkillPage(Player p)
        {
            Action? admn = p.id == ownerid ? () => { p.win?.CurrentTab.Open(AdminPage); p.SendWindow(); } : null;
            var onskill = (int arg) => { p.skillslist.selectedslot = arg; p.win = GUIWin(p); p.SendWindow(); };

            // Создаем базовый объект с общими свойствами
            var basePage = new UpPage
            {
                OnAdmin = admn,
                Skills = p.skillslist.GetSkills(),
                SlotAmount = p.skillslist.slots,
                OnSkill = onskill,
            };

            var oninstall = (int slot, SkillType skilltype) =>
            {
                var playerSkill = p.skillslist.skills.Values.FirstOrDefault(s => s?.type.GetCode() == skilltype.GetCode());

                var installPage = basePage with
                {
                    SkillIcon = skilltype,
                    Text = playerSkill != null ? playerSkill.Description : SkillTypeExtensions.GetDescription(skilltype),
                    Button = new MButton("Установить", "confirm", (args) =>
                    {
                        if (p.skillslist.InstallSkill(skilltype.GetCode(), p.skillslist.selectedslot))
                        {
                            p.SendLvl();
                            p.win = GUIWin(p);
                            p.SendWindow();
                        }
                    })
                };

                p.win?.CurrentTab.Replace(installPage);
                p.SendWindow();
            };

            var skillfromslot = p.skillslist.selectedslot > -1 ?
                (p.skillslist.skills.ContainsKey(p.skillslist.selectedslot) ? p.skillslist.skills[p.skillslist.selectedslot] : null) :
                null;

            UpPage uppage;

            if (p.skillslist.selectedslot == -1)
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
                    SelectedSlot = p.skillslist.selectedslot,
                    SkillsToInstall = skillfromslot == null ? p.skillslist.SkillToInstall() : null,
                    OnInstall = skillfromslot == null ? oninstall : null,
                    Text = skillfromslot?.Description,
                    Button = skillfromslot != null && skillfromslot.isUpReady() && p.money > skillfromslot.Cost ?
                        new MButton("Прокачать", "upgrade", (args) =>
                        {
                            using (var db = new DataBase())
                            {
                                long money = (long)(skillfromslot.Cost);

                                db.ups.Attach(this);
                                db.players.Attach(p);
                                p.money -= money;

                                // Добавляем 10% от стоимости в moneyinside
                                moneyinside += (long)(money * 0.1);
                                db.SaveChanges();
                            }

                            skillfromslot.Up(p);
                            p.win = GUIWin(p);
                            p.SendWindow();
                        }) :
                        null,
                    OnDelete = skillfromslot != null ?
                        (slot) =>
                        {
                            if (p.skillslist.DeleteSkill())
                            {
                                p.SendLvl();
                                p.win = GUIWin(p);
                                p.SendWindow();
                            }
                        } 
                    : 
                    null,
                    SkillIcon = skillfromslot?.type
                };
            }
            return new Tab()
            {
                Action = "Upgrade",
                Label = "Просмотр умений",
                InitialPage = uppage
            };
        }
        private Card MainTitle(Player p, SkillType skilltype = SkillType.Unknown)
        {
            string InfoPlayerOpp = "Баллов перепрошивки: <color=yellow>" + p.opp + "</color>";
            if (skilltype == SkillType.Unknown)
                return new Card(CardImageType.Skill, SkillType.Architecture.GetCode(),
                    "Удаляя умения вы получаете баллы перепрошивки которые открывают доступ в экспертным умениям.\n" +
                    "Эти умения не доступны в стандратной прошивке робота.\n" +
                    "(Рис.1 - Пример экспертного умения: Архитектура)\n\n" +
                    InfoPlayerOpp);
            else
            {
                return new Card(CardImageType.Skill, skilltype.GetCode(),
                    skilltype.GetName() + "\n" +
                    skilltype.GetDescription() + "\n" +
                    InfoPlayerOpp);
            }
        }
        private void UpdateSkillPage(Player p, SkillType currentDisplaySkill, SkillType? skillToShowPrice = null)
        {
            var allSkills = SkillTypeExtensions.GetAllInfos();
            var rich = new List<RichListEntry> { };

            foreach (var kvp in allSkills)
            {
                var skillType = kvp.Key;
                var info = kvp.Value;

                if (!info.IsExpertSkill)
                    continue;

                // Получаем строковый код умения
                string skillCode = skillType.GetCode();

                // Проверяем, куплено ли это умение игроком
                bool isPurchased = p.skillslist.PurchasedExpertSkills.Contains(skillCode);

                // Получаем цену для этого умения
                int price = skillType.GetBasePriceOPP();

                // Определяем текст и действие кнопки
                string buttonText;
                Action<ActionArgs> buttonAction;

                if (isPurchased)
                {
                    // Если умение уже куплено - показываем зеленую надпись "Куплено"
                    buttonText = "<color=lime>Куплено</color>";
                    buttonAction = _ =>
                    {
                        // Просто показываем информацию без возможности покупки
                        UpdateSkillPage(p, skillType, null);
                    };
                }
                else if (skillToShowPrice.HasValue && skillToShowPrice.Value == skillType)
                {
                    // Если это выбранный скилл для покупки - показываем цену и кнопку покупки
                    buttonText = $"Стоимость исследования: <color=yellow>{price}</color>";
                    buttonAction = _ =>
                    {
                        if (p.opp >= price)
                        {
                            using var db = new DataBase();
                            db.players.Attach(p);
                            db.skills.Attach(p.skillslist);

                            p.opp -= price;

                            // Добавляем умение в список купленных у игрока
                            if (!p.skillslist.PurchasedExpertSkills.Contains(skillCode))
                            {
                                p.skillslist.PurchasedExpertSkills.Add(skillCode);
                            }

                            // Сохраняем изменения
                            p.skillslist.Save();
                            db.SaveChanges();

                            // Обновляем страницу
                            UpdateSkillPage(p, currentDisplaySkill, null);
                        }
                        else
                        {
                            // Можно добавить сообщение о нехватке OPP
                            UpdateSkillPage(p, currentDisplaySkill, null);
                        }
                    };
                }
                else
                {
                    // Обычный режим - показываем "Подробнее" и переключаем отображение
                    buttonText = "Подробнее";
                    buttonAction = _ =>
                    {
                        UpdateSkillPage(p, skillType, skillType);
                    };
                }

                // Добавляем информацию о том, куплен ли скилл в название
                string skillName = info.Name;
                if (isPurchased)
                {
                    skillName = $"✓ {skillName}";
                }

                rich.Add(RichListEntry.Button(skillName, new MButton(buttonText, info.Name, buttonAction)));
            }

            var updatedPage = new Page
            {
                OnAdmin = p.id == ownerid ? () => { p.win?.CurrentTab.Open(AdminPage); p.SendWindow(); } : null,
                Card = MainTitle(p, currentDisplaySkill),
                RichList = new RichListConfig(rich.ToArray(), NoScroll: false),
                Buttons = []
            };

            p.win?.CurrentTab.Open(updatedPage);
            p.SendWindow();
        }
        private Tab TabFlashing(Player p)
        {
            var allSkills = SkillTypeExtensions.GetAllInfos();
            var rich = new List<RichListEntry> { };

            foreach (var kvp in allSkills)
            {
                var skillType = kvp.Key;
                var info = kvp.Value;

                if (!info.IsExpertSkill)
                    continue;

                // Получаем строковый код умения и проверяем, куплено ли оно
                string skillCode = skillType.GetCode();
                bool isPurchased = p.skillslist.PurchasedExpertSkills.Contains(skillCode);

                // Добавляем визуальное отличие для купленных умений
                string skillName = info.Name;
                if (isPurchased)
                {
                    skillName = $"✓ {skillName}"; // Добавляем галочку
                }

                rich.Add(RichListEntry.Button(skillName, new MButton("Подробнее", info.Name, _ =>
                {
                    UpdateSkillPage(p, skillType, skillType);
                })));
            }

            var basePage = new Page
            {
                OnAdmin = p.id == ownerid ? () => { p.win?.CurrentTab.Open(AdminPage); p.SendWindow(); } : null,
                Card = MainTitle(p),
                RichList = new RichListConfig(rich.ToArray(), NoScroll: false),
                Buttons = []
            };

            return new Tab()
            {
                Action = "Flashing",
                Label = "Эксперт. умения",
                InitialPage = basePage
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
            World.SetCell(x, y, 32, false); /* -> */ World.W.cells[x, y] = 32;
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x, y + 1, 32, false); /* -> */ World.W.cells[x, y + 1] = 32;
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.ups.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[2]++;
            }
        }
        #endregion
    }
}