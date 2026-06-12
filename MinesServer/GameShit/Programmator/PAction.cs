using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public ActionType type;
        public string label;
        public int num;
        public double delay;

        public PAction(ActionType t) : this(t, "", 0) { }
        public PAction(ActionType t, string label) : this(t, label, 0) { }
        public PAction(ActionType t, int num) : this(t, "", num) { }

        public PAction(ActionType t, string label, int num)
        {
            type = t;
            this.label = label ?? "";
            this.num = num;
            delay = 0;
        }

        private static readonly Dictionary<int, (int dx, int dy)> dirz = new()
        {
            { 0, (0, 1) },   // DOWN
            { 1, (-1, 0) },  // LEFT
            { 2, (0, -1) },  // UP
            { 3, (1, 0) }    // RIGHT
        };

        // Передаем father параметром
        private static void Check(PEntity p, Func<int, int, bool> func, PFunction father)
        {
            int checkX, checkY;
            if (father.startoffset != default)
            {
                var flip = p.programsData.flipstate ? -1 : 1;
                checkX = p.x + (flip * father.startoffset.x);
                checkY = p.y + (flip * father.startoffset.y);
            }
            else
            {
                checkX = p.x + (p.programsData.flipstate ?
                    -(p.programsData.shiftX + p.programsData.checkX) :
                    p.programsData.shiftX + p.programsData.checkX);
                checkY = p.y + (p.programsData.flipstate ?
                    -(p.programsData.shiftY + p.programsData.checkY) :
                    p.programsData.shiftY + p.programsData.checkY);
            }

            p.programsData.checkX = 0;
            p.programsData.checkY = 0;
            p.programsData.shiftX = 0;
            p.programsData.shiftY = 0;

            var result = func(checkX, checkY);

            if (father.state == null)
                father.state = result;
            else if (father.laststateaction == ActionType.Or)
                father.state = (bool)father.state || result;
            else if (father.laststateaction == ActionType.And)
                father.state = (bool)father.state && result;
            else
                father.state = result;
        }

        // Передаем father параметром
        private bool? CallWSAction(PEntity p)
        {
            switch (label.ToLower())
            {
                case "geo":
                    return type switch
                    {
                        ActionType.WritableState => p.geo.Count == num,
                        ActionType.WritableStateLower => p.geo.Count < num,
                        ActionType.WritableStateMore => p.geo.Count > num,
                        _ => null
                    };

                case "crys":
                    return type switch
                    {
                        ActionType.WritableState => p.crys?.AllCry == num,
                        ActionType.WritableStateLower => p.crys?.AllCry < num,
                        ActionType.WritableStateMore => p.crys?.AllCry > num,
                        _ => null
                    };

                case "red":
                    return type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Red] == num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Red] < num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Red] > num,
                        _ => null
                    };

                case "green":
                    return type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Green] == num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Green] < num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Green] > num,
                        _ => null
                    };

                case "blue":
                    return type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Blue] == num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Blue] < num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Blue] > num,
                        _ => null
                    };

                case "white":
                    return type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.White] == num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.White] < num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.White] > num,
                        _ => null
                    };

                case "violet":
                    return type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Violet] == num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Violet] < num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Violet] > num,
                        _ => null
                    };

                case "del":
                    delay = num;
                    return null;

                default:
                    return false;
            }
        }

        public object? Execute(PEntity p, PFunction father)
        {
            switch (type)
            {
                // === Движение ===
                case ActionType.MoveDown:
                    delay = p.ServerPause;
                    if (p.Move(p.x, p.y + 1))
                        delay += 200;
                    break;

                case ActionType.MoveUp:
                    delay = p.ServerPause;
                    if (p.Move(p.x, p.y - 1))
                        delay += 200;
                    break;

                case ActionType.MoveRight:
                    delay = p.ServerPause;
                    if (p.Move(p.x + 1, p.y))
                        delay += 200;
                    break;

                case ActionType.MoveLeft:
                    delay = p.ServerPause;
                    if (p.Move(p.x - 1, p.y))
                        delay += 200;
                    break;

                case ActionType.MoveForward:
                    delay = p.ServerPause;
                    var forward = p.GetDirCord();
                    if (p.Move(forward.x, forward.y))
                        delay += 200;
                    break;

                // === Вращение ===
                case ActionType.RotateDown:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Down);
                    break;

                case ActionType.RotateUp:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Up);
                    break;

                case ActionType.RotateLeft:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Left);
                    break;

                case ActionType.RotateRight:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Right);
                    break;

                case ActionType.RotateLeftRelative:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection((p.dir + 3) % 4));
                    break;

                case ActionType.RotateRightRelative:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection((p.dir + 1) % 4));
                    break;

                case ActionType.RotateRandom:
                    delay = p.ServerPause;
                    var rand = new Random(Guid.NewGuid().GetHashCode());
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection(rand.Next(4)));
                    break;

                // === Действия ===
                case ActionType.Dig:
                    delay = 100;
                    p.Bz();
                    break;

                case ActionType.BuildBlock:
                    delay = 100;
                    p.Build("G");
                    break;

                case ActionType.BuildPillar:
                    delay = 100;
                    p.Build("O");
                    break;

                case ActionType.BuildRoad:
                    delay = 100;
                    p.Build("R");
                    break;

                case ActionType.BuildMilitaryBlock:
                    delay = 100;
                    p.Build("V");
                    break;

                case ActionType.Geology:
                    delay = 100;
                    p.Geo();
                    break;

                case ActionType.Heal:
                    if (p.Heal())
                        delay = 200;
                    break;

                case ActionType.Beep:
                    p.Beep();
                    delay = 100;
                    break;

                // === Макросы ===
                case ActionType.MacrosMine:
                    var directions = new[] { p.dir, (p.dir + 1) % 4, (p.dir + 3) % 4 };
                    // Ищем первый кристалл за один проход
                    foreach (var dir in directions)
                    {
                        var (dx, dy) = dirz[dir];
                        if (World.isCry(p.x + dx, p.y + dy))
                        {
                            if (p.dir == dir)
                            {
                                p.Bz();
                                delay = 200;
                            }
                            else
                            {
                                p.Move(p.x, p.y, DirectionTypeExt.ToDirection(dir));
                                delay = p.ServerPause;
                            }
                            return true;
                        }
                    }
                    break;  // кристаллов нет

                case ActionType.MacrosHeal:
                    if (p.crys?[MinesServer.Enums.CrystalType.Red] > 0 && p.Health < p.MaxHealth)
                    {
                        if (p.Heal())
                        {
                            delay = 200;
                            return true;
                        }
                    }
                    break;

                case ActionType.MacrosDig:
                    var digPos = p.GetDirCord();
                    if (World.GetProp(digPos.x, digPos.y).is_diggable)
                    {
                        delay = 200;
                        p.Bz();
                        return true;
                    }
                    break;

                case ActionType.MacrosBuild:
                    var buildPos = p.GetDirCord();
                    if (World.GetProp(buildPos.x, buildPos.y).isEmpty)
                    {
                        delay = 200;
                        p.Build("G");
                        return true;
                    }
                    break;

                // === Сдвиги ===
                case ActionType.ShiftUp:
                    p.programsData.shiftY--;
                    break;

                case ActionType.ShiftDown:
                    p.programsData.shiftY++;
                    break;

                case ActionType.ShiftRight:
                    p.programsData.shiftX++;
                    break;

                case ActionType.ShiftLeft:
                    p.programsData.shiftX--;
                    break;

                case ActionType.ShiftForward:
                    p.programsData.shiftX += p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    p.programsData.shiftY += p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;

                // === Проверки направления ===
                case ActionType.CheckForward:
                    p.programsData.checkX = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    p.programsData.checkY = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckRightRelative:
                    p.programsData.checkX = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    p.programsData.checkY = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckLeftRelative:
                    p.programsData.checkX = p.dir switch
                    {
                        0 => -1,
                        2 => 1,
                        _ => 0
                    };
                    p.programsData.checkY = p.dir switch
                    {
                        1 => 1,
                        3 => -1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckUp:
                    p.programsData.checkX = 0;
                    p.programsData.checkY = -1;
                    break;

                case ActionType.CheckDown:
                    p.programsData.checkX = 0;
                    p.programsData.checkY = 1;
                    break;

                case ActionType.CheckRight:
                    p.programsData.checkX = 1;
                    p.programsData.checkY = 0;
                    break;

                case ActionType.CheckLeft:
                    p.programsData.checkX = -1;
                    p.programsData.checkY = 0;
                    break;

                case ActionType.CheckUpLeft:
                    p.programsData.checkX = -1;
                    p.programsData.checkY = -1;
                    break;

                case ActionType.CheckUpRight:
                    p.programsData.checkX = 1;
                    p.programsData.checkY = -1;
                    break;

                case ActionType.CheckDownLeft:
                    p.programsData.checkX = -1;
                    p.programsData.checkY = 1;
                    break;

                case ActionType.CheckDownRight:
                    p.programsData.checkX = 1;
                    p.programsData.checkY = 1;
                    break;

                // === Проверки состояния (теперь передаем father) ===
                case ActionType.IsHpLower100:
                    Check(p, (x, y) => p.Health < p.MaxHealth, father);
                    break;

                case ActionType.IsHpLower50:
                    Check(p, (x, y) => p.Health < p.MaxHealth / 2, father);
                    break;

                case ActionType.IsEmpty:
                    Check(p, (x, y) => World.GetProp(x, y).isEmpty, father);
                    break;

                case ActionType.IsNotEmpty:
                    Check(p, (x, y) => !World.GetProp(x, y).isEmpty, father);
                    break;

                case ActionType.IsAcid:
                    Check(p, (x, y) => ((CellType)World.GetCell(x, y)).IsAcid(), father);
                    break;
                case ActionType.IsRedRock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.RedRock, father);
                    break;

                case ActionType.IsBlackRock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.BlackRock, father);
                    break;

                case ActionType.IsBoulder:
                    Check(p, (x, y) => World.GetProp(x, y).isBoulder, father);
                    break;

                case ActionType.IsSand:
                    Check(p, (x, y) => World.GetProp(x, y).isSand, father);
                    break;

                case ActionType.IsUnbreakable:
                    Check(p, (x, y) => !World.GetProp(x, y).isEmpty && !World.GetProp(x, y).is_diggable, father);
                    break;

                case ActionType.IsBox:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.Box, father);
                    break;

                case ActionType.IsBreakableRock:
                    Check(p, (x, y) => World.GetProp(x, y).is_diggable, father);
                    break;

                case ActionType.IsCrystal:
                    Check(p, (x, y) => World.isCry(x, y), father);
                    break;

                case ActionType.IsGreenBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.GreenBlock, father);
                    break;

                case ActionType.IsYellowBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.YellowBlock, father);
                    break;

                case ActionType.IsRedBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.RedBlock, father);
                    break;

                case ActionType.IsFalling:
                    Check(p, (x, y) => World.GetProp(x, y).isSand || World.GetProp(x, y).isBoulder, father);
                    break;

                case ActionType.IsLivingCrystal:
                    Check(p, (x, y) => World.isAlive(x, y), father);
                    break;

                case ActionType.IsPillar:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.Support, father);
                    break;

                case ActionType.IsQuadBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.QuadBlock, father);
                    break;

                case ActionType.IsRoad:
                    Check(p, (x, y) => World.isRoad(x, y), father);
                    break;

                case ActionType.CheckGun:
                    Check(p, (x, y) => p.HasGun(), father);
                    break;

                // === Управление потоком ===
                case ActionType.RunSub:
                case ActionType.RunState:
                case ActionType.RunFunction:
                case ActionType.RunOnRespawn:
                    return label;

                case ActionType.ReturnFunction:
                case ActionType.ReturnState:
                    return father.state;

                case ActionType.Return:
                    return "";

                case ActionType.RunIfTrue:
                    if (father.state.HasValue && !father.state.Value)
                        return null;
                    father.state = null;
                    return label;

                case ActionType.RunIfFalse:
                    if (father.state.HasValue && father.state.Value)
                        return null;
                    father.state = null;
                    return label;

                case ActionType.Or:
                case ActionType.And:
                    father.laststateaction = type;
                    break;

                case ActionType.GoTo:
                    return label;

                // === Работа с памятью ===
                case ActionType.WritableState:
                case ActionType.WritableStateLower:
                case ActionType.WritableStateMore:
                    var res = CallWSAction(p);
                    if (res.HasValue)
                    {
                        Check(p, (x, y) => res.Value, father);
                        return res.Value;
                    }
                    break;

                // === Режимы ===
                case ActionType.EnableAutoDig:
                    p.programsData.autoDig = true;
                    break;

                case ActionType.DisableAutoDig:
                    p.programsData.autoDig = false;
                    break;

                case ActionType.EnableAgression:
                    p.programsData.aggressive = true;
                    break;

                case ActionType.DisableAgression:
                    p.programsData.aggressive = false;
                    break;

                case ActionType.EnableHandMode:
                    p.programsData.handMode = true;
                    break;

                case ActionType.DisableHandMode:
                    p.programsData.handMode = false;
                    break;

                // === Специальные команды ===
                case ActionType.BOOM:
                case ActionType.DISCHARGE:
                case ActionType.PROTON:
                case ActionType.VB:
                case ActionType.Geopack:
                case ActionType.ZZ:
                case ActionType.C190:
                case ActionType.Poly:
                case ActionType.Up:
                case ActionType.Craft:
                case ActionType.Nano:
                case ActionType.Rembot:
                    p.SpecialAction(type);
                    delay = 200;
                    break;

                case ActionType.InvDirUp:
                case ActionType.InvDirLeft:
                case ActionType.InvDirDown:
                case ActionType.InvDirRight:
                    p.InverseDirection(type);
                    break;

                // === Пропуск строки ===
                case ActionType.NextRow:
                // === Создание функции ===
                case ActionType.CreateFunction:
                // === Пустые действия ===
                case ActionType.None:
                default:
                    break;
            }

            return null;
        }
    }
}