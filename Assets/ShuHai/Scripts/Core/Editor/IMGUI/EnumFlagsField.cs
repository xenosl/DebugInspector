using System;
using System.Linq;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public sealed class EnumFlagsField<TEnum>
        where TEnum : struct
    {
        public string Label { get { return field.Label; } }

        public event Action<TEnum> ValueChanged;

        public TEnum Value { get { return ToEnum(field.Value); } set { field.Value = ToMask(value); } }

        public EnumFlagsField() : this(string.Empty) { }

        public EnumFlagsField(TEnum value) : this(string.Empty, value) { }

        public EnumFlagsField(string label, TEnum value = default(TEnum))
        {
            var type = typeof(TEnum);
            var names = Enum.GetNames(type);
            var values = (TEnum[])Enum.GetValues(type);
            var options = names.Select((n, i) => Tuple.Create(n, ToMask(values[i])));
            field = new MaskField(label, ToMask(value), options);
            field.ValueChanged += OnMaskValueChanged;
        }

        public TEnum GUI(GUIContent popupButtonContent = null, GUIStyle style = null)
        {
            return ToEnum(field.GUI(popupButtonContent, style));
        }

        private void OnMaskValueChanged(ulong oldValue) { ValueChanged.NPInvoke(ToEnum(oldValue)); }

        private readonly MaskField field;

        private static readonly Converter<TEnum, ulong> ToMask = CommonMethodsEmitter.CreateCast<TEnum, ulong>();
        private static readonly Converter<ulong, TEnum> ToEnum = CommonMethodsEmitter.CreateCast<ulong, TEnum>();

        private static void CheckType()
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new NotSupportedException("enum type expected.");
            if (!type.IsDefined(typeof(FlagsAttribute), false))
                throw new NotSupportedException("enum with FlagsAttribute expected.");
        }

        static EnumFlagsField() { CheckType(); }
    }
}