using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public sealed class Observatory : Pack
    {
        public override PackType type => PackType.Observatory;
        [NotMapped]
        public override int PackId => -1;
        private Observatory() { }
        public Observatory(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            using var db = new DataBase();
            db.observatory.Add(this);
            db.SaveChanges();
        }
        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false); /* -> */ World.W.cells[x, y] = 32;
            World.SetCell(x, y + 1, 32, false); /* -> */ World.W.cells[x + 1, y] = 32;
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x - 1, y, 32, false);
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x, y + 1, 37, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x - 1, y, 106, true);
            base.Build();
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.observatory.Remove(this);
            db.SaveChanges();
        }
        #endregion

        private CanvasElement[] BuildMissilePage()
        {
            List<CanvasElement> n = new();
            n.Add(CanvasElement.TextField("Из протонных бомб создаются <color=#BB29BB>Протонные ракеты</color>, которые смещают", originDX: -215, originDY: 180));
            n.Add(CanvasElement.TextField("курс метеорита", originDX: 180, originDY: -20));
            n.Add(CanvasElement.TextField("Стоимость одной протонной ракеты <color=#BB29BB>10 протонных бомб</color>", originDX: -135, originDY: -20));
            n.Add(CanvasElement.TextField("Текущие координаты метеорита:", originDX: 75, originDY: -20));
            n.Add(CanvasElement.TextField("<color=yellow>4097:6674</color>", originDX: 75, originDY: -20));

            n.Add(CanvasElement.Line(Color.Green, originDX: -250, originDY: -150, dx: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: 120, originDY: -120, dy: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: -120, originDY: 121, dx: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: 121, originDY: -121, dy: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: -121, originDY: 122, dx: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: 122, originDY: -122, dy: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: -122, originDY: 123, dx: 240));
            n.Add(CanvasElement.Line(Color.Green, originDX: 123, originDY: -123, dy: 240));

            n.Add(CanvasElement.Line(Color.Green, originDX: 9, originDY: 122, dx: -2, dy: 4));
            n.Add(CanvasElement.Line(Color.Green, originDX: -2, originDY: 4, dx: -3, dy: 4));
            n.Add(CanvasElement.Line(Color.Green, originDX: -3, originDY: 4, dx: -5, dy: 2));
            n.Add(CanvasElement.Line(Color.Green, originDX: -5, originDY: 2, dx: -4, dy: -2));
            n.Add(CanvasElement.Line(Color.Green, originDX: -4, originDY: -2, dx: -4, dy: -3));
            n.Add(CanvasElement.Line(Color.Green, originDX: -4, originDY: -4, dx: -2, dy: -4));
            n.Add(CanvasElement.Line(Color.Green, originDX: -2, originDY: -4, dx: 2, dy: -4));
            n.Add(CanvasElement.Line(Color.Green, originDX: 2, originDY: -4, dx: 3, dy: -4));
            n.Add(CanvasElement.Line(Color.Green, originDX: 3, originDY: -4, dx: 5, dy: -2));
            n.Add(CanvasElement.Line(Color.Green, originDX: 5, originDY: -2, dx: 5, dy: 2));
            n.Add(CanvasElement.Line(Color.Green, originDX: 5, originDY: 2, dx: 3, dy: 4));
            n.Add(CanvasElement.Line(Color.Green, originDX: 3, originDY: 3, dx: 2, dy: 5));
            n.Add(CanvasElement.Line(Color.Green, originDX: 20, originDY: 5, dx: -4, dy: 13));
            n.Add(CanvasElement.Line(Color.Green, originDX: -4, originDY: 13, dx: -10, dy: 11));
            n.Add(CanvasElement.Line(Color.Green, originDX: -10, originDY: 11, dx: -14, dy: 4));
            n.Add(CanvasElement.Line(Color.Green, originDX: -14, originDY: 4, dx: -13, dy: -4));
            n.Add(CanvasElement.Line(Color.Green, originDX: -13, originDY: -4, dx: -11, dy: -10));
            n.Add(CanvasElement.Line(Color.Green, originDX: -11, originDY: -11, dx: -4, dy: -13));
            n.Add(CanvasElement.Line(Color.Green, originDX: -4, originDY: -13, dx: 4, dy: -13));
            n.Add(CanvasElement.Line(Color.Green, originDX: 4, originDY: -13, dx: 10, dy: -11));
            n.Add(CanvasElement.Line(Color.Green, originDX: 10, originDY: -11, dx: 14, dy: -4));
            n.Add(CanvasElement.Line(Color.Green, originDX: 14, originDY: -4, dx: 14, dy: 4));
            n.Add(CanvasElement.Line(Color.Green, originDX: 14, originDY: 4, dx: 10, dy: 11));
            n.Add(CanvasElement.Line(Color.Green, originDX: 10, originDY: 10, dx: 4, dy: 14));
            n.Add(CanvasElement.Line(Color.Green, originDX: 22, originDY: 14, dx: -7, dy: 22));
            n.Add(CanvasElement.Line(Color.Green, originDX: -7, originDY: 22, dx: -16, dy: 17));
            n.Add(CanvasElement.Line(Color.Green, originDX: -16, originDY: 17, dx: -23, dy: 7));
            n.Add(CanvasElement.Line(Color.Green, originDX: -23, originDY: 7, dx: -22, dy: -7));
            n.Add(CanvasElement.Line(Color.Green, originDX: -22, originDY: -7, dx: -17, dy: -16));
            n.Add(CanvasElement.Line(Color.Green, originDX: -17, originDY: -17, dx: -7, dy: -22));
            n.Add(CanvasElement.Line(Color.Green, originDX: -7, originDY: -22, dx: 7, dy: -22));
            n.Add(CanvasElement.Line(Color.Green, originDX: 7, originDY: -22, dx: 16, dy: -17));
            n.Add(CanvasElement.Line(Color.Green, originDX: 16, originDY: -17, dx: 23, dy: -7));
            n.Add(CanvasElement.Line(Color.Green, originDX: 23, originDY: -7, dx: 23, dy: 7));
            n.Add(CanvasElement.Line(Color.Green, originDX: 23, originDY: 7, dx: 16, dy: 17));
            n.Add(CanvasElement.Line(Color.Green, originDX: 16, originDY: 16, dx: 7, dy: 23));
            n.Add(CanvasElement.Line(Color.Green, originDX: 25, originDY: 23, dx: -9, dy: 31));
            n.Add(CanvasElement.Line(Color.Green, originDX: -9, originDY: 31, dx: -23, dy: 24));
            n.Add(CanvasElement.Line(Color.Green, originDX: -23, originDY: 24, dx: -32, dy: 9));
            n.Add(CanvasElement.Line(Color.Green, originDX: -32, originDY: 9, dx: -31, dy: -9));
            n.Add(CanvasElement.Line(Color.Green, originDX: -31, originDY: -9, dx: -24, dy: -23));
            n.Add(CanvasElement.Line(Color.Green, originDX: -24, originDY: -24, dx: -9, dy: -31));
            n.Add(CanvasElement.Line(Color.Green, originDX: -9, originDY: -31, dx: 9, dy: -31));
            n.Add(CanvasElement.Line(Color.Green, originDX: 9, originDY: -31, dx: 23, dy: -24));
            n.Add(CanvasElement.Line(Color.Green, originDX: 23, originDY: -24, dx: 32, dy: -9));
            n.Add(CanvasElement.Line(Color.Green, originDX: 32, originDY: -9, dx: 32, dy: 9));
            n.Add(CanvasElement.Line(Color.Green, originDX: 32, originDY: 9, dx: 23, dy: 24));
            n.Add(CanvasElement.Line(Color.Green, originDX: 23, originDY: 23, dx: 9, dy: 32));
            n.Add(CanvasElement.Line(Color.Green, originDX: 27, originDY: 32, dx: -11, dy: 40));
            n.Add(CanvasElement.Line(Color.Green, originDX: -11, originDY: 40, dx: -30, dy: 31));
            n.Add(CanvasElement.Line(Color.Green, originDX: -30, originDY: 31, dx: -41, dy: 11));
            n.Add(CanvasElement.Line(Color.Green, originDX: -41, originDY: 11, dx: -40, dy: -11));
            n.Add(CanvasElement.Line(Color.Green, originDX: -40, originDY: -11, dx: -31, dy: -30));
            n.Add(CanvasElement.Line(Color.Green, originDX: -31, originDY: -31, dx: -11, dy: -40));
            n.Add(CanvasElement.Line(Color.Green, originDX: -11, originDY: -40, dx: 11, dy: -40));
            n.Add(CanvasElement.Line(Color.Green, originDX: 11, originDY: -40, dx: 30, dy: -31));
            n.Add(CanvasElement.Line(Color.Green, originDX: 30, originDY: -31, dx: 41, dy: -11));
            n.Add(CanvasElement.Line(Color.Green, originDX: 41, originDY: -11, dx: 41, dy: 11));
            n.Add(CanvasElement.Line(Color.Green, originDX: 41, originDY: 11, dx: 30, dy: 31));
            n.Add(CanvasElement.Line(Color.Green, originDX: 30, originDY: 30, dx: 11, dy: 41));
            n.Add(CanvasElement.Line(Color.Green, originDX: 29, originDY: 41, dx: -14, dy: 49));
            n.Add(CanvasElement.Line(Color.Green, originDX: -14, originDY: 49, dx: -36, dy: 37));
            n.Add(CanvasElement.Line(Color.Green, originDX: -36, originDY: 37, dx: -50, dy: 14));
            n.Add(CanvasElement.Line(Color.Green, originDX: -50, originDY: 14, dx: -49, dy: -14));
            n.Add(CanvasElement.Line(Color.Green, originDX: -49, originDY: -14, dx: -37, dy: -36));
            n.Add(CanvasElement.Line(Color.Green, originDX: -37, originDY: -37, dx: -14, dy: -49));
            n.Add(CanvasElement.Line(Color.Green, originDX: -14, originDY: -49, dx: 14, dy: -49));
            n.Add(CanvasElement.Line(Color.Green, originDX: 14, originDY: -49, dx: 36, dy: -37));
            n.Add(CanvasElement.Line(Color.Green, originDX: 36, originDY: -37, dx: 50, dy: -14));
            n.Add(CanvasElement.Line(Color.Green, originDX: 50, originDY: -14, dx: 50, dy: 14));
            n.Add(CanvasElement.Line(Color.Green, originDX: 50, originDY: 14, dx: 36, dy: 37));
            n.Add(CanvasElement.Line(Color.Green, originDX: 36, originDY: 36, dx: 14, dy: 50));
            n.Add(CanvasElement.Line(Color.Green, originDX: 32, originDY: 50, dx: -16, dy: 58));
            n.Add(CanvasElement.Line(Color.Green, originDX: -16, originDY: 58, dx: -43, dy: 44));
            n.Add(CanvasElement.Line(Color.Green, originDX: -43, originDY: 44, dx: -59, dy: 16));
            n.Add(CanvasElement.Line(Color.Green, originDX: -59, originDY: 16, dx: -58, dy: -16));
            n.Add(CanvasElement.Line(Color.Green, originDX: -58, originDY: -16, dx: -44, dy: -43));
            n.Add(CanvasElement.Line(Color.Green, originDX: -44, originDY: -44, dx: -16, dy: -58));
            n.Add(CanvasElement.Line(Color.Green, originDX: -16, originDY: -58, dx: 16, dy: -58));
            n.Add(CanvasElement.Line(Color.Green, originDX: 16, originDY: -58, dx: 43, dy: -44));
            n.Add(CanvasElement.Line(Color.Green, originDX: 43, originDY: -44, dx: 59, dy: -16));
            n.Add(CanvasElement.Line(Color.Green, originDX: 59, originDY: -16, dx: 59, dy: 16));
            n.Add(CanvasElement.Line(Color.Green, originDX: 59, originDY: 16, dx: 43, dy: 44));
            n.Add(CanvasElement.Line(Color.Green, originDX: 43, originDY: 43, dx: 16, dy: 59));

            n.Add(CanvasElement.Line(Color.Black, originDX: -224, originDY: -63, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: 240, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -240, dx: 240));
            n.Add(CanvasElement.Line(Color.Black, originDY: 240, dx: 244));
            n.Add(CanvasElement.Line(Color.Black, originDX: 1, originDY: -240, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: 240, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -240, originDY: 1, dx: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -1, originDY: 240, dx: 244));
            n.Add(CanvasElement.Line(Color.Black, originDX: 2, originDY: -241, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: 240, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -240, originDY: 2, dx: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -2, originDY: 240, dx: 244));
            n.Add(CanvasElement.Line(Color.Black, originDX: 3, originDY: -242, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: 240, dy: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -240, originDY: 3, dx: 240));
            n.Add(CanvasElement.Line(Color.Black, originDX: -3, originDY: 240, dx: 244));


            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=red>[]</color>", "O"),
                    originDX: 105, originDY: -171));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (1)</color>", "left1"),
                    originDX: 265, originDY: 49));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (10)</color>", "left10"),
                    originDX: -40));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (100)</color>", "left100"),
                    originDX: -40));


            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (1)</color>", "right1"),
                    originDX: 150));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (10)</color>", "right10"),
                    originDX: 40));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (100)</color>", "right100"),
                    originDX: 45));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (1)</color>", "down1"),
                    originDX: -120, originDY: -35));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (10)</color>", "down10"),
                    originDY: -40));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (100)</color>", "down100"),
                    originDY: -40));


            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (1)</color>", "up1"),
                    originDY: 150));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (10)</color>", "up10"),
                    originDY: 40));

            n.Add(CanvasElement.MicroButton(
                new MButton($"<color=white>< (100)</color>", "up100"),
                    originDY: 40));

            return n.ToArray();
        }

        private Tab MissileTab(Player p)
        {
            return new Tab()
            {
                Action = "MissilePage",
                Label = "ЗАПУСК РАКЕТЫ",
                InitialPage = new Page()
                {
                    Canvas = BuildMissilePage(),
                    Style = new Style()
                    {
                        Space = 0,
                        Canvas = new GridStyle()
                        {
                            Height = 400
                        }
                    },
                    Buttons = [],
                }
            };
        }

        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                Title = "Обсерватория",
                Tabs = [MissileTab(p)]
            };
        }
    }
}
