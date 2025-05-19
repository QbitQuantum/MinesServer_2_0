using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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
        }
        public PAction(ActionType t, string label)
        {
            this.label = label; type = t;
        }
        public PAction(ActionType t, string label,int number)
        {
            this.label = label; type = t;this.num = number;
        }
        public double delay = 0;
        public string label;
        public int num;
        public ActionType type;
        private void Check(PEntity p,Func<int,int,bool> func)
        {
            var x = p.x;
            var y = p.y;
            if (father.startoffset != default)
            {
                x += p.programsData.flipstate ? -father.startoffset.x : father.startoffset.x;
                y += p.programsData.flipstate ? -father.startoffset.y : father.startoffset.y;
            }
            else
            {
                x += p.programsData.flipstate ? -(p.programsData.shiftX + p.programsData.checkX) : p.programsData.shiftX + p.programsData.checkX;
                y += p.programsData.flipstate ? -(p.programsData.shiftY + p.programsData.checkY) : p.programsData.shiftY + p.programsData.checkY;
            }
            p.programsData.checkX = 0;p.programsData.checkY = 0;p.programsData.shiftX = 0;p.programsData.shiftY = 0;
                if (father.state == null)
                {
                    father.state = func(x, y);
                    return;
                }
                father.state = father.laststateaction switch
                {
                    null => func(x, y),
                    ActionType.Or => (bool)father.state || func(x, y),
                    ActionType.And => (bool)father.state && func(x, y)
                };
        }
        private bool IsAcid(CellType type) => type switch
        {
            CellType.AcidRock or CellType.CorrosiveActiveAcid or CellType.GrayAcid or CellType.GrayAcid or CellType.LivingActiveAcid or CellType.PassiveAcid or CellType.PurpleAcid => true,
            _ => false
        };
        private bool? CallWSAction(PEntity p)
        {
            switch(label.ToLower())
            {
                case "geo":
                    return type switch
                    {
                        ActionType.WritableState => p.geo.Count == num,
                        ActionType.WritableStateLower => p.geo.Count < num,
                        ActionType.WritableStateMore => p.geo.Count > num
                    };
                case "del":
                    delay = num;
                    return null;
            }
            return false;
        }
        private static Dictionary<int, (int dx, int dy)> dirz = new()
        {
             { 0, (0, 1) },
             { 1, (-1, 0) },
             { 2, (0, -1) },
             { 3, (1, 0) }
        };
        public object? Execute(PEntity p, ref object? template)
        {
            switch (type)
            {
                case ActionType.MacrosMine:
                    if (template is int)
                    {
                        var dir = p.GetDirCord();
                        if (World.isCry(World.GetCell(dir.x, dir.y)))
                        {
                            p.Bz();
                            delay = 200;
                            return true;
                        }
                    }
                    foreach(var i in dirz)
                    {
                        if (World.isCry(World.GetCell(p.x + i.Value.dx, p.y + i.Value.dy)))
                        if (p.dir == i.Key)
                        {
                            p.Bz();
                            delay = 200;
                            template = i.Key;
                            return true;
                        }
                        else
                        {
                            p.Move(p.x, p.y, i.Key);
                            delay = p.ServerPause;
                                return true;
                        }
                    }
                    template = null;
                    break;
                case ActionType.MacrosHeal:
                    if (p.crys is not null && p.crys[MinesServer.Enums.CrystalType.Red] > 0)
                    {
                        if (p.Health < p.MaxHealth && p.Heal())
                        {
                            delay = 200;
                            return true;
                        }
                    }
                    break;
                case ActionType.MacrosDig:
                    var c = p.GetDirCord();
                    if (World.GetProp(c.x,c.y).is_diggable)
                    {
                        delay = 200;
                        p.Bz();
                        return true;
                    }
                    break;
                case ActionType.MoveDown:
                    delay = p.ServerPause;
                    if (p.Move(p.x, p.y + 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveUp:
                    delay = p.ServerPause;
                    if (p.Move(p.x, p.y - 1))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveRight:
                    delay = p.ServerPause;
                    if (p.Move(p.x + 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveLeft:
                    delay = p.ServerPause;
                    if (p.Move(p.x - 1, p.y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.MoveForward:
                    delay = p.ServerPause;
                    if (p.Move((int)p.GetDirCord().x, (int)p.GetDirCord().y))
                    {
                        delay += 200;
                    }
                    break;
                case ActionType.RotateDown:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, 0);
                    break;
                case ActionType.RotateUp:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, 2);
                    break;
                case ActionType.RotateLeft:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, 1);
                    break;
                case ActionType.RotateRight:
                    delay = p.ServerPause;
                    p.Move(p.x, p.y, 3);
                    break;
                case ActionType.RotateLeftRelative:
                    delay = p.ServerPause;
                    var dirl = p.dir switch
                    {
                        0 => 3,
                        1 => 0,
                        2 => 1,
                        3 => 2
                    };
                    p.Move(p.x, p.y, dirl);
                    break;
                case ActionType.RotateRightRelative:
                    delay = p.ServerPause;
                    var dirr = p.dir switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 3,
                        3 => 0
                    };
                    p.Move(p.x, p.y, dirr);
                    break;
                case ActionType.RotateRandom:
                    delay = p.ServerPause;
                    var rand = new Random(Guid.NewGuid().GetHashCode());
                    p.Move(p.x, p.y, rand.Next(4));
                    break;
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
                    p.Heal();
                    break;
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
                        0 => -1,
                        2 => 1,
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
                    var t = this;
                    Check(p, (x, y) => t.IsAcid((CellType)World.GetCell(x, y)));
                    break;
                case ActionType.IsRedRock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.RedRock);
                    break;
                case ActionType.IsBlackRock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.NiggerRock);
                    break;
                case ActionType.IsBoulder:
                    Check(p, (x, y) => World.GetProp(x,y).isBoulder);
                    break;
                case ActionType.IsSand:
                    Check(p, (x, y) => World.GetProp(x, y).isSand);
                    break;
                case ActionType.IsUnbreakable:
                    Check(p, (x, y) => !World.GetProp(x,y).isEmpty && !World.GetProp(x, y).is_diggable);;
                    break;
                case ActionType.IsBox:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.Box);
                    break;
                case ActionType.IsBreakableRock:
                    Check(p, (x, y) => World.GetProp(x,y).is_diggable);
                    break;
                case ActionType.IsCrystal:
                    Check(p, (x, y) => World.isCry(World.GetCell(x,y)));
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
                    Check(p, (x, y) => World.GetProp(x,y).isSand || World.GetProp(x, y).isBoulder);
                    break;
                case ActionType.IsLivingCrystal:
                    Check(p, (x, y) => World.isAlive(World.GetCell(x,y))); 
                    break;
                case ActionType.IsPillar:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.Support);
                    break;
                case ActionType.IsQuadBlock:
                    Check(p, (x, y) => World.GetCell(x, y) == (byte)CellType.QuadBlock);
                    break;
                case ActionType.IsRoad:
                    Check(p, (x, y) => World.isRoad(World.GetCell(x,y)));
                    break;
                case ActionType.RunSub or ActionType.RunState or ActionType.RunFunction or ActionType.RunOnRespawn:
                    return label;
                case ActionType.ReturnFunction:
                    return father.state;
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
                case ActionType.Or or ActionType.And:
                    father.laststateaction = type;
                    break;
                case ActionType.GoTo:
                    return label;
                case ActionType.WritableState or ActionType.WritableStateLower or ActionType.WritableStateMore:
                    var res = CallWSAction(p);
                    if (res is not null)
                    {
                        Check(p, (x, y) => { return (bool)res; });
                        return res;
                    }
                    break;
                case 0 or _:
                    break;
            }
            return null;
        }
    }
}
