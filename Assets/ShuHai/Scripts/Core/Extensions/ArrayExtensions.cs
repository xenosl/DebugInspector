using System;

namespace ShuHai
{
    public static class ArrayExtensions
    {
        /// <summary>
        ///     See <see cref="Array.IndexOf{T}(T[], T)" />.
        /// </summary>
        public static int IndexOf<T>(this T[] array, T value) { return Array.IndexOf(array, value); }

        /// <summary>
        ///     See <see cref="Array.IndexOf{T}(T[], T, int)" />.
        /// </summary>
        public static int IndexOf<T>(this T[] array, T value, int startIndex)
        {
            return Array.IndexOf(array, value, startIndex);
        }

        /// <summary>
        ///     Strongly typed version of <see cref="Array.Clone()" />.
        /// </summary>
        public static T[] TypedClone<T>(this T[] array) { return (T[])array.Clone(); }
    }
}