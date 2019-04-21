using System;

namespace ShuHai.DebugInspector.Editor
{
    using UObject = UnityEngine.Object;

    public class SelectionEntry<TValue> : ValueEntry<UObject[], TValue>
        where TValue : UObject
    {
        public readonly int Index;

        public override TValue Value
        {
            get { return (TValue)Owner[Index]; }
            set { throw new NotSupportedException(); }
        }

        public override bool CanWrite { get { return false; } }

        public SelectionEntry(UObject[] owner, int index)
            : base(owner)
        {
            Index = index;
        }
    }

    public static class SelectionEntryFactory
    {
        public static IValueEntry Create(Type selectedType, UObject[] owner, int index)
        {
            return ValueEntryFactory.Create(RootEntryTypeDef, selectedType, owner, index);
        }

        private static readonly Type RootEntryTypeDef = typeof(SelectionEntry<>);
    }
}