using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;


namespace MinesServer.GameShit.Buildings
{

    public abstract class PackDamage : Pack
    {
        protected PackDamage() { }

        protected PackDamage(int x, int y, int ownerid, int maxHp) : base(x, y, ownerid)
        {
            maxhp = maxHp;
            hp = maxHp;
            brokentimer = null;
        }

        public virtual DateTime? brokentimer { get; set; }
        public virtual int maxhp { get; set; }
        public virtual int hp { get; set; }
        public virtual float charge { get; set; }
        public virtual float maxcharge { get; set; }
        private static double GetRepairProgressPercentage(DateTime? brokentimer)
        {
            if (brokentimer == null)
                return 0;

            var repairDuration = TimeSpan.FromHours(8);
            var totalMs = repairDuration.TotalMilliseconds;
            var elapsedMs = (ServerTime.Now - brokentimer.Value).TotalMilliseconds;

            var percent = Math.Min(100, Math.Max(0, (elapsedMs / totalMs) * 100));
            return Math.Round(percent, 2);
        }

        public virtual void Damage(int i, DamageTypePacks DamageType = DamageTypePacks.Time)
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
                    if (hp == 0 && brokentimer == null)
                        brokentimer = ServerTime.Now;
                    break;
                default:
                    break;
            }
        }

        public virtual bool CanDestroy()
        {
            if (brokentimer != null && ServerTime.Now - brokentimer.Value < TimeSpan.FromHours(8))
            {
                return false;
            }
            return hp == 0;
        }

        public virtual bool NeedEffect()
        {
            if (hp == 0 && brokentimer != null)
            {
                var percentPassed = GetRepairProgressPercentage(brokentimer);
                var random = Physics.r.Next(0, 101);
                return random > percentPassed;
            }
            return false;
        }

        public virtual void TrySendBrokenEffect()
        {
            if (NeedEffect())
            {
                SendBrokenEffect();
            }
        }

        protected void SendBrokenEffect()
        {
            World.W.GetChunk(x, y).SendFx(x, y, 12);
        }
    }
}