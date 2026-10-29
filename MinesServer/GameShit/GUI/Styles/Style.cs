namespace MinesServer.GameShit.GUI.Styles
{
    public readonly record struct Style(GridStyle Inventory, GridStyle Canvas, bool LargeInput, string FixScrollTag, bool DisableKeyboard, string InventoryButtonAction, float? ScrollHeight, float? Space) { }
}
