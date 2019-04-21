using System.Collections.Generic;

namespace ShuHai
{
    public static class CompareUtil
    {
        /// <summary>
        ///     General prerequisite compare logic for classes.
        /// </summary>
        /// <typeparam name="T"> Type of the object to compare. </typeparam>
        /// <param name="l"> Object to compare. </param>
        /// <param name="r"> Object to compare. </param>
        /// <param name="value"> Compare result. </param>
        /// <returns>
        ///     <see langword="true" /> to tell the caller the compare logic is good to go on; or <see langword="false" /> to
        ///     tell the caller the comparison is done and need to return.
        /// </returns>
        public static bool PreCompare<T>(T l, T r, out int value)
            where T : class
        {
            value = 0;

            // In case of "T" overloads operator "==" and "!=".
            object lobj = (object)l, robj = (object)r;

            if (lobj == null && robj == null)
            {
                value = 0;
                return false;
            }
            if (lobj != null && robj == null)
            {
                value = 1;
                return false;
            }
            if (lobj == null) // && robj != null
            {
                value = -1;
                return false;
            }

            if (ReferenceEquals(lobj, robj))
            {
                value = 0;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Prerequisite logic for overwritten method of <see cref="object.Equals(object)" /> in class.
        /// </summary>
        /// <typeparam name="T"> Type of the object to compare. </typeparam>
        /// <param name="self"> Current instance of the object. </param>
        /// <param name="obj"> The object to compare with the current object. </param>
        /// <returns>
        ///     Reference of <paramref name="obj" /> in the form of <typeparamref name="T" /> if <paramref name="obj" />
        ///     statisfied the prerequistes; otherwise, <see langword="null" />.
        /// </returns>
        public static T PreEquals<T>(T self, object obj)
            where T : class
        {
            var other = obj as T;
            // The explicit convert is required in case of "T" overloads operator "==" and "!=".
            if ((object)other == null)
                return null;
            if (ReferenceEquals(self, other))
                return other;
            return other;
        }

        public static bool OperatorEquals<T>(T l, T r)
            where T : class
        {
            // Note that the cast to object is required in case of T overloaded operator "!=".
            if ((object)l != null)
                return l.Equals(r);
            if ((object)r != null)
                return r.Equals(l);
            return true; // l == r == null
        }

        public static bool ListEquals<T>(IList<T> l, IList<T> r)
        {
            int count = l.Count;
            if (count != r.Count)
                return false;

            for (int i = 0; i < count; ++i)
            {
                if (!EqualityComparer<T>.Default.Equals(l[i], r[i]))
                    return false;
            }
            return true;
        }
    }
}