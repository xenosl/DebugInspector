using System;
using System.Collections;
using System.Collections.Generic;

namespace ShuHai
{
    public static class CollectionExtensions
    {
        #region Enumerate

        /// <summary>
        ///     Performs the specified action on each element of specified collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="self">The collection itself.</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            Ensure.Argument.NotNull(self, "self");

            foreach (var item in self)
                action(item);
        }

        /// <summary>
        ///     Enumerate a <see cref="IEnumerator" /> as <see cref="IEnumerable" />.
        /// </summary>
        public static IEnumerable ToEnumerable(this IEnumerator self)
        {
            Ensure.Argument.NotNull(self, "self");

            while (self.MoveNext())
                yield return self.Current;
        }

        /// <summary>
        ///     Enumerate a <see cref="IEnumerator{T}" /> as <see cref="IEnumerable{T}" />.
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> self)
        {
            Ensure.Argument.NotNull(self, "self");

            while (self.MoveNext())
                yield return self.Current;
        }

        #endregion Enumerate

        #region Collection

        /// <summary>
        ///     Add items from sepcified enumeration to the current collection instance.
        /// </summary>
        /// <param name="self"> The current collection instance. </param>
        /// <param name="items"> From where the items should be added to the current collection instance. </param>
        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> items)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(items, "items");

            foreach (var item in items)
                self.Add(item);
        }

        public static void RemoveRange<T>(this ICollection<T> self, IEnumerable<T> items)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(items, "items");

            foreach (var item in items)
                self.Remove(item);
        }

        #endregion Collection

        #region List

        /// <summary>
        ///     Get a value from list at specific <paramref name="index" />.
        /// </summary>
        /// <typeparam name="T"> The type of elements in the list. </typeparam>
        /// <param name="self"> The list instance. </param>
        /// <param name="index"> The zero-based index of the element to get. </param>
        /// <param name="valueOnOutOfRange"> The value that returns when <paramref name="index" /> out of range. </param>
        /// <returns> The element at the specified index. </returns>
        public static T At<T>(this IList<T> self, int index, T valueOnOutOfRange = default(T))
        {
            Ensure.Argument.NotNull(self, "self");

            if (!Index.IsValid(index, self.Count))
                return valueOnOutOfRange;
            return self[index];
        }

        /// <summary>
        ///     Performs the specified action on each element of list <paramref name="self" />.
        /// </summary>
        /// <typeparam name="T"> The type of elements in the list. </typeparam>
        /// <param name="self"> The list instance. </param>
        /// <param name="action"> Action to perform. </param>
        /// <param name="reverseOrder"> Is actions performed in reverse order of the list. </param>
        public static void ForEach<T>(this IList<T> self,
            Action<int, T> action, bool reverseOrder = false)
        {
            Ensure.Argument.NotNull(self, "self");

            if (reverseOrder)
            {
                for (int i = self.Count - 1; i >= 0; --i)
                    action(i, self[i]);
            }
            else
            {
                for (int i = 0; i < self.Count; ++i)
                    action(i, self[i]);
            }
        }

        #endregion List

        #region Dictionary

        /// <summary>
        ///     Get element with specified key.
        /// </summary>
        /// <typeparam name="TKey"> The type of keys in the dictionary. </typeparam>
        /// <typeparam name="TValue"> The type of values in the dictionary. </typeparam>
        /// <param name="self"> The dictionary instance. </param>
        /// <param name="key"> The key of the element to get. </param>
        /// <param name="fallback"> Value that returns when element with <paramref name="key" /> not found. </param>
        /// <returns>
        ///     Element with specified <paramref name="key" />, if found; otherwise, <paramref name="fallback" />.
        /// </returns>
        public static TValue GetValue<TKey, TValue>(
            this IDictionary<TKey, TValue> self, TKey key, TValue fallback = default(TValue))
        {
            Ensure.Argument.NotNull(self, "self");

            TValue value;
            if (!self.TryGetValue(key, out value))
                value = fallback;
            return value;
        }

        public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<TKey> keys)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(keys, "keys");

            foreach (var key in keys)
                self.Remove(key);
        }

        #region Multi-Value

        public static TValueCollection Add<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, TValue value,
            Func<TValueCollection> valueCollectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(valueCollectionFactory, "valueCollectionFactory");

            TValueCollection values;
            if (!self.TryGetValue(key, out values))
            {
                values = valueCollectionFactory();
                if (values == null)
                    throw new NullReferenceException();
                self.Add(key, values);
            }

            values.Add(value);
            return values;
        }

        public static TValueCollection Add<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, TValue value)
            where TValueCollection : ICollection<TValue>, new()
        {
            return Add(self, key, value, NewValueCollection<TValueCollection>);
        }

        // Avoid create delegate every time.
        private static TValueCollection NewValueCollection<TValueCollection>()
            where TValueCollection : new()
        {
            return new TValueCollection();
        }

        public static bool Remove<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, TValue value)
            where TValueCollection : ICollection<TValue>
        {
            Ensure.Argument.NotNull(self, "self");

            TValueCollection values;
            if (!self.TryGetValue(key, out values))
                return false;

            bool removed = values.Remove(value);
            if (removed && values.Count == 0)
                self.Remove(key);
            return removed;
        }

        #endregion Multi-Value

        #endregion Dictionary
    }
}