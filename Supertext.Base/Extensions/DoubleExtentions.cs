using System;


namespace Supertext.Base.Extensions
{
    public static class DoubleExtentions
    {
        /// <summary>
        /// Rounds the specified number to the nearest half number, i.e., the nearest 0.5.
        /// <example>
        /// <para>2.1 => 2</para>
        /// <para>2.6 => 2.5</para>
        /// <remarks>
        /// This function rounds away from zero. Consequently positive numbers will be rounded up and negative numbers rounded down.
        /// </remarks>
        /// </example>
        /// </summary>
        /// <param name="original">The number to be rounded.</param>
        /// <returns>A <see cref="double"/> ending in .0 or .5.</returns>
        public static double RoundToNearestHalf(this double original)
        {
            var x1 = Math.Floor(original);
            return x1 + Math.Round((original - x1) * 2, MidpointRounding.AwayFromZero) / 2.0;
        }
    }
}