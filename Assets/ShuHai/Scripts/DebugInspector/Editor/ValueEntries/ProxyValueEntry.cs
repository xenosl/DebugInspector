using System;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class ProxyValueEntry<TOwner, TValue> : ValueEntry<TOwner, TValue>
    {
        public override TValue Value { get { return (TValue)ActualEntry.Value; } set { ActualEntry.Value = value; } }

        public readonly IValueEntry ActualEntry;

        public ProxyValueEntry(IValueEntry actualEntry)
            : base((TOwner)actualEntry.Owner)
        {
            ActualEntry = actualEntry;
        }
    }

    public static class ProxyValueEntryFactory
    {
        public static IValueEntry Create(Type ownerType, Type valueType, IValueEntry actualEntry)
        {
            var def = typeof(ProxyValueEntry<,>);
            var type = def.MakeGenericType(ownerType, valueType);
            return (IValueEntry)Activator.CreateInstance(type, actualEntry);
        }
    }
}