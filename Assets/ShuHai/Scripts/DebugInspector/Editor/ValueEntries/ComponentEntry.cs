using System;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class ComponentEntry<TValue> : ValueEntry<GameObject, TValue>
        where TValue : Component
    {
        public override bool CanWrite { get { return false; } }

        public override TValue Value
        {
            get { return Owner.GetComponent<TValue>(); }
            set { throw new NotSupportedException(); }
        }

        public ComponentEntry(GameObject owner) : base(owner) { }
    }

    public static class ComponentEntryFactory
    {
        public static IValueEntry Create(Type componentType, GameObject owner)
        {
            return ValueEntryFactory.Create(RootEntryTypeDef, componentType, owner);
        }

        private static readonly Type RootEntryTypeDef = typeof(ComponentEntry<>);
    }
}