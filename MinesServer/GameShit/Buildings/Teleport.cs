using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Canvas;
using MinesServer.GameShit.GUI.Styles;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

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
            World.SetCell(x, y, 32, false);
            World.SetCell(x, y + 1, 32, false);
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

            TryToReturnInventory(p);
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

        static Color Hex(string s) => Color.FromArgb(255,
            Convert.ToByte(s[..2], 16),
            Convert.ToByte(s[2..4], 16),
            Convert.ToByte(s[4..6], 16));

        public override Window? GUIWin(Player p)
        {
            var canvas = new List<CanvasElement>
            {
                // 1. фон-карта
                CanvasElement.Image(
                    // Можно достать пример из папки ресурсов
                    "https://mines3.firetype.ru/assets/mines3/telemap/019fcd26-6ea4-7968-92cb-f910ad5787f0.m3g",
                    width: 375, height: 375,
                    originDX: -100, originDY: 5),

                // 2–6. линии (толщина 2, цвет 800080)
                CanvasElement.Line(Hex("800080"), dx: -24, dy: -13, thickness: 2, offsetY: -1),
                CanvasElement.Line(Hex("800080"), dx:  -1, dy:  87, thickness: 2),
                CanvasElement.Line(Hex("800080"), dx:  -1, dy:  86, thickness: 2),
                CanvasElement.Line(Hex("800080"), dx:  10, dy: 124, thickness: 2),
                CanvasElement.Line(Hex("800080"), dx:  46, dy:  -1, thickness: 2),
                CanvasElement.Line(Hex("800080"), dx:  52, dy: -117, thickness: 2),

                // 7. серый прямоугольник
                CanvasElement.Line(Hex("808080"), dx: 226, dy: 189, originDX: -24, originDY: -13),

                // 8. MicroButton $1K
                CanvasElement.MicroButton(new MButton("<color=lime>$1K</color>", "tpbutt11222:4606"),
                                          originDX: 239, originDY: 189),

                // блок 11222:4606
                CanvasElement.TextField("<color=#4169E1>[XXX] 11222:4606</color>", originDX: 55, originDY: -10),
                CanvasElement.Line(Hex("808080"), dx: 203, dy: 64,  originDX: -271, originDY: -79),
                CanvasElement.MicroButton(new MButton("<color=lime>$4K</color>", "tpbutt11405:3796"),
                                          originDX: 216, originDY: 64),

                // блок 11405:3796
                CanvasElement.TextField("<color=white>11405:3796</color>", originDX: 55, originDY: -10),
                CanvasElement.Line(Hex("808080"), dx: 203, dy: 40,  originDX: -271, originDY: -55),
                CanvasElement.MicroButton(new MButton("<color=lime>$4K</color>", "tpbutt11405:3802"),
                                          originDX: 216, originDY: 40),

                // блок 11405:3802
                CanvasElement.TextField("<color=white>11405:3802</color>", originDX: 55, originDY: -10),
                CanvasElement.Line(Hex("808080"), dx: 192, dy: -23, originDX: -260, originDY: 8),
                CanvasElement.MicroButton(new MButton("<color=lime>$4K</color>", "tpbutt11500:3500"),
                                          originDX: 205, originDY: -23),

                // блок 11500:3500
                CanvasElement.TextField("<color=white>11500:3500</color>", originDX: 55, originDY: -10),
                CanvasElement.Line(Hex("808080"), dx: 156, dy: 77,  originDX: -224, originDY: -92),
                CanvasElement.MicroButton(new MButton("<color=lime>$2K</color>", "tpbutt11791:4505"),
                                          originDX: 169, originDY: 77),

                // блок 11791:4505
                CanvasElement.TextField("<color=white>11791:4505</color>", originDX: 55, originDY: -10),
                CanvasElement.Line(Hex("808080"), dx: 150, dy: 168, originDX: -218, originDY: -183),
                CanvasElement.MicroButton(new MButton("<color=lime>$6K</color>", "tpbutt11840:5435"),
                                          originDX: 163, originDY: 168),

                // блок 11840:5435
                CanvasElement.TextField("<color=white>11840:5435</color>", originDX: 55, originDY: -10),

                // внутренняя иконка
                CanvasElement.Image("inner:CLAN:47", width: 1, height: 1, originDX: -284, originDY: -53),

                // ── маркеры ──
                // 11222:4606 (голубой)
                CanvasElement.Rect(Hex("00bfff"), width: 8, height: 8, originDX: -10, originDY: -1),
                CanvasElement.Rect(Hex("000000"), width: 6, height: 6),
                CanvasElement.TPButton(new MButton("", "tp:11222:4606")),

                // 11405:3796 (белый)
                CanvasElement.Rect(Hex("ffffff"), width: 8, height: 8, originDX: 23, originDY: 100),
                CanvasElement.Rect(Hex("000000"), width: 6, height: 6),
                CanvasElement.TPButton(new MButton("", "tp:11405:3796")),

                // 11405:3802 (белый, OriginX = null)
                CanvasElement.Rect(Hex("ffffff"), width: 8, height: 8, originDY: -1),
                CanvasElement.Rect(Hex("000000"), width: 6, height: 6),
                CanvasElement.TPButton(new MButton("", "tp:11405:3802")),

                // 11500:3500 (белый)
                CanvasElement.Rect(Hex("ffffff"), width: 8, height: 8, originDX: 11, originDY: 38),
                CanvasElement.Rect(Hex("000000"), width: 6, height: 6),
                CanvasElement.TPButton(new MButton("", "tp:11500:3500")),

                // серый декор
                CanvasElement.Rect(Hex("808080"), width: 7, height: 7, originDY: -249),
                CanvasElement.Rect(Hex("808080"), width: 5, height: 5),

                // 11791:4505 (белый)
                CanvasElement.Rect(Hex("ffffff"), width: 8, height: 8, originDX: 36, originDY: 124),
                CanvasElement.Rect(Hex("000000"), width: 6, height: 6),
                CanvasElement.TPButton(new MButton("", "tp:11791:4505")),

                // 11840:5435 (белый)
                CanvasElement.Rect(Hex("ffffff"), width: 8, height: 8, originDX: 6, originDY: -116),
                CanvasElement.Rect(Hex("000000"), width: 6, height: 6),
                CanvasElement.TPButton(new MButton("", "tp:11840:5435")),

                // серый декор
                CanvasElement.Rect(Hex("808080"), width: 7, height: 7, originDX: 83, originDY: 117),
                CanvasElement.Rect(Hex("808080"), width: 5, height: 5),

                // белый (−135, OriginY = null)
                CanvasElement.Rect(Hex("ffffff"), width: 9, height: 9, originDX: -135),

                // мигающий белый
                CanvasElement.Rect(Hex("ffffff"), width: 5, height: 5, isBlinking: true),
            };

            // ─── WINDOW ───────────────────────────────────────────────────────────────
            var window = new Window
            {
                Title = "Телепорт",
                Tabs = new[] 
                { 
                    new Tab 
                    { 
                        Label = "Карта",
                        Action = "tp_map", 
                        InitialPage = new Page
                        {
                            Buttons = [],
                            Style = new Style()
                            {
                                Space = 0,
                                Canvas = new GridStyle()
                                {
                                    Height = 375
                                }
                            },
                            Canvas = canvas.ToArray()
                        }, 
                    },
                    new Tab
                    {
                        Label = "Список",
                        Action = "tp_list",
                        InitialPage = new Page
                        {
                            Buttons = [],
                        },
                    }
                },

                ShowTabs = true
            };

            return window;
        }

        public Window? GUIWin2(Player p)
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
