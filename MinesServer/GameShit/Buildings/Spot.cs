using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.GUI.Horb.List.Rich;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinesServer.Enums;
using MinesServer.GameShit.SysMarket;
using Newtonsoft.Json;

namespace MinesServer.GameShit.Buildings
{
    public class Spot : PackDamage
    {
        public override PackType type => PackType.Spot;
        #region Shit
        [NotMapped]
        public override float charge { get; set; }
        [NotMapped]
        public override float maxcharge { get; set; }
        [NotMapped]
        public override int cid { get; set; }
        public override int PackId => 47;
        #endregion
        public Program? selected { get; set; }
        public BotSpot? entity;
        [NotMapped]
        public int botx { get; set; }
        [NotMapped]
        public int boty { get; set; }
        [NotMapped]
        public string basket { get; set; } = string.Empty;
        private Spot() { }
        public Spot(int x, int y, int ownerid) : base(x, y, ownerid, 100)
        {
            using var db = new DataBase();
            db.spots.Add(this);
            db.SaveChanges();
        }
        protected override void ClearBuilding()
        {
            World.SetCell(x, y, 32, true);
        }
        public override void Build()
        {
            World.SetCell(x, y, 32, true);
            base.Build();
        }
        public override void Destroy(Player p)
        {
            ClearBuilding();
            World.RemovePack(x, y);
            using var db = new DataBase();
            db.spots.Remove(this);
            db.SaveChanges();
        }

        public override void Update()
        {
            entity?.Update();
            base.Update();
        }

        /// <summary>
        /// Ensures the spot has a bot entity; creates and restores it from DB if needed.
        /// </summary>
        public void EnsureEntity(Player owner)
        {
            if (entity != null)
                return;
            if (owner.id != ownerid)
                return;
            entity = new BotSpot(x, y, owner);
            try
            {
                var arr = JsonConvert.DeserializeObject<long[]>(basket ?? "[0,0,0,0,0,0]");
                if (arr != null && arr.Length >= 6)
                    entity.crys.Boxcrys(arr);
            }
            catch { /* ignore invalid basket */ }
        }

        private void GiveCrystals(long[]? sliders, Player p)
        {
            if (sliders == null || entity == null)
                return;
            for (int i = 0; i < 6; i++)
            {
                var count = p.crys.cry[i] + entity.crys.cry[i];
                if (count - sliders[i] >= 0 && sliders[i] >= 0)
                {
                    p.crys.cry[i] = count - sliders[i];
                    entity.crys.cry[i] = sliders[i];
                }
            }
            entity.crys.NotifyChanged();
            p.SendCrys();
            ReopenWindow(p, TabCrystals);
        }
        private void UninstallProgram(Player p)
        {
            using var db = new DataBase();

            var spot = db.spots.FirstOrDefault(s => s.id == this.id);
            if (spot == null) return;

            spot.selected = null;

            db.SaveChanges();

            this.selected = null;

            ReopenWindow(p, TabPrograms);
        }
        private void InstallProgram(int progId, Player p)
        {
            using var db = new DataBase();

            var prog = db.progs.FirstOrDefault(pr => pr.id == progId);
            if (prog == null || prog.owner?.id != p.id)
                return;

            var spot = db.spots.FirstOrDefault(s => s.id == this.id);

            if (spot == null) return;

            spot.selected = prog;

            db.SaveChanges();

            this.selected = prog;

            ReopenWindow(p, TabPrograms);
        }

        private void LaunchProgram(Player p)
        {
            EnsureEntity(p);
            if (entity == null || selected == null)
                return; // Нет выбранной программы
            entity.programsData.Run(selected);
        }
        private void StopProgram(Player p)
        {
            EnsureEntity(p);
            if (entity == null)
                return;

            if (entity.programsData.ProgRunning)
            {
                entity.programsData.Run();
            }
            entity.Death();
        }

        private const string TabPrograms = "SpotProgs";
        private const string TabCrystals = "SpotCrys";

        private void ReopenWindow(Player p, string tabAction)
        {
            p.win = GUIWin(p);
            p.win?.OpenTab(tabAction);
        }
        private static string SafeDropDownLabel(string text)
        {
            if (string.IsNullOrEmpty(text)) return "—";
            // RichListEntry.DropDown запрещает ':' внутри значений.
            return text.Replace(":", "·");
        }
        private IPage ProgramsPage(Player p)
        {
            EnsureEntity(p);

            using var db = new DataBase();
            var freshSpot = db.spots
                .Include(s => s.selected)
                .FirstOrDefault(s => s.id == this.id);
            if (freshSpot == null) return null;

            this.selected = freshSpot.selected;

            var progs = db.progs
                .Include(pg => pg.owner)
                .Where(pr => pr.owner != null && pr.owner.id == p.id)
                .ToList();

            var progIds = progs.Select(x => x.id).ToList();
            var dropValues = new[] { "— нет —" }.Concat(progs.Select(x => SafeDropDownLabel(x.name))).ToArray();

            var currentIndex = 0;
            if (freshSpot.selected != null)
            {
                var idx = progIds.IndexOf(freshSpot.selected.id);
                currentIndex = idx >= 0 ? idx + 1 : 0;
            }

            var isRunning = entity?.programsData.ProgRunning == true;

            var rich = new List<RichListEntry>
            {
                RichListEntry.Text("title", "<size=18><color=#b7c6ff>Установка программы</color></size>"),
                RichListEntry.DropDown("Программа", "prog", dropValues, currentIndex),
                RichListEntry.Text("hint", "<color=#7c88a6>Нажми SAVE чтобы применить выбор.</color>"),
            };

            if (freshSpot.selected != null && !isRunning)
                rich.Add(RichListEntry.Button("Запуск установленной", new MButton("LAUNCH", "spotlaunch", _ => LaunchProgram(p))));
            if (isRunning)
                rich.Add(RichListEntry.Button("Остановка", new MButton("STOP", "spotstop", _ => StopProgram(p))));

            var programList = progs.Select(pr => new ListEntry(
                pr.name + (freshSpot.selected?.id == pr.id ? " ✓" : ""),
                new MButton(freshSpot.selected?.id == pr.id ? "Выбрано" : "Установить",
                           freshSpot.selected?.id == pr.id ? "uninstallprog" : $"installprog:{pr.id}",
                           _ =>
                           {
                               if (freshSpot.selected?.id == pr.id) UninstallProgram(p);
                               else InstallProgram(pr.id, p);
                           })
            )).ToArray();

            return new Page
            {
                Title = "Программы",
                Card = new Card(CardImageType.Item, ((int)Item.SpotBot).ToString(),
                    $"<color=white>Программы Spot</color>\n<color=#7c88a6>Выбрано:</color> <color=#d5ffe8>{(freshSpot.selected?.name ?? "—")}</color>"),
                RichList = new RichListConfig(rich.ToArray(), NoScroll: false),
                List = programList,
                Buttons =
                [
                    new MButton("SAVE", $"spotprogsave:{ActionMacros.RichList}", args =>
                    {
                        if (!args.RichList.TryGetValue("prog", out var raw) || !int.TryParse(raw, out var index))
                        {
                            ReopenWindow(p, TabPrograms);
                            return;
                        }

                        if (index <= 0)
                        {
                            UninstallProgram(p);
                            return;
                        }

                        var progIndex = index - 1;
                        if (progIndex < 0 || progIndex >= progIds.Count)
                        {
                            ReopenWindow(p, TabPrograms);
                            return;
                        }

                        InstallProgram(progIds[progIndex], p);
                    }),
                    new MButton("Кристаллы", TabCrystals),
                ]
            };
        }

        private IPage CrystalsPage(Player p)
        {
            EnsureEntity(p);

            using var db = new DataBase();
            var freshSpot = db.spots
                .Include(s => s.selected)
                .FirstOrDefault(s => s.id == this.id);
            if (freshSpot == null) return null;
            this.selected = freshSpot.selected;

            var botCrys = entity?.crys?.cry ?? new long[6];
            var totalCrys = Enumerable.Range(0, 6).Select(i => p.crys.cry[i] + (i < botCrys.Length ? botCrys[i] : 0)).ToArray();
            var crysLines = totalCrys.Select((_, id) => new CrysLine("", 0, 0, totalCrys[id], id < botCrys.Length ? botCrys[id] : 0)).ToArray();

            return new Page
            {
                Title = "Кристаллы",
                Card = new Card(CardImageType.Item, ((int)Item.SpotBot).ToString(),
                    $"<color=white>TRANSFER</color>\n<color=#7c88a6>Перетащи кристаллы между игроком и спотом.</color>"),
                CrystalConfig = new CrystalConfig("у игрока", "в споте", crysLines),
                RichList = new RichListConfig(
                    [
                        RichListEntry.Text("total_title", "<size=18><color=#b7c6ff>Итого</color></size>"),
                        RichListEntry.Text("player_crys", $"У игрока: <color=#d5ffe8>{p.crys.cry.Sum()}</color>"),
                        RichListEntry.Text("spot_crys", $"В споте: <color=#ffe6a6>{botCrys.Sum()}</color>"),
                        RichListEntry.Text("sum_crys", $"Сумма: <color=white>{totalCrys.Sum()}</color>"),
                        RichListEntry.Text("spacer", " "),
                    ],
                    NoScroll: false
                ),
                Buttons =
                [
                    new MButton("Применить перевод", $"spotgive:{ActionMacros.CrystalSliders}", args => GiveCrystals(args.CrystalSliders, p)),
                    new MButton("Программы", TabPrograms),
                ]
            };
        }

        public override Window? GUIWin(Player p)
        {
            if (p.id != ownerid) return null;
            return new Window()
            {
                ShowTabs = true,
                Title = "BOT-SPOT",
                Tabs =
                [
                    new Tab()
                    {
                        Action = TabPrograms,
                        Label = "Программы",
                        InitialPage = ProgramsPage(p)
                    },
                    new Tab()
                    {
                        Action = TabCrystals,
                        Label = "Кристаллы",
                        InitialPage = CrystalsPage(p)
                    }
                ]
            };
        }
    }
}
