using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.HubEvents.Bots;
using MinesServer.Network.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinesServer.Server;

namespace MinesServer.GameShit.Entities
{
    public class BotSpot : PEntity
    {
        public BotSpot(int x,int y,Player owner)  {
            id = -owner.id;
            _pdata = new(this);
            this.x = x;this.y = y;this.owner = owner;
            crys = new(true);
            crys.Changed += Translate;
            MaxHealth = 100;
            Health = 100;
        }
        public int tail => 1;
        public int skin => 3;
        public override int cid => owner.cid;
        public Player? owner { get; set; }
        public override Basket crys { get; set; }
        private void Translate()
        {
            if (owner is null)
            {
                return;
            }

            try
            {
                using var db = new DataBase();
                var spot = db.spots.FirstOrDefault(s => s.ownerid == owner.id);
                if (spot is null)
                {
                    return;
                }

                db.Attach(spot);
                spot.botx = x;
                spot.boty = y;
                spot.basket = crys.serialazed ?? string.Empty;
                db.SaveChanges();
            }
            catch
            {
                // ignore persistence issues for bot state
            }
        }

        public override void Build(string type)
        {
           
        }
        private float cb;
        public override void Bz()
        {
            if (owner is null)
            {
                return;
            }
            ResourceExtractionService.PerformDig(this, owner, owner.skillslist.skills.Values, ref cb, crys);
        }
        private void OnDestroy(byte type)
        {
            // Logic moved into ResourceExtractionService.PerformDig
        }
        
        public override void Death()
        {
            if (crys.AllCry > 0 && owner is not null)
            {
                var (bx, by) = World.FindEmptyForBox(x, y);
                Box.BuildBox(bx, by, crys.cry, owner, true);
            }

            SendFXoBots(x, y, 2);
            Health = MaxHealth;
            Translate();
        }
        public override void Geo() => base.Geo();

        public override bool Heal(int num = -1)
        {
            if (owner is null)
            {
                return false;
            }

            var heal = owner.skillslist.skills.Values.FirstOrDefault(i => i is not null && i.type == SkillType.Repair);
            if (Health == MaxHealth || heal == default)
                return false;
            num = (int)heal.Effect;
            if (num == -1)
                return false;
            if (crys.RemoveCrys(2, 1))
            {
                heal.AddExp(owner);
                Health += num;
                if (Health > MaxHealth)
                    Health = MaxHealth;
                SendDFToBots(5, 0, 0, id, 0);
                Translate();
                return true;
            }
            return false;
        }
        public override void Hurt(int num, DamageTypePlayer type = DamageTypePlayer.Pure)
        {
            if (owner is null)
            {
                return;
            }
            foreach (var c in owner.skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnHealth, owner))
                {
                    if (c.type == SkillType.Health)
                    {
                        c.AddExp(owner);
                    }
                }
                if (c != null && c.UseSkill(SkillEffectType.OnHurt, owner) && type == DamageTypePlayer.Gun)
                {
                    if (c.type == SkillType.Induction)
                    {
                        c.AddExp(owner);
                    }
                    if (c.type == SkillType.AntiGun)
                    {
                        c.AddExp(owner);
                        var eff = (int)(num * (c.Effect / 100));
                        if (num - eff >= 0)
                        {
                            num -= eff;
                        }
                        else
                        {
                            num = 0;
                        }
                    }
                }
            }
            if (Health - num > 0)
            {
                Health -= num;
                SendDFToBots(6, 0, 0, id, 0);
            }
            else
            {
                Death();
            }
        }

        public override bool Move(int x, int y, int dir = -1, bool prog = false)
        {
            if (!World.ValidCoord(x, y))
            {
                return false;
            }

            if (dir > 9)
                dir -= 10;
            if (dir == -1 || this.x != x || this.y != y)
                this.dir = this.x > x ? 1 : this.x < x ? 3 : this.y > y ? 2 : 0;
            else
                this.dir = dir;

            var cell = World.GetCell(x, y);
            if (!World.GetProp(cell).isEmpty)
            {
                return false;
            }

            this.x = x;
            this.y = y;
            Translate();
            return true;
        }
        public void SendMyMove()
        {
            SendBotsInfo(id, x, y, dir, skin, cid, tail);
        }
        public override void Update()
        {
            if (_pdata.ProgRunning)
            {
                SendMyMove();
                _pdata.Step();
            }
        }
    }
}
