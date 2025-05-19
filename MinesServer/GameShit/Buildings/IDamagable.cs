using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public interface IDamagable
    {
        public void Damage(int i)
        {
            if (ownerid == 0)
                return;
            if (i > 5)
            {
                if (charge - 100 > 0)
                {
                    charge -= 100;
                }
                else
                {
                    charge = 0;
                }
            }
            if (hp == 0)
                return;
            if (hp - i >= 0)
            {
                hp -= i;
                if (hp == 0)
                {
                    brokentimer = ServerTime.Now;
                }
                return;
            }
            hp = 0;
            brokentimer = ServerTime.Now;
        }
        public bool CanDestroy()
        {
            if (ServerTime.Now - brokentimer < TimeSpan.FromHours(8))
            {
                return false;
            }
            return hp == 0;
        }
        public bool NeedEffect()
        {
            if (hp == 0)
            {
                var value = Math.Round((((brokentimer.AddHours(8) - brokentimer) - (brokentimer.AddHours(8) - ServerTime.Now)) / (brokentimer.AddHours(8) - brokentimer)) * 100, 2);
                var r = Physics.r.Next(0, 101);
                if (r > value)
                    return hp == 0;
            }
            return false;
        }
        public abstract void Destroy(Player p);
        public void SendBrokenEffect()
        {
            World.W.GetChunk(x, y).SendFx(x, y, 12);
        }
        public DateTime brokentimer { get; set; }
        public int ownerid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public float charge { get; set; }
        public int hp { get; set; }
        public int maxhp { get; set; }
    }
}
