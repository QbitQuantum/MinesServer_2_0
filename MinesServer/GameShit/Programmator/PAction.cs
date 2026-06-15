using MinesServer.GameShit.Entities;
using MinesServer.GameShit.Enums;
using MinesServer.GameShit.WorldSystem;

namespace MinesServer.GameShit.Programmator
{
    public struct PAction
    {
        public ActionType type;
        public string label;
        public int num;
        public double delay;

        public PAction(ActionType t) : this(t, "", 0) { }
        public PAction(ActionType t, string label) : this(t, label, 0) { }
        public PAction(ActionType t, int num) : this(t, "", num) { }

        public PAction(ActionType t, string label, int num)
        {
            type = t;
            this.label = label ?? "";
            this.num = num;
            delay = 0;
        }
    }
}