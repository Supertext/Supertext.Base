using Supertext.Base.Common;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Supertext.Base.Extensions
{
    public static class EnumerableExtensions
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


        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="source" />. The value can be <see langword="null" /> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the entire <see cref="source" />, if found; otherwise, –1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            Validate.NotNull(source);

            var index = 0;
            var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
            foreach (var sourceItem in source)
            {
                if (comparer.Equals(sourceItem, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }


        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        /// <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        [JetBrains.Annotations.ContractAnnotation("enumerable:null => true")]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case null:
                    return true;

                /* If this is a list, use the Count property for efficiency.
                 * The Count property is O(1) while IEnumerable.Count() is O(N).
                */
                case ICollection<T> collection:
                    return collection.Count < 1;

                default:
                    return !enumerable.Any();
            }
        }


        /// <summary>
        /// Returns the item in the sequence with the greatest of the specified property.
        /// </summary>
        /// <example>
        /// <c>myObjects(o => o.Id)</c> will return the object with the highest-order <c>Id</c> property.
        /// </example>
        public static T ItemWithMax<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var first = true;
            var maxObj = default(T);
            var maxKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    var currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) <= 0)
                    {
                        continue;
                    }

                    maxKey = currentKey;
                    maxObj = item;
                }
            }

            if (first)
            {
                throw new InvalidOperationException("Sequence is empty.");
            }

            return maxObj;
        }


        /// <summary>
        /// Moves items matching the predicate function to the start of the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="recursive">
        ///     <para>
        ///         Determines whether the move occurs for each item or only for the first item which meets the condition.
        ///     </para>
        ///     <para>
        ///         [<c>true</c> (default value) for each item; <c>false</c> for only the first matching item.]
        ///     </para>
        /// </param>
        /// <returns>An implementation of IList in which the items matching the predicate function have been moved to the start.</returns>
        public static IList<T> MoveToFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool recursive = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var array = source.ToArray();

            for (var e = 0; e < array.Length; e++)
            {
                if (!predicate(array[e]))
                {
                    continue;
                }

                var item = array[e];
                for (var i = e; i > 0; i--)
                {
                    array[i] = array[i - 1];
                }

                array[0] = item;

                // if the 'recursive' param indicates to move only the first matching item then exit here
                if (!recursive)
                {
                    return array.ToList();
                }
            }

            return array.ToList();
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


        /// <summary>
        /// Returns the collection of strings as comma-separated strings, where each element in the <see cref="source"/> is surrounded by quotes.
        /// </summary>
        /// <param name="source">A collection of strings.</param>
        /// <param name="separator">The symbol which will separate each of the elements in <see cref="source"/> Default value is a comma (",").</param>
        /// <returns>A non-zero-length string, if <see cref="source"/> contains elements; otherwise returns <c>null</c>.</returns>
        public static string ToCommaSeparatedStringWithQuotes(this IEnumerable<string> source, string separator = ", ")
        {
            return source == null
                       ? null
                       : source.None()
                           ? String.Empty
                           : String.Join(separator, source.Select(s => $"\"{s}\""));
        }
    }
}