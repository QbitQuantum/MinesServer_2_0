namespace MinesServer.GameShit.Enums
{
    public enum DirectionType
    {
        Unknown = -1,
        Down,
        Left,
        Up,
        Right,
    }

    public static class DirectionTypeExt
    {
        public static DirectionType ToDirection(int dir)
        {
            return dir switch
            {
                0 => DirectionType.Down,
                1 => DirectionType.Left,
                2 => DirectionType.Up,
                3 => DirectionType.Right,
                _ => DirectionType.Unknown
            };
        }
    }
}
