
using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Buildings
{
    // TODO: Установка чувствуется тяжелой
    // Понять почему
    public sealed class Bomb : Pack
    {
        private Bomb() { }
        public Bomb(int ownerid, int x, int y, BombType type) : base(x, y, ownerid)
        {
            Bombtype = type;
        }

        #region fields

        private readonly BombType Bombtype = BombType.PlasmaBomb;
        public override PackType type => PackType.Bomb;
        public override int off { get { return  (int)Bombtype; } }
        public override int PackId
        {
            get
            {
                return Bombtype switch
                {
                    BombType.PlasmaBomb => (int)Item.PlasmaBomb,
                    BombType.ProtonBomb => (int)Item.ProtonBomb,
                    BombType.DischargeBomb => (int)Item.DischargeBomb,
                    _ => -1
                };
            }
        }

        #endregion;
        
        #region affectworld
        protected override void ClearBuilding() { }
        public override void Destroy(Player p)
        {
            World.RemovePack(this);
        }
        #endregion
        public override Window? GUIWin(Player p)
        {
            return null;
        }
    }
}
