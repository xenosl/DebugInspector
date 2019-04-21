using System;

namespace ShuHai
{
    public static class ArrayUtil
    {
        #region New

        /// <summary>
        ///     Create an array and populate with default value.
        /// </summary>
        /// <typeparam name="T">Element type of the array.</typeparam>
        /// <param name="length">Length of the array.</param>
        public static T[] New<T>(int length)
            where T : new()
        {
            return New(length, DefaultNew<T>);
        }

        /// <summary>
        ///     Create an array and populate with specified value.
        /// </summary>
        /// <typeparam name="T">Element type of the array.</typeparam>
        /// <param name="length">Length of the array.</param>
        /// <param name="value">Value that populate to the array.</param>
        public static T[] New<T>(int length, T value)
        {
            var array = new T[length];
            for (int i = 0; i < length; ++i)
                array[i] = value;
            return array;
        }

        public static T[] New<T>(int length, Func<int, T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

            var array = new T[length];
            for (int i = 0; i < length; ++i)
                array[i] = valueFactory(i);
            return array;
        }

        private static T DefaultNew<T>(int l) where T : new() { return new T(); }

        #endregion New

        /// <summary>
        ///     Returns an empty array.
        /// </summary>
        /// <typeparam name="T"> The type of the elements of the array. </typeparam>
        /// <returns> Returns an empty Array. </returns>
        /// <remarks> Same as Array.Empty&lt;T&gt;() in .Net 4.6. </remarks>
        public static T[] Empty<T>() { return EmptyArray<T>.Value; }
    }

    internal static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}