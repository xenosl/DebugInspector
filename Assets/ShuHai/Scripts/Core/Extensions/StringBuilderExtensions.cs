using System;
using System.Collections.Generic;
using System.Text;

namespace ShuHai
{
    public static class StringBuilderExtensions
    {
        #region Append

        /// <summary>
        ///     Join strings of specified values and append the joined string to the <see cref="StringBuilder" /> instance.
        /// </summary>
        /// <typeparam name="T"> Type of values to join. </typeparam>
        /// <param name="self"> The <see cref="StringBuilder" /> instance to append. </param>
        /// <param name="values"> Values to join. </param>
        /// <param name="valueToString"> Method that defined how a value is converted to a string. </param>
        /// <param name="separator"> Separator between string of values. </param>
        /// <returns> The <see cref="StringBuilder" /> instance. </returns>
        /// <seealso cref="StringUtil.Join{T}(StringBuilder, IEnumerable{T}, Func{T, string}, string)" />
        public static StringBuilder AppendJoin<T>(this StringBuilder self,
            IEnumerable<T> values, Func<T, string> valueToString, string separator = StringUtil.DefaultSeparator)
        {
            return StringUtil.Join(self, values, valueToString, separator);
        }

        public static StringBuilder AppendJoin<T>(this StringBuilder self,
            IEnumerable<T> values, string separator = StringUtil.DefaultSeparator)
        {
            return StringUtil.Join(self, values, separator);
        }

        public static StringBuilder AppendJoin<T>(this StringBuilder self, IEnumerable<T> values,
            Func<StringBuilder, T, StringBuilder> valueToString, string separator = StringUtil.DefaultSeparator)
        {
            return StringUtil.Join(self, values, valueToString, separator);
        }

        #endregion

        #region Insert At Head

        public static StringBuilder InsertHead(this StringBuilder self, bool value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, char value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, byte value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, sbyte value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, short value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, ushort value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, int value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, uint value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, long value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, ulong value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, float value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, double value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, string value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, decimal value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, char[] value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, object value) { return self.Insert(0, value); }

        #endregion

        #region Remove

        /// <summary>
        ///     Remove specified number of tail characters.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to remove from. </param>
        /// <param name="count"> Number of characters to remove. </param>
        public static StringBuilder RemoveTail(this StringBuilder self, int count)
        {
            Ensure.Argument.NotNull(self, "self");

            Ensure.Argument.Satisfy(count >= 0, "count", "Negative value is not allowed.");
            int len = self.Length;
            Ensure.Argument.Satisfy(count <= len, "count",
                string.Format("Count({0}) can not be greater than length({1})", count, len));

            self.Remove(len - count, count);
            return self;
        }

        /// <summary>
        ///     Remove all tail line feed characters.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to remove from. </param>
        public static StringBuilder RemoveTailLineFeed(this StringBuilder self)
        {
            Ensure.Argument.NotNull(self, "self");

            int length = self.Length, lastIndex = length - 1;
            if (length > 0 && self[lastIndex] == '\n')
            {
                int nextToLastIndex = lastIndex - 1;
                if (self[nextToLastIndex] == '\r') // The line feed is "\r\n"
                    self.Remove(nextToLastIndex, 2);
                else
                    self.Remove(lastIndex, 1);
            }
            return self;
        }

        #endregion

        #region Indented Append

        /// <summary>
        ///     Appends a character with indentation which defined by specified indent level and unit.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to append to. </param>
        /// <param name="value"> The character to append. </param>
        /// <param name="indentLevel"> Number of unit string to append. </param>
        /// <param name="indentUnit"> Unit string of each indent level. </param>
        /// <returns> The <paramref name="self" /> instance. </returns>
        public static StringBuilder IndentedAppend(
            this StringBuilder self, char value, byte indentLevel, string indentUnit)
        {
            if (indentLevel == 0)
                return self.Append(value);

            self.AppendHeadIndent(indentLevel, indentUnit);

            char last = self[self.Length - 1];
            if (last == '\n' && value != '\n')
                self.AppendIndent(indentLevel, indentUnit);
            self.Append(value);
            return self;
        }

        /// <summary>
        ///     Appends an array of characters with indentation which defined by specified indent level and unit.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to append to. </param>
        /// <param name="value"> The array of characters to append. </param>
        /// <param name="startIndex"> The starting position in <paramref name="value" />. </param>
        /// <param name="charCount"> The number of characters to append. </param>
        /// <param name="indentLevel"> Number of unit string to append. </param>
        /// <param name="indentUnit"> Unit string of each indent level. </param>
        /// <returns> The <paramref name="self" /> instance. </returns>
        public static StringBuilder IndentedAppend(this StringBuilder self,
            char[] value, int startIndex, int charCount, byte indentLevel, string indentUnit)
        {
            if (indentLevel == 0)
                return self.Append(value, startIndex, charCount);

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if (charCount < 0)
                throw new ArgumentOutOfRangeException("charCount");

            if (value == null)
            {
                if (startIndex == 0 && charCount == 0)
                    return self;
                throw new ArgumentNullException("value");
            }

            if (charCount > value.Length - startIndex)
                throw new ArgumentOutOfRangeException("charCount");

            if (charCount == 0)
                return self;

            self.AppendHeadIndent(indentLevel, indentUnit);

            int count = value.Length;
            int endIndex = startIndex + charCount;
            for (int i = startIndex; i < endIndex; ++i)
            {
                var c = value[i];
                self.Append(c);
                if (c == '\n' && i != count - 1)
                    self.AppendIndent(indentLevel, indentUnit);
            }
            return self;
        }

        /// <summary>
        ///     Appends an array of characters with indentation which defined by specified indent level and unit.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to append to. </param>
        /// <param name="value"> The array of characters to append. </param>
        /// <param name="indentLevel"> Number of unit string to append. </param>
        /// <param name="indentUnit"> Unit string of each indent level. </param>
        /// <returns> The <paramref name="self" /> instance. </returns>
        public static StringBuilder IndentedAppend(
            this StringBuilder self, char[] value, byte indentLevel, string indentUnit)
        {
            if (indentLevel == 0)
                return self.Append(value);

            if (CollectionUtil.IsNullOrEmpty(value))
                return self;

            self.AppendHeadIndent(indentLevel, indentUnit);

            int count = value.Length;
            for (int i = 0; i < count; ++i)
            {
                var c = value[i];
                self.Append(c);
                if (c == '\n' && i != count - 1)
                    self.AppendIndent(indentLevel, indentUnit);
            }
            return self;
        }

        /// <summary>
        ///     Appends a string with indentation which defined by specified indent level and unit.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to append to. </param>
        /// <param name="value"> The string to append. </param>
        /// <param name="indentLevel"> Number of unit string to append. </param>
        /// <param name="indentUnit"> Unit string of each indent level. </param>
        /// <returns> The <paramref name="self" /> instance. </returns>
        public static StringBuilder IndentedAppend(
            this StringBuilder self, string value, byte indentLevel, string indentUnit)
        {
            if (indentLevel == 0)
                return self.Append(value);

            if (string.IsNullOrEmpty(value))
                return self;

            self.AppendHeadIndent(indentLevel, indentUnit);

            int count = value.Length;
            for (int i = 0; i < count; ++i)
            {
                var c = value[i];
                self.Append(c);
                if (c == '\n' && i != count - 1)
                    self.AppendIndent(indentLevel, indentUnit);
            }
            return self;
        }

        /// <summary>
        ///     Appends an indent string combined by specified level and unit if the <see cref="StringBuilder" /> is empty.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to append to. </param>
        /// <param name="indentLevel"> Number of unit string to append. </param>
        /// <param name="indentUnit"> Unit string of each indent level. </param>
        /// <returns> The <paramref name="self" /> instance. </returns>
        public static StringBuilder AppendHeadIndent(this StringBuilder self, byte indentLevel, string indentUnit)
        {
            if (self.Length == 0 || self[self.Length - 1] == '\n')
                self.AppendIndent(indentLevel, indentUnit);
            return self;
        }

        /// <summary>
        ///     Appends an indent string combined by specified level and unit.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to append to. </param>
        /// <param name="indentLevel"> Number of unit string to append. </param>
        /// <param name="indentUnit"> Unit string of each indent level. </param>
        /// <returns> The <paramref name="self" /> instance. </returns>
        public static StringBuilder AppendIndent(this StringBuilder self, byte indentLevel, string indentUnit)
        {
            for (int i = 0; i < indentLevel; ++i)
                self.Append(indentUnit);
            return self;
        }

        #endregion Indentation
    }
}