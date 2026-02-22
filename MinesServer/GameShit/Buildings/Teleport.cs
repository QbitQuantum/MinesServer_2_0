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
    public struct TeleportPoint
    {
        public string Name;
        public int CanvasX;
        public int CanvasY;
        public Color PointColor;

        public TeleportPoint(string name, int canvasX, int canvasY, Color pointColor)
        {
            Name = name;
            CanvasX = canvasX;
            CanvasY = canvasY;
            PointColor = pointColor;
        }
    }
    public class Teleport : Pack, IDamagable
    {
        public override PackType type => PackType.Teleport;
        public DateTime brokentimer { get; set; }
        public float maxcharge { get; set; }
        public int hp { get; set; }
        public int maxhp { get; set; }
        public int cost { get; set; }
        [NotMapped]
        public override int off => charge > 0 ? 1 : 0;
        public override int PackId => 0;
        private Teleport() {}
        public Teleport(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            cost = 10;
            charge = 1000;
            maxcharge = 10000;
            hp = 1000;
            maxhp = 1000;
            using var db = new DataBase();
            db.teleports.Add(this);
            db.SaveChanges();
        }
        private TeleportPoint[] TeleportPoints = new TeleportPoint[] { };
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
            World.RemovePack(x, y);
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
        private void AddPointMarker(List<CanvasElement> list, TeleportPoint point, bool isCurrent)
        {
            // Если это текущая позиция - белый квадрат, иначе - заданный цвет точки
            Color pointColor = isCurrent ? Color.White : point.PointColor;
            int pointSize = isCurrent ? 10 : 5;

            // Сама точка
            list.Add(CanvasElement.Rect(
                color: pointColor,
                width: pointSize,
                height: pointSize,
                offsetX: point.CanvasX,
                offsetY: point.CanvasY
            ));

            // Название системы справа
            if (point.Name != "")
            {
                if (true)
                {
                    list.Add(CanvasElement.MicroButton(
                        new MButton($"<color=white><size=14>{point.Name}</size></color>", ""),
                        offsetX: point.CanvasX + 45,
                        offsetY: point.CanvasY - 20
                    ));
                }
                else
                {
                    list.Add(CanvasElement.TextField(
                        $"<color=white><size=12>{point.Name}</size></color>",
                        offsetX: point.CanvasX + 45,
                        offsetY: point.CanvasY - 20
                    ));
                }
            }
        }

        private List<CanvasElement> CreateLine(TeleportPoint start, TeleportPoint end, Color color, bool dashed = false, int dashLength = 20, int gapLength = 10)
        {
            return CreateLine(start, end.CanvasX, end.CanvasY, color, dashed, dashLength, gapLength);
        }

        private List<CanvasElement> CreateLine(TeleportPoint start, int endX, int endY, Color color, bool dashed = false, int dashLength = 20, int gapLength = 10)
        {
            if (!dashed)
            {
                // Сплошная линия - один элемент
                return new List<CanvasElement>
                {
                    CanvasElement.Line(
                        color,
                        offsetX: start.CanvasX,
                        offsetY: start.CanvasY,
                        dx: endX,
                        dy: endY)
                };
            }

            // Пунктирная линия - множество элементов
            var elements = new List<CanvasElement>();

            int totalDx = endX - start.CanvasX;
            int totalDy = endY - start.CanvasY;
            double totalLength = Math.Sqrt(totalDx * totalDx + totalDy * totalDy);

            if (totalLength < 0.1) return elements;

            double dirX = totalDx / totalLength;
            double dirY = totalDy / totalLength;
            double currentDist = 0;

            while (currentDist < totalLength)
            {
                double dashStart = currentDist;
                double dashEnd = Math.Min(currentDist + dashLength, totalLength);

                if (dashEnd > dashStart)
                {
                    int startX = start.CanvasX + (int)(dirX * dashStart);
                    int startY = start.CanvasY + (int)(dirY * dashStart);
                    int segmentEndX = start.CanvasX + (int)(dirX * dashEnd);
                    int segmentEndY = start.CanvasY + (int)(dirY * dashEnd);

                    elements.Add(CanvasElement.Line(
                        color,
                        offsetX: startX,
                        offsetY: startY,
                        dx: segmentEndX,
                        dy: segmentEndY
                    ));
                }

                currentDist = dashEnd + gapLength;
            }

            return elements;
        }
        private CanvasElement[] Buttonsg()
        {
            List<CanvasElement> elements = new List<CanvasElement>();

            // Фон
            elements.Add(CanvasElement.Image("", 1600, 1200, CanvasElementPivot.Default, 0, 0));
            elements.Add(CanvasElement.Rect(color: Color.Black, width: 1600, height: 1200, offsetX: 0, offsetY: 0));

            //elements.Add(CanvasElement.Rect(color: Color.Black, width: 0, height: 0, originDX: -20, originDY: -100));

            var Urlen = new TeleportPoint("Urlen 1.0", 20, 300, Color.Blue);
            var Maurasi = new TeleportPoint("Maurasi 0.9", -356, 263, Color.Blue);
            var Niyabainen = new TeleportPoint("Niyabainen 1.0", 356, 263, Color.Blue);
            var T_H_E__C_I_T_A_D_E_L = new TeleportPoint("T H E  C I T A D E L", 386, 150, Color.Green);
            
            var IKuchi = new TeleportPoint("IKuchi 1.0", 256, 83, Color.Blue);
            var Jita = new TeleportPoint("Jita 0.9", 0, 10, Color.Blue);
            var New_Caldari = new TeleportPoint("New Caldari 1.0", 156, -20, Color.Blue);

            var Uncknow_1 = new TeleportPoint("", 60, 240, Color.Green);
            var Uncknow_2 = new TeleportPoint("", 40, 220, Color.Blue);
            var Uncknow_3 = new TeleportPoint("", 326, 180, Color.Purple);
            var Uncknow_4 = new TeleportPoint("", 200, -100, Color.Green);
            var Uncknow_5 = new TeleportPoint("", -200, 50, Color.Orange);

            TeleportPoints = new TeleportPoint[]
            {
                Urlen, Uncknow_1, Uncknow_2, Maurasi, Niyabainen, 
                T_H_E__C_I_T_A_D_E_L, Uncknow_3, IKuchi, Jita, New_Caldari,
                Uncknow_4, Uncknow_5
            };

            for (int i = 0; i < TeleportPoints.Length; i++)
            {
                TeleportPoint point = TeleportPoints[i];
                bool isCurrent = (point.Name == "Jita");
                AddPointMarker(elements, point, isCurrent);
            }

            elements.AddRange(CreateLine(Jita, New_Caldari, Color.Blue));
            elements.AddRange(CreateLine(Jita, Niyabainen, Color.Blue));
            elements.AddRange(CreateLine(Jita, Maurasi, Color.Blue));
            elements.AddRange(CreateLine(Jita, IKuchi, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Jita, 30, -500, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Jita, 630, -500, Color.Blue, dashed: true));

            elements.AddRange(CreateLine(New_Caldari, Niyabainen, Color.Blue));
            elements.AddRange(CreateLine(New_Caldari, 510, -500, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(New_Caldari, 710, -500, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(New_Caldari, Uncknow_2, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(New_Caldari, -600, -50, Color.Blue, dashed: true));

            elements.AddRange(CreateLine(Niyabainen, 200, -500, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Niyabainen, 200, 600, Color.Blue));

            elements.AddRange(CreateLine(Urlen, 156, 600, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Urlen, 456, 600, Color.Blue));
            elements.AddRange(CreateLine(Urlen, -100, 600, Color.Blue));
            elements.AddRange(CreateLine(Urlen, -300, 600, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Urlen, -380, 600, Color.Blue, dashed: true));

            elements.AddRange(CreateLine(Maurasi, 0, -500, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Maurasi, -900, 350, Color.Blue, dashed: true));
            elements.AddRange(CreateLine(Maurasi, 350, 600, Color.Blue));

            return elements.ToArray();
        }
        public override Window? GUIWin(Player p)
        {
            CanvasElement[] canvas = [];
            var chunk = World.W.GetChunkPosByCoords(x, y);
            canvas = canvas.Concat(Buttonsg()).ToArray();

            var WindowsW = new Window()
            {
                Tabs = [new Tab() {
                    InitialPage = new Page()
                    {
                        Canvas = canvas,
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
