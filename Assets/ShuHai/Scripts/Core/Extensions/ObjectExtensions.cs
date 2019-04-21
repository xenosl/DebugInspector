using System.Collections.Generic;

namespace ShuHai
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value) { yield return value; }
    }
}