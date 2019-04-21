using System;
using System.Collections;
using System.Collections.Generic;

namespace ShuHai.Collections
{
    /// <summary>
    ///     Represents <see cref="SortedList{TKey,TValue}" /> with same key and value.
    /// </summary>
    /// <typeparam name="T">The type of values in the collection.</typeparam>
    public class OrderedList<T> : IList<T>
    {
        public T this[int index]
        {
            get { return values[index]; }
            set
            {
                throw new NotSupportedException(
                    "Set value by index is not supported since the list is auto ordered.");
            }
        }

        public int Count { get { return values.Count; } }

        public OrderedList() { list = new SortedList<T, T>(); }
        public OrderedList(int capacity) { list = new SortedList<T, T>(capacity); }
        public OrderedList(IComparer<T> comparer) { list = new SortedList<T, T>(comparer); }
        public OrderedList(int capacity, IComparer<T> comparer) { list = new SortedList<T, T>(capacity, comparer); }

        public void ReplaceOrAdd(T item) { list[item] = item; }
        public void Add(T item) { list.Add(item, item); }
        public void Clear() { list.Clear(); }
        public bool Remove(T item) { return list.Remove(item); }
        public void RemoveAt(int index) { list.RemoveAt(index); }

        public bool Contains(T item) { return values.Contains(item); }
        public int IndexOf(T item) { return values.IndexOf(item); }

        public void CopyTo(T[] array, int arrayIndex) { values.CopyTo(array, arrayIndex); }

        public IEnumerator<T> GetEnumerator() { return values.GetEnumerator(); }

        private IList<T> values { get { return list.Values; } }
        private readonly SortedList<T, T> list;

        #region Explicit Implementations

        void IList<T>.Insert(int index, T item) { throw new NotSupportedException(); }

        bool ICollection<T>.IsReadOnly { get { return false; } }

        IEnumerator IEnumerable.GetEnumerator() { return values.GetEnumerator(); }

        #endregion
    }
}