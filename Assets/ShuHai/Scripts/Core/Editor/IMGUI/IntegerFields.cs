using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public class IntegerField<T>
        where T : struct
    {
        public IntegerField() { }

        public IntegerField(string label, T value = default(T))
        {
            Label = label;
            Value = value;
        }

        public T GUI()
        {
            EditorGUILayout.BeginHorizontal();
            LabelGUI();
            ValueGUI();
            HexadecimalToggleGUI();
            EditorGUILayout.EndHorizontal();
            return Value;
        }

        #region Label

        public string Label { get { return labelContent.text; } set { labelContent.text = value; } }
        public string LabelTooltip { get { return labelContent.tooltip; } set { labelContent.tooltip = value; } }
        public Texture LabelImage { get { return labelContent.image; } set { labelContent.image = value; } }

        private readonly GUIContent labelContent = new GUIContent();

        private void LabelGUI()
        {
            if (!string.IsNullOrEmpty(Label))
                EditorGUILayout.PrefixLabel(labelContent);
        }

        #endregion Label

        #region Value

        public bool Hexadecimal;

        public T Value
        {
            get { return value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, this.value))
                    return;
                this.value = value;
                valueTextDirty = true;
            }
        }

        public string ValueText
        {
            get
            {
                if (valueTextDirty)
                    valueText = ValueToString(Value, Hexadecimal ? "X" : null);
                return valueText;
            }
            private set
            {
                if (value == valueText)
                    return;

                if (string.IsNullOrEmpty(value))
                {
                    Value = default(T);
                    return;
                }

                var numStyle = Hexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer;
                T parsedValue;
                if (TryParseValue(value, numStyle, null, out parsedValue))
                    Value = parsedValue;
            }
        }

        private T value;
        private bool valueTextDirty = true;
        private string valueText;

        private GUILayoutOption[] valueLayoutOptions = ArrayUtil.Empty<GUILayoutOption>();

        private void ValueGUI() { ValueText = GUILayout.TextField(ValueText, valueLayoutOptions); }

        //public int MaxValueTextLength { get { return Hexadecimal ? MaxHexadecimalTextLength : MaxDecimalTextLength; } }

        //private static readonly int MaxDecimalTextLength = T.MaxValue.ToString().Length;
        //private static readonly int MaxHexadecimalTextLength = T.MaxValue.ToString("X").Length;

        #endregion Value

        #region Hexadecimal Toggle

        /// <summary>
        ///     <see cref="GUIStyle" /> for hexadecimal toggle. <see cref="EditorStylesEx.HexadecimalToggle" /> is used if the
        ///     value is not set.
        /// </summary>
        public GUIStyle HexadecimalToggleStyle
        {
            get { return hexadecimalToggleStyle ?? EditorStylesEx.HexadecimalToggle; }
            set { hexadecimalToggleStyle = value; }
        }

        private GUIStyle hexadecimalToggleStyle;

        private void HexadecimalToggleGUI()
        {
            // The hexadecimal switch toggle is always enabled it doesn't change value.
            using (new EditorGUIEx.DisabledScope(false))
            {
                var rect = GetHexadecimalToggleRect();
                Hexadecimal = EditorGUI.Toggle(rect, Hexadecimal, HexadecimalToggleStyle);
            }
        }

        private Rect GetHexadecimalToggleRect()
        {
            var width = HexadecimalToggleStyle.fixedWidth;
            return EditorGUILayout.GetControlRect(GUILayout.Width(width));
        }

        #endregion Hexadecimal Toggle

        #region Value Type Methods

        private static readonly ValueToStringFunc ValueToString = CreateValueToStringMethod();
        private static readonly TryParseValueFunc TryParseValue = CreateTryParseValueMethod();

        private delegate string ValueToStringFunc(T value, string format);

        private delegate bool TryParseValueFunc(
            string text, NumberStyles style, IFormatProvider provider, out T value);

        private static ValueToStringFunc CreateValueToStringMethod()
        {
            const string methodName = "ToString";

            var type = typeof(T);
            var methodInfo = type.GetMethod(methodName, false, false, typeof(string));
            var dynamicMethod = new DynamicMethod(type.FullName + "." + methodName,
                typeof(string), new[] { type, typeof(string) });

            var gen = dynamicMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarga_S, 0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, methodInfo);
            gen.Emit(OpCodes.Ret);

            return (ValueToStringFunc)dynamicMethod.CreateDelegate(typeof(ValueToStringFunc));
        }

        private static TryParseValueFunc CreateTryParseValueMethod()
        {
            Type type = typeof(T), typeRef = type.MakeByRefType();
            var methodInfo = type.GetMethod("TryParse", true, false,
                typeof(string), typeof(NumberStyles), typeof(IFormatProvider), typeRef);
            return (TryParseValueFunc)Delegate.CreateDelegate(typeof(TryParseValueFunc), methodInfo);
        }

        #endregion Value Type Methods

        private static void EnsureValueType()
        {
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    break;
                default:
                    throw new InvalidProgramException(string.Format("Integer type expected for 'T', got {0}.", type));
            }
        }

        static IntegerField() { EnsureValueType(); }
    }

    public sealed class SByteField : IntegerField<sbyte>
    {
        public SByteField() { }
        public SByteField(string label, sbyte value = default(sbyte)) : base(label, value) { }
    }

    public sealed class ByteField : IntegerField<byte>
    {
        public ByteField() { }
        public ByteField(string label, byte value = default(byte)) : base(label, value) { }
    }

    public sealed class ShortField : IntegerField<short>
    {
        public ShortField() { }
        public ShortField(string label, short value = default(short)) : base(label, value) { }
    }

    public sealed class UShortField : IntegerField<ushort>
    {
        public UShortField() { }
        public UShortField(string label, ushort value = default(ushort)) : base(label, value) { }
    }

    public sealed class IntField : IntegerField<int>
    {
        public IntField() { }
        public IntField(string label, int value = default(int)) : base(label, value) { }
    }

    public sealed class UIntField : IntegerField<uint>
    {
        public UIntField() { }
        public UIntField(string label, uint value = default(uint)) : base(label, value) { }
    }

    public sealed class LongField : IntegerField<long>
    {
        public LongField() { }
        public LongField(string label, long value = default(long)) : base(label, value) { }
    }

    public sealed class ULongField : IntegerField<ulong>
    {
        public ULongField() { }
        public ULongField(string label, ulong value = default(ulong)) : base(label, value) { }
    }
}