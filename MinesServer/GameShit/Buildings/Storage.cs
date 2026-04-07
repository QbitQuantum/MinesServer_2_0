using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

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

        private long[] _crysinside = new long[6];
        public long this[int index]
        {
            get => _crysinside[index];
            set => _crysinside[index] = value;
        }

        public long ze
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public long cr
        {
            get { return this[1]; }
            set { this[1] = value; }
        }

        public long si
        {
            get { return this[2]; }
            set { this[2] = value; }
        }

        public long be
        {
            get { return this[3]; }
            set { this[3] = value; }
        }

        public long fi
        {
            get { return this[4]; }
            set { this[4] = value; }
        }

        public long go
        {
            get { return this[5]; }
            set { this[5] = value; }
        }

        #endregion

        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false); /* -> */ World.W.cells[x, y] = 32;
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y + 1, 35, false);
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
                Box.BuildBox(x, y, _crysinside, null);
                _crysinside = new long[6];
            }
            using var db = new DataBase();
            db.storages.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[29]++;
            }
        }
        #endregion
        private void StockTransfer(long[]? sliders, Player p)
        {
            if (sliders == null)
                return;
            for (int i = 0; i < 6; i++)
            {
                var count = p.crys.cry[i] + this[i];
                if (count - sliders[i] >= 0 && sliders[i] >= 0)
                {
                    p.crys.cry[i] = count - sliders[i];
                    this[i] = sliders[i];
                }
            }
            p.win = GUIWin(p);
        }
        private Page MainPage(Player p) => new Page(){
                Title = "Склад",
                CrystalConfig = new CrystalConfig(
                    " ",
                    " ",
                    _crysinside.Select((cry, id) => new CrysLine("", 0, 0, p.crys.cry[id] + cry, (int)(cry))).ToArray()
                    ),
                Buttons = [
                    new MButton("transfer", $"transfer:{ActionMacros.CrystalSliders}", 
                    (args) =>
                    StockTransfer(args.CrystalSliders, p))]
            };
        
        public override Window? GUIWin(Player p) => new Window(){
            Tabs = [new Tab()
                {
                    Action = "хй",
                    Label = "хуху",
                    InitialPage = MainPage(p)
                }]
        };
    }
}
