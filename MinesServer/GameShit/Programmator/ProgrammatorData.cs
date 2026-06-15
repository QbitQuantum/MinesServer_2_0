using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.Enums;
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
        public int checkX;
        public int checkY;
        public int shiftX;
        public int shiftY;
        public (string name, int pos) startpoint;
        public bool flipstate = false;

        public bool autoDig = false; 
        public bool aggressive = false;  
        public bool handMode = false;

        private void Drop()
        {
            startpoint = ("", 0);
            GotoDeath = null;
            cFunction = "";
            checkX = 0;
            checkY = 0;
            shiftX = 0;
            shiftY = 0;
            flipstate = false;
            // Сброс режимов
            autoDig = false;
            aggressive = false;
            handMode = false;

            foreach (var function in CurrentProg)
                function.Value.Reset();
        }

        public bool Running { get; set; }
        public Dictionary<string, PFunction> CurrentProg { get; set; }
        public List<string> Functions { get; set; } = [];

        public DateTime delay;
        private string cFunction;
        public Program? selected { get; set; }

        private PFunction current
        {
            get => CurrentProg[cFunction];
        }

        public void Run(Program p)
        {
            selected = p;
            CurrentProg = p.programm;
            Functions = CurrentProg.Keys.ToList();

            // Логирование функций
            foreach (var i in CurrentProg)
            {
                Console.WriteLine($"{i.Key} - {string.Join(' ', i.Value.actions.Select(i => $"{i.type} {(i.label is not null ? $"({i.label})" : "")}"))}");
            }

            delay = DateTime.UtcNow;
            Drop();
            Running = true;
        }

        public bool RespawnOnProg
        {
            get => entity is Player && (entity as Player).resp.cost == 0 && GotoDeath != null;
        }

        public void OnDeath()
        {
            current.Reset();
            cFunction = GotoDeath;
        }

        private string? GotoDeath;

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
            var i = Functions.IndexOf(cFunction);
            if (Functions.Count > i + 1)
                cFunction = Functions[i + 1];
            else
                cFunction = Functions[0];
        }

        public void IncreaseDelay(double ms) => delay = ServerTime.Now + TimeSpan.FromMilliseconds(ms);

        public void Step()
        {
            if (!current.ValidPosition)
            {
                current.Reset();
                Next();
            }

            while (current.ValidPosition && ServerTime.Now >= delay)
                ExecuteCurrentAction();
        }

        // Выносим логику выполнения одного действия в отдельный метод
        private void ExecuteCurrentAction()
        {
            ref PAction action = ref current.GetCurrentAction();
            current.MoveNext();

            (ExecResult Result, string Label, bool Bool, long Delay) result = action.Execute(entity, current);

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

            IncreaseDelay(action.delay);
        }

        public void HandleNoneResult(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.CheckDown or ActionType.CheckUp or ActionType.CheckRight or ActionType.CheckLeft
                        or ActionType.CheckDownLeft or ActionType.CheckDownRight or ActionType.CheckUpLeft or ActionType.CheckUpRight
                        or ActionType.ShiftUp or ActionType.ShiftLeft or ActionType.ShiftDown or ActionType.ShiftRight or ActionType.ShiftForward:
                    if (current.startoffset != default)
                    {
                        current.startoffset = (0, 0);
                    }
                    break;

                case ActionType.Return:
                    current.Reset();
                    if (current.calledfrom is not null)
                    {
                        cFunction = current.calledfrom;
                    }
                    break;

                case ActionType.ReturnState:
                    current.Reset();
                    if (current.calledfrom is not null)
                    {
                        if (shiftX != 0 || shiftY != 0 || checkX != 0 || checkY != 0)
                            CurrentProg[current.calledfrom].startoffset = (shiftX + checkX, shiftY + checkY);
                        CurrentProg[current.calledfrom].state = current.state;
                        CurrentProg[current.calledfrom].laststateaction = current.laststateaction;
                        cFunction = current.calledfrom;
                    }
                    break;

                case ActionType.Last:
                    break;
                case ActionType.Stop:
                    Run();
                    ((Player)entity)?.ProgStatus();
                    break;

                case ActionType.Start:
                    //startpoint = (cFunction, current.current);
                    break;
                case ActionType.Restart:
                    Run(); // Завершаем программу
                    Run(); // Запускаем программу
                    ((Player)entity)?.ProgStatus();
                    break;

                case ActionType.Flip:
                    flipstate = !flipstate;
                    break;
            }
        }

        public void HandleBoolResult(ActionType actionType, bool Bool)
        {
            switch (actionType)
            {
                case ActionType.ReturnFunction:
                    current.Reset();
                    current.startoffset = (0, 0);
                    if (current.calledfrom is not null)
                    {
                        cFunction = current.calledfrom;
                    }
                    current.state = Bool;
                    current.startoffset = (0, 0);
                    break;

                case ActionType.MacrosDig or ActionType.MacrosHeal or ActionType.MacrosMine:
                    if (Bool) current.position--;
                    break;
            }
        }

        public void HandleLabelResult(ActionType actionType, string label)
        {
            switch (actionType)
            {
                case ActionType.GoTo:
                    if (CurrentProg.TryGetValue(label, out var _))
                    {
                        current.Reset();
                        if (label == "")
                        {
                            label = startpoint.name;
                            CurrentProg[label].position = startpoint.pos;
                        }
                        cFunction = label;
                    }
                    else
                    {
                        cFunction = startpoint.name;
                        current.position = startpoint.pos;
                    }
                    break;

                case ActionType.RunSub:
                    if (CurrentProg.TryGetValue(label, out var _))
                    {
                        CurrentProg[label].calledfrom = cFunction;
                        cFunction = label;
                    }
                    break;

                case ActionType.RunFunction:
                    if (CurrentProg.TryGetValue(label, out var _))
                    {
                        if (shiftX != 0 || shiftY != 0 || checkX != 0 || checkY != 0)
                            CurrentProg[label].startoffset = (shiftX + checkX, shiftY + checkY);
                        CurrentProg[label].calledfrom = cFunction;
                        cFunction = label;
                    }
                    break;

                case ActionType.RunState:
                    if (CurrentProg.TryGetValue(label, out var _))
                    {
                        if (shiftX != 0 || shiftY != 0 || checkX != 0 || checkY != 0)
                            CurrentProg[label].startoffset = (shiftX + checkX, shiftY + checkY);
                        CurrentProg[label].state = current.state;
                        CurrentProg[label].laststateaction = current.laststateaction;
                        CurrentProg[label].calledfrom = cFunction;
                        cFunction = label;
                    }
                    break;

                case ActionType.RunIfTrue or ActionType.RunIfFalse:
                    if (CurrentProg.TryGetValue(label, out var _))
                    {
                        current.Reset();
                        if (label == "")
                        {
                            cFunction = startpoint.name;
                            current.position = startpoint.pos;
                            break;
                        }
                        CurrentProg[label].calledfrom = current.calledfrom;
                        cFunction = label;
                    }
                    break;

                case ActionType.RunOnRespawn:
                    if (CurrentProg.TryGetValue(label, out var _))
                    {
                        GotoDeath = label;
                    }
                    break;
            }
        }
    }
}