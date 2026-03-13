using MinesServer.Enums;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

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
        ref float cb,
        Basket basket,
        byte cell,
        int x,
        int y,
        float multiplier)
    {

        // Применяем множитель в зависимости от типа кристалла
        multiplier *= (CellType)cell switch
        {
            CellType.XGreen => 4,
            CellType.XBlue => 3,
            CellType.XRed => 2,
            CellType.XViolet => 2,
            CellType.XCyan => 2,
            _ => 1
        };

        cb -= (float)Math.Truncate(cb);
        long odob = (long)Math.Truncate(multiplier);
        var type = ParseCryType((CellType)cell);
        cb += multiplier - odob;

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

    /// <summary>
    /// Performs a dig / mining action in front of the actor, including damage,
    /// crystal extraction, and skill experience.
    /// </summary>
    public static void PerformDig(
        PEntity actor,
        Player skillOwner,
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

        bool IsCry = World.isCry(x, y);
        if (IsCry)
        {
            hitdmg = 1f;

            // Получаем множитель добычи через навыки владельца
            float multiplier = skillOwner.skillslist.GetMiningMultiplier(skillOwner, ref cb);

            // Передаём множитель в Mine
            Mine(actor, skillOwner, ref cb, basket, cell, x, y, multiplier);

            // Начисляем опыт за добычу кристаллов
            skillOwner.skillslist.HandleMiningExperience(skillOwner, 1);

            // Начисляем опыт за удар по кристаллу
            skillOwner.skillslist.HandleDiggingExperience(skillOwner, 1);

        }
        else
        {
            // Получаем множитель урона от навыка Digging
            hitdmg = skillOwner.skillslist.GetDiggingDamageMultiplier(hitdmg);
        }

        if (World.DamageCell(x, y, hitdmg))
        {
            // Начисляем опыт за разрушение блока
            if (!IsCry)
                skillOwner.skillslist.HandleDestructionExperience(skillOwner);
        }

        if (prop.isBoulder)
        {
            var plusy = actor.dir == 2 ? -1 : actor.dir == 0 ? 1 : 0;
            var plusx = actor.dir == 3 ? 1 : actor.dir == 1 ? -1 : 0;
            if (World.GetProp(World.GetCell(x + plusx, y + plusy)).isEmpty)
            {
                World.MoveCell(x, y, plusx, plusy);

                // Начисляем опыт за перемещение валуна
                skillOwner.skillslist.HandleBoulderMoveExperience(skillOwner);
            }
        }
    }

    public static bool PerformRepair(
        PEntity actor,
        Player skillOwner)
    {
        if (actor == null || skillOwner == null)
            return false;

        if (actor.Health == actor.MaxHealth)
            return false;

        var heal = skillOwner.skillslist.skills.Values.FirstOrDefault(s => s.type == SkillType.Repair);
        if (heal == default)
            return false;

        if (!actor.crys.RemoveCrys(2, 1))
            return false;

        heal.AddExp(skillOwner);

        actor.Health = Math.Min(actor.Health + (int)heal.Effect, actor.MaxHealth);

        actor.SendDFToBots(5, 0, 0, actor.id, 0);
        if (actor is Player)
        {
            ((Player)actor).SendHealth();
        }
        return true;
    }
}