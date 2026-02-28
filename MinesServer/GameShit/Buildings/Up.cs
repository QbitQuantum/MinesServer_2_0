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
    public class Up : Pack, IDamagable
    {
        #region fields
        [NotMapped]
        public override float charge { get; set; }
        public override int PackId => 2;
        public override PackType type => PackType.Up;
        public int hp { get; set; }
        public int maxhp { get; set; }
        public DateTime brokentimer { get; set; }
        public long moneyinside { get; set; }

        public static Dictionary<int, long> PriceSlotsMoney =
            new Dictionary<int, long>()
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
            };

        public static Dictionary<int, int> PriceSlotsCreds =
            new Dictionary<int, int>()
            {
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
        public Up(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            using var db = new DataBase();
            hp = 1000;
            maxhp = 1000;
            db.ups.Add(this);
            db.SaveChanges();
        }
        private Up() {  }
        private IPage AdminPage => new Page()
        {
            Title ="UP",
            RichList = new RichListConfig() {
                Entries = [RichListEntry.Text($"hp {hp}/{maxhp}"), RichListEntry.Text("динаху")]
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
                if (PriceSlotsMoney.TryGetValue(nextSlot, out long requiredMoney) && p.money >= requiredMoney)
                {
                    p.money -= requiredMoney;
                    p.skillslist.slots++;
                    db.SaveChanges();
                    return true;
                }
            }
            else if (nextSlot <= 34)
            {
                if (PriceSlotsCreds.TryGetValue(nextSlot, out int requiredCreds) && p.creds >= requiredCreds)
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
            string buttonText;

            if (nextSlot <= 10)
                buttonText = $"Купить слот за <color=green>{PriceSlotsMoney[nextSlot]:N0} $</color>";
            else
                buttonText = $"Купить слот за  <color=yellow>{PriceSlotsCreds[nextSlot]:N0} C</color>";

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
        public override Window? GUIWin(Player p)
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
                Title = "Здание прокачки умений"
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
                        p.skillslist.InstallSkill(skilltype.GetCode(), p.skillslist.selectedslot, p);
                        p.win = GUIWin(p);
                        p.SendWindow();
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
                    SkillsToInstall = skillfromslot == null ? p.skillslist.SkillToInstall(p) : null,
                    OnInstall = skillfromslot == null ? oninstall : null,
                    Text = skillfromslot?.Description,
                    Button = skillfromslot != null && skillfromslot.isUpReady() ?
                        new MButton("Прокачать", "upgrade", (args) =>
                        {
                            skillfromslot.Up(p);
                            p.win = GUIWin(p);
                            p.SendWindow();
                        }) :
                        null,
                    OnDelete = skillfromslot != null ?
                        (slot) =>
                        {
                            p.skillslist.DeleteSkill(p);
                            p.win = GUIWin(p);
                            p.SendWindow();
                        }
                    :
                        null,
                    SkillIcon = skillfromslot?.type
                };
            }
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "Upgrade",
                    Label = "Здание прокачки",
                    InitialPage = uppage
                }]
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
