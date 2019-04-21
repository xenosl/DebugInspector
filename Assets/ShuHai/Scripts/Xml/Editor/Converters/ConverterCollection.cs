using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.Xml
{
    public class ConverterCollection : ICollection<XConverter>
    {
        #region Access By Convert Type

        public XConverter FindByConvertType(Type type) { return EnumByConvertType(type).FirstOrDefault(); }

        public XConverter[] FindAllByConvertType(Type type) { return EnumByConvertType(type).ToArray(); }

        /// <summary>
        ///     Enumerate all converters that can convert specified type.
        /// </summary>
        /// <param name="type">The type check.</param>
        /// <returns>
        ///     A enumerable collection that contains all converters that can convert <paramref name="type" />.
        ///     The items in the enumerable collection is ordered by its priority.
        /// </returns>
        public IEnumerable<XConverter> EnumByConvertType(Type type)
        {
            using (var comparer = new ConverterPriorityComparer(type))
                return items.Where(c => c.CanConvert(type)).OrderByDescending(c => c, comparer);
        }

        #endregion

        #region ICollection

        public int Count { get { return items.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(XConverter item)
        {
            Ensure.Argument.NotNull(item, "item");
            items.Add(item);
        }

        public bool Remove(XConverter item)
        {
            Ensure.Argument.NotNull(item, "item");
            return items.Remove(item);
        }

        public void Clear() { items.Clear(); }

        public bool Contains(XConverter item)
        {
            Ensure.Argument.NotNull(item, "item");
            return items.Contains(item);
        }

        public void CopyTo(XConverter[] array, int arrayIndex) { items.CopyTo(array, arrayIndex); }

        private readonly List<XConverter> items = new List<XConverter>();

        #endregion

        #region IEnumerable

        public IEnumerator<XConverter> GetEnumerator() { return items.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion
    }
}