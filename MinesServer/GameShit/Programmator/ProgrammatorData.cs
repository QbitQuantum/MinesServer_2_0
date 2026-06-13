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

            foreach (var function in currentprog)
                function.Value.Reset();
        }

        public bool ProgRunning { get; set; }
        public Dictionary<string, PFunction> currentprog { get; set; }
        public DateTime delay;
        private string cFunction;
        public Program? selected { get; set; }

        private PFunction current
        {
            get => currentprog[cFunction];
        }

        public void Run(Program p)
        {
            selected = p;
            currentprog = p.programm;

            // Логирование функций
            foreach (var i in currentprog)
            {
                Console.WriteLine($"{i.Key} - {string.Join(' ', i.Value.actions.Select(i => $"{i.type} {(i.label is not null ? $"({i.label})" : "")}"))}");
            }

            delay = DateTime.UtcNow;
            Drop();
            ProgRunning = true;
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
            if (ProgRunning || selected == null)
            {
                ProgRunning = false;
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
            var i = currentprog.Keys.ToList().IndexOf(cFunction);
            if (currentprog.Count > i + 1)
                cFunction = currentprog.ElementAt(i + 1).Key;
            else
                cFunction = currentprog.First().Key;
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

            object? result = action.Execute(entity, current);

            switch (result)
            {
                case string label:
                    switch (action.type)
                    {
                        case ActionType.GoTo:
                            if (currentprog.TryGetValue(label, out var _))
                            {
                                current.Reset();
                                if (label == "")
                                {
                                    label = startpoint.name;
                                    currentprog[label].position = startpoint.pos;
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
                            if (currentprog.TryGetValue(label, out var _))
                            {
                                currentprog[label].calledfrom = cFunction;
                                cFunction = label;
                            }
                            break;

                        case ActionType.RunFunction:
                            if (currentprog.TryGetValue(label, out var _))
                            {
                                if (shiftX != 0 || shiftY != 0 || checkX != 0 || checkY != 0)
                                    currentprog[label].startoffset = (shiftX + checkX, shiftY + checkY);
                                currentprog[label].calledfrom = cFunction;
                                cFunction = label;
                            }
                            break;

                        case ActionType.RunState:
                            if (currentprog.TryGetValue(label, out var _))
                            {
                                if (shiftX != 0 || shiftY != 0 || checkX != 0 || checkY != 0)
                                    currentprog[label].startoffset = (shiftX + checkX, shiftY + checkY);
                                currentprog[label].state = current.state;
                                currentprog[label].laststateaction = current.laststateaction;
                                currentprog[label].calledfrom = cFunction;
                                cFunction = label;
                            }
                            break;

                        case ActionType.RunIfTrue or ActionType.RunIfFalse:
                            if (currentprog.TryGetValue(label, out var _))
                            {
                                current.Reset();
                                if (label == "")
                                {
                                    cFunction = startpoint.name;
                                    current.position = startpoint.pos;
                                    break;
                                }
                                currentprog[label].calledfrom = current.calledfrom;
                                cFunction = label;
                            }
                            break;

                        case ActionType.RunOnRespawn:
                            if (currentprog.TryGetValue(label, out var _))
                            {
                                GotoDeath = label;
                            }
                            break;
                    }
                    break;

                case bool state:
                    switch (action.type)
                    {
                        case ActionType.ReturnFunction:
                            current.Reset();
                            current.startoffset = (0, 0);
                            if (current.calledfrom is not null)
                            {
                                cFunction = current.calledfrom;
                            }
                            current.state = state;
                            current.startoffset = (0, 0);
                            break;

                        case ActionType.MacrosDig or ActionType.MacrosHeal or ActionType.MacrosMine:
                            if (state) current.position--;
                            break;
                    }
                    break;

                case null:
                    switch (action.type)
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
                                    currentprog[current.calledfrom].startoffset = (shiftX + checkX, shiftY + checkY);
                                currentprog[current.calledfrom].state = current.state;
                                currentprog[current.calledfrom].laststateaction = current.laststateaction;
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
                    break;
            }
            IncreaseDelay(action.delay);
        }
    }
}