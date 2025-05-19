using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion
        public DateTime brokentimer { get; set; }
        public int hp { get; set; }
        public int maxhp { get; set; }
        public Program? selected { get; set; }
        public BotSpot? entity;
        public int botx => entity.x;
        public int boty => entity.y;
        public string basket => entity.crys.serialazed;
        private Spot() { }
        public Spot(int x, int y, int ownerid) : base(x, y, ownerid)
        {
            maxhp = 100;
            hp = 100;
            using var db = new DataBase();
            db.spots.Add(this);
            db.SaveChanges();
        }
        public override void Build()
        {
            World.SetCell(x, y, 32, true);
            base.Build();
        }
        public void Destroy(Player p)
        {
            World.SetCell(x, y, 32, false);
            //idk
        }

        public override Window? GUIWin(Player p)
        {
            if (p.id != ownerid) return null;
            return new Window()
            { Tabs = [] };
        }

        protected override void ClearBuilding()
        {
            //No dick no balls
        }
    }
}
