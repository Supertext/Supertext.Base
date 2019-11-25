using System;
using System.Linq;


namespace Supertext.Base.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces a substring by location.
        /// </summary>
        /// <param name="str">The source <see cref="String"/>.</param>
        /// <param name="index">The start location of the replacement (0-indexed).</param>
        /// <param name="length">The number of characters to be removed before inserting.</param>
        /// <param name="replace">The <see cref="String"/> replacing the existing characters.</param>
        /// <returns></returns>
        public static string ReplaceAt(this string str,
                                       int index,
                                       int length,
                                       string replace)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str), "Cannot be null.");
            }

            if (length < 0)
            {
                throw new ArgumentException("Cannot be less than 0.", nameof(length));
            }

            if (length > str.Length)
            {
                throw new ArgumentException("Cannot be greater than source string length.", nameof(length));
            }

            return str.Remove(index, Math.Min(length, str.Length - index))
                      .Insert(index, replace);
        }


        /// <summary>
        /// Returns the reverse of the input string.
        /// </summary>
        public static string Reverse(this string input)
        {
            if (input == null)
            {
                return null;
            }

            return new string(input.ToCharArray()
                                   .Reverse()
                                   .ToArray());
        }

        /// <summary>
        /// Returns the string with the initial letter converted to lower-case.
        /// </summary>
        public static string ToLowerInitial(this string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var a = input.ToCharArray();
            a[0] = Char.ToLower(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Returns the string with the initial letter converted to upper-case.
        /// </summary>
        public static string ToUpperInitial(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            var a = input.ToCharArray();
            a[0] = Char.ToUpper(a[0]);
            return new string(a);
        }
    }
}