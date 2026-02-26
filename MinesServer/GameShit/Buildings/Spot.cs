using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
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
using Newtonsoft.Json;

namespace MinesServer.GameShit.Buildings
{
    public class Spot : Pack, IDamagable
    {
        public override PackType type => PackType.Spot;
        #region Shit
        [NotMapped]
        public override float charge { get; set; }
        [NotMapped]
        public override int cid { get; set; }
        public override int PackId => 47;
        #endregion
        public DateTime brokentimer { get; set; }
        public int hp { get; set; }
        public int maxhp { get; set; }
        public Program? selected { get; set; }
        public BotSpot? entity;
        [NotMapped]
        public int botx { get; set; }
        [NotMapped]
        public int boty { get; set; }
        [NotMapped]
        public string basket { get; set; } = string.Empty;
        private Spot() { }
        public Spot(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            maxhp = 100;
            hp = 100;
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
            p.win = GUIWin(p);
        }
        private void UninstallProgram(Player p)
        {
            using var db = new DataBase();

            var spot = db.spots.FirstOrDefault(s => s.id == this.id);
            if (spot == null) return;

            spot.selected = null;

            db.SaveChanges();

            this.selected = null;

            p.win = GUIWin(p);
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

            p.win = GUIWin(p);
        }

        private void LaunchProgram(Player p)
        {
            EnsureEntity(p);
            if (entity == null || selected == null)
                return; // Нет выбранной программы
            entity.programsData.Run(selected);
            p.win = GUIWin(p);
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
            p.win = GUIWin(p);
        }

        private IPage MainPage(Player p)
        {
            EnsureEntity(p);

            using (var db = new DataBase())
            {
                // Получаем свежую копию с загруженным selected
                var freshSpot = db.spots
                    .Include(s => s.selected)
                    .FirstOrDefault(s => s.id == this.id);

                if (freshSpot == null) return null;

                // Копируем нужные поля
                this.selected = freshSpot.selected;

                var botCrys = entity?.crys?.cry ?? new long[6];
                var totalCrys = Enumerable.Range(0, 6).Select(i => p.crys.cry[i] + (i < botCrys.Length ? botCrys[i] : 0)).ToArray();
                var crysLines = totalCrys.Select((_, id) => new CrysLine("", 0, 0, totalCrys[id], id < botCrys.Length ? botCrys[id] : 0)).ToArray();

                var progs = db.progs
                    .Include(pg => pg.owner)
                    .Where(pr => pr.owner != null && pr.owner.id == p.id)
                    .ToList();

                var programList = progs.Select(pr => new ListEntry(
                    pr.name + (freshSpot.selected?.id == pr.id ? " ✓" : ""),
                    new MButton(freshSpot.selected?.id == pr.id ? "Выбрано" : "Установить",
                               freshSpot.selected?.id == pr.id ? "uninstallprog" : $"installprog:{pr.id}",
                               (args) => {
                                   if (freshSpot.selected?.id == pr.id)
                                       UninstallProgram(p);
                                   else
                                       InstallProgram(pr.id, p);
                               })
                )).ToArray();

                var launchEnabled = freshSpot.selected != null;
                var isRunning = entity?.programsData.ProgRunning == true;

                var buttons = new List<MButton>
                {
                    new MButton("Передать кристаллы", $"spotgive:{ActionMacros.CrystalSliders}",
                               (args) => GiveCrystals(args.CrystalSliders, p))
                };

                if (launchEnabled && !isRunning)
                    buttons.Add(new MButton("Запустить программу", "spotlaunch",
                               (args) => LaunchProgram(p)));

                if (isRunning)
                    buttons.Add(new MButton("Остановить программу", "spotstop",
                               (args) => StopProgram(p)));

                return new Page()
                {
                    Title = "Спот",
                    CrystalConfig = new CrystalConfig("у игрока", "в споте", crysLines),
                    List = programList,
                    Buttons = buttons.ToArray()
                };
            }
        }

        public override Window? GUIWin(Player p)
        {
            if (p.id != ownerid) return null;
            return new Window()
            {
                Title = "Спот",
                Tabs = [new Tab()
                {
                    Action = "Spot",
                    Label = "Спот",
                    InitialPage = MainPage(p)
                }]
            };
        }
    }
}
