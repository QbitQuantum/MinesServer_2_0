using System.Text.RegularExpressions;

namespace MinesServer.Utils
{
    public static partial class InputValidation
    {
        /// <summary>
        /// Разрешённые символы: русские и латинские буквы, цифры, пробел.
        /// </summary>
        public static readonly Regex AllowedCharsOnly = AllowedCharsOnlyRegex();

        [GeneratedRegex("^[а-яА-ЯёЁa-zA-Z 0-9]+$")]
        private static partial Regex AllowedCharsOnlyRegex();
    }
}