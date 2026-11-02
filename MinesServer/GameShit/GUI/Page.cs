using MinesServer.GameShit.Enums;
using MinesServer.GameShit.GUI.Canvas;
using MinesServer.GameShit.GUI.Cards;
using MinesServer.GameShit.GUI.Crystal;
using MinesServer.GameShit.GUI.Inputs;
using MinesServer.GameShit.GUI.Inventory;
using MinesServer.GameShit.GUI.List;
using MinesServer.GameShit.GUI.Rich;
using MinesServer.GameShit.GUI.Styles;
using System.Text.RegularExpressions;

namespace MinesServer.GameShit.GUI
{
    public readonly struct Page : IPage
    {
        public string? Title { get; init; }
        public string? Text { get; init; }
        public Action? OnAdmin { get; init; }
        public required MButton[] Buttons { get; init; }
        public CrystalConfig? CrystalConfig { get; init; }
        public ListEntry[]? List { get; init; }
        public RichListConfig? RichList { get; init; }
        public ClanListEntry[]? ClanList { get; init; }
        public InputConfig? Input { get; init; }
        public CanvasElement[]? Canvas { get; init; }
        public InventoryItem[]? Inventory { get; init; }
        public Style? Style { get; init; }
        public Card? Card { get; init; }
        public Action<int>? OnInventory { get; init; }

        public bool ProcessButton(string action)
        {
            foreach (var btn in Buttons)
                if (btn.ProcessButton(action))
                    return true;

            foreach (var i in List ?? Enumerable.Empty<ListEntry>())
                if (i.Button?.ProcessButton(action) == true)
                    return true;

            foreach (var i in RichList?.Entries ?? Enumerable.Empty<RichListEntry>())
                switch (i.Type)
                {
                    case RichListEntryType.Button: if (i.Buttons![0].ProcessButton(action)) return true; break;
                    case RichListEntryType.Fill: if (i.Buttons!.Any(btn => btn.ProcessButton(action))) return true; break;
                    case RichListEntryType.Card: if (i.Cards!.Any(e => e.Button.ProcessButton(action))) return true; break;
                    default: break;
                }

            foreach (var i in ClanList ?? Enumerable.Empty<ClanListEntry>())
                if (i.Button.ProcessButton(action))
                    return true;

            foreach (var i in Canvas ?? Enumerable.Empty<CanvasElement>())
                if (i.Content?.ProcessButton(action) == true)
                    return true;

            var invAction = Style?.InventoryButtonAction ?? "choose";
            if (string.IsNullOrWhiteSpace(invAction)) invAction = "choose";
            var match = Regex.Match(action, @$"^{invAction}:(.+)$");
            if (OnInventory is not null && match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out var code)) OnInventory?.Invoke(code);
                else OnInventory?.Invoke((int)Mines3Enums.SkillFromCode(match.Groups[1].Value));
                return true;
            }

            return false;
        }

    }
}
