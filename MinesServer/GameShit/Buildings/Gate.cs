using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public sealed class Gate : Pack
    {
        private Gate() { }

        public Gate(int x, int y, int cid) : base(x, y, 0)
        {
            this.x = x; this.y = y; this.cid = cid;
        }

        #region fields

        [NotMapped] public override int PackId => 27;
        [NotMapped] public override PackType type => PackType.None;
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }

        #endregion;

        public override void Build()
        {
            World.SetCell(x, y, 30);
            base.Build();
        }
        public override void Destroy(Player p)
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
