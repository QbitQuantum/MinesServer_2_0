using MinesServer.Enums;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

public static class ResourceExtractionService
{
    public static CrystalType ParseCryType(CellType cell)
    {
        return cell switch
        {
            CellType.XGreen or CellType.Green => CrystalType.Green,
            CellType.XBlue or CellType.Blue => CrystalType.Blue,
            CellType.XRed or CellType.Red => CrystalType.Red,
            CellType.XViolet or CellType.Violet => CrystalType.Violet,
            CellType.White => CrystalType.White,
            CellType.XCyan or CellType.Cyan => CrystalType.Cyan,
            _ => CrystalType.Unknown
        };
    }
    public static void PerformDig(
        PEntity actor,
        Player skillOwner,
        ref float mainCb,           // CB для основных кристаллов (MineGeneral)
        ref CrystalCBStorage allCb,  // CB для всех типов кристаллов
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

        CellType cellType = (CellType)cell;

        if (prop.damage > 0)
        {
            actor.Hurt(prop.damage);
        }

        if (!prop.is_diggable)
        {
            return;
        }

        if (cellType == CellType.Box)
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

        if (cellType == CellType.MilitaryBlock)
        {
            World.DamageCell(x, y, 1);
            return;
        }

        float hitdmg = 0.2f;

        bool IsCry = World.isCry(x, y);
        if (IsCry)
        {
            hitdmg = 1f;

            CrystalType crystalType = ParseCryType(cellType);

            // Добываем основные кристаллы через MineGeneral
            float mainMultiplier = skillOwner.skillslist.GetMiningMultiplier(ref mainCb, SkillType.MineGeneral);
            Mine(actor, ref mainCb, basket, cellType, x, y, mainMultiplier, crystalType);

            // Проверяем наличие навыков для дополнительной добычи
            bool hasAdjacentExtraction = skillOwner.skillslist.HasSkill(SkillType.AdjacentExtraction);
            bool hasSort = skillOwner.skillslist.HasSkill(SkillType.Sort);

            // Смежное извлечение (зеленые <-> синие)
            if (hasAdjacentExtraction)
            {
                CrystalType? additionalType = null;

                if (crystalType == CrystalType.Green)
                    additionalType = CrystalType.Blue;
                else if (crystalType == CrystalType.Blue)
                    additionalType = CrystalType.Green;

                if (additionalType.HasValue)
                {
                    float adjacentMultiplier = skillOwner.skillslist.GetSkillEffect(SkillType.AdjacentExtraction);
                    // Используем общее хранилище CB для этого типа кристаллов
                    float typeCb = allCb.Get(additionalType.Value);
                    Mine(actor, ref typeCb, basket, cellType, x, y, adjacentMultiplier, additionalType.Value);
                    allCb.Set(additionalType.Value, typeCb);

                    // Начисляем опыт
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.AdjacentExtraction, 1f);
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Extraction, 1f);
                }
            }

            // Сортировка (красные -> фиолетовые -> голубые -> белые -> красные)
            if (hasSort)
            {
                var conversionChain = new Dictionary<CrystalType, CrystalType>
                {
                    { CrystalType.Red, CrystalType.Violet },
                    { CrystalType.Violet, CrystalType.Cyan },
                    { CrystalType.Cyan, CrystalType.White },
                    { CrystalType.White, CrystalType.Red }
                };

                if (conversionChain.TryGetValue(crystalType, out CrystalType convertedType))
                {
                    float sortMultiplier = skillOwner.skillslist.GetSkillEffect(SkillType.Sort);
                    // Используем общее хранилище CB для этого типа кристаллов
                    float typeCb = allCb.Get(convertedType);
                    Mine(actor, ref typeCb, basket, cellType, x, y, sortMultiplier, convertedType);
                    allCb.Set(convertedType, typeCb);

                    // Начисляем опыт
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Sort, 1f);
                }
            }

            // Начисляем опыт за основные кристаллы
            switch (crystalType)
            {
                case CrystalType.Green:
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineGreen, 1f);
                    break;
                case CrystalType.Blue:
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineBlue, 1f);
                    break;
                case CrystalType.Red:
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineRed, 1f);
                    break;
                case CrystalType.Violet:
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineViolet, 1f);
                    break;
                case CrystalType.White:
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineWhite, 1f);
                    break;
                case CrystalType.Cyan:
                    skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineCyan, 1f);
                    break;
            }

            // Начисляем общий опыт за добычу
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

    private static void Mine(
        PEntity actor,
        ref float cb,
        Basket basket,
        CellType cell,
        int x,
        int y,
        float multiplier,
        CrystalType forcedType)
    {
        bool isAdditional = forcedType != ParseCryType(cell);

        // Применяем множитель в зависимости от типа кристалла
        if (!isAdditional)
        {
            // Это основной кристалл - умножаем на тип ячейки
            multiplier *= cell switch
            {
                CellType.XGreen => 4,
                CellType.XBlue => 3,
                CellType.XRed => 2,
                CellType.XViolet => 2,
                CellType.XCyan => 2,
                _ => 1
            };
        }

        // Добавляем накопленный CB к множителю
        float totalWithCb = multiplier + cb;

        // Определяем количество целых кристаллов
        long odob = (long)Math.Truncate(totalWithCb);

        // Обновляем CB - оставляем только дробную часть
        cb = totalWithCb - odob;

        if (odob <= 0)
            return;

        // Отправляем пакет с добычей
        int sendType = forcedType switch
        {
            CrystalType.Blue => 3,
            CrystalType.Red => 1,
            CrystalType.Violet => 2,
            _ => (int)forcedType
        };

        // Добавляем кристаллы в корзину
        basket.AddCrys(forcedType, odob);
        // Добавляем в мировую статистику
        World.AddDob(forcedType, odob);

        actor.SendDFToBots(
            2,
            x,
            y,
            actor.id,
            (int)(odob < 255 ? odob : 255),
            sendType);
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

        if (!actor.crys.RemoveCrys(CrystalType.Red, 1))
            return false;

        heal.AddExp(skillOwner);

        actor.AddHp((int)heal.Effect);

        actor.SendDFToBots(5, 0, 0, actor.id, 0);
        if (actor is Player)
        {
            ((Player)actor).SendHealth();
        }
        return true;
    }
}