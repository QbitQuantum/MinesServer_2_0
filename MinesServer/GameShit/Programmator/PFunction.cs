namespace MinesServer.GameShit.Programmator
{
    public class PFunction
    {
        private PAction[] _actions = [];
        private int _actionCount = 0;

        public int current = 0;
        public (int x, int y) startoffset;
        public string? calledfrom;
        public bool? state;
        public ActionType? laststateaction;

        // Свойство для логирования (используется для вывода в консоль)
        public List<PAction> actions => _actions.Take(_actionCount).ToList();

        public void AddAction(PAction action)
        {
            if (_actionCount >= _actions.Length)
            {
                int newSize = _actions.Length == 0 ? 16 : _actions.Length * 2;
                Array.Resize(ref _actions, newSize);
            }
            _actions[_actionCount++] = action;
        }

        public ref PAction GetCurrentAction()
        {
            if (current >= _actionCount)
                throw new IndexOutOfRangeException();
            return ref _actions[current];
        }

        public Span<PAction> GetActionsSpan() => _actions.AsSpan(0, _actionCount);

        public PAction Next
        {
            get
            {
                // Прямой доступ к массиву без List
                var action = _actions[current];
                current++;
                return action;
            }
        }

        public void Reset()
        {
            current = 0;
            startoffset = (0, 0);
            state = null;
            laststateaction = null;
        }

        public void MoveNext() => current++;

        public int ActionCount => _actionCount;

        public static PFunction operator +(PFunction a, PAction b)
        {
            a.AddAction(b);
            return a;
        }
    }
}