using System.Text;
using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.Programmator.SevenZip.LZMA;
using MinesServer.Server;

namespace MinesServer.GameShit.Programmator
{
    public class ProgrammatorData
    {
        public ProgrammatorData(PEntity e)
        {
            ProgRunning = false;
            Entity = e;
        }

        private readonly PEntity Entity;

        private string CurrentFunction { get; set; }
        private string? GotoDeath { get; set; }

        public int CheckX { get; set; }
        public int CheckY { get; set; }
        public int ShiftX { get; set; }
        public int ShiftY { get; set; }

        public bool FlipState { get; set; }
        public bool ProgRunning { get; set; }

        public DateTime DelayProgramm { get; set; }
        public Dictionary<string, PFunction> CurrentProg { get; set; } = [];
        public List<string> FunctionOrder { get; set; } = [];
        public Program? Selected { get; set; }

        public (string Name, int Pos) StartPoint { get; set; }

        public bool RespawnOnProg => Entity is Player && (Entity as Player).resp.cost == 0 && GotoDeath != null;

        private PFunction Function => CurrentProg[CurrentFunction];

        private void Drop()
        {
            StartPoint = ("", 0);
            GotoDeath = null;
            CurrentFunction = "";
            CheckX = 0;
            CheckY = 0;
            ShiftX = 0;
            ShiftY = 0;
            FlipState = false;
            foreach (var function in CurrentProg)
                function.Value.Reset();
        }

        private static (byte[] DecompressedData, int NumBit, string[] ArrayStrings) Decode(string DataProgramm)
        {
            byte[] DecompressedData = SevenZipHelper.Decompress(Convert.FromBase64String(DataProgramm));
            int NumBit = BitConverter.ToInt32(DecompressedData, 0);
            string[] ArrayStrings = Encoding.UTF8.GetString(DecompressedData, NumBit + 4, DecompressedData.Length - NumBit - 4).Split(':');
            return (DecompressedData, NumBit, ArrayStrings);
        }

        private static (Dictionary<string, PFunction>, List<string>) ParseProgramm(string DataProgramm)
        {
            Dictionary<string, PFunction> ParseCurrentProg = [];
            List<string> ParseFunctionOrder = [];

            ParseCurrentProg[""] = new PFunction();
            string currentFunc = "";
            var (DecompressedData, NumBit, ArrayStrings) = Decode(DataProgramm);
            int index = 0;

            for (int i = 0; i < NumBit; i++)
            {
                var atype = ActionTypeExtensions.GetActionType(Convert.ToInt16(DecompressedData[i + 4]));

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
                    else
                        name = ArrayStrings[i];
                }

                // Добавляем команду в текущую функцию
                switch (atype)
                {
                    case ActionType.NextRow:
                        // Сбрасываем счетчик строки
                        index = 0;
                        continue;

                    // Управление потоком
                    case ActionType.CreateFunction:
                        ParseCurrentProg.Add(name, new PFunction());
                        currentFunc = name;
                        index = 0;
                        break;

                    // Команды проверки состояния (без параметров)
                    case ActionType.IsNotEmpty:
                    case ActionType.IsEmpty:
                    case ActionType.IsFalling:
                    case ActionType.IsCrystal:
                    case ActionType.IsLivingCrystal:
                    case ActionType.IsBoulder:
                    case ActionType.IsSand:
                    case ActionType.IsBreakableRock:
                    case ActionType.IsUnbreakable:
                    case ActionType.IsRedRock:
                    case ActionType.IsBlackRock:
                    case ActionType.IsAcid:
                    case ActionType.IsQuadBlock:
                    case ActionType.IsRoad:
                    case ActionType.IsRedBlock:
                    case ActionType.IsYellowBlock:
                    case ActionType.IsBox:
                    case ActionType.IsPillar:
                    case ActionType.IsGreenBlock:
                    case ActionType.CheckGun:

                    // Команды перемещения и вращения
                    case ActionType.MoveUp:
                    case ActionType.MoveLeft:
                    case ActionType.MoveDown:
                    case ActionType.MoveRight:
                    case ActionType.MoveForward:
                    case ActionType.RotateUp:
                    case ActionType.RotateLeft:
                    case ActionType.RotateDown:
                    case ActionType.RotateRight:
                    case ActionType.RotateLeftRelative:
                    case ActionType.RotateRightRelative:
                    case ActionType.RotateRandom:

                    // Команды проверки направления
                    case ActionType.CheckUp:
                    case ActionType.CheckLeft:
                    case ActionType.CheckDown:
                    case ActionType.CheckRight:
                    case ActionType.CheckForward:
                    case ActionType.CheckUpLeft:
                    case ActionType.CheckUpRight:
                    case ActionType.CheckDownLeft:
                    case ActionType.CheckDownRight:
                    case ActionType.CheckForwardLeft:
                    case ActionType.CheckForwardRight:

                    // Команды сдвига
                    case ActionType.ShiftUp:
                    case ActionType.ShiftLeft:
                    case ActionType.ShiftDown:
                    case ActionType.ShiftRight:
                    case ActionType.ShiftForward:

                    // Логические операторы
                    case ActionType.Or:
                    case ActionType.And:

                    // Действия
                    case ActionType.Dig:
                    case ActionType.BuildBlock:
                    case ActionType.Geology:
                    case ActionType.BuildRoad:
                    case ActionType.Heal:
                    case ActionType.BuildPillar:
                    case ActionType.Beep:

                    // Макросы
                    case ActionType.MacrosDig:
                    case ActionType.MacrosBuild:
                    case ActionType.MacrosHeal:
                    case ActionType.MacrosMine:

                    // Специальные команды
                    case ActionType.Flip:
                    case ActionType.FillGun:

                    // Режимы
                    case ActionType.EnableAutoDig:
                    case ActionType.DisableAutoDig:
                    case ActionType.EnableAgression:
                    case ActionType.DisableAgression:
                    case ActionType.EnableHandMode:
                    case ActionType.DisableHandMode:

                    // Специальные действия
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
                    case ActionType.InvDirUp:
                    case ActionType.InvDirLeft:
                    case ActionType.InvDirDown:
                    case ActionType.InvDirRight:

                    // Проверка состояние робота
                    case ActionType.IsHpLower100:
                    case ActionType.IsHpLower50:

                    // Старт/Стоп
                    case ActionType.Start:
                    case ActionType.Stop:
                    case ActionType.Last:
                        ParseCurrentProg[currentFunc].AddAction(new PAction(atype));
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
                        ParseCurrentProg[currentFunc].AddAction(new PAction(atype, name));
                        break;

                    // Команды с меткой и числовым параметром
                    case ActionType.WritableState:
                    case ActionType.WritableStateLower:
                    case ActionType.WritableStateMore:
                        ParseCurrentProg[currentFunc].AddAction(new PAction(atype, name, number));
                        break;

                    // None или неизвестные команды
                    case ActionType.None:
                    default:
                        if (atype != ActionType.None)
                        {
                            ParseCurrentProg[currentFunc].AddAction(new PAction(atype));
                        }
                        break;
                }

                index++;

                // Проверяем, нужно ли обработать конец строки
                if (index >= 15) index = 0;
            }

            ParseFunctionOrder = ParseCurrentProg.Keys.ToList();
            return (ParseCurrentProg, ParseFunctionOrder);
        }

        public void Run(Program p)
        {
            Selected = p;
            var (ParseCurrentProg, ParseFunctionOrder) = ParseProgramm(p.data);
            CurrentProg = ParseCurrentProg;
            FunctionOrder = ParseFunctionOrder;

            // Логирование функций
            foreach (var i in CurrentProg)
            {
                Console.WriteLine($"{i.Key} - {string.Join(' ', i.Value.actions.Select(i => $"{i.Type} {(i.Label is not null ? $"({i.Label})" : "")}"))}");
            }

            DelayProgramm = DateTime.UtcNow;
            Drop();
            ProgRunning = true;
        }

        public void OnDeath()
        {
            Function.Reset();
            CurrentFunction = GotoDeath;
        }

        // TODO: Разделить методы на Run()/Stop()
        public void Run()
        {
            if (ProgRunning || Selected == null)
            {
                ProgRunning = false;
                return;
            }
            Run(Selected);
        }

        private void Next()
        {
            var i = FunctionOrder.IndexOf(CurrentFunction);
            if (FunctionOrder.Count > i + 1)
                CurrentFunction = FunctionOrder[i + 1];
            else
                CurrentFunction = FunctionOrder[0];
        }

        public void IncreaseDelay(double ms) => DelayProgramm = ServerTime.Now + TimeSpan.FromMilliseconds(ms);

        public void Step()
        {
            if (!Function.ValidPosition)
            {
                Function.Reset();
                Next();
            }

            while (Function.ValidPosition && ServerTime.Now >= DelayProgramm)
                ExecuteCurrentAction();
        }

        // Выносим логику выполнения одного действия в отдельный метод
        private void ExecuteCurrentAction()
        {
            ref PAction action = ref Function.GetCurrentAction();
            Function.MoveNext();

            (ExecResult Result, string Label, bool Bool, double Delay) = action.Execute(Entity, Function);

            switch (Result)
            {
                case ExecResult.None:
                    HandleNoneResult(action.Type);
                    break;
                case ExecResult.Bool:
                    HandleBoolResult(action.Type, Bool);
                    break;
                case ExecResult.Label:
                    HandleLabelResult(action.Type, Label);
                    break;
            }
            IncreaseDelay(action.DelayAction);
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
                                spFunc.Position = StartPoint.Pos;
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
                            spFunc.Position = StartPoint.Pos;
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
                        bool hasOffset = ShiftX != 0 || ShiftY != 0 || CheckX != 0 || CheckY != 0;
                        if (hasOffset)
                        {
                            var offset = (ShiftX + CheckX, ShiftY + CheckY);
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
                            ? (callerFunc.State, callerFunc.LastStateAction)
                            : (null, null);
                        bool hasOffset = ShiftX != 0 || ShiftY != 0 || CheckX != 0 || CheckY != 0;
                        if (hasOffset)
                        {
                            var offset = (ShiftX + CheckX, ShiftY + CheckY);
                            if (CurrentProg.TryGetValue(label, out var offsetFunc))
                                offsetFunc.StartOffset = offset;
                        }
                        if (CurrentProg.TryGetValue(label, out var stateFunc))
                        {
                            stateFunc.State = stateVal;
                            stateFunc.LastStateAction = lastState;
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
                                spFunc.Position = StartPoint.Pos;
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

        public void HandleBoolResult(ActionType actionType, bool state)
        {
            switch (actionType)
            {
                case ActionType.ReturnFunction:
                    Function.Reset();
                    Function.StartOffset = (0, 0);
                    if (Function.CalledFrom is not null)
                    {
                        CurrentFunction = Function.CalledFrom;
                    }
                    Function.State = state;
                    Function.StartOffset = (0, 0);
                    break;

                case ActionType.MacrosDig or ActionType.MacrosHeal or ActionType.MacrosMine:
                    if (state) Function.Position--;
                    break;
            }
        }

        public void HandleNoneResult(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.CheckDown or ActionType.CheckUp or ActionType.CheckRight or ActionType.CheckLeft
                        or ActionType.CheckDownLeft or ActionType.CheckDownRight or ActionType.CheckUpLeft or ActionType.CheckUpRight
                        or ActionType.ShiftUp or ActionType.ShiftLeft or ActionType.ShiftDown or ActionType.ShiftRight or ActionType.ShiftForward:
                    if (Function.StartOffset != default)
                    {
                        Function.StartOffset = (0, 0);
                    }
                    break;

                case ActionType.Return:
                    Function.Reset();
                    if (Function.CalledFrom is not null)
                    {
                        CurrentFunction = Function.CalledFrom;
                    }
                    break;

                case ActionType.ReturnState:
                    Function.Reset();
                    if (Function.CalledFrom is not null)
                    {
                        if (ShiftX != 0 || ShiftY != 0 || CheckX != 0 || CheckY != 0)
                            CurrentProg[Function.CalledFrom].StartOffset = (ShiftX + CheckX, ShiftY + CheckY);
                        CurrentProg[Function.CalledFrom].State = Function.State;
                        CurrentProg[Function.CalledFrom].LastStateAction = Function.LastStateAction;
                        CurrentFunction = Function.CalledFrom;
                    }
                    break;

                case ActionType.Last:
                    break;
                case ActionType.Stop:
                    Run();
                    ((Player)Entity)?.ProgStatus();
                    break;

                case ActionType.Start:
                    //startpoint = (cFunction, current.current);
                    break;
                case ActionType.Restart:
                    Run(); // Завершаем программу
                    Run(); // Запускаем программу
                    ((Player)Entity)?.ProgStatus();
                    break;

                case ActionType.Flip:
                    FlipState = !FlipState;
                    break;
            }
        }
    }
}