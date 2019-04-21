using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShuHai.DebugInspector.Editor
{
    public class GenericArguments : IEnumerable<Type>
    {
        /// <summary>
        /// Number of generic arguments.
        /// </summary>
        public int Count { get { return arguments.Length; } }

        /// <summary>
        /// Get generic argument at specified index.
        /// </summary>
        /// <param name="index"> Index of the argument. </param>
        public Type this[int index] { get { return arguments[index]; } }

        /// <summary>
        /// Initialize a new instance of <see cref="GenericArguments"/>.
        /// </summary>
        public GenericArguments(Type type)
        {
            Ensure.Argument.NotNull(type, "type");
            Ensure.Argument.Satisfy(type.IsGenericType, "type", "Generic type is required.");

            arguments = type.GetGenericArguments();

            // Hash code is constant since this is an immutable class.
            InitHashCode();
        }

        public Type[] ToArray() { return arguments.ToArray(); }

        public int IndexOf(Type arg) { return arguments.IndexOf(arg); }

        public bool Contains(Type arg) { return IndexOf(arg) >= 0; }

        private readonly Type[] arguments;

        #region Equality Comparison

        public static bool operator ==(GenericArguments l, GenericArguments r) { return Equals(l, r); }
        public static bool operator !=(GenericArguments l, GenericArguments r) { return !Equals(l, r); }

        public override bool Equals(object obj)
        {
            var other = CompareUtil.PreEquals(this, obj);
            return (object)other != null && arguments.SequenceEqual(other.arguments);
        }

        protected int hashCode;

        public override int GetHashCode() { return hashCode; }

        private void InitHashCode() { hashCode = HashCode.Calculate(arguments); }

        #endregion Equality Comparison

        #region ToString

        public string ToString(bool fullname, bool ommitSelf)
        {
            var builder = new StringBuilder();
            if (!ommitSelf)
            {
                var type = GetType();
                builder.Append(type.GetName(fullname));
            }
            builder.Append('[').AppendJoin(this, t => t.GetName(fullname)).Append(']');
            return builder.ToString();
        }

        public override string ToString()
        {
            return ToString(false, false);
        }

        #endregion ToString

        #region IEnumerable

        public IEnumerator<Type> GetEnumerator() { return ((IList<Type>)arguments).GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion IEnumerable
    }
}