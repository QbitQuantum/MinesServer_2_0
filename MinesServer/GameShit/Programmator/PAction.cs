using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public ActionType Type;
        public string Label;
        public int Num;
        public double DelayAction;

        public PAction(ActionType t) : this(t, "", 0) { }
        public PAction(ActionType t, string label) : this(t, label, 0) { }

        public PAction(ActionType t, string label, int num)
        {
            Type = t;
            this.Label = label ?? "";
            this.Num = num;
            DelayAction = 0;
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
            var flip = p.programsData.FlipState ? -1 : 1;
            int checkX, checkY;

            if (father.StartOffset != default)
            {
                checkX = p.x + (flip * father.StartOffset.X);
                checkY = p.y + (flip * father.StartOffset.Y);
            }
            else
            {
                checkX = p.x + flip * (p.programsData.ShiftX + p.programsData.CheckX);
                checkY = p.y + flip * (p.programsData.ShiftY + p.programsData.CheckY);
            }

            p.programsData.CheckX = 0;
            p.programsData.CheckY = 0;
            p.programsData.ShiftX = 0;
            p.programsData.ShiftY = 0;

            var result = func(checkX, checkY);

            if (father.State == null)
                father.State = result;
            else if (father.LastStateAction == ActionType.Or)
                father.State = (bool)father.State || result;
            else if (father.LastStateAction == ActionType.And)
                father.State = (bool)father.State && result;
            else
                father.State = result;
        }

        // Передаем father параметром
        private bool? CallWSAction(PEntity p)
        {
            switch (Label.ToLower())
            {
                case "geo":
                    return Type switch
                    {
                        ActionType.WritableState => p.geo.Count == Num,
                        ActionType.WritableStateLower => p.geo.Count < Num,
                        ActionType.WritableStateMore => p.geo.Count > Num,
                        _ => null
                    };

                case "crys":
                    return Type switch
                    {
                        ActionType.WritableState => p.crys?.AllCry == Num,
                        ActionType.WritableStateLower => p.crys?.AllCry < Num,
                        ActionType.WritableStateMore => p.crys?.AllCry > Num,
                        _ => null
                    };

                case "red":
                    return Type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Red] == Num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Red] < Num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Red] > Num,
                        _ => null
                    };

                case "green":
                    return Type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Green] == Num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Green] < Num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Green] > Num,
                        _ => null
                    };

                case "blue":
                    return Type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Blue] == Num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Blue] < Num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Blue] > Num,
                        _ => null
                    };

                case "white":
                    return Type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.White] == Num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.White] < Num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.White] > Num,
                        _ => null
                    };

                case "violet":
                    return Type switch
                    {
                        ActionType.WritableState => p.crys?[MinesServer.Enums.CrystalType.Violet] == Num,
                        ActionType.WritableStateLower => p.crys?[MinesServer.Enums.CrystalType.Violet] < Num,
                        ActionType.WritableStateMore => p.crys?[MinesServer.Enums.CrystalType.Violet] > Num,
                        _ => null
                    };

                case "del":
                    DelayAction = Num;
                    return null;

                default:
                    return false;
            }
        }

        public (ExecResult Result, string Label, bool Bool, double Delay) Execute(PEntity p, PFunction father)
        {
            switch (Type)
            {
                // === Движение ===
                case ActionType.MoveDown:
                    DelayAction = p.ServerPause;
                    if (p.Move(p.x, p.y + 1))
                        DelayAction += 200;
                    break;

                case ActionType.MoveUp:
                    DelayAction = p.ServerPause;
                    if (p.Move(p.x, p.y - 1))
                        DelayAction += 200;
                    break;

                case ActionType.MoveRight:
                    DelayAction = p.ServerPause;
                    if (p.Move(p.x + 1, p.y))
                        DelayAction += 200;
                    break;

                case ActionType.MoveLeft:
                    DelayAction = p.ServerPause;
                    if (p.Move(p.x - 1, p.y))
                        DelayAction += 200;
                    break;

                case ActionType.MoveForward:
                    DelayAction = p.ServerPause;
                    var forward = p.GetDirCord();
                    if (p.Move(forward.x, forward.y))
                        DelayAction += 200;
                    break;

                // === Вращение ===
                case ActionType.RotateDown:
                    DelayAction = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Down);
                    break;

                case ActionType.RotateUp:
                    DelayAction = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Up);
                    break;

                case ActionType.RotateLeft:
                    DelayAction = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Left);
                    break;

                case ActionType.RotateRight:
                    DelayAction = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Right);
                    break;

                case ActionType.RotateLeftRelative:
                    DelayAction = p.ServerPause;
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection((p.dir + 3) % 4));
                    break;

                case ActionType.RotateRightRelative:
                    DelayAction = p.ServerPause;
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection((p.dir + 1) % 4));
                    break;

                case ActionType.RotateRandom:
                    DelayAction = p.ServerPause;
                    var rand = new Random(Guid.NewGuid().GetHashCode());
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection(rand.Next(4)));
                    break;

                // === Действия ===
                case ActionType.Dig:
                    DelayAction = 100;
                    p.Bz();
                    break;

                case ActionType.BuildBlock:
                    DelayAction = 100;
                    p.Build("G");
                    break;

                case ActionType.BuildPillar:
                    DelayAction = 100;
                    p.Build("O");
                    break;

                case ActionType.BuildRoad:
                    DelayAction = 100;
                    p.Build("R");
                    break;

                case ActionType.BuildMilitaryBlock:
                    DelayAction = 100;
                    p.Build("V");
                    break;

                case ActionType.Geology:
                    DelayAction = 100;
                    p.Geo();
                    break;

                case ActionType.Heal:
                    if (p.Heal())
                        DelayAction = 200;
                    break;

                case ActionType.Beep:
                    p.Beep();
                    DelayAction = 100;
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
                                DelayAction = 200;
                            }
                            else
                            {
                                p.Move(p.x, p.y, DirectionTypeExt.ToDirection(dir));
                                DelayAction = p.ServerPause;
                            }
                            return (ExecResult.Bool, "", true, DelayAction);
                        }
                    }
                    break;  // кристаллов нет

                case ActionType.MacrosHeal:
                    if (p.crys?[MinesServer.Enums.CrystalType.Red] > 0 && p.Health < p.MaxHealth)
                    {
                        if (p.Heal())
                        {
                            DelayAction = 200;
                            return (ExecResult.Bool, "", true, DelayAction);
                        }
                    }
                    break;

                case ActionType.MacrosDig:
                    var digPos = p.GetDirCord();
                    if (World.GetProp(digPos.x, digPos.y).is_diggable)
                    {
                        DelayAction = 200;
                        p.Bz();
                        return (ExecResult.Bool, "", true, DelayAction);
                    }
                    break;

                case ActionType.MacrosBuild:
                    var (x, y) = p.GetDirCord();
                    if (World.GetProp(x, y).isEmpty)
                    {
                        DelayAction = 200;
                        p.Build("G");
                        return (ExecResult.Bool, "", true, DelayAction);
                    }
                    break;

                // === Сдвиги ===
                case ActionType.ShiftUp:
                    p.programsData.ShiftY--;
                    break;

                case ActionType.ShiftDown:
                    p.programsData.ShiftY++;
                    break;

                case ActionType.ShiftRight:
                    p.programsData.ShiftX++;
                    break;

                case ActionType.ShiftLeft:
                    p.programsData.ShiftX--;
                    break;

                case ActionType.ShiftForward:
                    p.programsData.ShiftX += p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    p.programsData.ShiftY += p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;

                // === Проверки направления ===
                case ActionType.CheckForward:
                    p.programsData.CheckX = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    p.programsData.CheckY = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckRightRelative:
                    p.programsData.CheckX = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    p.programsData.CheckY = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckLeftRelative:
                    p.programsData.CheckX = p.dir switch
                    {
                        0 => -1,
                        2 => 1,
                        _ => 0
                    };
                    p.programsData.CheckY = p.dir switch
                    {
                        1 => 1,
                        3 => -1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckUp:
                    p.programsData.CheckX = 0;
                    p.programsData.CheckY = -1;
                    break;

                case ActionType.CheckDown:
                    p.programsData.CheckX = 0;
                    p.programsData.CheckY = 1;
                    break;

                case ActionType.CheckRight:
                    p.programsData.CheckX = 1;
                    p.programsData.CheckY = 0;
                    break;

                case ActionType.CheckLeft:
                    p.programsData.CheckX = -1;
                    p.programsData.CheckY = 0;
                    break;

                case ActionType.CheckUpLeft:
                    p.programsData.CheckX = -1;
                    p.programsData.CheckY = -1;
                    break;

                case ActionType.CheckUpRight:
                    p.programsData.CheckX = 1;
                    p.programsData.CheckY = -1;
                    break;

                case ActionType.CheckDownLeft:
                    p.programsData.CheckX = -1;
                    p.programsData.CheckY = 1;
                    break;

                case ActionType.CheckDownRight:
                    p.programsData.CheckX = 1;
                    p.programsData.CheckY = 1;
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
                    return (ExecResult.Label, Label, false, 0);

                case ActionType.ReturnFunction:
                case ActionType.ReturnState:
                    return (ExecResult.Bool, "", father.State ?? false, 0);

                case ActionType.Return:
                    return (ExecResult.Label, "", false, 0);

                case ActionType.RunIfTrue:
                    if (father.State.HasValue && !father.State.Value)
                        return (ExecResult.None, "", false, 0);
                    father.State = null;
                    return (ExecResult.Label, Label, false, 0);

                case ActionType.RunIfFalse:
                    if (father.State.HasValue && father.State.Value)
                        return (ExecResult.None, "", false, 0);
                    father.State = null;
                    return (ExecResult.Label, Label, false, 0);

                case ActionType.Or:
                case ActionType.And:
                    father.LastStateAction = Type;
                    break;

                case ActionType.GoTo:
                    return (ExecResult.Label, Label, false, DelayAction);

                // === Работа с памятью ===
                case ActionType.WritableState:
                case ActionType.WritableStateLower:
                case ActionType.WritableStateMore:
                    var res = CallWSAction(p);
                    if (res.HasValue)
                    {
                        Check(p, (x, y) => res.Value, father);
                        return (ExecResult.Bool, Label, res.Value, DelayAction);
                    }
                    break;

                // === Режимы ===
                case ActionType.EnableAutoDig:
                    p.autoDig = true;
                    break;

                case ActionType.DisableAutoDig:
                    p.autoDig = false;
                    break;

                case ActionType.EnableAgression:
                    p.agression = true;
                    break;

                case ActionType.DisableAgression:
                    p.agression = false;
                    break;

                case ActionType.EnableHandMode:
                    p.handMode = true;
                    break;

                case ActionType.DisableHandMode:
                    p.handMode = false;
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
                    p.SpecialAction(Type);
                    DelayAction = 200;
                    break;

                case ActionType.InvDirUp:
                case ActionType.InvDirLeft:
                case ActionType.InvDirDown:
                case ActionType.InvDirRight:
                    p.InverseDirection(Type);
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

            return (ExecResult.None, "", false, DelayAction);
        }
    }
}