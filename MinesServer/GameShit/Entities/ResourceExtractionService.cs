using MinesServer.Enums;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Skills;
using MinesServer.GameShit.WorldSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinesServer.GameShit.Entities
{
    /// <summary>
    /// Shared logic for resource extraction (digging/mining) for player-like entities.
    /// Players and BotSpots differ only by which skills owner is used.
    /// </summary>
    public static class ResourceExtractionService
    {
        public static int ParseCryType(CellType cell)
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

        private static void Mine(
            PEntity actor,
            Player skillOwner,
            IEnumerable<Skill?> skills,
            ref float cb,
            Basket basket,
            byte cell,
            int x,
            int y)
        {
            float dob = 1 + (float)Math.Truncate(cb);

            foreach (var c in skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDigCrys, skillOwner))
                {
                    if (c.type == SkillType.MineGeneral)
                    {
                        dob += c.Effect;
                        c.AddExp(skillOwner, (float)Math.Truncate(dob));
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

            basket.AddCrys(type, odob);
            World.AddDob(type, odob);

            actor.SendDFToBots(
                2,
                x,
                y,
                actor.id,
                (int)(odob < 255 ? odob : 255),
                type == 1 ? 3 : type == 2 ? 1 : type == 3 ? 2 : type);
        }

        private static void OnDestroy(Player skillOwner, IEnumerable<Skill?> skills)
        {
            foreach (var c in skills)
            {
                if (c != null && c.UseSkill(SkillEffectType.OnDig, skillOwner))
                {
                    c.AddExp(skillOwner);
                }
            }
        }

        /// <summary>
        /// Performs a dig / mining action in front of the actor, including damage,
        /// crystal extraction, and skill experience.
        /// </summary>
        public static void PerformDig(
            PEntity actor,
            Player skillOwner,
            IEnumerable<Skill?> skills,
            ref float cb,
            Basket basket)
        {
            var cord = actor.GetDirCord();
            int x = cord.x, y = cord.y;
            if (!World.ValidCoord(x, y))
            {
                return;
            }

            actor.SendDFToBots(0, actor.x, actor.y, actor.id, actor is Player p ? p.dir : actor.dir);

            var cell = World.GetCell(x, y);
            var prop = World.GetProp(cell);

            if (prop.damage > 0)
            {
                actor.Hurt(prop.damage);
            }

            if (!prop.is_diggable)
            {
                return;
            }

            if (cell == 90)
            {
                if (actor is Player player)
                {
                    player.GetBox(x, y);
                }
                else
                {
                    actor.GetBox(x, y);
                }

                World.DamageCell(x, y, 1);
                return;
            }

            if (cell == (byte)CellType.MilitaryBlock)
            {
                World.DamageCell(x, y, 1);
                return;
            }

            float hitdmg = 0.2f;

            if (World.isCry(x, y))
            {
                hitdmg = 1f;
                Mine(actor, skillOwner, skills, ref cb, basket, cell, x, y);
            }
            else
            {
                foreach (var c in skills)
                {
                    if (c != null && c.UseSkill(SkillEffectType.OnDig, skillOwner))
                    {
                        hitdmg = c.type switch
                        {
                            SkillType.Digging => hitdmg * (c.Effect / 100f),
                            _ => 1f
                        };
                    }
                }
            }

            if (World.DamageCell(x, y, hitdmg))
            {
                OnDestroy(skillOwner, skills);
            }

            if (prop.isBoulder)
            {
                var plusy = actor.dir == 2 ? -1 : actor.dir == 0 ? 1 : 0;
                var plusx = actor.dir == 3 ? 1 : actor.dir == 1 ? -1 : 0;
                if (World.GetProp(World.GetCell(x + plusx, y + plusy)).isEmpty)
                {
                    World.MoveCell(x, y, plusx, plusy);
                    foreach (var c in skills)
                    {
                        if (c != null && c.UseSkill(SkillEffectType.OnDig, skillOwner))
                        {
                            c.AddExp(skillOwner);
                        }
                    }
                }
            }
        }
    }
}

