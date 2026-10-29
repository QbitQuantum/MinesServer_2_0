namespace MinesServer.GameShit.GUI.Crystal
{
    public readonly record struct CrystalConfig(string LeftText, string RightText, CrysLine[] Lines, bool BuyMode = false) { }
}
