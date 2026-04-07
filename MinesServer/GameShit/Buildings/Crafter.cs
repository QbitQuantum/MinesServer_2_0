using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Sys_Craft;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Buildings
{
    /// <summary>
    /// Crafter building. Stores a single timed crafting job and exposes a clean crafting UI.
    /// </summary>
    public sealed class Crafter : PackDamage
    {
        public Crafter(int x, int y, int ownerid) : base(x, y, ownerid, 1000)
        {
            using var db = new DataBase();
            db.crafts.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Craft;
        [NotMapped] public override int PackId => 24;
        [NotMapped] public override int off => currentcraft is null ? 0 : 1;

        public CraftEntry? currentcraft { get; set; }

        #endregion;

        #region affectworld
        public override void Build()
        {
            // Main body
            World.SetCell(x, y, 37, true);
            World.SetCell(x, y + 1, 37, true);

            // Surrounding decorative / functional cells
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);

            // Side lamps / details
            World.SetCell(x + 1, y - 1, 38, true);
            World.SetCell(x - 1, y - 1, 38, true);

            base.Build();
        }
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.crafts.Remove(this);
            db.SaveChanges();

            // Small chance to refund the Crafter item back to the owner.
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "Крафтер разобран. Часть деталей сохранена.")]));
                p.inventory[24]++;
            }
        }
        #endregion

        public override Window? GUIWin(Player p)
        {
            // Only the owner (or clan owner in future) can manage this crafter
            if (p.id != ownerid)
                return null;

            var initialPage = StaticSystem.GetPageForCrafter(p, this);

            return new Window
            {
                Title = "Крафтер",
                Tabs =
                [
                    new Tab
                    {
                        Action = "crafter_main",
                        Label = "Крафтер",
                        InitialPage = initialPage!,
                    }
                ]
            };
        }
    }
}
