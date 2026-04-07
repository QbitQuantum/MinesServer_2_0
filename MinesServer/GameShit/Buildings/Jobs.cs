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
    public sealed class Jobs : Pack
    {
        private Jobs() { }
        public Jobs(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            using var db = new DataBase();
            db.jobs.Add(this);
            db.SaveChanges();
        }

        #region fields

        [NotMapped] public override PackType type => PackType.Jobs;
        [NotMapped] public override int PackId { get; set; }
        [NotMapped] public override int cid { get; set; }
        [NotMapped] public override int off { get; set; }

        #endregion

        #region affectworld
        protected override void ClearBuilding()
        {
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

        }
        public override void Build()
        {
            World.SetCell(x - 2, y - 1, 35, true);
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

            base.Build();
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(this);
            using var db = new DataBase();
            db.jobs.Remove(this);
            db.SaveChanges();
        }
        #endregion

        private CanvasElement[] BuildTasksPage()
        {
            List<CanvasElement> n = new();
            n.Add(CanvasElement.TextField("<size=15><color=white>|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n</color></size>", originDX: 103, originDY: 105));
            n.Add(CanvasElement.TextField("<size=15><color=white>|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n</color></size>", originDX: -205));
            n.Add(CanvasElement.Button(new MButton("Выбрать", "claim_miss:0"), originDX: -125, originDY: -200));
            n.Add(CanvasElement.TextField("<size=15><color=white>Миссия №1</color></size>\nОписание:", originDY: 210));
            n.Add(CanvasElement.Image("inner:ITEM:49", originDY: -164, width: 1, height: 1));
            n.Add(CanvasElement.TextField("<color=white>Награда:\n\n      X </color><color=lime>14KK</color>", originDX: 10, originDY: 24));
            n.Add(CanvasElement.TextField("<size=13> Вскопайте <color=purple>13155 фио.\n кристаллов</color> для\n<color=yellow> Федерации</color>,чтобы\n получить награду</size>", originDX: -40, originDY: 90));
            n.Add(CanvasElement.Button(new MButton("Выбрать", "claim_miss:1"), originDX: 230, originDY: -160));
            n.Add(CanvasElement.TextField("<size=15><color=white>Миссия №2</color></size>\nОписание:", originDY: 210));
            n.Add(CanvasElement.Image("inner:ITEM:8", originDY: -164, width: 1, height: 1));
            n.Add(CanvasElement.TextField("<color=white>Награда:\n\n      X </color><color=yellow>2</color>", originDX: 10, originDY: 24));
            n.Add(CanvasElement.TextField("<size=13>Найдите разработчика!\nГоворят, чаще всего\nони находятся на\n<color=yellow>Фед. Базах\n</color></size>", originDX: -40, originDY: 90));
            n.Add(CanvasElement.Button(new MButton("Выбрать", "claim_miss:2"), originDX: 230, originDY: -160));
            n.Add(CanvasElement.TextField("<size=15><color=white>Миссия №3</color></size>\nОписание:", originDY: 210));
            n.Add(CanvasElement.Image("inner:ITEM:50", originDY: -164, width: 1, height: 1));
            n.Add(CanvasElement.TextField("<color=white>Награда:\n\n      X </color><color=#FF0090>4</color>", originDX: 10, originDY: 24));
            n.Add(CanvasElement.TextField("Съедите <size=13><color=cyan>5 жив.</color>\nдля <color=yellow>Федерации</color>,\nчтобы получить\nнаграду</size>", originDX: -40, originDY: 90));
            return n.ToArray();
        }

        private Tab TasksTab(Player p)
        {
            return new Tab()
            {
                Action = "TasksPage",
                Label = "Задания",
                InitialPage = new Page()
                {
                    Text = "Выполняйте задания <color=cyan>Рабочего центра</color> и получайте <color=white>награды!</color>\n" +
                    "До обновления осталось: <color=white>23 ч. 39 мин. 22 сек.</color>\n" +
                    "Успейте выполнить <color=white>задания</color>, до их обновления, иначе они <color=red>сбросятся</color>",
                    Canvas = BuildTasksPage(),
                    Buttons = [],
                }
            };
        }

        public override Window? GUIWin(Player p)
        {
            return new Window()
            {
                Title = "Рабочий центр",
                Tabs = [TasksTab(p)]
            };
        }
    }
}
