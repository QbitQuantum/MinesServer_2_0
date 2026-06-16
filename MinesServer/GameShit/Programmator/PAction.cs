using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public ActionType ActionType { get; set; }
        public string Label { get; set; }
        public int Num { get; set; }

        public PAction(ActionType t) : this(t, "", 0) { }
        public PAction(ActionType t, string label) : this(t, label, 0) { }
        public PAction(ActionType t, int num) : this(t, "", num) { }

        public PAction(ActionType t, string label, int num)
        {
            ActionType = t;
            Label = label ?? "";
            Num = num;
        }
    }
}