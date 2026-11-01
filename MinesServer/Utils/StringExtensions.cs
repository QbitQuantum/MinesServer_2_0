namespace MinesServer.Utils
{
    public static class StringExtensions
    {
        public static bool ToBool(this string s) => s != "0";
    }
}
