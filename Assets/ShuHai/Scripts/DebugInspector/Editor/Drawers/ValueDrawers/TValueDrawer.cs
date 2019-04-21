using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ShuHai.Editor;
using UnityEditor;
using UnityEngine.Assertions;

namespace ShuHai.DebugInspector.Editor
{
    public class ValueDrawer<TOwner, TValue> : ValueDrawer
    {
        public override string ToString() { return string.Format("{0} ({1})", base.ToString(), Name); }

        #region Value Access

        public override Type DrawType { get { return typeof(TValue); } }

        public override bool CanDraw(IValueEntry valueEntry)
        {
            return base.CanDraw(valueEntry)
                && typeof(TOwner).IsAssignableFrom(valueEntry.TypeOfOwner)
                && typeof(TValue).IsAssignableFrom(valueEntry.TypeOfValue);
        }

        public ValueAccessResult GetValue(out TValue value, bool handleErrorLogs = true, bool catchException = true)
        {
            var entry = ValueEntry;
            if (entry == null)
                throw new InvalidOperationException("Unable to get value while ValueEntry does not exist.");
            return valueGetter(ref entry, out value, handleErrorLogs, catchException);
        }

        public ValueAccessResult SetValue(TValue value, bool handleErrorLogs = true, bool catchException = true)
        {
            var entry = ValueEntry;
            if (entry == null)
                throw new InvalidOperationException("Unable to get value while ValueEntry does not exist.");

            var result = valueSetter(ref entry, value, handleErrorLogs, catchException);
            AfterSetValue(result);
            return result;
        }

        private ValueGetter valueGetter;
        private ValueSetter valueSetter;

        protected override void OnValueEntryChanged(IValueEntry old)
        {
            valueGetter = ValueEntry != null ? CreateValueGetter(ValueEntry) : null;
            valueSetter = ValueEntry != null ? CreateValueSetter(ValueEntry) : null;

            base.OnValueEntryChanged(old);
        }

        #region Comaprison

        public new IEqualityComparer<TValue> ValueComparer;

        protected bool ValueEquals(TValue l, TValue r)
        {
            return ValueComparer != null ? ValueComparer.Equals(l, r) : Equals(l, r);
        }

        #endregion Comaprison

        #region Before Draw

        public override object DrawingValue
        {
            get { return TypedDrawingValue; }
            protected set { TypedDrawingValue = value != null ? (TValue)value : default(TValue); }
        }

        /// <summary>
        ///     Strongly typed <see cref="DrawingValue" />.
        /// </summary>
        public virtual TValue TypedDrawingValue
        {
            get { return typedDrawingValue; }
            protected set
            {
                if (ValueEquals(typedDrawingValue, value))
                    return;

                var old = typedDrawingValue;
                typedDrawingValue = value;
                OnDrawingValueChanged(old);
            }
        }

        private TValue typedDrawingValue;

        protected sealed override void OnDrawingValueChanged(object oldValue)
        {
            // Nothing to do...
            // See OnDrawingValueChanged(TValue).
        }

        protected virtual void OnDrawingValueChanged(TValue oldValue)
        {
            var newValue = TypedDrawingValue;
            Type oldType = ObjectUtil.GetType(oldValue), newType = ObjectUtil.GetType(newValue);
            bool typeChanged = oldType != newType;

            if (typeChanged)
            {
                interfaceDrawersNeedUpdate = true;
                memberGroupInstancesNeedUpdate = true;
            }
            memberGroupContentsNeedUpdate = true;

            UpdateReferencedParent();
        }

        protected override void UpdateDrawingValue()
        {
            if (ValueEntry != null && ValueEntry.CanRead)
            {
                TValue value;
                drawingValueGetResult = GetValue(out value, true, true);
                TypedDrawingValue = value;
            }
            else
            {
                drawingValueGetResult = null;
                TypedDrawingValue = default(TValue);
            }
        }

        #endregion Before Draw

        #region After Draw

        protected override object drawnValue
        {
            get { return typedDrawnValue; }
            set { typedDrawnValue = value != null ? (TValue)value : default(TValue); }
        }

        protected TValue typedDrawnValue
        {
            get { return _typedDrawnValue; }
            set
            {
                if (ValueEquals(_typedDrawnValue, value))
                    return;
                _typedDrawnValue = value;
                drawnValueNeedApply = true;
            }
        }

        private TValue _typedDrawnValue;

        protected override bool ApplyDrawnValue()
        {
            if (!drawnValueNeedApply || !CanSetValue)
                return false;

            drawnValueSetResult = SetValue(typedDrawnValue, true, true);
            if (drawnValueSetResult.Value)
                UpdateDrawingValue();

            drawnValueNeedApply = false;
            return true;
        }

        #endregion After Draw

        #region Emits

        private delegate ValueAccessResult ValueGetter(
            ref IValueEntry @this, out TValue value, bool handleErrorLogs, bool catchException);

        private delegate ValueAccessResult ValueSetter(
            ref IValueEntry @this, TValue value, bool handleErrorLogs, bool catchException);

        private static ValueGetter CreateValueGetter(IValueEntry valueEntry)
        {
            var entryTypeInfo = ValueEntryTypeInfo.Get(valueEntry.GetType());
            var entryType = entryTypeInfo.Type;
            var entryValueType = entryTypeInfo.RootGenericArguments[1];
            var methodInfo = entryType.GetMethod("GetValue", false, false,
                entryValueType.MakeByRefType(), typeof(bool), typeof(bool));
            Assert.IsNotNull(methodInfo);

            // NOTE: This is a WRONG approach since valueType may different from TValue of current instance.
            //return (ValueGetter)Delegate.CreateDelegate(typeof(ValueGetter), method);

            var drawerValueType = typeof(TValue);
            Assert.IsTrue(entryValueType.IsAssignableFrom(drawerValueType));
            var parameterTypes = new[]
                { typeof(IValueEntry).MakeByRefType(), drawerValueType.MakeByRefType(), typeof(bool), typeof(bool) };
            var dynamicMethod = new DynamicMethod(
                entryType.FullName + ".GetValue", typeof(ValueAccessResult), parameterTypes, entryType);

            var gen = dynamicMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0); // 0:entry
            gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Castclass, entryType); // 0:typedEntry
            if (drawerValueType == entryValueType)
            {
                /* Example Implementation:
                static ValueAccessResult ViewILGet(
                    ref IValueEntry valueEntry, out int value, bool handleErrorLogs, bool catchException)
                {
                    return ((FixedValueEntry<GameObject, int>)valueEntry)
                        .GetValue(out value, handleErrorLogs, catchException);
                }
                */
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldarg_3);
                gen.Emit(OpCodes.Callvirt, methodInfo); // 0:result
            }
            else
            {
                /* Example Implementation:
                static ValueAccessResult ViewILGet(ref IValueEntry valueEntry,
                    out Component value, bool handleErrorLogs, bool catchException)
                {
                    Transform entryValue;
                    var result = ((FixedValueEntry<GameObject, Transform>)valueEntry)
                        .GetValue(out entryValue, handleErrorLogs, catchException);
                    value = entryValue;
                    return result;
                }
                */
                var valueInfo = gen.DeclareLocal(entryValueType);
                //var resultInfo = gen.DeclareLocal(typeof(ValueAccessResult));
                gen.Emit(OpCodes.Ldloca_S, valueInfo.LocalIndex); // 0:typedEntry 1:entryValue
                gen.Emit(OpCodes.Ldarg_2); // 0:typedEntry 1:entryValue 2:handleErrorLogs
                gen.Emit(OpCodes.Ldarg_3); // 0:typedEntry 1:entryValue 2:handleErrorLogs 3:catchException
                gen.Emit(OpCodes.Callvirt, methodInfo); // 0:result
                gen.Emit(OpCodes.Ldarg_1); // 0:result 1:value
                gen.Emit(OpCodes.Ldloc, valueInfo.LocalIndex); // 0:result 1:value 2:entryValue
                gen.Emit(OpCodes.Stind_Ref); // value = entryValue 0:result
            }
            gen.Emit(OpCodes.Ret);

            return (ValueGetter)dynamicMethod.CreateDelegate(typeof(ValueGetter));
        }

        private static ValueSetter CreateValueSetter(IValueEntry valueEntry)
        {
            var entryTypeInfo = ValueEntryTypeInfo.Get(valueEntry.GetType());
            var entryType = entryTypeInfo.Type;
            var entryValueType = entryTypeInfo.RootGenericArguments[1];
            var methodInfo = entryType.GetMethod("SetValue", false, false, entryValueType, typeof(bool), typeof(bool));
            Assert.IsNotNull(methodInfo);

            var drawerValueType = typeof(TValue);
            Assert.IsTrue(entryValueType.IsAssignableFrom(drawerValueType));
            var parameterTypes = new[]
                { typeof(IValueEntry).MakeByRefType(), drawerValueType, typeof(bool), typeof(bool) };
            var dynamicMethod = new DynamicMethod(
                entryType.FullName + ".SetValue", typeof(ValueAccessResult), parameterTypes, entryType);

            /* Example Implementation:
            static ValueAccessResult SetValue(ref IValueEntry valueEntry,
                Component value, bool handleErrorLogs, bool catchException)
            {
                return ((FixedValueEntry<GameObject, Transform>)valueEntry)
                    .SetValue((Transform)value, handleErrorLogs, catchException);
            }
            */
            var gen = dynamicMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0); // 0:entry
            gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Castclass, entryType); // 0:typedEntry
            gen.Emit(OpCodes.Ldarg_1); // 0:typedEntry 1:drawerTypeValue
            gen.EmitTypeCast(drawerValueType, entryValueType); // 0:typedEntry 1:entryValueType
            gen.Emit(OpCodes.Ldarg_2); // 0:typedEntry 1:entryTypeValue 2:handleErrorLogs
            gen.Emit(OpCodes.Ldarg_3); // 0:typedEntry 1:entryTypeValue 2:handleErrorLogs 3:catchException
            gen.Emit(OpCodes.Callvirt, methodInfo); // 0:result
            gen.Emit(OpCodes.Ret);

            return (ValueSetter)dynamicMethod.CreateDelegate(typeof(ValueSetter));
        }

        #endregion Emits

        #endregion Value Access

        #region GUI

        protected override void ValueGUIImpl()
        {
            valueGUIContent.text = TypedDrawingValue != null ? TypedDrawingValue.ToString() : "(null)";
            EditorGUILayout.LabelField(valueGUIContent);

            typedDrawnValue = TypedDrawingValue;
        }

        #endregion GUI

        protected override bool childrenAvailable
        {
            get
            {
                if (!typeof(TValue).IsValueType && TypedDrawingValue == null)
                    return false;
                return base.childrenAvailable;
            }
        }
    }
}