using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinesServer.GameShit.Buildings
{
    public abstract class Pack
    {
        public Pack() {
        }
        public Pack(int x, int y, int ownerid)
        {
            if (x == 0 && y == 0)
                throw new Exception("ЕБАТЬ ЧЕ ЭТО НАХУЙ");
            this.x = x; this.y = y; this.ownerid = ownerid;
        }
        public virtual int id { get; set; }
        public virtual int x { get; set; }
        public virtual int y { get; set; }
        public virtual int cid { get; set; }
        [NotMapped]
        public virtual int off { get; set; }
        public abstract PackType type { get; }
        public virtual int ownerid { get; set; }
        public virtual float charge { get; set; }
        public abstract Window? GUIWin(Player p);
        public virtual void Build()
        {
            World.AddPack(x, y, this);
        }
        protected abstract void ClearBuilding();
        public void Dizz()
        {
            ClearBuilding();
            using var db = new DataBase();
            World.RemovePack(x, y);
            db.Remove(this);
            db.SaveChanges();
        }
        public virtual void Update()
        {
            World.W.GetChunk(x, y).ResendPack(this);
        }
    }
}
