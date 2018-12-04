using System;


namespace Supertext.Base.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string within the content of this StringBuilder.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="ignoreCase">If set to <c>true</c> it will ignore case; Default: <c>false</c>.</param>
        /// <returns></returns>
        public static int IndexOf(this System.Text.StringBuilder sb,
                                  string value,
                                  int startIndex = 0,
                                  bool ignoreCase = false)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            int index;
            var length = value.Length;
            var maxSearchLength = sb.Length - length + 1;

            if (ignoreCase)
            {
                for (var i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(sb[i]) != Char.ToLower(value[0]))
                    {
                        continue;
                    }

                    index = 1;
                    while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                    {
                        ++index;
                    }

                    if (index == length)
                    {
                        return i;
                    }
                }

                return -1;
            }

            for (var i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] != value[0])
                {
                    continue;
                }

                index = 1;
                while ((index < length) && (sb[i + index] == value[index]))
                {
                    ++index;
                }

                if (index == length)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Returns a value indicating whether a specified substring occurs within this content of this StringBuilder.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value">The string to seek.</param>
        /// <param name="ignoreCase">If set to <c>true</c> it will ignore case; Default: <c>false</c>.</param>
        /// <returns></returns>
        public static bool Contains(this System.Text.StringBuilder sb,
                                    string value,
                                    bool ignoreCase = false)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            return sb.IndexOf(value,
                              0,
                              ignoreCase) > -1;
        }
    }
}