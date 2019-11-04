using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Supertext.Base.Common
{
    public static class Random
    {
        private static readonly Lazy<CultureInfo[]> AllCultures = new Lazy<CultureInfo[]>(() => CultureInfo.GetCultures(CultureTypes.AllCultures));
        private static readonly System.Random Rdm = new System.Random(54321);

        /// <summary>
        /// Generates a collection of random bytes.
        /// </summary>
        /// <example>
        /// var randomBytes = Random.Bytes().Take(1000);   // generate a 1000-element array of random bytes
        /// </example>
        public static IEnumerable<byte> GetBytes()
        {
            var buffer = new byte[32];
            while (true)
            {
                Rdm.NextBytes(buffer);
                foreach (var ret in buffer)
                {
                    yield return ret;
                }
            }
        }

        /// <summary>
        /// Returns a random <see cref="CultureInfo"/> object.
        /// </summary>
        public static CultureInfo GetCultureInfo()
        {
            return AllCultures.Value[Rdm.Next(0, AllCultures.Value.Length)];
        }

        /// <summary>
        /// Returns a random string in the format [5 chars]@[3 chars].[2 chars].
        /// </summary>
        public static string GetEmail()
        {
            return $"{GetString(5, false)}@{GetString(3, false)}.{GetString(2, false)}";
        }

        /// <summary>
        /// A 32-bit signed integer that is greater than or equal to 0 and less than <see cref="int.MaxValue"/>.
        /// </summary>
        public static int GetInt()
        {
            return Rdm.Next();
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <see cref="maxValue"/> must be greater than or equal to 0.</param>
        public static int GetInt(int maxValue)
        {
            return Rdm.Next(maxValue);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. <see cref="maxValue"/> must be greater than or equal to <see cref="minValue"/>.</param>
        public static int GetInt(int minValue, int maxValue)
        {
            return Rdm.Next(minValue, maxValue);
        }

        /// <summary>
        /// A 64-bit signed <c>long</c> that is greater than or equal to 0 and less than <see cref="int.MaxValue"/>.
        /// </summary>
        public static long GetLong()
        {
            return Rdm.Next();
        }

        /// <summary>
        /// Returns a non-negative, 64-bit signed <c>long</c> that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <see cref="maxValue"/> must be greater than or equal to 0.</param>
        public static long GetLong(int maxValue)
        {
            return Rdm.Next(maxValue);
        }

        /// <summary>
        /// Returns a non-negative, 64-bit signed <c>long</c> that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. <see cref="maxValue"/> must be greater than or equal to <see cref="minValue"/>.</param>
        public static long GetLong(int minValue, int maxValue)
        {
            return Rdm.Next(minValue, maxValue);
        }

        /// <summary>
        /// A random alphanumeric string with the specified length
        /// </summary>
        /// <param name="length">The number of characters required.</param>
        /// <param name="allowNumeric">Whether the string may contain numeric characters.</param>
        public static string GetString(int length, bool allowNumeric = true)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            const string nonNumChars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            return new string(Enumerable.Repeat(allowNumeric ? chars : nonNumChars, length)
                                        .Select(s => s[Rdm.Next(s.Length)])
                                        .ToArray());
        }

        /// <summary>
        /// Returns a unique URL.
        /// </summary>
        /// <param name="additionalInfo">Optional string which will be appended to the URL - useful for debugging or tracking.</param>
        public static Uri GetUri(string additionalInfo = null)
        {
            return new Uri($"https://www.{GetString(8)}.ch/{additionalInfo}");
        }
    }
}