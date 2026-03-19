
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Enums;

public abstract class PackCharge : PackDamage
{
    protected PackCharge() { }

    protected PackCharge(int x, int y, int ownerid, int maxHp, int maxCharge) : base(x, y, ownerid, maxHp)
    {
        charge = maxCharge;
        maxcharge = maxCharge;
    }

    public virtual float charge { get; set; }
    public virtual float maxcharge { get; set; }

    // TODO: Стоит разделить эти методы на явные
    public override void Damage(int i, DamageTypePacks DamageType = DamageTypePacks.Time)
    {
        if (ownerid == 0)
            return;

        switch (DamageType)
        {
            case DamageTypePacks.Raz:
                charge = Math.Max(0, charge - 100);
                // TODO: Наверное стоит убрать
                // Потому что игровой цикл вызывает потоянный апдейт пака
                if (charge == 0)
                    Update();
                break;
            default:
                base.Damage(i, DamageType);
                break;
        }
    }
}