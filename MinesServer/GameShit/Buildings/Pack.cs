using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;
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
        public virtual int off { get; set; }
        public virtual int PackId { get; set; } = -1;
        public virtual PackType type { get; }
        public virtual int ownerid { get; set; }

        public abstract Window? GUIWin(Player p);
        protected abstract void ClearBuilding();
        public abstract void Destroy(Player p);
        
        public virtual void Update()
        {
            World.UpdatePack(this);
        }
        public virtual void Build()
        {
            World.AddPack(this);
        }
    }
}
