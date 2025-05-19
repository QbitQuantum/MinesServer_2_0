using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.VulkSystem
{
    public class Vulkan : Pack
    {
        #region shit
        [NotMapped]
        public override int cid { get; set; }
        [NotMapped]
        public override float charge { get; set; }
        [NotMapped]
        public override int ownerid { get; set; }
        #endregion
        public override PackType type => PackType.Vulkan;
        private Vulkan() { }
        public DateTime starttime { get; set; }
        public Vulkan(int x,int y) : base(x,y,0)
        {
            starttime = ServerTime.Now;
            using var db = new DataBase();
            db.vulkans.Add(this);
            db.SaveChanges();
        }
        public override void Build()
        {
            base.Build();
        }
        public override Window? GUIWin(Player p) => null;

        protected override void ClearBuilding()
        {

        }
        public void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.vulkans.Remove(this);
            db.SaveChanges();
        }
    }
}
