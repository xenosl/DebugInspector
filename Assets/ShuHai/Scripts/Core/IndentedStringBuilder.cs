using System;
using System.Globalization;
using System.Text;

namespace ShuHai
{
    /// <summary>
    ///     Almost the same with <see cref="StringBuilder" />, except that an indented string is added after every line
    ///     break if <see cref="IndentLevel" /> is not 0.
    /// </summary>
    /// <remarks>
    ///     The class is implemented internal by <see cref="StringBuilder" />, so the members are almost the same with
    ///     <see cref="StringBuilder" />, the only difference is that all operations are affected by
    ///     <see cref="IndentLevel" /> and <see cref="IndentUnit" />.
    /// </remarks>
    public class IndentedStringBuilder
    {
        /// <summary>
        ///     The <see cref="StringBuilder" /> used by current <see cref="IndentedStringBuilder" /> instance.
        /// </summary>
        public readonly StringBuilder Builder;

        public char this[int index] { get { return Builder[index]; } set { Builder[index] = value; } }

        public int Length { get { return Builder.Length; } }

        public int Capacity { get { return Builder.Capacity; } set { Builder.Capacity = value; } }

        public int MaxCapacity { get { return Builder.MaxCapacity; } }

        #region Constructors

        /// <summary>
        ///     <see cref="StringBuilder()" />.
        /// </summary>
        /// <param name="indentUnit"> The indent string of each indent level. </param>
        public IndentedStringBuilder(string indentUnit = DefaultIndentUnit)
            : this(new StringBuilder(), indentUnit) { }

        /// <summary>
        ///     <see cref="StringBuilder(int)" />
        /// </summary>
        /// <param name="indentUnit"> The indent string of each indent level. </param>
        public IndentedStringBuilder(int capacity, string indentUnit = DefaultIndentUnit)
            : this(new StringBuilder(capacity), indentUnit) { }

        /// <summary>
        ///     <see cref="StringBuilder(string)" />
        /// </summary>
        /// <param name="indentUnit"> The indent string of each indent level. </param>
        public IndentedStringBuilder(string value, string indentUnit = DefaultIndentUnit)
            : this(new StringBuilder(value), indentUnit) { }

        /// <summary>
        ///     <see cref="StringBuilder(int, int)" />
        /// </summary>
        /// <param name="indentUnit"> The indent string of each indent level. </param>
        public IndentedStringBuilder(int capacity, int maxCapacity, string indentUnit = DefaultIndentUnit)
            : this(new StringBuilder(capacity, maxCapacity), indentUnit) { }

        /// <summary>
        ///     <see cref="StringBuilder(string, int)" />
        /// </summary>
        /// <param name="indentUnit"> The indent string of each indent level. </param>
        public IndentedStringBuilder(string value, int capacity, string indentUnit = DefaultIndentUnit)
            : this(new StringBuilder(value, capacity), indentUnit) { }

        /// <summary>
        ///     <see cref="StringBuilder(string, int, int, int)" />
        /// </summary>
        /// <param name="indentUnit"> The indent string of each indent level. </param>
        public IndentedStringBuilder(string value, int startIndex, int length, int capacity,
            string indentUnit = DefaultIndentUnit)
            : this(new StringBuilder(value, startIndex, length, capacity), indentUnit) { }

        private IndentedStringBuilder(StringBuilder builder, string indentUnit)
        {
            this.Builder = builder;
            IndentUnit = indentUnit;
        }

        #endregion Constructors

        #region Append

        public IndentedStringBuilder Append(float value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(double value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(sbyte value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(byte value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(short value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(ushort value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(int value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(uint value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(long value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(ulong value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(decimal value)
        {
            return Append(value.ToString(CultureInfo.CurrentCulture));
        }

        public IndentedStringBuilder Append(bool value) { return Append(value.ToString(CultureInfo.CurrentCulture)); }

        public IndentedStringBuilder Append(object value)
        {
            if (value == null)
                return this;
            return Append(value.ToString());
        }

        public IndentedStringBuilder Append(char value, int repeatCount)
        {
            Builder.Append(value, repeatCount);
            return this;
        }

        public IndentedStringBuilder Append(char value)
        {
            Builder.IndentedAppend(value, IndentLevel, IndentUnit);
            return this;
        }

        public IndentedStringBuilder Append(char[] value)
        {
            Builder.IndentedAppend(value, IndentLevel, IndentUnit);
            return this;
        }

        public IndentedStringBuilder Append(char[] value, int startIndex, int charCount)
        {
            Builder.IndentedAppend(value, startIndex, charCount, IndentLevel, IndentUnit);
            return this;
        }

        public IndentedStringBuilder Append(string value)
        {
            Builder.IndentedAppend(value, IndentLevel, IndentUnit);
            return this;
        }

        /// <summary>
        ///     Append line with indentation.
        /// </summary>
        /// <param name="value"> The line text to append. </param>
        public IndentedStringBuilder AppendLine(string value) { return AppendHeadIndent().Append(value).AppendLine(); }

        /// <summary>
        ///     Proxy method of <see cref="StringBuilder.AppendLine()" /> with return value of
        ///     <see cref="IndentedStringBuilder" />.
        /// </summary>
        public IndentedStringBuilder AppendLine()
        {
            Builder.AppendLine();
            return this;
        }

        /// <summary>
        ///     Call <see cref="StringBuilderExtensions.AppendHeadIndent(StringBuilder, byte, string)" /> with context of
        ///     this <see langword="class" />.
        /// </summary>
        private IndentedStringBuilder AppendHeadIndent()
        {
            Builder.AppendHeadIndent(IndentLevel, IndentUnit);
            return this;
        }

        /// <summary>
        ///     Call <see cref="StringBuilderExtensions.AppendIndent(StringBuilder, byte, string)" /> with context of
        ///     this <see langword="class" />.
        /// </summary>
        private IndentedStringBuilder AppendIndent()
        {
            Builder.AppendIndent(IndentLevel, IndentUnit);
            return this;
        }

        #endregion Append

        #region Indent

        /// <summary>
        ///     The indent unit used if indent unit argument is omitted when calling indent methods.
        /// </summary>
        public const string DefaultIndentUnit = "  ";

        /// <summary>
        ///     Indent unit string used by current <see cref="IndentedStringBuilder" /> instance.
        /// </summary>
        public readonly string IndentUnit;

        /// <summary>
        ///     Current indent level that applies when appending string.
        /// </summary>
        public byte IndentLevel;

        /// <summary>
        ///     Increase <see cref="IndentLevel" />.
        /// </summary>
        /// <param name="level"> Number indent level to increase. </param>
        public IndentedStringBuilder Indent(byte level = 1)
        {
            checked
            {
                IndentLevel += level;
            }
            return this;
        }

        /// <summary>
        ///     Decrease <see cref="IndentLevel" />.
        /// </summary>
        /// <param name="level"> Number of indent level to decrease. </param>
        public IndentedStringBuilder Dedent(byte level = 1)
        {
            checked
            {
                IndentLevel -= level;
            }
            return this;
        }

        #endregion Indent

        #region ToString

        public override string ToString() { return Builder.ToString(); }

        public string ToString(int startIndex, int length) { return Builder.ToString(startIndex, length); }

        #endregion ToString

        /// <summary>
        ///     Define a code block that all string build operations are indented by specified level.
        /// </summary>
        public class Scope : IDisposable
        {
            public readonly IndentedStringBuilder Builder;
            public readonly byte IndentLevel;

            public Scope(IndentedStringBuilder builder, byte indentLevel = 1)
            {
                disposed = false;

                Builder = builder;
                IndentLevel = indentLevel;

                Builder.Indent(IndentLevel);
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                Builder.Dedent(IndentLevel);

                disposed = true;
            }

            private bool disposed;
        }
    }
}