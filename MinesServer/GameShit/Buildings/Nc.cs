using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public class NC : Pack
    {
        #region fields
        public override PackType type => PackType.NC;
        public int maxhp { get; set; }
        public int hp { get; set; }
        public override int PackId => 46;
        #endregion;
        private NC() { }
        public NC(int ownerid, int x, int y) : base(ownerid, x, y)
        {
            using var db = new DataBase();
            hp = 100;
            db.ncs.Add(this);
            db.SaveChanges();
        }
        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x - 2, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x + 2, y - 1, 32, false);
            World.SetCell(x - 3, y, 32, false);
            World.SetCell(x - 2, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 2, y, 32, false);
            World.SetCell(x + 3, y, 32, false);
            World.SetCell(x - 4, y + 1, 32, false);
            World.SetCell(x - 3, y + 1, 32, false);
            World.SetCell(x - 2, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x + 2, y + 1, 32, false);
            World.SetCell(x + 3, y + 1, 32, false);
            World.SetCell(x + 4, y + 1, 32, false);
            World.SetCell(x - 4, y + 2, 32, false);
            World.SetCell(x - 3, y + 2, 32, false);
            World.SetCell(x - 2, y + 2, 32, false);
            World.SetCell(x + 2, y + 2, 32, false);
            World.SetCell(x + 3, y + 2, 32, false);
            World.SetCell(x + 4, y + 2, 32, false);
            World.SetCell(x - 4, y + 3, 32, false);
            World.SetCell(x - 3, y + 3, 32, false);
            World.SetCell(x - 2, y + 3, 32, false);
            World.SetCell(x + 2, y + 3, 32, false);
            World.SetCell(x + 3, y + 3, 32, false);
            World.SetCell(x + 4, y + 3, 32, false);
            World.SetCell(x - 1, y + 2, 32, false);
            World.SetCell(x, y + 2, 32, false);
            World.SetCell(x + 1, y + 2, 32, false);
            World.SetCell(x - 1, y + 3, 32, false);
            World.SetCell(x, y + 3, 32, false);
            World.SetCell(x + 1, y + 3, 32, false);
            World.SetCell(x - 1, y + 4, 32, false);
            World.SetCell(x, y + 4, 32, false);
            World.SetCell(x + 1, y + 4, 32, false);
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.ncs.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[3]++;
            }
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x, y + 1, 37, true);
            World.SetCell(x - 2, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x + 2, y - 1, 106, true);
            World.SetCell(x - 3, y, 106, true);
            World.SetCell(x - 2, y, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 2, y, 106, true);
            World.SetCell(x + 3, y, 106, true);
            World.SetCell(x - 4, y + 1, 106, true);
            World.SetCell(x - 3, y + 1, 106, true);
            World.SetCell(x - 2, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x + 2, y + 1, 106, true);
            World.SetCell(x + 3, y + 1, 106, true);
            World.SetCell(x + 4, y + 1, 106, true);
            World.SetCell(x - 4, y + 2, 106, true);
            World.SetCell(x - 3, y + 2, 106, true);
            World.SetCell(x - 2, y + 2, 106, true);
            World.SetCell(x + 2, y + 2, 106, true);
            World.SetCell(x + 3, y + 2, 106, true);
            World.SetCell(x + 4, y + 2, 106, true);
            World.SetCell(x - 4, y + 3, 106, true);
            World.SetCell(x - 3, y + 3, 106, true);
            World.SetCell(x - 2, y + 3, 106, true);
            World.SetCell(x + 2, y + 3, 106, true);
            World.SetCell(x + 3, y + 3, 106, true);
            World.SetCell(x + 4, y + 3, 106, true);
            World.SetCell(x - 1, y + 2, 35, true);
            World.SetCell(x, y + 2, 35, true);
            World.SetCell(x + 1, y + 2, 35, true);
            World.SetCell(x - 1, y + 3, 35, true);
            World.SetCell(x, y + 3, 35, true);
            World.SetCell(x + 1, y + 3, 35, true);
            World.SetCell(x - 1, y + 4, 35, true);
            World.SetCell(x, y + 4, 35, true);
            World.SetCell(x + 1, y + 4, 35, true);
            base.Build();
        }
        #endregion
        
        private Tab TabNC(Player p)
        {
            return new Tab()
            {
                Label = "",
                Action = "",
                InitialPage = new Page()
                {
                    Buttons = [],
                }
            };
        }
        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "Научный центр",
                Tabs = [TabNC(p)]
            };
        }
    }
}
