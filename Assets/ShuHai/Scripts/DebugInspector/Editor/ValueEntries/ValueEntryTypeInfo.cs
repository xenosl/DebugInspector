using System;
using System.Collections.Generic;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class ValueEntryTypeInfo : DITypeInfo
    {
        public readonly Type Root;

        public readonly GenericArguments RootGenericArguments;

        private ValueEntryTypeInfo(Type type)
            : base(type)
        {
            Root = RootOf(type);
            RootGenericArguments = new GenericArguments(Root);
        }

        private static Type RootOf(Type type)
        {
            var t = type;
            while (!IsRoot(t))
                t = t.BaseType;
            return t;
        }

        private static bool IsRoot(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueEntry<,>);
        }

        #region Instances

        public static ValueEntryTypeInfo Get(Type entryType)
        {
            Ensure.Argument.NotNull(entryType, "entryType");
            Ensure.Argument.Is<IValueEntry>(entryType, "drawerType");
            return GetOrCreate(instances, entryType, Create);
        }

        private static ValueEntryTypeInfo Create(Type type) { return new ValueEntryTypeInfo(type); }

        private static readonly Dictionary<Type, ValueEntryTypeInfo>
            instances = new Dictionary<Type, ValueEntryTypeInfo>();

        #endregion Instances
    }
}