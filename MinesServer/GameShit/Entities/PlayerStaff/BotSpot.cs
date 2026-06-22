using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using MinesServer.Enums;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit.Entities.PlayerStaff
{
    public class BotSpot : PEntity
    {
        public BotSpot() { }
        public BotSpot(int x, int y, Player owner)
        {
            _pdata = new(this);
            id = DataBase.GetNextId();
            this.x = x;
            this.y = y;
            this.owner = owner;
            crys = new Basket(true);
            MaxHealth = 100;
            Health = 100;

            name = owner?.name ?? "BotSpot #" + id;
            using var db = new DataBase();
            db.botspots.Add(this);
            db.SaveChanges();
        }
        public override int tail => 1;
        public override int skin => 3;
        public string name { get; set; } = string.Empty;
        public int owner_id { get; set; } = 0;
        public override int cid => owner?.cid ?? 0;
        [NotMapped] public Player? owner { get; set; }
        public override Basket crys { get; set; }

        public override void Build(string type) { } // Боты не строят

        public override void Bz()
        {
            if (owner is null)
                return;

            // Просто вызываем сервис, опыт начисляется внутри через owner.skillslist
            ResourceExtractionService.PerformDig(this, owner, crys);
        }

        public override void Death()
        {
            if (programsData.ProgRunning)
                programsData.Run();

            if (crys.AllCry > 0 && owner is not null)
            {
                var (bx, by) = World.FindEmptyForBox(x, y);
                Box.BuildBox(bx, by, crys.cry, owner, true);
            }
            ResetHp();

            DataBase.botspotplayer.Remove(this);

            World.W.SendFx(x, y, 2);
            World.W.SendLeaveBot(id, x, y);

            using var db = new DataBase();
            db.botspots.Remove(this);
            db.SaveChanges();
        }

        public override void Geo()
        {
            if (owner == null)
                return;

            ResourceExtractionService.PerformGeo(this, owner);
        }

        public override bool Heal()
        {
            if (owner == null)
                return false;

            return ResourceExtractionService.PerformRepair(this, owner);
        }

        public override void Hurt(int damage, DamageTypePlayer type = DamageTypePlayer.Pure)
        {
            if (owner is null)
                return;

            // Обработка опыта через PlayerSkills 
            owner.skillslist.HandleDamageExperience(owner, type, 1);

            // Получение модифицированного урона
            int modifiedDamage = owner.skillslist.HandleDamageReceived(damage);

            if (Health - modifiedDamage > 0)
            {
                Health -= modifiedDamage;
                SendDFToBots(6, 0, 0, id, 0);
            }
            else
            {
                Death();
            }
        }

        public override bool Move(int x, int y, DirectionType Direction = DirectionType.Unknown)
        {
            if (!World.ValidCoord(x, y))
                return false;

            UpdateDirection(x, y, Direction);

            var cell = World.GetCell(x, y);
            if (!World.GetProp(cell).isEmpty)
                return false;

            this.x = x;
            this.y = y;

            if (Vector2.Distance(new Vector2(this.x, this.y), new Vector2(x, y)) < 1.2f)
            {
                owner.skillslist.HandleExperience(owner, SkillType.Movement, 1);
            }
            return true;
        }

        public override void Update()
        {
            if (HadleProgramm())
                BotsRender();
        }
    }
}