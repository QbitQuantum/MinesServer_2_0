namespace MinesServer.Enums
{
    public enum CrystalType
    {
        Unknown = -1,
        Green,
        Blue,
        Red,
        Violet,
        White,
        Cyan
    }
    public static class CrystalTypeExt
    {
        // Массив для преобразования индексов в типы кристаллов
        public static readonly CrystalType[] CrysType =
        [
            CrystalType.Green,
            CrystalType.Blue,
            CrystalType.Red,
            CrystalType.Violet,
            CrystalType.White,
            CrystalType.Cyan
        ];
    }

}
