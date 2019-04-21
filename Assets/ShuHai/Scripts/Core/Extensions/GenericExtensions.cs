using System;

namespace ShuHai
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T value, T min, T max)
            where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min
                : (value.CompareTo(max) > 0 ? max : value);
        }
    }
}