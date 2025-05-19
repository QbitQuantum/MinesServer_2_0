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
        public override PackType type => PackType.Up;
        public int hp { get; set; }
        public int maxhp { get; set; }
        public DateTime brokentimer { get; set; }
        public long moneyinside { get; set; }
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
        public override Window? GUIWin(Player p)
        {
            Action? admn = p.id == ownerid ? () => { p.win?.CurrentTab.Open(AdminPage); p.SendWindow(); }
            : null;
            var onskill = (int arg) => { p.skillslist.selectedslot = arg; p.win = GUIWin(p); p.SendWindow(); };
            var oninstall = (int slot, SkillType skilltype) =>
            {
                p.win?.CurrentTab.Replace(new UpPage()
                {
                    OnAdmin = admn,
                    Skills = p.skillslist.GetSkills(),
                    OnSkill = onskill,
                    SlotAmount = p.skillslist.slots,
                    Title = "титле",
                    SkillIcon = skilltype,
                    Text = "описание и цена установки",
                    Button = new MButton("Установить", "confirm", (args) => { p.skillslist.InstallSkill(skilltype.GetCode(), p.skillslist.selectedslot, p); p.win = GUIWin(p); p.SendWindow(); })
                });
                p.SendWindow();
            };
            var skillfromslot = p.skillslist.selectedslot > -1 ? (p.skillslist.skills.ContainsKey(p.skillslist.selectedslot) ? p.skillslist.skills[p.skillslist.selectedslot] : null) : null;
            var uppage = p.skillslist.selectedslot == -1 ? new UpPage()
            {
                OnAdmin = admn,
                Skills = p.skillslist.GetSkills(),
                SkillsToInstall = null,
                SlotAmount = p.skillslist.slots,
                OnSkill = onskill,
                Title = "xxx",
                Text = "Выберите скилл или пустой слот",
                Button = p.skillslist.slots < 34 ? new MButton("buyslotcost", "buyslot", (args) => { if (p.creds > 1000 && p.skillslist.slots < 34) { 
                        using var db = new DataBase();
                        db.Attach(p.skillslist);
                        p.skillslist.slots++;
                        db.SaveChanges();
                    } p.win = GUIWin(p); p.SendWindow(); }) : null,
                SkillIcon = SkillType.Unknown
            } : new UpPage()
            {
                OnAdmin = admn,
                SelectedSlot = p.skillslist.selectedslot,
                Skills = p.skillslist.GetSkills(),
                SkillsToInstall = skillfromslot == null ? p.skillslist.SkillToInstall(p) : null,
                SlotAmount = p.skillslist.slots,
                OnInstall = skillfromslot == null ? oninstall : null,
                OnSkill = onskill,
                Title = "penis",
                Text = skillfromslot?.Description,
                Button = skillfromslot != null && skillfromslot.isUpReady() ? new MButton("ап", "upgrade", (args) => { skillfromslot.Up(p); p.win = GUIWin(p); p.SendWindow(); }) : null,
                OnDelete = skillfromslot != null ? (slot) => { p.skillslist.DeleteSkill(p); p.win = GUIWin(p); p.SendWindow(); } : null,
                SkillIcon = skillfromslot?.type
            };
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "хй",
                    Label = "хуху",
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
            World.SetCell(x, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x, y + 1, 32, false);
        }
        public void Destroy(Player p)
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
