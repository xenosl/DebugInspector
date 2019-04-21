using System;

namespace ShuHai.DebugInspector.Editor
{
    public abstract class ValueEntry<TOwner, TValue> : IValueEntry<TOwner, TValue>
    {
        public TOwner Owner { get { return owner; } set { owner = value; } }

        public virtual Type TypeOfOwner { get { return typeof(TOwner); } }
        public virtual Type TypeOfValue { get { return typeof(TValue); } }

        public virtual bool CanRead { get { return true; } }
        public virtual bool CanWrite { get { return true; } }

        protected TOwner owner;

        protected ValueEntry(TOwner owner)
        {
            this.owner = owner;
            valueAccessHelper = new ValueAccesHelper<TValue>(() => Value, v => Value = v);
        }

        #region Value Access

        public abstract TValue Value { get; set; }

        public virtual ValueAccessResult GetValue(out TValue value, bool handleErrorLogs, bool catchException)
        {
            var result = valueAccessHelper.GetValue(out value);
            ProcessValueAccessError(result, handleErrorLogs, catchException);
            return result;
        }

        public virtual ValueAccessResult SetValue(TValue value, bool handleErrorLogs, bool catchException)
        {
            var result = valueAccessHelper.SetValue(value);
            ProcessValueAccessError(result, handleErrorLogs, catchException);
            return result;
        }

        protected ValueAccesHelper<TValue> valueAccessHelper;

        protected static void ProcessValueAccessError(
            ValueAccessResult result, bool handleErrorLogs, bool catchException)
        {
            if (!handleErrorLogs && result.HasErrorLog)
            {
                foreach (var info in result.ErrorLogs)
                    DebugEx.UnityLogger.Log(info.Type, info.Message);
            }

            if (!catchException && result.Exception != null)
                throw result.Exception;
        }

        #endregion

        #region Explicit Implementations

        private IValueEntry<TOwner, TValue> TBase { get { return this; } }

        object IValueEntry.Owner { get { return TBase.Owner; } set { TBase.Owner = (TOwner)value; } }
        object IValueEntry.Value { get { return TBase.Value; } set { TBase.Value = (TValue)value; } }

        ValueAccessResult IValueEntry.GetValue(out object value, bool handleErrorLogs, bool catchException)
        {
            TValue typedValue;
            var result = GetValue(out typedValue, handleErrorLogs, catchException);
            value = typedValue;
            return result;
        }

        ValueAccessResult IValueEntry.SetValue(object value, bool handleErrorLogs, bool catchException)
        {
            return SetValue((TValue)value, handleErrorLogs, catchException);
        }

        #endregion

        public override string ToString() { return GetType().GetReadableName(); }
    }

    /// <summary>
    ///     Provides methods to create value entry from generic type definition of <see cref="ValueEntry{TOwner, TValue}" />
    ///     and its type arguments.
    /// </summary>
    public static class ValueEntryFactory
    {
        /// <summary>
        ///     Create a value entry instance from its generic type definition, type arguments, and constructor arguments.
        /// </summary>
        /// <param name="entryTypeDef"> Generic type definition of the value entry. </param>
        /// <param name="typeOfValue"> Type arguments of the generic type definition. </param>
        /// <param name="args"> Constructor arguments of the constructed value entry type. </param>
        /// <returns> Newly created instance of the value entry. </returns>
        public static IValueEntry Create(Type entryTypeDef, Type typeOfValue, params object[] args)
        {
            var type = MakeType(entryTypeDef, typeOfValue);
            return (IValueEntry)Activator.CreateInstance(type, args);
        }

        /// <summary>
        ///     Make a constructed value entry type from its generic type definition and its generic type parameters.
        /// </summary>
        /// <param name="entryTypeDef"> Generic type definition of the value entry. </param>
        /// <param name="typeArg"> First generic type parameter of <paramref name="entryTypeDef" />. </param>
        /// <returns></returns>
        public static Type MakeType(Type entryTypeDef, Type typeArg)
        {
            Ensure.Argument.NotNull(entryTypeDef, "entryTypeDef");
            Ensure.Argument.Is<IValueEntry>(entryTypeDef, "entryTypeDef");
            return entryTypeDef.MakeGenericType(typeArg);
        }
    }
}