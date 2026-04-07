using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MinesServer.GameShit.Buildings
{
    public class TeleportPoint
    {
        public string Name { get; }
        public int CanvasX { get; }
        public int CanvasY { get; }
        public Color PointColor { get; }

        /// <summary>
        /// If true, this point will be highlighted as the current position.
        /// </summary>
        public bool IsCurrent { get; set; }

        public TeleportPoint(string name, int canvasX, int canvasY, Color pointColor)
        {
            Name = name;
            CanvasX = canvasX;
            CanvasY = canvasY;
            PointColor = pointColor;
        }

        /// <summary>
        /// Create a connection from this point to another point.
        /// </summary>
        public Connection ConnectTo(TeleportPoint target, Color color, bool dashed = false, int dashLength = 20, int gapLength = 10)
            => new(this, target, color, dashed, dashLength, gapLength);

        /// <summary>
        /// Create a connection from this point to arbitrary coordinates.
        /// </summary>
        public Connection ConnectTo(int targetX, int targetY, Color color, bool dashed = false, int dashLength = 20, int gapLength = 10)
            => new(this, targetX, targetY, color, dashed, dashLength, gapLength);

        /// <summary>
        /// Convert logical point into drawable canvas elements.
        /// </summary>
        public IEnumerable<CanvasElement> Render()
        {
            var elements = new List<CanvasElement>();

            Color pointColor = IsCurrent ? Color.White : PointColor;
            int pointSize = IsCurrent ? 10 : 5;

            // Point marker
            elements.Add(CanvasElement.Rect(
                color: pointColor,
                width: pointSize,
                height: pointSize,
                offsetX: CanvasX,
                offsetY: CanvasY
            ));

            // Name label (clickable micro button) if present
            if (!string.IsNullOrEmpty(Name))
            {
                elements.Add(CanvasElement.MicroButton(
                    new MButton($"<color=white><size=14>{Name}</size></color>", ""),
                    offsetX: CanvasX + 45,
                    offsetY: CanvasY - 20
                ));
            }

            return elements;
        }
    }
    public sealed class Teleport : PackCharge
    {
        private Teleport() { }
        public Teleport(int x, int y, int ownerid) : base(x, y, ownerid, 1000, 10000)
        {
            charge = 1000;
            using var db = new DataBase();
            db.teleports.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Teleport;
        [NotMapped] public override int PackId { get; set; }
        public override int off => charge > 0 ? 1 : 0;
        public int cost { get; set; }

        #endregion

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
            if (charge > 0)
            {
                var temp = new long[] { 0, 0, 0, (long)charge, 0, 0 };
                Box.BuildBox(x, y, temp,null);
            }
            using var db = new DataBase();
            db.teleports.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[0]++;
            }
        }
        #endregion
        private CanvasElement[] BuildGraph()
        {
            var panel = new CanvasPanel("", 1600, 1200, Color.Black);
            var connections = new List<Connection>();

            var Urlen = new TeleportPoint("Urlen 1.0", 20, 300, Color.Blue);
            var Maurasi = new TeleportPoint("Maurasi 0.9", -356, 263, Color.Blue);
            var Niyabainen = new TeleportPoint("Niyabainen 1.0", 356, 263, Color.Blue);
            var T_H_E__C_I_T_A_D_E_L = new TeleportPoint("T H E  C I T A D E L", 386, 150, Color.Green);

            var IKuchi = new TeleportPoint("IKuchi 1.0", 256, 83, Color.Blue);
            var Jita = new TeleportPoint("Jita 0.9", 0, 10, Color.Blue) { IsCurrent = true };
            var New_Caldari = new TeleportPoint("New Caldari 1.0", 156, -20, Color.Blue);

            var Uncknow_1 = new TeleportPoint("", 60, 240, Color.Green);
            var Uncknow_2 = new TeleportPoint("", 40, 220, Color.Blue);
            var Uncknow_3 = new TeleportPoint("", 326, 180, Color.Purple);
            var Uncknow_4 = new TeleportPoint("", 200, -100, Color.Green);
            var Uncknow_5 = new TeleportPoint("", -200, 50, Color.Orange);

            connections.Add(Jita.ConnectTo(New_Caldari, Color.Blue));
            connections.Add(Jita.ConnectTo(Niyabainen, Color.Blue));
            connections.Add(Jita.ConnectTo(Maurasi, Color.Blue));
            connections.Add(Jita.ConnectTo(IKuchi, Color.Blue, dashed: true));
            connections.Add(Jita.ConnectTo(30, -500, Color.Blue, dashed: true));
            connections.Add(Jita.ConnectTo(630, -500, Color.Blue, dashed: true));

            connections.Add(New_Caldari.ConnectTo(Niyabainen, Color.Blue));
            connections.Add(New_Caldari.ConnectTo(510, -500, Color.Blue, dashed: true));
            connections.Add(New_Caldari.ConnectTo(710, -500, Color.Blue, dashed: true));
            connections.Add(New_Caldari.ConnectTo(Uncknow_2, Color.Blue, dashed: true));
            connections.Add(New_Caldari.ConnectTo(-600, -50, Color.Blue, dashed: true));

            connections.Add(Niyabainen.ConnectTo(200, -500, Color.Blue, dashed: true));
            connections.Add(Niyabainen.ConnectTo(200, 600, Color.Blue));

            connections.Add(Urlen.ConnectTo(156, 600, Color.Blue, dashed: true));
            connections.Add(Urlen.ConnectTo(456, 600, Color.Blue));
            connections.Add(Urlen.ConnectTo(-100, 600, Color.Blue));
            connections.Add(Urlen.ConnectTo(-300, 600, Color.Blue, dashed: true));
            connections.Add(Urlen.ConnectTo(-380, 600, Color.Blue, dashed: true));

            connections.Add(Maurasi.ConnectTo(0, -500, Color.Blue, dashed: true));
            connections.Add(Maurasi.ConnectTo(-900, 350, Color.Blue, dashed: true));
            connections.Add(Maurasi.ConnectTo(350, 600, Color.Blue));

            panel
                .Add(Urlen, Maurasi, Niyabainen, T_H_E__C_I_T_A_D_E_L,
                     IKuchi, Jita, New_Caldari,
                     Uncknow_1, Uncknow_2, Uncknow_3, Uncknow_4, Uncknow_5)
                .AddConnection(connections.ToArray());

            return panel.Render();
        }
        public override Window? GUIWin(Player p)
        {
            var WindowsW = new Window()
            {
                Tabs = [new Tab() {
                    InitialPage = new Page()
                    {
                        Canvas = BuildGraph(),
                        Style = new Style()
                        {
                            Canvas = new GridStyle()
                            {
                                Height = 1600, 
                                Width = 1200
                            } 
                        },
                        Buttons = []
                    },
                    Action = "TP",
                    Label = "Teleport",
                    Title = "Тп"}],
                Title = "Тп"
            };
            return WindowsW;
        }
    }
}
