namespace MinesServer.GameShit.GUI
{
    public class ActionArgs
    {
        public string? Input { get; set; }
        public bool[]? PaintGrid { get; set; }
        public long[]? CrystalSliders { get; set; }
        public Dictionary<string, string>? RichList { get; set; }

        public string Action { get; set; } = "";        // реально пришедший action
        public string ActionFormat { get; set; } = "";  // шаблон кнопки
        public string Label { get; set; } = "";         // подпись кнопки
    }
}
