using System.Collections;
using System.Collections.Generic;

namespace ShuHai
{
    public static class CollectionUtil
    {
        /// <summary>
        ///     Indicates whether the specified collection is <see langword="null" /> or empty.
        /// </summary>
        /// <typeparam name="T"> Type of collection element. </typeparam>
        /// <param name="collection"> The collection to test. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="collection" /> is <see langword="null" /> or a empty collection;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                return true;

            var genericCollection = collection as ICollection<T>;
            if (genericCollection != null)
                return genericCollection.Count == 0;

            var objectCollection = collection as ICollection;
            if (objectCollection != null)
                return objectCollection.Count == 0;

            using (var e = collection.GetEnumerator())
            {
                if (e.MoveNext())
                    return false;
            }
            return true;
        }
    }
}