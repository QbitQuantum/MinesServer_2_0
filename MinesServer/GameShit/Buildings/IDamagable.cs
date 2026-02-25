using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public interface IDamagable
    {
        private double GetRepairProgressPercentage()
        {
            var repairDuration = TimeSpan.FromHours(8);
            var totalMs = repairDuration.TotalMilliseconds;
            var elapsedMs = (ServerTime.Now - brokentimer).TotalMilliseconds;

            // Ограничиваем от 0 до 100%
            var percent = Math.Min(100, Math.Max(0, (elapsedMs / totalMs) * 100));
            return Math.Round(percent, 2);
        }
        public void Damage(int i, DamageTypePacks DamageType = DamageTypePacks.Time)
        {
            if (ownerid == 0)
                return;

            switch (DamageType)
            {
                case DamageTypePacks.Raz:
                    charge = Math.Max(0, charge - 100);
                    break;
                case DamageTypePacks.Time:
                    hp = Math.Max(0, hp - i);
                    break;
                default: break;
            }
            
            if (hp == 0)
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
                var percentPassed = GetRepairProgressPercentage();
                var random = Physics.r.Next(0, 101);
                return random > percentPassed;
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
