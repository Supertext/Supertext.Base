using System;
using System.Collections.Generic;
using System.Linq;
using Supertext.Base.Common;

namespace Supertext.Base.Collections
{
    public static class EnumerableExtension
    {

        /// <summary>
        /// if enumerable is null or Count == 0, true is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">can be null</param>
        /// <returns>true, if the given enumerable is null or empty</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// reverse method of IsEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns>true, if given enumerable is not null and not empty</returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> collection)
        {
            return !IsEmpty(collection);
        }

        /// <summary>
        /// performs the action on every item T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">can be null</param>
        /// <param name="action">must not be null</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            Validate.NotNull(action, nameof(action));

            if (enumerable == null)
            {
                return;
            }

            foreach (var value in enumerable)
            {
                action(value);
            }
        }

        /// <summary>
        /// Better readable version of Enumerable.Any() = false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">can be null</param>
        /// <returns>true, if enumerable is empty or null</returns>
        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            return !enumerable.Any();
        }

        /// <summary>
        /// Better readable version of Enumerable.Any() = false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">can be null</param>
        /// <param name="predicate">must not be null</param>
        /// <returns>true, if enumerable is empty, null or predicate doesn't match</returns>
        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            Validate.NotNull(predicate, nameof(predicate));

            if (enumerable == null)
            {
                return true;
            }

            return !enumerable.Any(predicate);
        }

        /// <summary>
        /// Distinct by specific property
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            Validate.NotNull(source, nameof(source));
            Validate.NotNull(keySelector, nameof(keySelector));

            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// Says if at least one item in collection2 exists in
        /// collection1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection1">can be null</param>
        /// <param name="collection2">can be null</param>
        /// <returns>true, if at least one item of collection2 exists in collection1</returns>
        public static bool ContainsAny<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            if (collection1.IsEmpty())
            {
                return false;
            }

            if (collection2.IsEmpty())
            {
                return false;
            }

            return collection1.Any(collection2.Contains);
        }
    }
}