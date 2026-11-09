using System.ComponentModel.DataAnnotations.Schema;

using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Crystal;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using Newtonsoft.Json;

namespace MinesServer.GameShit.Buildings
{
    public sealed class Box : Pack
    {
        private Box() { }

        public Box(int x, int y, int ownerid, long[] crystalls) : base(x, y, ownerid)
        {
            for (int i = 0; i < 6 && i < crystalls.Length; i++)
                _crysinside[i] = crystalls[i];

            Crystalls = JsonConvert.SerializeObject(_crysinside);
            using var db = new DataBase();
            db.boxes.Add(this);
            db.SaveChanges();
            db.Save(this, t => t.Crystalls);
        }

        #region fields

        [NotMapped] public override PackType type => PackType.None;

        [NotMapped] public override int PackId => -1;

        [NotMapped] public override int cid { get; set; }

        [NotMapped] public override int off { get; set; }

        public string Crystalls { get; set; } = "[]";

        private long[] _crysinside = new long[6];

        public long AllCrys => _crysinside.Sum();

        public long[] BoxCrystall 
        { 
            get 
            {
                _crysinside = JsonConvert.DeserializeObject<long[]>(Crystalls) ?? new long[6];
                return _crysinside;
            } 
        }

        #endregion

        #region affectworld

        public override void Build()
        {
            World.SetCell(x, y, 90, true);
        }

        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
        }

        public override void Destroy(Player p)
        {
            ClearBuilding();
            using var db = new DataBase();
            db.boxes.Remove(this);
            db.SaveChanges();
        }

        #endregion

        #region gui

        public override Window? GUIWin(Player p) => null;

        private static void BBox(Player p, long[]? crystalls)
        {
            if (crystalls == null) return;
            
            var (x, y) = p.GetDirCord();

            if (!World.ValidCoord(x, y))
                return;

            BuildBox(x, y, crystalls, p);
            p.connection?.CloseWindow();
        }

        public static Window GUIWinBox(Player p)
        {
            return new Window()
            {
                ShowTabs = false,
                Title = "Создание бокса",
                Tabs = [new Tab()
                {
                    Label = "Box",
                    Action = "create_box",
                    InitialPage = new Page()
                    {
                        CrystalConfig = new CrystalConfig("  останется", "будет в боксе",
                            [
                                new CrysLine("", 0, 0, p.crys[CrystalType.Green], 0),
                                new CrysLine("", 0, 0, p.crys[CrystalType.Blue], 0),
                                new CrysLine("", 0, 0, p.crys[CrystalType.Red], 0),
                                new CrysLine("", 0, 0, p.crys[CrystalType.Violet], 0),
                                new CrysLine("", 0, 0, p.crys[CrystalType.White], 0),
                                new CrysLine("", 0, 0, p.crys[CrystalType.Cyan], 0)
                            ]),
                        Text = "\nИспользуйте полосы прокрутки, чтобы положить в бокс.\n" +
                        "ВНИМАНИЕ!\nПри создании бокса, количество кристаллов не уменьшается\n",
                        Buttons = [new MButton("<color=green>Добавить в бокс</color>", $"create_box:{ActionMacros.CrystalSliders}", (args) =>
                        {
                            BBox(p, args.CrystalSliders);
                        })]
                    }
                }]
            };
        }

        #endregion

        #region static factory

        /// <summary>
        /// Установка бокса на месте разрушения пака.
        /// </summary>
        public static void BuildBox(int x, int y, long[] crystalls)
            => BuildBox(x, y, crystalls, null, false);

        public static void BuildBox(int x, int y, long[] crystalls, Player? p, bool force = false)
        {
            var (boxX, boxY) = force ? World.FindEmptyForBox(x, y) : (x, y);

            var cell = World.GetCell(boxX, boxY);

            if (!force && !(World.GetProp(cell).can_place_over && World.IsEmpty(boxX, boxY)))
                return;

            var filtered = new long[6];
            bool any = false;

            for (int i = 0; i < 6; i++)
            {
                long count = crystalls[i];
                if (count <= 0) continue;

                if (p == null)
                {
                    filtered[i] = count;
                    any = true;
                }
                else if (p.crys.RemoveCrys(i, count))
                {
                    filtered[i] = count;
                    any = true;
                }
            }

            if (!any) return;

            int OwnerId = p == null ? 0 : p.id;
            var box = new Box(boxX, boxY, OwnerId, filtered);
            box.Build();
        }

        #endregion
    }
}