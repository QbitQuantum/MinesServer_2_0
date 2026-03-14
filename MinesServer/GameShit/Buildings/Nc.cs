using System.Drawing;
using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.Canvas;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents;
using MinesServer.Network.World;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public class NC : Pack
    {
        #region fields
        public override PackType type => PackType.NC;
        public override int PackId => 46;
        #endregion;
        private NC() { }
        public NC(int ownerid, int x, int y) : base(ownerid, x, y)
        {
            using var db = new DataBase();
            db.ncs.Add(this);
            db.SaveChanges();
        }
        #region affectworld
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, false);
            World.SetCell(x, y + 1, 32, false);
            World.SetCell(x - 2, y - 1, 32, false);
            World.SetCell(x - 1, y - 1, 32, false);
            World.SetCell(x, y - 1, 32, false);
            World.SetCell(x + 1, y - 1, 32, false);
            World.SetCell(x + 2, y - 1, 32, false);
            World.SetCell(x - 3, y, 32, false);
            World.SetCell(x - 2, y, 32, false);
            World.SetCell(x - 1, y, 32, false);
            World.SetCell(x + 1, y, 32, false);
            World.SetCell(x + 2, y, 32, false);
            World.SetCell(x + 3, y, 32, false);
            World.SetCell(x - 4, y + 1, 32, false);
            World.SetCell(x - 3, y + 1, 32, false);
            World.SetCell(x - 2, y + 1, 32, false);
            World.SetCell(x - 1, y + 1, 32, false);
            World.SetCell(x + 1, y + 1, 32, false);
            World.SetCell(x + 2, y + 1, 32, false);
            World.SetCell(x + 3, y + 1, 32, false);
            World.SetCell(x + 4, y + 1, 32, false);
            World.SetCell(x - 4, y + 2, 32, false);
            World.SetCell(x - 3, y + 2, 32, false);
            World.SetCell(x - 2, y + 2, 32, false);
            World.SetCell(x + 2, y + 2, 32, false);
            World.SetCell(x + 3, y + 2, 32, false);
            World.SetCell(x + 4, y + 2, 32, false);
            World.SetCell(x - 4, y + 3, 32, false);
            World.SetCell(x - 3, y + 3, 32, false);
            World.SetCell(x - 2, y + 3, 32, false);
            World.SetCell(x + 2, y + 3, 32, false);
            World.SetCell(x + 3, y + 3, 32, false);
            World.SetCell(x + 4, y + 3, 32, false);
            World.SetCell(x - 1, y + 2, 32, false);
            World.SetCell(x, y + 2, 32, false);
            World.SetCell(x + 1, y + 2, 32, false);
            World.SetCell(x - 1, y + 3, 32, false);
            World.SetCell(x, y + 3, 32, false);
            World.SetCell(x + 1, y + 3, 32, false);
            World.SetCell(x - 1, y + 4, 32, false);
            World.SetCell(x, y + 4, 32, false);
            World.SetCell(x + 1, y + 4, 32, false);
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.ncs.Remove(this);
            db.SaveChanges();
            if (Physics.r.Next(1, 101) < 40)
            {
                p.connection?.SendB(new HBPacket([new HBChatPacket(0, x, y, "ШПАААК ВЫПАЛ")]));
                p.inventory[3]++;
            }
        }
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x, y + 1, 37, true);
            World.SetCell(x - 2, y - 1, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x + 2, y - 1, 106, true);
            World.SetCell(x - 3, y, 106, true);
            World.SetCell(x - 2, y, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 2, y, 106, true);
            World.SetCell(x + 3, y, 106, true);
            World.SetCell(x - 4, y + 1, 106, true);
            World.SetCell(x - 3, y + 1, 106, true);
            World.SetCell(x - 2, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x + 2, y + 1, 106, true);
            World.SetCell(x + 3, y + 1, 106, true);
            World.SetCell(x + 4, y + 1, 106, true);
            World.SetCell(x - 4, y + 2, 106, true);
            World.SetCell(x - 3, y + 2, 106, true);
            World.SetCell(x - 2, y + 2, 106, true);
            World.SetCell(x + 2, y + 2, 106, true);
            World.SetCell(x + 3, y + 2, 106, true);
            World.SetCell(x + 4, y + 2, 106, true);
            World.SetCell(x - 4, y + 3, 106, true);
            World.SetCell(x - 3, y + 3, 106, true);
            World.SetCell(x - 2, y + 3, 106, true);
            World.SetCell(x + 2, y + 3, 106, true);
            World.SetCell(x + 3, y + 3, 106, true);
            World.SetCell(x + 4, y + 3, 106, true);
            World.SetCell(x - 1, y + 2, 35, true);
            World.SetCell(x, y + 2, 35, true);
            World.SetCell(x + 1, y + 2, 35, true);
            World.SetCell(x - 1, y + 3, 35, true);
            World.SetCell(x, y + 3, 35, true);
            World.SetCell(x + 1, y + 3, 35, true);
            World.SetCell(x - 1, y + 4, 35, true);
            World.SetCell(x, y + 4, 35, true);
            World.SetCell(x + 1, y + 4, 35, true);
            base.Build();
        }
        #endregion
        
        private void Buy(string _value, Player p)
        {
            // TODO: Реализовать
        }

        private CanvasElement[] BuildExploringWorldGraph()
        {
            List<CanvasElement> n = new();

            n.Add(CanvasElement.TextField("Глобальная <color=yellow>КРИТИЧЕСКАЯ ГЛУБИНА</color>:", originDX: -100, originDY: -25));
            n.Add(CanvasElement.TextField("Клановая <color=yellow>КРИТИЧЕСКАЯ ГЛУБИНА</color>:", originDX: 5, originDY: -100));
            n.Add(CanvasElement.Rect(Color.Green, width: 53, height: 31, originDX: -126, originDY: 80));
            n.Add(CanvasElement.Rect(Color.Green, width: 49, height: 27));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: -29, originDY: 15, dx: 500));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, dy: -34));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDY: -34, dx: 500));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 500, originDY: 35, dy: -35));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: -500, dx: 500));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 1, originDY: -1, dy: -34));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDY: -33, dx: 499));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 498, originDY: 34, dy: -35));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: -498, originDY: -35, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>0</color></size>", originDX: 32, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 51, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>10000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>20000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>30000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>40000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>50000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>60000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Rect(Color.Green, width: 5, height: 31, originDX: -511, originDY: -53));
            n.Add(CanvasElement.Rect(Color.Green, width: 1, height: 27));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: -5, originDY: 15, dx: 500));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, dy: -34));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDY: -34, dx: 500));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 500, originDY: 35, dy: -35));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: -500, dx: 500));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 1, originDY: -1, dy: -34));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDY: -33, dx: 499));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 498, originDY: 34, dy: -35));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: -498, originDY: -35, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>0</color></size>", originDX: 32, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 51, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>10000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>20000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>30000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>40000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>50000</color></size>", originDX: 17, originDY: -28));
            n.Add(CanvasElement.Line(Color.White, thickness: 2, originDX: 66, originDY: 28, dy: -11));
            n.Add(CanvasElement.TextField("<size=15><color=white>60000</color></size>", originDX: 17, originDY: -28));
            return n.ToArray();
        }
        private Tab MiningExploration(Player p)
        {
            RichListConfig richList = new RichListConfig()
            {
                Entries = [
                    RichListEntry.Button("<color=purple>ФИОЛЕТОВЫЕ ЖИВЫЕ КРИСТАЛЛЫ</color>",
                        new MButton(". . .", "purple", (args) => {  })),
                    RichListEntry.Button("<color=blue>СИНИЕ ЖИВЫЕ КРИСТАЛЛЫ</color>",
                        new MButton(". . .", "blue", (args) => {  })),
                    RichListEntry.Button("<color=red>КРАСНЫЕ ЖИВЫЕ КРИСТАЛЛЫ</color>",
                        new MButton(". . .", "red", (args) => {  })),
                    RichListEntry.Button("<color=white>БЕЛЫЕ ЖИВЫЕ КРИСТАЛЛЫ</color>",
                        new MButton(". . .", "white", (args) => {  })),
                    RichListEntry.Button("<color=green>РАДУЖНЫЕ ЖИВЫЕ КРИСТАЛЛЫ</color>",
                        new MButton(". . .", "green", (args) => {  })),
                    RichListEntry.Button("<color=cyan>ГОЛУБЫЕ ЖИВЫЕ КРИСТАЛЛЫ</color>",
                        new MButton(". . .", "cyan", (args) => {  })),
                ]
            };

            return new Tab()
            {
                Label = "ИЗУЧ. ДОБЫЧИ",
                Action = "MiningExploration",
                InitialPage = new Page()
                {
                    Text = "Собирайте <color=yellow>живые кристаллы</color> и увеличивайте доход клана:\n" +
                    "При вложении живых кристаллов, клан получает бонус сразу.\n" +
                    "Вклад расходуется сразу со временем <color=yellow>(5% в сутки)</color>\n" +
                    "Максимальный бонус к добыче каждого крситалла составляет не более <color=red>+15 кристаллов</color>",
                    Style = new Style()
                    {
                        Space = 2,
                    },
                    RichList = richList,
                    Buttons = []
                }
            };
        }
        private Tab ExploringWorld(Player p)
        {

            RichListConfig richList = new RichListConfig()
            {
                Entries = [
                    RichListEntry.Text("",""),
                ]
            };

            return new Tab()
            {
                Label = "ИЗУЧ. МИРА",
                Action = "ExploringWorld",
                InitialPage = new Page()
                {
                    Text = "Изучение <color=cyan>протоплазменных технологий</color> для увелечения мощности и глубины\n" +
                        "Общая <color=yellow>КРИТИЧЕСКАЯ ГЛУБИНА</color>: <color=white>6 056</color>\n" +
                        "Клановая <color=yellow>КРИТИЧЕСКАЯ ГЛУБИНА</color>: <color=white>212</color>\n" +
                        "Формула высчитывания ГЛОБАЛЬНОЙ КРИТ. глубины: \n" +
                        "<color=red>Сумма клановых ворот крит. глубин / кол-во кланов </color>\n" +
                        "<color=yellow>Плазменная бомба х 2</color> = <color=cyan>1 блок</color>",
                    Canvas = BuildExploringWorldGraph(),
                    Style = new Style()
                    {
                        Space = 2,
                        Canvas = new GridStyle()
                        {
                            Height = 0
                        }
                    },
                    RichList = richList,
                    Buttons = [
                        new MButton("Вложить <color=yellow>Плазму x 2</color>", $"boomtonc2", (args) => Buy(args.Input, p)),
                        new MButton("Вложить <color=yellow>Плазму x 20</color>", $"boomtonc20", (args) => Buy(args.Input, p)),
                        new MButton("Вложить <color=yellow>Плазму x 200</color>", $"boomtonc200", (args) => Buy(args.Input, p))
                    ],
                }
            };
        }
        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                ShowTabs = true,
                Title = "Научный центр",
                Tabs = [ExploringWorld(p), MiningExploration(p)]
            };
        }
    }
}
