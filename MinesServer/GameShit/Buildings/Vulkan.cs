using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.VulkSystem
{
    public class Vulkan : Pack
    {
        #region fields

        [NotMapped] public override PackType type => PackType.Vulkan;
        [NotMapped] public override int PackId { get; set; }
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }
        [NotMapped] public override int ownerid { get; set; }
        public DateTime starttime { get; set; }

        #endregion

        private Vulkan() { }
        
        public Vulkan(int x,int y) : base(x,y,0)
        {
            starttime = ServerTime.Now;
            using var db = new DataBase();
            db.vulkans.Add(this);
            db.SaveChanges();
        }

        public override Window? GUIWin(Player p) => null;

        protected override void ClearBuilding()
        {

        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.vulkans.Remove(this);
            db.SaveChanges();
        }
    }
}
