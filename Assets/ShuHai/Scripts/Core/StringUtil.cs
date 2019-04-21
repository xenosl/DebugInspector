using System;
using System.Collections.Generic;
using System.Text;

namespace ShuHai
{
    public static class StringUtil
    {
        #region Join

        public const string DefaultSeparator = ", ";

        public static StringBuilder Join<T>(StringBuilder builder,
            IList<T> values, Func<int, T, string> valueToString = null, string separator = DefaultSeparator)
        {
            Ensure.Argument.NotNull(builder, "builder");
            Ensure.Argument.NotNull(values, "values");

            int count = values.Count;
            for (int i = 0; i < count; ++i)
            {
                var value = values[i];
                builder.Append(valueToString != null ? valueToString(i, value) : ValueToString(value));
                if (i < count - 1)
                    builder.Append(separator);
            }
            return builder;
        }

        public static string Join<T>(IList<T> values,
            Func<int, T, string> valueToString = null, string separator = DefaultSeparator)
        {
            return Join(new StringBuilder(), values, valueToString, separator).ToString();
        }

        public static StringBuilder Join<T>(StringBuilder builder,
            IEnumerable<T> values, string separator = DefaultSeparator)
        {
            return Join(builder, values, (Func<StringBuilder, T, StringBuilder>)null, separator);
        }

        public static StringBuilder Join<T>(StringBuilder builder,
            IEnumerable<T> values, Func<T, string> valueToString, string separator = DefaultSeparator)
        {
            int count = 0;
            foreach (var value in values)
            {
                builder.Append(valueToString != null ? valueToString(value) : ValueToString(value))
                    .Append(separator);
                count++;
            }
            if (count > 0)
            {
                int sepLen = separator.Length;
                builder.Remove(builder.Length - sepLen, sepLen);
            }
            return builder;
        }

        public static StringBuilder Join<T>(StringBuilder builder, IEnumerable<T> values,
            Func<StringBuilder, T, StringBuilder> valueBuild, string separator = DefaultSeparator)
        {
            Ensure.Argument.NotNull(builder, "builder");
            Ensure.Argument.NotNull(values, "values");

            int count = 0;
            foreach (var value in values)
            {
                if (valueBuild != null)
                    valueBuild(builder, value);
                else
                    builder.Append(ValueToString(value));
                builder.Append(separator);
                count++;
            }
            if (count > 0)
            {
                int sepLen = separator.Length;
                builder.Remove(builder.Length - sepLen, sepLen);
            }
            return builder;
        }

        public static string Join<T>(IEnumerable<T> values,
            Func<StringBuilder, T, StringBuilder> valueBuild, string separator = DefaultSeparator)
        {
            return Join(new StringBuilder(), values, valueBuild, separator).ToString();
        }

        public static string Join<T>(IEnumerable<T> values,
            Func<T, string> valueToString, string separator = DefaultSeparator)
        {
            return Join(new StringBuilder(), values, valueToString, separator).ToString();
        }

        public static string Join<T>(IEnumerable<T> values, string separator = DefaultSeparator)
        {
            return Join(values, (Func<StringBuilder, T, StringBuilder>)null, separator);
        }

        private static string ValueToString<T>(T value)
        {
            return value is ValueType
                ? value.ToString()
                : (object)value == null
                    ? "null"
                    : value.ToString();
        }

        #endregion Join

        #region Indent

        public const string DefaultIndentUnit = "  ";

        public static string Indent(string str, byte level = 1, string unit = DefaultIndentUnit)
        {
            if (level == 0)
                return str;

            var builder = new IndentedStringBuilder(unit) { IndentLevel = level };
            builder.Append(str);
            return builder.ToString();
        }

        #endregion Indent
    }
}