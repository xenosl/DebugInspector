using System;
using System.Collections.Generic;

namespace ShuHai
{
    public static class Ensure
    {
        public static class Argument
        {
            public static void NotNull<T>(T arg, string name)
            {
                if (arg == null)
                    throw new ArgumentNullException(name);
            }

            public static void NotNull<T>(T arg, string name, string message)
            {
                if (arg == null)
                    throw new ArgumentNullException(name, message);
            }

            public static void NotNullOrEmpty<T>(IEnumerable<T> arg, string name)
            {
                if (CollectionUtil.IsNullOrEmpty(arg))
                    throw new ArgumentException("Argument is null or empty.", name);
            }

            public static void NotNullOrEmpty<T>(IEnumerable<T> arg, string name, string message)
            {
                if (CollectionUtil.IsNullOrEmpty(arg))
                    throw new ArgumentException(message, name);
            }

            public static void NotNullOrEmpty(string arg, string name)
            {
                if (string.IsNullOrEmpty(arg))
                    throw new ArgumentException("Argument is null or empty.", name);
            }

            public static void Satisfy(bool condition, string name, string message)
            {
                if (!condition)
                    throw new ArgumentException(message, name);
            }

            /// <summary>
            ///     Throws <see cref="ArgumentOutOfRangeException" /> if argument is not in specified closed interval.
            /// </summary>
            /// <param name="arg">The argument to test.</param>
            /// <param name="name">Name of the argument.</param>
            /// <param name="min">Minimum side of the closed interval.</param>
            /// <param name="max">Maximum side of the closed interval.</param>
            public static void InRange(int arg, string name, int min, int max)
            {
                if (arg < min || arg > max)
                {
                    throw new ArgumentOutOfRangeException(name,
                        string.Format("A value in [{0},{1}] expected, got {2}", min, max, arg));
                }
            }

            /// <summary>
            ///     Throws <see cref="ArgumentOutOfRangeException" /> if argument is not a valid index within specified length.
            /// </summary>
            /// <param name="arg">The argument to test.</param>
            /// <param name="name">Name of the argument.</param>
            /// <param name="length">Length of the range for checking.</param>
            public static void IsValidIndex(int arg, string name, int length) { InRange(arg, name, 0, length - 1); }

            #region Mock Generic Constraints

            public static void Is<TConstraint>(Type arg, string name) { Is(arg, name, typeof(TConstraint)); }

            /// <summary>
            ///     Ensure <paramref Name="arg" /> is sub type of <paramref Name="constraint" /> or same as
            ///     <paramref Name="constraint" />.
            /// </summary>
            public static void Is(Type arg, string name, Type constraint)
            {
                if (!constraint.IsAssignableFrom(arg))
                {
                    throw new ArgumentException(
                        string.Format("'{0}' or subtype of '{0}' expected, got '{1}'.", constraint, arg),
                        name);
                }
            }

            public static void IsStruct(Type arg, string name)
            {
                if (!arg.IsValueType)
                    throw new ArgumentException(string.Format("'{0}' is not a struct.", arg), name);
            }

            public static void IsClass(Type arg, string name)
            {
                if (!arg.IsClass)
                    throw new ArgumentException(string.Format("'{0}' is not a class.", arg), name);
            }

            #endregion Mock Generic Constraints
        }
    }
}