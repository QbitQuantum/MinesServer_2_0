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

        public bool ValidPosition => position < _actionCount;

        public ActionType? laststateaction = null;

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

        public void AddActionGotoType()
        {
            bool shouldadd = actions.Count > 0 && actions.Last().type != ActionType.GoTo;
            if (shouldadd) AddAction(new PAction(ActionType.GoTo));
        }

        public ref PAction GetCurrentAction()
        {
            if (!ValidPosition)
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
    }
}