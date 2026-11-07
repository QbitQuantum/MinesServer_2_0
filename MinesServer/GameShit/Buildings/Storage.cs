using System.ComponentModel.DataAnnotations.Schema;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Crystal;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public sealed class Storage : PackDamage
    {
        private Storage() { }
        public Storage(int x, int y, int ownerid) : base(x, y, ownerid, 1000)
        {
            using var db = new DataBase();
            db.storages.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Storage;
        [NotMapped] public override int PackId => 29;
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }

        private readonly long[] _crysinside = new long[6];

        #endregion

        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y + 1, 32, false);
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x, y + 1, 35, true);
            base.Build();
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            if (_crysinside.Sum() > 0)
            {
                Box.BuildBox(x, y, _crysinside);
            }
            using var db = new DataBase();
            db.storages.Remove(this);
            db.SaveChanges();

            TryToReturnInventory(p);
        }
        #endregion
        private void StockTransfer(long[]? sliders, Player p)
        {
            if (sliders == null) return;

            for (int i = 0; i < 6; i++)
            {
                var count = p.crys.crystall[i] + _crysinside[i];

                if (count - sliders[i] < 0 || sliders[i] < 0)
                    continue;

                p.crys.crystall[i] = count - sliders[i];

                _crysinside[i] = sliders[i];
            }
            p.win = GUIWin(p);
        }

        private Page MainPage(Player p) => new Page()
        {
            Title = "Склад",
            CrystalConfig = new CrystalConfig
            (
                " ", " ", _crysinside.Select((cry, id) =>
                new CrysLine("", 0, 0, p.crys.crystall[id] + cry, cry)).ToArray()
            ),
            Buttons = [
                new MButton("transfer", $"transfer:{ActionMacros.CrystalSliders}", (args) => StockTransfer(args.CrystalSliders, p))
                ]
        };
        
        public override Window? GUIWin(Player p) => new Window(){
            Tabs = [new Tab()
            {
                Action = "OpenStorage",
                Label = "Склад",
                InitialPage = MainPage(p)
            }]
        };
    }
}
