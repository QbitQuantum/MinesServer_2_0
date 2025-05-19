using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        public int tail => 1;
        public int skin => 3;
        public override int cid => owner.cid;
        public Player? owner { get; set; }
        public override Basket crys { get; set; }
        private void Translate()
        {
            Console.WriteLine("should save basket and pos");
        }

        public override void Build(string type)
        {
           
        }
        private float cb;
        private void Mine(byte cell, int x, int y)
        {
            float dob = 1 + (float)Math.Truncate(cb);
            foreach (var c in owner.skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDigCrys, owner))
                {
                    if (c.type == SkillType.MineGeneral)
                    {
                        dob += c.Effect;
                        c.AddExp(owner, (float)Math.Truncate(dob));
                    }
                }
            }
            dob *= (CellType)cell switch
            {
                CellType.XGreen => 4,
                CellType.XBlue => 3,
                CellType.XRed => 2,
                CellType.XViolet => 2,
                CellType.XCyan => 2,
                _ => 1
            };
            cb -= (float)Math.Truncate(cb);
            long odob = (long)Math.Truncate(dob);
            var type = ParseCryType((CellType)cell);
            cb += dob - odob;
            crys.AddCrys(type, odob);
            World.AddDob(type, odob);
            SendDFToBots(2, x, y, id, (int)(odob < 255 ? odob : 255), type == 1 ? 3 : type == 2 ? 1 : type == 3 ? 2 : type);
        }
        private int ParseCryType(CellType cell)
        {
            return cell switch
            {
                CellType.XGreen or CellType.Green => 0,
                CellType.XBlue or CellType.Blue => 1,
                CellType.XRed or CellType.Red => 2,
                CellType.XViolet or CellType.Violet => 3,
                CellType.White => 4,
                CellType.XCyan or CellType.Cyan => 5,
                _ => 0
            };
        }
        public override void Bz()
        {
            var cord = GetDirCord();
            int x = cord.x, y = cord.y;
            if (!World.W.ValidCoord(x, y))
            {
                return;
            }
            SendDFToBots(0, this.x, this.y, id, dir);
            var cell = World.GetCell(x, y);
            if (World.GetProp(cell).damage > 0)
            {
                Hurt(World.GetProp(cell).damage);
            }
            if (!World.GetProp(cell).is_diggable)
            {
                return;
            }
            if (cell == 90)
            {
                GetBox(x, y);
                World.DamageCell(x, y, 1);
                return;
            }
            if (cell == (byte)CellType.MilitaryBlock)
            {
                World.DamageCell(x, y, 1);
                return;
            }
            float hitdmg = 0.2f;
            if (World.isCry(cell))
            {
                hitdmg = 1f;
                Mine(cell, x, y);
            }
            else
            {
                foreach (var c in owner.skillslist.skills.Values)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnDig, owner))
                    {
                        hitdmg = c.type switch
                        {
                            SkillType.Digging => hitdmg * (c.Effect / 100f),
                            _ => 1f
                        };
                    }
                }
            }
            if (World.DamageCell(x, y, hitdmg)) OnDestroy(cell);
            if (World.GetProp(cell).isBoulder)
            {
                var plusy = dir == 2 ? -1 : dir == 0 ? 1 : 0;
                var plusx = dir == 3 ? 1 : dir == 1 ? -1 : 0;
                if (World.GetProp(World.GetCell(x + plusx, y + plusy)).isEmpty)
                {
                    World.MoveCell(x, y, plusx, plusy);
                    foreach (var c in owner.skillslist.skills.Values)
                    {
                        if (c != null && c.UseSkill(SkillEffectType.OnDig, owner))
                        {
                            c.AddExp(owner);
                        }
                    }
                }
            }
        }
        private void OnDestroy(byte type)
        {
            foreach (var c in owner.skillslist.skills.Values)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDig, owner))
                {
                    c.AddExp(owner);
                }
            }
        }

        public override void Death()
        {
           
        }

        public override void Geo() => base.Geo();

        public override bool Heal(int num = -1)
        {
            return false;
        }

        public override void Hurt(int num, DamageType type = DamageType.Pure)
        {
            
        }

        public override bool Move(int x, int y, int dir = -1, bool prog = false)
        {
            return false;
        }

        public override void Update() => _pdata.Step();
    }
}
