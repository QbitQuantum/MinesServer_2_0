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
    ref float mainCb,
    ref CrystalCBStorage allCb,
    Basket basket)
    {
        // Базовая валидация
        if (!ValidateDig(actor, out int x, out int y, out Cell prop, out CellType cellType))
            return;

        // Обработка спецблоков
        if (TryHandleSpecialBlocks(actor, x, y, cellType))
            return;

        // Добыча кристаллов или обычных блоков
        if (World.isCry(x, y))
        {
            ProcessCrystalMining(skillOwner, x, y, cellType, basket, ref mainCb, ref allCb);
        }
        else
        {
            ProcessRegularDigging(skillOwner, cellType, x, y, basket);
        }

        skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Digging, 1f);

        // Физика валунов
        if (prop.isBoulder)
        {
            TryMoveBoulder(actor, skillOwner, x, y);
        }
    }

    private static bool ValidateDig(PEntity actor, out int x, out int y, out Cell prop, out CellType cellType)
    {
        var cord = actor.GetDirCord();
        x = cord.x;
        y = cord.y;

        if (!World.ValidCoord(x, y))
        {
            prop = null;
            cellType = 0;
            return false;
        }

        actor.SendDFToBots(0, actor.x, actor.y, actor.id, actor is Player p ? p.dir : actor.dir);

        var cell = World.GetCell(x, y);
        prop = World.GetProp(cell);
        cellType = (CellType)cell;

        if (prop.damage > 0)
        {
            actor.Hurt(prop.damage);
        }

        return prop.is_diggable;
    }

    private static bool TryHandleSpecialBlocks(PEntity actor, int x, int y, CellType cellType)
    {
        if (cellType == CellType.Box)
        {
            if (actor is Player player)
                player.GetBox(x, y);
            else
                actor.GetBox(x, y);

            World.DamageCell(x, y, 1);
            return true;
        }

        if (cellType == CellType.MilitaryBlock)
        {
            World.DamageCell(x, y, 1);
            return true;
        }

        return false;
    }

    private static void TryMoveBoulder(PEntity actor, Player skillOwner, int x, int y)
    {
        var plusy = actor.dir == 2 ? -1 : actor.dir == 0 ? 1 : 0;
        var plusx = actor.dir == 3 ? 1 : actor.dir == 1 ? -1 : 0;

        if (World.GetProp(World.GetCell(x + plusx, y + plusy)).isEmpty)
        {
            World.MoveCell(x, y, plusx, plusy);
            skillOwner.skillslist.HandleBoulderMoveExperience(skillOwner);
        }
    }

    // (баланс, навыки, опыт)
    private static void ProcessCrystalMining(
        Player skillOwner,
        int x,
        int y,
        CellType cellType,
        Basket basket,
        ref float mainCb,
        ref CrystalCBStorage allCb)
    {
        CrystalType crystalType = ParseCryType(cellType);

        // Основная добыча
        float mainMultiplier = skillOwner.skillslist.GetMiningMultiplier(ref mainCb, SkillType.MineGeneral);
        Mine(skillOwner, ref mainCb, basket, cellType, x, y, mainMultiplier, crystalType);

        // Смежное извлечение (зеленые <-> синие)
        if (skillOwner.skillslist.HasSkill(SkillType.AdjacentExtraction))
        {
            ProcessAdjacentExtraction(skillOwner, x, y, cellType, basket, crystalType, ref allCb);
        }

        // Сортировка (красные -> фиолетовые -> голубые -> белые -> красные)
        if (skillOwner.skillslist.HasSkill(SkillType.Sort))
        {
            ProcessSorting(skillOwner, x, y, cellType, basket, crystalType, ref allCb);
        }

        // Начисление опыта за кристаллы
        AwardCrystalExperience(skillOwner, crystalType);
        skillOwner.skillslist.HandleMiningExperience(skillOwner, 1);
        skillOwner.skillslist.HandleDiggingExperience(skillOwner, 1);
    }

    private static void ProcessAdjacentExtraction(
        Player skillOwner,
        int x,
        int y,
        CellType cellType,
        Basket basket,
        CrystalType originalType,
        ref CrystalCBStorage allCb)
    {
        CrystalType? additionalType = null;

        if (originalType == CrystalType.Green)
            additionalType = CrystalType.Blue;
        else if (originalType == CrystalType.Blue)
            additionalType = CrystalType.Green;

        if (additionalType.HasValue)
        {
            float adjacentMultiplier = skillOwner.skillslist.GetSkillEffect(SkillType.AdjacentExtraction);
            float typeCb = allCb.Get(additionalType.Value);
            Mine(skillOwner, ref typeCb, basket, cellType, x, y, adjacentMultiplier, additionalType.Value);
            allCb.Set(additionalType.Value, typeCb);

            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.AdjacentExtraction, 1f);
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Extraction, 1f);
        }
    }

    private static void ProcessSorting(
        Player skillOwner,
        int x,
        int y,
        CellType cellType,
        Basket basket,
        CrystalType originalType,
        ref CrystalCBStorage allCb)
    {
        CrystalType convertedType;

        switch (originalType)
        {
            case CrystalType.Red:
                convertedType = CrystalType.Violet;
                break;
            case CrystalType.Violet:
                convertedType = CrystalType.Cyan;
                break;
            case CrystalType.Cyan:
                convertedType = CrystalType.White;
                break;
            case CrystalType.White:
                convertedType = CrystalType.Red;
                break;
            default:
                return;
        }

        float sortMultiplier = skillOwner.skillslist.GetSkillEffect(SkillType.Sort);
        float typeCb = allCb.Get(convertedType);
        Mine(skillOwner, ref typeCb, basket, cellType, x, y, sortMultiplier, convertedType);
        allCb.Set(convertedType, typeCb);

        skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Sort, 1f);
    }

    private static void AwardCrystalExperience(Player skillOwner, CrystalType crystalType)
    {
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
    }

    private static void ProcessDetection(
    Player skillOwner,
    int x,
    int y,
    CellType cellType,
    Basket basket)
    {

        // Проверяем, что копаем обычную породу (не кристаллы)
        if (World.isCry(x, y))
            return;

        // Проверяем тип породы - только для легкой скалы и тяжелой скалы
        if (!cellType.IsRock())
            return;

        var skill = cellType.IsLightRock() ? skillOwner.skillslist.GetSkill(SkillType.Detection) :
            cellType.IsHeavyRock() ? skillOwner.skillslist.GetSkill(SkillType.MineDeep) : null;

        if (skill == null)
            return;

        bool[] TypeCrys = new bool[CrystalTypeExt.CrysType.Length];

        int multi = 1;
        switch (cellType)
        {
            case CellType.Rock:
                TypeCrys[(int)CrystalType.Green] = true;
                multi *= 2; break;
            case CellType.BlueRock:
                TypeCrys[(int)CrystalType.Green] = true;
                TypeCrys[(int)CrystalType.Blue] = true;
                multi *= 3; break;
            case CellType.GoldenRock:
                TypeCrys[(int)CrystalType.Green] = true;
                TypeCrys[(int)CrystalType.Blue] = true;
                TypeCrys[(int)CrystalType.White] = true;
                multi *= 4; break;
            case CellType.GRock:
                TypeCrys[(int)CrystalType.Green] = true;
                TypeCrys[(int)CrystalType.Blue] = true;
                TypeCrys[(int)CrystalType.White] = true;
                TypeCrys[(int)CrystalType.Cyan] = true;
                multi *= 5; break;
            case CellType.Obsidian:
            case CellType.Coralite:
                TypeCrys[(int)CrystalType.Green] = true;
                TypeCrys[(int)CrystalType.Blue] = true;
                TypeCrys[(int)CrystalType.White] = true;
                TypeCrys[(int)CrystalType.Cyan] = true;
                TypeCrys[(int)CrystalType.Violet] = true;
                multi *= 6; break;
            case CellType.EtherealRock:
            case CellType.Ultralit:
                TypeCrys[(int)CrystalType.Green] = true;
                TypeCrys[(int)CrystalType.Blue] = true;
                TypeCrys[(int)CrystalType.White] = true;
                TypeCrys[(int)CrystalType.Cyan] = true;
                TypeCrys[(int)CrystalType.Violet] = true;
                TypeCrys[(int)CrystalType.Red] = true;
                multi *= 7; break;
        }

        // Считаем итоговое количество кристаллов: базовый эффект навыка * множитель породы
        float totalEffect = skill.Effect * multi;

        // Округляем до целого, но хотя бы 1 кристалл всегда даём
        int totalCrystals = Math.Max(1, (int)Math.Round(totalEffect));

        // Собираем список типов кристаллов, которые могут выпасть из этой породы
        var availableCrystalTypes = new List<CrystalType>();
        for (int i = 0; i < TypeCrys.Length; i++)
        {
            if (TypeCrys[i])
                availableCrystalTypes.Add((CrystalType)i);
        }

        if (availableCrystalTypes.Count == 0)
            return;

        // Раскидываем totalCrystals по типам так, чтобы каждый тип получил хотя бы 1 кристалл
        int remaining = totalCrystals;
        var distribution = new Dictionary<CrystalType, int>();

        // Идём по всем типам, кроме последнего
        for (int i = 0; i < availableCrystalTypes.Count - 1; i++)
        {
            // Сколько максимум можем взять сейчас, оставляя остальным хотя бы по 1
            int maxTake = remaining - (availableCrystalTypes.Count - i - 1);
            // Берём случайное число от 1 до maxTake
            int take = maxTake > 0 ? Physics.r.Next(1, maxTake + 1) : 1;
            distribution[availableCrystalTypes[i]] = take;
            remaining -= take;
        }
        // Последнему типу отдаём всё, что осталось
        distribution[availableCrystalTypes[availableCrystalTypes.Count - 1]] = remaining;

        foreach (var kvp in distribution)
        {
            CrystalType forcedType = kvp.Key;
            int amount = kvp.Value;

            int sendType = forcedType switch
            {
                CrystalType.Blue => 3,
                CrystalType.Red => 1,
                CrystalType.Violet => 2,
                _ => (int)forcedType
            };

            // Добавляем кристаллы в корзину
            basket.AddCrys(forcedType, amount);
            // Добавляем в мировую статистику
            World.AddDob(forcedType, amount);

            skillOwner.SendDFToBots(
                2,
                x,
                y,
                skillOwner.id,
                (int)(amount < 255 ? amount : 255),
                sendType);
        }
    }

    private static void ProcessRegularDigging(Player skillOwner, CellType cellType, int x, int y, Basket basket)
    {
        float hitdmg = 0.2f;
        hitdmg = skillOwner.skillslist.GetDiggingDamageMultiplier(hitdmg);

        if (World.DamageCell(x, y, hitdmg))
        {
            ProcessDetection(skillOwner, x, y, cellType, basket);
            AwardDestructionExperience(skillOwner, cellType);
        }
    }

    private static void AwardDestructionExperience(Player skillOwner, CellType cellType)
    {
        if (cellType.IsBoulder())
        {
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Fracturing, 1f);
        }

        if (cellType.IsSand())
        {
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Annihilation, 1f);
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.DeMagnetizing, 1f);
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Washing, 1f);
        }

        if (cellType.IsAcid() || cellType.IsActiveAcid())
        {
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.AntiSlime, 1f);
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Deactivation, 1f);
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineSlime, 1f);
        }

        if (cellType.IsRock())
        {
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Destruction, 1f);
            skillOwner.skillslist.HandleExperience(skillOwner, SkillType.TotalDestruction, 1f);

            if (cellType.IsLightRock())
            {
                skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Detection, 1f);
            }

            if (cellType.IsHeavyRock())
            {
                skillOwner.skillslist.HandleExperience(skillOwner, SkillType.MineDeep, 1f);
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

        World.DamageCell(x, y, 1);

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

    public static void PerformGeo(
        PEntity actor,
        Player skillOwner)
    {
        if (actor == null || skillOwner == null)
            return;

        var Geology = skillOwner.skillslist.skills.Values.FirstOrDefault(s => s.type == SkillType.Geology);
        
        if (Geology == default)
            return;

        var (x, y) = actor.GetDirCord();

        if (!World.ValidCoord(x, y)) return;

        var access = World.AccessGun(x, y, actor.cid).access;
        if (!access) return;

        var cell = World.GetCell(x, y);

        if (World.IsCollectable(x, y) && actor.geo.Count < Geology.Effect)
        {
            actor.geo.Push(cell);
            World.Destroy(x, y);
        }
        else if (actor.geo.Count > 0 && World.IsBlockedForPlacement(x, y) && !World.PackPart(x, y))
        {
            var placeable = actor.geo.Pop();
            World.SetCell(x, y, placeable);

            // Выносим проверку крио-блока и случайную прочность
            int durability = World.isCry(x, y) ? 0 :
                            (Physics.r.Next(1, 101) > 99 ? 0 : World.GetProp(placeable).durability);
            World.SetDurability(x, y, durability);
            
        }
        skillOwner.skillslist.HandleExperience(skillOwner, SkillType.Geology, 1f);
        skillOwner.SendGeo();
    }
}