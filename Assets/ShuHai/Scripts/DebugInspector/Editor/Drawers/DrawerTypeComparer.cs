using System;
using System.Collections.Generic;

namespace ShuHai.DebugInspector.Editor
{
    public class DrawerTypeComparer : IComparer<Type>
    {
        public static readonly DrawerTypeComparer Default = new DrawerTypeComparer();

        public bool Reversed;

        public Type ValueType;

        public int Compare(Type l, Type r)
        {
            int result = CompareImpl(l, r);
            return Reversed ? -result : result;
        }

        private int CompareImpl(Type l, Type r)
        {
            int value;
            if (!CompareUtil.PreCompare(l, r, out value))
                return value;

            // Use default compare if any argument is not type of ValueDrawer.
            var root = ValueDrawerTypes.Root;
            if (!root.IsAssignableFrom(l) || !root.IsAssignableFrom(r))
                return Comparer<Type>.Default.Compare(l, r);

            var priorities = ValueDrawerPrioritiesForType.Get(ValueType ?? typeof(object));
            if (priorities != null)
            {
                var lg = l.IsGenericType ? l.GetGenericTypeDefinition() : l;
                var rg = r.IsGenericType ? r.GetGenericTypeDefinition() : r;
                int lp = priorities.GetPriority(lg), rp = priorities.GetPriority(rg);
                return lp.CompareTo(rp);
            }

            ValueDrawerTypeInfo lInfo = ValueDrawerTypeInfo.Get(l), rInfo = ValueDrawerTypeInfo.Get(r);
            if (lInfo != null && rInfo != null)
            {
                // Interface drawer is more appropriate for interface type.
                if (lInfo.IsInterfaceDrawer && !rInfo.IsInterfaceDrawer)
                    return ValueType.IsInterface ? 1 : -1;
                if (!lInfo.IsInterfaceDrawer && rInfo.IsInterfaceDrawer)
                    return ValueType.IsInterface ? -1 : 1;

                GenericParameterConstraints lcov = lInfo.ConstraintsOnValue, rcov = rInfo.ConstraintsOnValue;
                if (lcov != null && rcov != null)
                {
                    if (lcov.TypeCount == 1 && rcov.TypeCount == 1)
                    {
                        Type lIf = rcov.GetType(0), rIf = rcov.GetType(0);
                        if (lIf != rIf)
                        {
                            if (lIf.IsAssignableFrom(rIf))
                                return -1;
                            if (rIf.IsAssignableFrom(lIf))
                                return 1;
                        }
                    }
                }

                // The more the drawer type derived, the higher priority it owned.
                return lInfo.DeriveDepth.CompareTo(rInfo.DeriveDepth);
            }

            return 0;
        }

        public struct Scope : IDisposable
        {
            public readonly DrawerTypeComparer Comparer;

            public readonly Type ReservedValueType;
            public readonly bool ReservedReversed;

            public Scope(Type valueType) : this(Default, valueType, false) { }
            public Scope(Type valueType, bool reversed) : this(Default, valueType, reversed) { }

            public Scope(DrawerTypeComparer comparer, Type valueType, bool reversed)
                : this()
            {
                Ensure.Argument.NotNull(comparer, "comparer");

                disposed = false;

                Comparer = comparer;

                ReservedValueType = Comparer.ValueType;
                Comparer.ValueType = valueType;

                ReservedReversed = Comparer.Reversed;
                Comparer.Reversed = reversed;
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                Comparer.Reversed = ReservedReversed;
                Comparer.ValueType = ReservedValueType;

                disposed = true;
            }

            private bool disposed;
        }
    }
}