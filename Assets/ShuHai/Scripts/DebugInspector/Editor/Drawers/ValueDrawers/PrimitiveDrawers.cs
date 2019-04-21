using System;
using ShuHai.Editor.IMGUI;
using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    [DrawerStateType(typeof(PrimitiveDrawerState))]
    internal abstract class PrimitiveDrawer<TOwner, TValue> : ValueDrawer<TOwner, TValue>
    {
        protected override bool childrenAvailable { get { return false; } }
    }

    internal sealed class BooleanDrawer<TOwner> : PrimitiveDrawer<TOwner, Boolean>
    {
        protected override void ValueGUIImpl() { typedDrawnValue = EditorGUILayout.Toggle(TypedDrawingValue); }
    }

    internal sealed class CharDrawer<TOwner> : PrimitiveDrawer<TOwner, Char>
    {
        protected override void ValueGUIImpl()
        {
            var valueString = EditorGUILayout.TextField(TypedDrawingValue.ToString());
            typedDrawnValue = string.IsNullOrEmpty(valueString) ? '\0' : valueString[0];
        }
    }

    #region Integer Drawers

    [DrawerStateType(typeof(IntegerDrawerState))]
    internal abstract class IntegerDrawer<TOwner, TValue> : PrimitiveDrawer<TOwner, TValue>
        where TValue : struct
    {
        protected readonly IntegerField<TValue> valueField = new IntegerField<TValue>();

        protected override void OnDrawingValueChanged(TValue oldValue)
        {
            base.OnDrawingValueChanged(oldValue);
            valueField.Value = TypedDrawingValue;
        }

        protected override void ValueGUIImpl() { typedDrawnValue = valueField.GUI(); }

        #region State

        protected override void PopulateState(DrawerState state)
        {
            base.PopulateState(state);

            var s = state as IntegerDrawerState;
            if (s == null)
                return;

            s.Hexadecimal = valueField.Hexadecimal;
        }

        public override void ApplyState(DrawerState state)
        {
            base.ApplyState(state);

            var s = state as IntegerDrawerState;
            if (s == null)
                return;

            valueField.Hexadecimal = s.Hexadecimal;
        }

        #endregion State
    }

    internal sealed class SByteDrawer<TOwner> : IntegerDrawer<TOwner, SByte> { }

    internal sealed class ByteDrawer<TOwner> : IntegerDrawer<TOwner, Byte> { }

    internal sealed class Int16Drawer<TOwner> : IntegerDrawer<TOwner, Int16> { }

    internal sealed class UInt16Drawer<TOwner> : IntegerDrawer<TOwner, UInt16> { }

    internal sealed class Int32Drawer<TOwner> : IntegerDrawer<TOwner, Int32> { }

    internal sealed class UInt32Drawer<TOwner> : IntegerDrawer<TOwner, UInt32> { }

    internal sealed class Int64Drawer<TOwner> : IntegerDrawer<TOwner, Int64> { }

    internal sealed class UInt64Drawer<TOwner> : IntegerDrawer<TOwner, UInt64> { }

    #endregion Integer Drawers

    internal sealed class SingleDrawer<TOwner> : PrimitiveDrawer<TOwner, Single>
    {
        protected override void ValueGUIImpl() { typedDrawnValue = EditorGUILayout.FloatField(TypedDrawingValue); }
    }

    internal sealed class DoubleDrawer<TOwner> : PrimitiveDrawer<TOwner, Double>
    {
        protected override void ValueGUIImpl() { typedDrawnValue = EditorGUILayout.DoubleField(TypedDrawingValue); }
    }

    internal sealed class IntPtrDrawer<TOwner> : PrimitiveDrawer<TOwner, IntPtr>
    {
        private readonly IntegerField<int> intField = new IntegerField<int> { Hexadecimal = true };
        private readonly IntegerField<long> longField = new IntegerField<long> { Hexadecimal = true };

        protected override void OnDrawingValueChanged(IntPtr oldValue)
        {
            base.OnDrawingValueChanged(oldValue);

            if (IntPtr.Size == 4)
                intField.Value = (int)TypedDrawingValue;
            else
                longField.Value = (long)TypedDrawingValue;
        }

        protected override void ValueGUIImpl()
        {
            typedDrawnValue = IntPtr.Size == 4 ? (IntPtr)intField.GUI() : (IntPtr)longField.GUI();
        }
    }

    internal sealed class UIntPtrDrawer<TOwner> : PrimitiveDrawer<TOwner, UIntPtr>
    {
        private readonly IntegerField<uint> uintField = new IntegerField<uint> { Hexadecimal = true };
        private readonly IntegerField<ulong> ulongField = new IntegerField<ulong> { Hexadecimal = true };

        protected override void OnDrawingValueChanged(UIntPtr oldValue)
        {
            base.OnDrawingValueChanged(oldValue);

            if (IntPtr.Size == 4)
                uintField.Value = (uint)TypedDrawingValue;
            else
                ulongField.Value = (ulong)TypedDrawingValue;
        }

        protected override void ValueGUIImpl()
        {
            typedDrawnValue = UIntPtr.Size == 4 ? (UIntPtr)uintField.GUI() : (UIntPtr)ulongField.GUI();
        }
    }
}