using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Buildings
{
    public class Gate : Pack
    {
        public override PackType type => PackType.None;
        private Gate() { }

        public Gate(int x, int y, int cid) : base(x, y, 0)
        {
            this.x = x; this.y = y; this.cid = cid;
            World.SetCell(x, y, 30);
            base.Build();
        }
        public override void Build()
        {
            World.SetCell(x, y, 30);
            base.Build();
        }
        public void Destroy()
        {
            World.SetCell(x, y, 32);
            using var db = new DataBase();
            db.gates.Remove(this);
            db.SaveChanges();
        }
        public override Window? GUIWin(Player p)
        {
            return null;
        }

        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32);
        }
    }
}
