namespace MinesServer.GameShit.Programmator
{
    public class PFunction
    {
        private PAction[] _actions = [];
        private int _actionCount = 0;

        public int position = 0;
        public (int x, int y) startoffset;
        public string? calledfrom;
        public bool? state;
        public ActionType? laststateaction;

        public int ActionCount => _actionCount;

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

        public bool ValidPosition() => position < ActionCount;

        public ref PAction GetCurrentAction()
        {
            if (position >= _actionCount)
                throw new IndexOutOfRangeException();
            return ref _actions[position];
        }

        public void Reset()
        {
            position = 0;
            startoffset = (0, 0);
            state = null;
            laststateaction = null;
        }

        public void MoveNext() => position++;

        public static PFunction operator +(PFunction a, PAction b)
        {
            a.AddAction(b);
            return a;
        }
    }
}