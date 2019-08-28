namespace Supertext.Base.Common
{
    public static class StringTruncator
    {
        public static string Truncate(string text, int length)
        {
            Validate.NotNull(text, nameof(text));

            if (text.Length >= length)
            {
                return text.Substring(0, length);
            }

            return text;
        }

        /// <summary>
        /// Truncates string to a given length - 2 chars. The result will be padded (by default with two dots).
        /// Eg. "Company X.."
        /// </summary>
        public static string TruncateWithPaddingRight(string text, int length, char paddingChar = '.')
        {
            Validate.NotNull(text, nameof(text));
            var lengthExceptPadding = length - 2;

            if (text.Length >= lengthExceptPadding)
            {
                return $"{text.Substring(0, lengthExceptPadding)}{paddingChar}{paddingChar}";
            }

            return text;
        }
    }
}