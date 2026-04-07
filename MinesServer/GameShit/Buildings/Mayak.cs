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
    public sealed class Mayak : Pack
    {
        private Mayak() { }
        public Mayak(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            using var db = new DataBase();
            db.mayak.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Mayak;
        [NotMapped] public override int PackId { get; set; }
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }

        #endregion;

        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x - 2, y - 2, 32, false);
            World.SetCell(x - 1, y - 2, 32, false);
            World.SetCell(x, y - 2, 32, false);
            World.SetCell(x + 1, y - 2, 32, false);
            World.SetCell(x + 2, y - 2, 35, false);

            World.SetCell(x - 2, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x + 2, y - 1, 32, false);

            World.SetCell(x - 2, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 2, y, 32, false);

            World.SetCell(x - 2, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x + 2, y + 1, 32, false);

            World.SetCell(x - 2, y + 2, 35, false);
            World.SetCell(x - 1, y + 2, 32, false);
            World.SetCell(x, y + 2, 32, false);
            World.SetCell(x + 1, y + 2, 32, false);
            World.SetCell(x + 2, y + 2, 35, false);
        }
        public override void Build()
        {
            World.SetCell(x - 2, y - 2, 35, true);
            World.SetCell(x - 1, y - 2, 106, true);
            World.SetCell(x, y - 2, 106, true);
            World.SetCell(x + 1, y - 2, 106, true);
            World.SetCell(x + 2, y - 2, 35, true);

            World.SetCell(x - 2, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x + 2, y - 1, 106, true);

            World.SetCell(x - 2, y, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x, y, 37, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 2, y, 106, true);

            World.SetCell(x - 2, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x, y + 1, 37, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x + 2, y + 1, 106, true);

            World.SetCell(x - 2, y + 2, 35, true);
            World.SetCell(x - 1, y + 2, 106, true);
            World.SetCell(x, y + 2, 37, true);
            World.SetCell(x + 1, y + 2, 106, true);
            World.SetCell(x + 2, y + 2, 35, true);

            base.Build();
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.mayak.Remove(this);
            db.SaveChanges();
        }
        #endregion

        private CanvasElement[] ChunkEffectPage()
        {
            List<CanvasElement> n = new();
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -275, originDY: -150, dx: 251));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: 250, dy: 251));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDY: 250, dx: -251));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -250, dy: -251));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 75, originDY: -175));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: -100, originDY: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: -100, originDY: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 50));
            n.Add(CanvasElement.Rect(Color.Purple, width: 50, height: 50, originDX: 50));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -125, originDY: -175, dy: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -50, originDY: 50, dx: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: 100, originDY: -50, dy: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -100, originDY: 100, dx: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: 150, originDY: -100, dy: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -150, originDY: 150, dx: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: 200, originDY: -150, dy: 250));
            n.Add(CanvasElement.Line(Color.Green, thickness: 2, originDX: -200, originDY: 200, dx: 250));
            n.Add(CanvasElement.Rect(Color.Red, width: 10, height: 10, originDX: 127, originDY: -75));
            n.Add(CanvasElement.Rect(Color.Red, width: 6, height: 6));
            n.Add(CanvasElement.TextField("Зона действия МАЯКА <color=yellow>(чанки)</color>", originDX: -72, originDY: 130));
            n.Add(CanvasElement.TextField(
                "<size=16>Координаты маяка: <color=white>14934:488</color>\n" +
                "Кем открыт: <color=white>%2% D M I T R O V</color>\n" +
                "Тип эффекта: <color=cyan>БЕЗ ЭФФЕКТА</color>\n" +
                "Коэффициент эффекта: <color=yellow>1,00</color>\n\n" +
                "<color=lime>МАЯК АКТИВИРОВАН!</color></size>", originDX: 265, originDY: -5));

            return n.ToArray();
        }

        private Tab ChunkEffectTab(Player p)
        {
            return new Tab()
            {
                Action = "ChunkEffect",
                Label = "Эффект маяка",
                InitialPage = new Page()
                {
                    Canvas = ChunkEffectPage(),
                    Buttons = [new MButton()],
                }
            };
        }

        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "Маяк",
                Tabs = [ChunkEffectTab(p)]
            };
        }
    }
}
