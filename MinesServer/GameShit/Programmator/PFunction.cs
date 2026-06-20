namespace MinesServer.GameShit.Programmator
{
    public class PFunction
    {
        private PAction[] _actions = [];
        private int _actionCount = 0;

        public int Position { get; set; } = 0;

        public (int X, int Y) StartOffset { get; set; }
        public ActionType? LastStateAction { get; set; } = null;
        public string? CalledFrom { get; set; }
        public bool? State { get; set; }

        public bool ValidPosition => Position < _actionCount;

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
            if (!ValidPosition)
                throw new IndexOutOfRangeException();
            return ref _actions[Position];
        }

        public void Reset()
        {
            Position = 0;
            StartOffset = (0, 0);
            State = null;
            LastStateAction = null;
        }

        public void MoveNext() => Position++;
    }
}