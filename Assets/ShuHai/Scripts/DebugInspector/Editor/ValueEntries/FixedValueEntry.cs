using System;

namespace ShuHai.DebugInspector.Editor
{
    public class FixedValueEntry<TOwner, TValue> : ValueEntry<TOwner, TValue>
    {
        public override TValue Value { get { return value; } set { throw new NotSupportedException(); } }
        private readonly TValue value;

        public override Type TypeOfValue { get { return value != null ? value.GetType() : typeof(object); } }

        public override bool CanWrite { get { return false; } }

        public readonly ValueAccessResult ValueGetResult;

        public FixedValueEntry(TOwner owner, TValue value,
            ValueAccessResult valueGetResult = default(ValueAccessResult))
            : base(owner)
        {
            this.value = value;
            ValueGetResult = valueGetResult;
        }
    }
}