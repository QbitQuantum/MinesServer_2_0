using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.Server;
using MoreLinq;

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
        public Dictionary<string, PFunction> CurrentProg { get; set; }
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

        public void Run(Program p)
        {
            Selected = p;
            CurrentProg = p.programm;

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
                if (Selected != null)
                {
                    using var db = new DataBase();
                    var dbProg = db.progs.Find(Selected.id);
                    if (dbProg != null)
                    {
                        dbProg.data = Selected.data;
                        db.SaveChanges();
                    }
                }
                return;
            }
            Run(Selected);
        }

        private void Next()
        {
            var i = CurrentProg.Keys.ToList().IndexOf(CurrentFunction);
            if (CurrentProg.Count > i + 1)
                CurrentFunction = CurrentProg.ElementAt(i + 1).Key;
            else
                CurrentFunction = CurrentProg.First().Key;
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

            object? result = action.Execute(Entity, Function);

            switch (result)
            {
                case string label:
                    switch (action.Type)
                    {
                        case ActionType.GoTo:
                            if (CurrentProg.TryGetValue(label, out var _))
                            {
                                Function.Reset();
                                if (label == "")
                                {
                                    label = StartPoint.Name;
                                    CurrentProg[label].Position = StartPoint.Pos;
                                }
                                CurrentFunction = label;
                            }
                            else
                            {
                                CurrentFunction = StartPoint.Name;
                                Function.Position = StartPoint.Pos;
                            }
                            break;

                        case ActionType.RunSub:
                            if (CurrentProg.TryGetValue(label, out var _))
                            {
                                CurrentProg[label].CalledFrom = CurrentFunction;
                                CurrentFunction = label;
                            }
                            break;

                        case ActionType.RunFunction:
                            if (CurrentProg.TryGetValue(label, out var _))
                            {
                                if (ShiftX != 0 || ShiftY != 0 || CheckX != 0 || CheckY != 0)
                                    CurrentProg[label].StartOffset = (ShiftX + CheckX, ShiftY + CheckY);
                                CurrentProg[label].CalledFrom = CurrentFunction;
                                CurrentFunction = label;
                            }
                            break;

                        case ActionType.RunState:
                            if (CurrentProg.TryGetValue(label, out var _))
                            {
                                if (ShiftX != 0 || ShiftY != 0 || CheckX != 0 || CheckY != 0)
                                    CurrentProg[label].StartOffset = (ShiftX + CheckX, ShiftY + CheckY);
                                CurrentProg[label].State = Function.State;
                                CurrentProg[label].LastStateAction = Function.LastStateAction;
                                CurrentProg[label].CalledFrom = CurrentFunction;
                                CurrentFunction = label;
                            }
                            break;

                        case ActionType.RunIfTrue or ActionType.RunIfFalse:
                            if (CurrentProg.TryGetValue(label, out var _))
                            {
                                Function.Reset();
                                if (label == "")
                                {
                                    CurrentFunction = StartPoint.Name;
                                    Function.Position = StartPoint.Pos;
                                    break;
                                }
                                CurrentProg[label].CalledFrom = Function.CalledFrom;
                                CurrentFunction = label;
                            }
                            break;

                        case ActionType.RunOnRespawn:
                            if (CurrentProg.TryGetValue(label, out var _))
                            {
                                GotoDeath = label;
                            }
                            break;
                    }
                    break;

                case bool state:
                    switch (action.Type)
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
                    break;

                case null:
                    switch (action.Type)
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
                    break;
            }
            IncreaseDelay(action.DelayAction);
        }
    }
}