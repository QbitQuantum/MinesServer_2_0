using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
namespace MinesServer.GameShit.Buildings
{
    public abstract class Pack : Entity
    {
        public Pack() {
        }
        public Pack(int x, int y, int ownerid)
        {
            if (x == 0 && y == 0)
                throw new Exception("Админ, не занимайся фигнёй");
            this.x = x; this.y = y; this.ownerid = ownerid;
        }
        public virtual int cid { get; set; }
        [NotMapped]
        public virtual int off { get; set; }
        [NotMapped]
        public virtual int PackId { get; set; } = -1;
        public abstract PackType type { get; }
        public virtual int ownerid { get; set; }
        public virtual float charge { get; set; }
        public abstract Window? GUIWin(Player p);
        public virtual void Build()
        {
            World.AddPack(x, y, this);
        }
        protected abstract void ClearBuilding();
        public abstract void Destroy(Player p);
        public virtual void Update()
        {
            World.W.GetChunk(x, y).ResendPack(this);
        }
    }
}
