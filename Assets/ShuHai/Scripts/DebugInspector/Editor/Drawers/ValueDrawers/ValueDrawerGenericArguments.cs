using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    /// Represents a collection that contains generic type arguments of a class which implemented
    /// <see cref="ValueDrawer{TOwner, TValue}"/>.
    /// </summary>
    public class ValueDrawerGenericArguments : GenericArguments
    {
        /// <summary>
        /// "TOwner" of <see cref="ValueDrawer{TOwner, TValue}"/>.
        /// </summary>
        public Type Owner { get { return Roots[0]; } }

        /// <summary>
        /// "TValue" of <see cref="ValueDrawer{TOwner, TValue}"/>.
        /// </summary>
        public Type Value { get { return Roots[1]; } }

        /// <summary>
        /// Number of root arguments.
        /// </summary>
        public int RootCount { get { return Roots.Count; } }

        /// <summary>
        /// Generic arguments of <see cref="ValueDrawer{TOwner, TValue}"/>.
        /// </summary>
        public readonly GenericArguments Roots;

        /// <summary>
        /// Initialize a new instance of <see cref="ValueDrawerGenericArguments"/>.
        /// </summary>
        public ValueDrawerGenericArguments(Type drawerType)
            : base(drawerType)
        {
            Ensure.Argument.Is(drawerType, "drawerType", ValueDrawerTypes.Root);

            var rootType = EnumHierarchicalTypes(drawerType).Last();
            Roots = new GenericArguments(rootType);

            // Hash code is constant since this is an immutable class.
            InitHashCode();
        }

        private IEnumerable<Type> EnumHierarchicalTypes(Type drawerType)
        {
            var type = drawerType;
            while (type != typeof(ValueDrawer))
            {
                yield return type;
                type = type.BaseType;
            }
        }

        #region Equality Comparison

        public static bool operator ==(ValueDrawerGenericArguments l, ValueDrawerGenericArguments r) { return Equals(l, r); }
        public static bool operator !=(ValueDrawerGenericArguments l, ValueDrawerGenericArguments r) { return !Equals(l, r); }

        public override bool Equals(object obj)
        {
            var other = obj as ValueDrawerGenericArguments;
            return base.Equals(other) && Roots.Equals(other.Roots);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        private void InitHashCode() { unchecked { hashCode = hashCode * 23 + Roots.GetHashCode(); } }

        #endregion Equality Comparison
    }
}