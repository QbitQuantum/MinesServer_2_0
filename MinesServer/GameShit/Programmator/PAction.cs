using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;
using System.Drawing;
using System.Numerics;
using System.Security.AccessControl;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public PFunction father { get; set; }

        public PAction(ActionType t)
        {
            type = t;
            label = "";
            num = 0;
            delay = 0;
        }

        public PAction(ActionType t, string label)
        {
            type = t;
            this.label = label ?? "";
            num = 0;
            delay = 0;
        }

        public PAction(ActionType t, int number)
        {
            type = t;
            label = "";
            num = number;
            delay = 0;
        }

        public PAction(ActionType t, string label, int number)
        {
            type = t;
            this.label = label ?? "";
            num = number;
            delay = 0;
        }

        public double delay;
        public string label;
        public int num;
        public ActionType type;

        private static readonly Dictionary<int, (int dx, int dy)> dirz = new()
        {
            { 0, (0, 1) },   // DOWN
            { 1, (-1, 0) },  // LEFT
            { 2, (0, -1) },  // UP
            { 3, (1, 0) }    // RIGHT
        };

        private void Check(PEntity p, Func<int, int, bool> func)
        {
            var (checkX, checkY) = GetCheckCoordinates(p);

            p.programsData.checkX = 0;
            p.programsData.checkY = 0;
            p.programsData.shiftX = 0;
            p.programsData.shiftY = 0;

            if (father.state == null)
            {
                father.state = func(checkX, checkY);
                return;
            }

            father.state = father.laststateaction switch
            {
                null => func(checkX, checkY),
                ActionType.Or => (bool)father.state || func(checkX, checkY),
                ActionType.And => (bool)father.state && func(checkX, checkY),
                _ => func(checkX, checkY)
            };
        }

        private (int x, int y) GetCheckCoordinates(PEntity p)
        {
            if (father.startoffset != default)
            {
                return (
                    p.x + (p.programsData.flipstate ? -father.startoffset.x : father.startoffset.x),
                    p.y + (p.programsData.flipstate ? -father.startoffset.y : father.startoffset.y)
                );
            }

            return (
                p.x + (p.programsData.flipstate ?
                    -(p.programsData.shiftX + p.programsData.checkX) :
                    p.programsData.shiftX + p.programsData.checkX),
                p.y + (p.programsData.flipstate ?
                    -(p.programsData.shiftY + p.programsData.checkY) :
                    p.programsData.shiftY + p.programsData.checkY)
            );
        }

        private static bool IsAcid(CellType type) => type switch
        {
            CellType.AcidRock or CellType.CorrosiveActiveAcid or CellType.GrayAcid or
            CellType.LivingActiveAcid or CellType.PassiveAcid or CellType.PurpleAcid => true,
            _ => false
        };

        private static bool IsMineral(CellType type) => type switch
        {
            CellType.Red or CellType.Green or CellType.Blue or
            CellType.White or CellType.Violet => true,
            _ => false
        };

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

        public object? Execute(PEntity p, ref object? template)
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
                    if (template is int currentDir)
                    {
                        var dir = dirz[currentDir];
                        if (World.isCry(p.x + dir.dx, p.y + dir.dy))
                        {
                            p.Bz();
                            delay = 200;
                            return true;
                        }
                    }

                    foreach (var kv in dirz)
                    {
                        if (World.isCry(p.x + kv.Value.dx, p.y + kv.Value.dy))
                        {
                            if (p.dir == kv.Key)
                            {
                                p.Bz();
                                delay = 200;
                                template = kv.Key;
                                return true;
                            }

                            p.Move(p.x, p.y, DirectionTypeExt.ToDirection(kv.Key));
                            delay = p.ServerPause;
                            return true;
                        }
                    }

                    template = null;
                    break;

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

                // === Проверки состояния ===
                case ActionType.IsHpLower100:
                    Check(p, (x, y) => p.Health < p.MaxHealth);
                    break;

                case ActionType.IsHpLower50:
                    Check(p, (x, y) => p.Health < p.MaxHealth / 2);
                    break;

                case ActionType.IsEmpty:
                    Check(p, (x, y) => World.GetProp(x, y).isEmpty);
                    break;

                case ActionType.IsNotEmpty:
                    Check(p, (x, y) => !World.GetProp(x, y).isEmpty);
                    break;

                case ActionType.IsAcid:
                    Check(p, (x, y) => IsAcid((CellType)World.GetCell(x, y)));
                    break;
                case ActionType.IsRedRock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.RedRock);
                    break;

                case ActionType.IsBlackRock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.BlackRock);
                    break;

                case ActionType.IsBoulder:
                    Check(p, (x, y) => World.GetProp(x, y).isBoulder);
                    break;

                case ActionType.IsSand:
                    Check(p, (x, y) => World.GetProp(x, y).isSand);
                    break;

                case ActionType.IsUnbreakable:
                    Check(p, (x, y) => !World.GetProp(x, y).isEmpty && !World.GetProp(x, y).is_diggable);
                    break;

                case ActionType.IsBox:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.Box);
                    break;

                case ActionType.IsBreakableRock:
                    Check(p, (x, y) => World.GetProp(x, y).is_diggable);
                    break;

                case ActionType.IsCrystal:
                    Check(p, (x, y) => World.isCry(x, y));
                    break;

                case ActionType.IsGreenBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.GreenBlock);
                    break;

                case ActionType.IsYellowBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.YellowBlock);
                    break;

                case ActionType.IsRedBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.RedBlock);
                    break;

                case ActionType.IsFalling:
                    Check(p, (x, y) => World.GetProp(x, y).isSand || World.GetProp(x, y).isBoulder);
                    break;

                case ActionType.IsLivingCrystal:
                    Check(p, (x, y) => World.isAlive(x, y));
                    break;

                case ActionType.IsPillar:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.Support);
                    break;

                case ActionType.IsQuadBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.QuadBlock);
                    break;

                case ActionType.IsRoad:
                    Check(p, (x, y) => World.isRoad(x, y));
                    break;

                case ActionType.CheckGun:
                    Check(p, (x, y) => p.HasGun());
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
                    return ""; // Пустая строка для возврата из подпрограммы

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
                        Check(p, (x, y) => res.Value);
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
                case ActionType.ZM:
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

                /*
                // === Отладка ===
                case ActionType.DebugBreak:
                    p.DebugBreak(label);
                    break;

                case ActionType.DebugSet:
                    p.DebugSet(label);
                    break;
                */

                // === Старт/Стоп ===
                case ActionType.Start:
                    p.programsData.ProgRunning = true;
                    break;

                case ActionType.Stop:
                case ActionType.Last:
                    p.programsData.ProgRunning = false;
                    break;

                case ActionType.Restart:
                    p.RestartProgram();
                    break;

                // === Пропуск строки ===
                case ActionType.NextRow:
                    break;

                // === Создание функции ===
                case ActionType.CreateFunction:
                    break;

                // === Пустые действия ===
                case ActionType.None:
                default:
                    break;
            }

            return null;
        }
    }
}