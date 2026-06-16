using System.Text;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator.SevenZip.LZMA;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Server;

namespace MinesServer.GameShit.Programmator
{
    public class ProgrammatorData
    {
        public ProgrammatorData(PEntity e)
        {
            Running = false;
            entity = e;
        }

        PEntity entity;

        private int СheckX { get; set; }
        private int СheckY { get; set; }
        private int ShiftX { get; set; }
        private int ShiftY { get; set; }
        private bool FlipState { get; set; }
        private string CurrentFunction { get; set; }
        private string? GotoDeath { get; set; }
        private (string Name, int Position) StartPoint { get; set; }
        private DateTime Delay { get; set; }

        public bool Running { get; set; }
        public Dictionary<string, PFunction> CurrentProg { get; set; }
        public List<string> Functions { get; set; } = [];
        public Program? selected { get; set; }

        private PFunction Function
        {
            get => CurrentProg[CurrentFunction];
        }

        public bool RespawnOnProg
        {
            get => entity is Player && (entity as Player).resp.cost == 0 && GotoDeath != null;
        }

        private static readonly Dictionary<int, (int dx, int dy)> dirz = new()
        {
            { 0, (0, 1) },   // DOWN
            { 1, (-1, 0) },  // LEFT
            { 2, (0, -1) },  // UP
            { 3, (1, 0) }    // RIGHT
        };

        private void Drop()
        {
            StartPoint = ("", 0);
            GotoDeath = null;
            CurrentFunction = "";
            СheckX = 0;
            СheckY = 0;
            ShiftX = 0;
            ShiftY = 0;
            FlipState = false;

            foreach (var function in CurrentProg)
                function.Value.Reset();
        }

        public void Run(Program p)
        {
            selected = p;
            CurrentProg = ParseProgramm(selected.data);
            Functions = CurrentProg.Keys.ToList();

            // Логирование функций
            foreach (var i in CurrentProg)
            {
                Console.WriteLine($"{i.Key} - {string.Join(' ', i.Value.actions.Select(i => $"{i.type} {(i.label is not null ? $"({i.label})" : "")}"))}");
            }

            Delay = DateTime.UtcNow;
            Drop();
            Running = true;
        }

        private static (byte[] DecompressedData, int NumBit, string[] ArrayStrings) Decode(string DataProgramm)
        {
            byte[] DecompressedData = SevenZipHelper.Decompress(Convert.FromBase64String(DataProgramm));
            int NumBit = BitConverter.ToInt32(DecompressedData, 0);
            string[] ArrayStrings = Encoding.UTF8.GetString(DecompressedData, NumBit + 4, DecompressedData.Length - NumBit - 4).Split(':');
            return (DecompressedData, NumBit, ArrayStrings);
        }

        private static Dictionary<string, PFunction> ParseProgramm(string DataProgramm)
        {
            Dictionary<string, PFunction> functions = [];
            functions[""] = new PFunction();
            string currentFunc = "";
            var (DecompressedData, NumBit, ArrayStrings) = Decode(DataProgramm);
            int index = 0;
            bool ContainsNextRow = false;

            for (int i = 0; i < NumBit; i++)
            {
                var atype = GetActionType(Convert.ToInt16(DecompressedData[i + 4]));

                var name = "0";
                var number = 0;
                if (ArrayStrings.Length > i)
                {
                    if (ArrayStrings[i].Contains('@'))
                    {
                        var Label = ArrayStrings[i].Split('@');
                        name = Label[0];
                        if (int.TryParse(Label[1], out var n))
                            number = n;
                    }
                    else name = ArrayStrings[i];
                }

                // Добавляем команду в текущую функцию
                switch (atype)
                {
                    case ActionType.NextRow:
                        ContainsNextRow = true;
                        break;

                    // Управление потоком
                    case ActionType.CreateFunction:
                        functions.Add(name, new PFunction());
                        currentFunc = name;
                        index = 0;
                        break;

                    case ActionType.GoTo:
                    case ActionType.RunSub:
                    case ActionType.RunFunction:
                    case ActionType.RunState:
                    case ActionType.RunIfFalse:
                    case ActionType.RunIfTrue:
                    case ActionType.RunOnRespawn:
                    case ActionType.Return:
                    case ActionType.ReturnFunction:
                    case ActionType.ReturnState:
                    case ActionType.Restart:

                    // Отладка
                    case ActionType.DebugBreak:
                    case ActionType.DebugSet:
                        functions[currentFunc].AddAction(new PAction(atype, name));
                        break;

                    // Команды с числовым параметром
                    case ActionType.IsHpLower100:
                    case ActionType.IsHpLower50:
                        functions[currentFunc].AddAction(new PAction(atype, number));
                        break;

                    // Команды с меткой и числовым параметром
                    case ActionType.WritableState:
                    case ActionType.WritableStateLower:
                    case ActionType.WritableStateMore:
                        functions[currentFunc].AddAction(new PAction(atype, name, number));
                        break;
                    // Остальные команды
                    default:
                        if (atype != ActionType.None)
                        {
                            functions[currentFunc].AddAction(new PAction(atype));
                        }
                        break;
                }

                // Проверяем, нужно ли обработать конец строки
                if (index >= 15)
                {
                    if (!ContainsNextRow)
                        functions[currentFunc].AddActionGotoType();
                    ContainsNextRow = true;
                    index = 0;
                }

                index++;
            }
            return functions;
        }

        private static ActionType GetActionType(int id)
        {
            return id switch
            {
                0 => ActionType.None,
                1 => ActionType.NextRow,
                2 => ActionType.Start,
                3 => ActionType.Stop,
                4 => ActionType.MoveUp,
                5 => ActionType.MoveLeft,
                6 => ActionType.MoveDown,
                7 => ActionType.MoveRight,
                8 => ActionType.Dig,
                9 => ActionType.RotateUp,
                10 => ActionType.RotateLeft,
                11 => ActionType.RotateDown,
                12 => ActionType.RotateRight,
                13 => ActionType.Last,                    // LAST
                14 => ActionType.MoveForward,
                15 => ActionType.RotateLeftRelative,
                16 => ActionType.RotateRightRelative,
                17 => ActionType.BuildBlock,
                18 => ActionType.Geology,
                19 => ActionType.BuildRoad,
                20 => ActionType.Heal,
                21 => ActionType.BuildPillar,
                22 => ActionType.RotateRandom,
                23 => ActionType.Beep,
                24 => ActionType.GoTo,
                25 => ActionType.RunSub,
                26 => ActionType.RunFunction,
                27 => ActionType.Return,
                28 => ActionType.ReturnFunction,
                29 => ActionType.CheckUpLeft,
                30 => ActionType.CheckDownRight,
                31 => ActionType.CheckUp,
                32 => ActionType.CheckUpRight,
                33 => ActionType.CheckLeft,
                35 => ActionType.CheckRight,
                36 => ActionType.CheckDownLeft,
                37 => ActionType.CheckDown,
                38 => ActionType.Or,
                39 => ActionType.And,
                40 => ActionType.CreateFunction,
                43 => ActionType.IsNotEmpty,
                44 => ActionType.IsEmpty,
                45 => ActionType.IsFalling,
                46 => ActionType.IsCrystal,
                47 => ActionType.IsLivingCrystal,
                48 => ActionType.IsBoulder,
                49 => ActionType.IsSand,
                50 => ActionType.IsBreakableRock,
                51 => ActionType.IsUnbreakable,
                52 => ActionType.IsRedRock,
                53 => ActionType.IsBlackRock,
                54 => ActionType.IsAcid,
                57 => ActionType.IsQuadBlock,
                58 => ActionType.IsRoad,
                59 => ActionType.IsRedBlock,
                60 => ActionType.IsYellowBlock,
                74 => ActionType.IsBox,
                76 => ActionType.IsPillar,
                77 => ActionType.IsGreenBlock,
                119 => ActionType.WritableStateMore,
                120 => ActionType.WritableStateLower,
                123 => ActionType.WritableState,
                131 => ActionType.ShiftUp,
                132 => ActionType.ShiftLeft,
                133 => ActionType.ShiftDown,
                134 => ActionType.ShiftRight,
                135 => ActionType.CheckForward,
                136 => ActionType.ShiftForward,
                137 => ActionType.RunState,
                138 => ActionType.ReturnState,
                139 => ActionType.RunIfFalse,
                140 => ActionType.RunIfTrue,
                141 => ActionType.MacrosDig,
                142 => ActionType.MacrosBuild,
                143 => ActionType.MacrosHeal,
                144 => ActionType.Flip,
                145 => ActionType.MacrosMine,
                146 => ActionType.CheckGun,
                147 => ActionType.FillGun,
                148 => ActionType.IsHpLower100,
                149 => ActionType.IsHpLower50,
                156 => ActionType.CheckForwardLeft,
                157 => ActionType.CheckForwardRight,
                158 => ActionType.EnableAutoDig,
                159 => ActionType.DisableAutoDig,
                160 => ActionType.EnableAgression,
                161 => ActionType.DisableAgression,
                162 => ActionType.BOOM,                    // ACTION_BOOM
                163 => ActionType.DISCHARGE,               // ACTION_DISCHARGE
                164 => ActionType.PROTON,                  // ACTION_PROTON
                165 => ActionType.VB,                      // ACTION_WB
                166 => ActionType.RunOnRespawn,
                167 => ActionType.Geopack,                  // ACTION_GEOPACK
                168 => ActionType.ZZ,                       // ACTION_ZZ
                169 => ActionType.C190,                     // ACTION_C190
                170 => ActionType.Poly,                      // ACTION_POLY
                171 => ActionType.Up,                        // ACTION_UP
                172 => ActionType.Craft,                     // ACTION_CRAFT
                173 => ActionType.Nano,                      // ACTION_NANO
                174 => ActionType.Rembot,                    // ACTION_REMBOT
                175 => ActionType.InvDirUp,                  // INVDIR_W
                176 => ActionType.InvDirLeft,                // INVDIR_A
                177 => ActionType.InvDirDown,                // INVDIR_S
                178 => ActionType.InvDirRight,               // INVDIR_D
                179 => ActionType.EnableHandMode,            // HANDMODE_ON
                180 => ActionType.DisableHandMode,           // HANDMODE_OFF
                181 => ActionType.DebugBreak,                // DEBUG_BREAK
                182 => ActionType.DebugSet,                  // DEBUG_SET
                200 => ActionType.Restart,                    // RESTART
                _ => ActionType.None
            };
        }

        public void OnDeath()
        {
            Function.Reset();
            CurrentFunction = GotoDeath;
        }

        // TODO: Разделить методы на Run()/Stop()
        public void Run()
        {
            if (Running || selected == null)
            {
                Running = false;
                if (selected != null)
                {
                    using var db = new DataBase();
                    var dbProg = db.progs.Find(selected.id);
                    if (dbProg != null)
                    {
                        dbProg.data = selected.data;
                        db.SaveChanges();
                    }
                }
                return;
            }
            Run(selected);
        }

        private void Next()
        {
            var i = Functions.IndexOf(CurrentFunction);
            if (Functions.Count > i + 1)
                CurrentFunction = Functions[i + 1];
            else
                CurrentFunction = Functions[0];
        }

        public void IncreaseDelay(double ms) => Delay = ServerTime.Now + TimeSpan.FromMilliseconds(ms);

        public void Step()
        {
            if (!Function.ValidPosition)
            {
                Function.Reset();
                Next();
            }

            while (Function.ValidPosition && ServerTime.Now >= Delay)
                ExecuteCurrentAction();
        }

        private void Check(PEntity p, Func<int, int, bool> function)
        {
            var (sx, sy) = CurrentProg.TryGetValue(CurrentFunction, out var f) &&  f.StartOffset != (0, 0) ? 
                f.StartOffset : (ShiftX + СheckX, ShiftY + СheckY);

            var flip = FlipState ? -1 : 1;

            int x = p.x + flip * sx;
            int y = p.y + flip * sy;

            СheckX = 0;
            СheckY = 0;
            ShiftX = 0;
            ShiftY = 0;

            var result = function(x, y);

            if (CurrentProg.TryGetValue(CurrentFunction, out var father))
            {
                if (father.State == null)
                    father.State = result;
                else if (father.laststateaction == ActionType.Or)
                    father.State = (bool)father.State || result;
                else if (father.laststateaction == ActionType.And)
                    father.State = (bool)father.State && result;
                else
                    father.State = result;
            }
            
        }

        public (ExecResult Result, string Label, bool Bool, long Delay) 
            Execute(PEntity p, PAction action)
        {
            ExecResult Result = ExecResult.None;
            string Label = "";
            bool Bool = false;
            long Delay = 0;

            switch (action.type)
            {
                // === Движение ===
                case ActionType.MoveDown:
                    Delay = p.ServerPause;
                    if (p.Move(p.x, p.y + 1))
                        Delay += 200;
                    break;

                case ActionType.MoveUp:
                    Delay = p.ServerPause;
                    if (p.Move(p.x, p.y - 1))
                        Delay += 200;
                    break;

                case ActionType.MoveRight:
                    Delay = p.ServerPause;
                    if (p.Move(p.x + 1, p.y))
                        Delay += 200;
                    break;

                case ActionType.MoveLeft:
                    Delay = p.ServerPause;
                    if (p.Move(p.x - 1, p.y))
                        Delay += 200;
                    break;

                case ActionType.MoveForward:
                    Delay = p.ServerPause;
                    var forward = p.GetDirCord();
                    if (p.Move(forward.x, forward.y))
                        Delay += 200;
                    break;

                // === Вращение ===
                case ActionType.RotateDown:
                    Delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Down);
                    break;

                case ActionType.RotateUp:
                    Delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Up);
                    break;

                case ActionType.RotateLeft:
                    Delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Left);
                    break;

                case ActionType.RotateRight:
                    Delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionType.Right);
                    break;

                case ActionType.RotateLeftRelative:
                    Delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection((p.dir + 3) % 4));
                    break;

                case ActionType.RotateRightRelative:
                    Delay = p.ServerPause;
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection((p.dir + 1) % 4));
                    break;

                case ActionType.RotateRandom:
                    Delay = p.ServerPause;
                    var rand = new Random(Guid.NewGuid().GetHashCode());
                    p.Move(p.x, p.y, DirectionTypeExt.ToDirection(rand.Next(4)));
                    break;

                // === Действия ===
                case ActionType.Dig:
                    Delay = 100;
                    p.Bz();
                    break;

                case ActionType.BuildBlock:
                    Delay = 100;
                    p.Build("G");
                    break;

                case ActionType.BuildPillar:
                    Delay = 100;
                    p.Build("O");
                    break;

                case ActionType.BuildRoad:
                    Delay = 100;
                    p.Build("R");
                    break;

                case ActionType.BuildMilitaryBlock:
                    Delay = 100;
                    p.Build("V");
                    break;

                case ActionType.Geology:
                    Delay = 100;
                    p.Geo();
                    break;

                case ActionType.Heal:
                    if (p.Heal())
                        Delay = 200;
                    break;

                case ActionType.Beep:
                    p.Beep();
                    Delay = 100;
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
                                Delay = 200;
                            }
                            else
                            {
                                p.Move(p.x, p.y, DirectionTypeExt.ToDirection(dir));
                                Delay = p.ServerPause;
                            }
                            return (ExecResult.Bool, "", true, Delay);
                        }
                    }
                    break;  // кристаллов нет

                case ActionType.MacrosHeal:
                    if (p.crys?[MinesServer.Enums.CrystalType.Red] > 0 && p.Health < p.MaxHealth)
                    {
                        if (p.Heal())
                        {
                            Delay = 200;
                            return (ExecResult.Bool, "", true, Delay);
                        }
                    }
                    break;

                case ActionType.MacrosDig:
                    var digPos = p.GetDirCord();
                    if (World.GetProp(digPos.x, digPos.y).is_diggable)
                    {
                        Delay = 200;
                        p.Bz();
                        return (ExecResult.Bool, "", true, Delay);
                    }
                    break;

                case ActionType.MacrosBuild:
                    var buildPos = p.GetDirCord();
                    if (World.GetProp(buildPos.x, buildPos.y).isEmpty)
                    {
                        Delay = 200;
                        p.Build("G");
                        Bool = true;
                        break;
                    }
                    break;

                // === Сдвиги ===
                case ActionType.ShiftUp:
                    ShiftY--;
                    break;

                case ActionType.ShiftDown:
                    ShiftY++;
                    break;

                case ActionType.ShiftRight:
                    ShiftX++;
                    break;

                case ActionType.ShiftLeft:
                    ShiftX--;
                    break;

                case ActionType.ShiftForward:
                    ShiftX += p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    ShiftY += p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;

                // === Проверки направления ===
                case ActionType.CheckForward:
                    СheckX = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    СheckY = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckRightRelative:
                    СheckX = p.dir switch
                    {
                        0 => 1,
                        2 => -1,
                        _ => 0
                    };
                    СheckY = p.dir switch
                    {
                        1 => -1,
                        3 => 1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckLeftRelative:
                    СheckX = p.dir switch
                    {
                        0 => -1,
                        2 => 1,
                        _ => 0
                    };
                    СheckY = p.dir switch
                    {
                        1 => 1,
                        3 => -1,
                        _ => 0
                    };
                    break;

                case ActionType.CheckUp:
                    СheckX = 0;
                    СheckY = -1;
                    break;

                case ActionType.CheckDown:
                    СheckX = 0;
                    СheckY = 1;
                    break;

                case ActionType.CheckRight:
                    СheckX = 1;
                    СheckY = 0;
                    break;

                case ActionType.CheckLeft:
                    СheckX = -1;
                    СheckY = 0;
                    break;

                case ActionType.CheckUpLeft:
                    СheckX = -1;
                    СheckY = -1;
                    break;

                case ActionType.CheckUpRight:
                    СheckX = 1;
                    СheckY = -1;
                    break;

                case ActionType.CheckDownLeft:
                    СheckX = -1;
                    СheckY = 1;
                    break;

                case ActionType.CheckDownRight:
                    СheckX = 1;
                    СheckY = 1;
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
                    Check(p, (x, y) => ((CellType)World.GetCell(x, y)).IsAcid());
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
                case ActionType.GoTo:
                case ActionType.RunSub:
                case ActionType.RunState:
                case ActionType.RunFunction:
                case ActionType.RunOnRespawn:
                    Label = action.label;
                    return (ExecResult.Label, Label, false, 0);

                case ActionType.ReturnFunction:
                    Bool = CurrentProg.TryGetValue(CurrentFunction, out var returnFunctionAction) 
                        ? (returnFunctionAction.State ?? false) : false;
                    return (ExecResult.Label, "", Bool, 0);

                case ActionType.RunIfTrue:
                    bool? RunIfTrueStateVal = CurrentProg.TryGetValue(CurrentFunction, out var runIfTrueAction) 
                        ? runIfTrueAction.State : null;
                    if (CurrentProg.TryGetValue(CurrentFunction, out var runIfTrueActionReset))
                        runIfTrueActionReset.State = null;
                    if (RunIfTrueStateVal == false)
                        return (ExecResult.None, "", false, 0);
                    Label = action.label;
                    return (ExecResult.Label, Label, false, 0);

                case ActionType.RunIfFalse:
                    bool? RunIfFalseStateVal = CurrentProg.TryGetValue(CurrentFunction, out var runIfalseAction)
                        ? runIfalseAction.State : null;
                    if (CurrentProg.TryGetValue(CurrentFunction, out var runIfalseActionReset))
                        runIfalseActionReset.State = null;
                    if (RunIfFalseStateVal == true)
                        return (ExecResult.None, "", false, 0);
                    Label = action.label;
                    return (ExecResult.Label, Label, false, 0);

                case ActionType.Or:
                case ActionType.And:
                    if (CurrentProg.TryGetValue(CurrentFunction, out var Action))
                        Action.laststateaction = action.type;
                    break;

                // === Работа с памятью ===
                case ActionType.WritableState:
                case ActionType.WritableStateLower:
                case ActionType.WritableStateMore:
                    // Сброс состояние. Пофиксить
                    if (action.label == "del")
                        Delay = action.num;
                    break;

                // === Режимы ===
                /*
                case ActionType.EnableAutoDig:
                    autoDig = true;
                    break;

                case ActionType.DisableAutoDig:
                    autoDig = false;
                    break;

                case ActionType.EnableAgression:
                    aggressive = true;
                    break;

                case ActionType.DisableAgression:
                    aggressive = false;
                    break;

                case ActionType.EnableHandMode:
                    handMode = true;
                    break;

                case ActionType.DisableHandMode:
                    handMode = false;
                    break; 
                */

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
                    p.SpecialAction(action.type);
                    Delay = 200;
                    break;

                case ActionType.InvDirUp:
                case ActionType.InvDirLeft:
                case ActionType.InvDirDown:
                case ActionType.InvDirRight:
                    p.InverseDirection(action.type);
                    break;

                default:
                    break;
            }
            return (ExecResult.None, "", false, Delay);
        }

        // Выносим логику выполнения одного действия в отдельный метод
        private void ExecuteCurrentAction()
        {
            ref PAction action = ref Function.GetCurrentAction();
            Function.MoveNext();

            (ExecResult Result, string Label, bool Bool, long Delay) result = Execute(entity, action);

            switch (result.Result)
            {
                case ExecResult.None:
                    HandleNoneResult(action.type);
                    break;
                case ExecResult.Bool:
                    HandleBoolResult(action.type, result.Bool);
                    break;
                case ExecResult.Label:
                    HandleLabelResult(action.type, result.Label);
                    break;
            }

            IncreaseDelay(result.Delay);
        }

        public void HandleNoneResult(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.CheckDown or ActionType.CheckUp or ActionType.CheckRight or ActionType.CheckLeft
                        or ActionType.CheckDownLeft or ActionType.CheckDownRight or ActionType.CheckUpLeft or ActionType.CheckUpRight
                        or ActionType.ShiftUp or ActionType.ShiftLeft or ActionType.ShiftDown or ActionType.ShiftRight or ActionType.ShiftForward:
                    if (CurrentProg.TryGetValue(CurrentFunction, out var shiftFunc) && shiftFunc.StartOffset != (0, 0))
                        shiftFunc.StartOffset = (0, 0);
                    break;

                case ActionType.Return:
                    if (CurrentProg.TryGetValue(CurrentFunction, out var retFunc))
                        retFunc.Reset();
                    string? calledFrom = CurrentProg.TryGetValue(CurrentFunction, out var cfFunc) ? cfFunc.CalledFrom : null;
                    if (calledFrom != null)
                        CurrentFunction = calledFrom;
                    break;

                case ActionType.ReturnState:
                    if (CurrentProg.TryGetValue(CurrentFunction, out var rsFunc))
                        rsFunc.Reset();
                    var (stateVal, lastState, calledFromState) = CurrentProg.TryGetValue(CurrentFunction, out var stateFunc)
                        ? (stateFunc.State, stateFunc.laststateaction, stateFunc.CalledFrom) : (null, null, null);
                    if (calledFromState != null)
                    {
                        bool hasOffset = ShiftX != 0 || ShiftY != 0 || СheckX != 0 || СheckY != 0;
                        if (hasOffset && CurrentProg.TryGetValue(calledFromState, out var offsetFunc))
                            offsetFunc.StartOffset = (ShiftX + СheckX, ShiftY + СheckY);
                        if (CurrentProg.TryGetValue(calledFromState, out var callerFunc))
                        {
                            callerFunc.State = stateVal;
                            callerFunc.laststateaction = lastState;
                        }
                        CurrentFunction = calledFromState;
                    }
                    break;
                case ActionType.Start:
                    int pos = CurrentProg.TryGetValue(CurrentFunction, out var startFunc) ? startFunc.Position : 0;
                    StartPoint = (CurrentFunction, pos);
                    break;
                case ActionType.Flip:
                    FlipState = !FlipState;
                    break;
            }
        }

        public void HandleBoolResult(ActionType actionType, bool state)
        {
            switch (actionType)
            {
                case ActionType.ReturnFunction:
                    if (CurrentProg.TryGetValue(CurrentFunction, out var returnFunction))
                    {
                        returnFunction.Reset();
                        returnFunction.StartOffset = (0, 0);
                    }
                    string? callFromFunc = CurrentProg.TryGetValue(CurrentFunction, out var called) ? called.CalledFrom : null;
                    if (callFromFunc != null)
                    {
                        CurrentFunction = callFromFunc;
                        if (CurrentProg.TryGetValue(callFromFunc, out var callerFunc))
                        {
                            callerFunc.State = state;
                            callerFunc.StartOffset = (0, 0);
                        }
                    }
                    break;

                case ActionType.MacrosDig or ActionType.MacrosHeal or ActionType.MacrosMine:
                    if (state && CurrentProg.TryGetValue(CurrentFunction, out var MacrosFunction) && MacrosFunction.ValidPosition)
                        MacrosFunction.Position--;
                    break;
            }
        }

        public void HandleLabelResult(ActionType actionType, string label)
        {
            switch (actionType)
            {
                case ActionType.GoTo:
                    if (CurrentProg.ContainsKey(label))
                    {
                        if (CurrentProg.TryGetValue(CurrentFunction, out var gotoFunc))
                            gotoFunc.Reset();
                        if (string.IsNullOrEmpty(label))
                        {
                            CurrentFunction = StartPoint.Name;
                            if (CurrentProg.TryGetValue(CurrentFunction, out var spFunc))
                                spFunc.Position = StartPoint.Position;
                        }
                        else
                        {
                            CurrentFunction = label;
                        }
                    }
                    else
                    {
                        CurrentFunction = StartPoint.Name;
                        if (CurrentProg.TryGetValue(CurrentFunction, out var spFunc))
                            spFunc.Position = StartPoint.Position;
                    }
                    break;

                case ActionType.RunSub:
                    if (CurrentProg.ContainsKey(label))
                    {
                        if (CurrentProg.TryGetValue(label, out var subFunc))
                            subFunc.CalledFrom = CurrentFunction;
                        CurrentFunction = label;
                    }
                    break;

                case ActionType.RunFunction:
                    if (CurrentProg.ContainsKey(label))
                    {
                        string caller = CurrentFunction;
                        bool hasOffset = ShiftX != 0 || ShiftY != 0 || СheckX != 0 || СheckY != 0;
                        if (hasOffset)
                        {
                            var offset = (ShiftX + СheckX, ShiftY + СheckY);
                            if (CurrentProg.TryGetValue(label, out var offsetFunc))
                                offsetFunc.StartOffset = offset;
                        }
                        if (CurrentProg.TryGetValue(label, out var runFunc))
                            runFunc.CalledFrom = caller;
                        CurrentFunction = label;
                    }
                    break;

                case ActionType.RunState:
                    if (CurrentProg.ContainsKey(label))
                    {
                        string caller = CurrentFunction;
                        var (stateVal, lastState) = CurrentProg.TryGetValue(caller, out var callerFunc)
                            ? (callerFunc.State, callerFunc.laststateaction)
                            : (null, null);
                        bool hasOffset = ShiftX != 0 || ShiftY != 0 || СheckX != 0 || СheckY != 0;
                        if (hasOffset)
                        {
                            var offset = (ShiftX + СheckX, ShiftY + СheckY);
                            if (CurrentProg.TryGetValue(label, out var offsetFunc))
                                offsetFunc.StartOffset = offset;
                        }
                        if (CurrentProg.TryGetValue(label, out var stateFunc))
                        {
                            stateFunc.State = stateVal;
                            stateFunc.laststateaction = lastState;
                            stateFunc.CalledFrom = caller;
                        }
                        CurrentFunction = label;
                    }
                    break;

                case ActionType.RunIfTrue:
                case ActionType.RunIfFalse:
                    if (CurrentProg.ContainsKey(label))
                    {
                        if (CurrentProg.TryGetValue(CurrentFunction, out var resetFunc))
                            resetFunc.Reset();
                        if (string.IsNullOrEmpty(label))
                        {
                            CurrentFunction = StartPoint.Name;
                            if (CurrentProg.TryGetValue(CurrentFunction, out var spFunc))
                                spFunc.Position = StartPoint.Position;
                        }
                        else
                        {
                            string? calledFrom = CurrentProg.TryGetValue(CurrentFunction, out var cfFunc)
                                ? cfFunc.CalledFrom : null;
                            if (CurrentProg.TryGetValue(label, out var condFunc))
                                condFunc.CalledFrom = calledFrom;
                            CurrentFunction = label;
                        }
                    }
                    break;

                case ActionType.RunOnRespawn:
                    if (CurrentProg.ContainsKey(label))
                        GotoDeath = label;
                    break;
            }
        }
    }
}