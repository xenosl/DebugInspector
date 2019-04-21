using System;

namespace ShuHai.DebugInspector.Editor
{
    public class ValueAccesHelper<TValue>
    {
        public Func<TValue> ValueGetter;
        public Action<TValue> ValueSetter;

        public ValueAccesHelper() { }
        public ValueAccesHelper(Func<TValue> valueGetter) : this(valueGetter, null) { }
        public ValueAccesHelper(Action<TValue> valueSetter) : this(null, valueSetter) { }

        public ValueAccesHelper(Func<TValue> valueGetter, Action<TValue> valueSetter)
        {
            ValueGetter = valueGetter;
            ValueSetter = valueSetter;
        }

        public ValueAccessResult GetValue(out TValue value, TValue fallback = default(TValue))
        {
            ValueAccesHelper.EnsureValueAccessor(ValueGetter);

            var lh = ValueAccesHelper.PrepareLogHandler();
            Exception exception = null;

            using (new LogHandlerScope(lh))
            {
                try
                {
                    value = ValueGetter();
                }
                catch (Exception e)
                {
                    value = fallback;
                    exception = e;
                }
            }

            return new ValueAccessResult(lh.Records, exception);
        }

        public ValueAccessResult SetValue(TValue value)
        {
            ValueAccesHelper.EnsureValueAccessor(ValueSetter);

            var lh = ValueAccesHelper.PrepareLogHandler();
            Exception exception = null;

            using (new LogHandlerScope(lh))
            {
                try
                {
                    ValueSetter(value);
                }
                catch (Exception e)
                {
                    exception = e;
                }
            }

            return new ValueAccessResult(lh.Records, exception);
        }
    }

    public class ValueAccesHelper<TArg, TValue>
    {
        public Func<TArg, TValue> ValueGetter;
        public Action<TValue, TArg> ValueSetter;

        public ValueAccesHelper() { }
        public ValueAccesHelper(Func<TArg, TValue> valueGetter) : this(valueGetter, null) { }
        public ValueAccesHelper(Action<TValue, TArg> valueSetter) : this(null, valueSetter) { }

        public ValueAccesHelper(Func<TArg, TValue> valueGetter, Action<TValue, TArg> valueSetter)
        {
            ValueGetter = valueGetter;
            ValueSetter = valueSetter;
        }

        public ValueAccessResult GetValue(TArg arg, out TValue value, TValue fallback = default(TValue))
        {
            ValueAccesHelper.EnsureValueAccessor(ValueGetter);

            var lh = ValueAccesHelper.PrepareLogHandler();
            Exception exception = null;

            using (new LogHandlerScope(lh))
            {
                try
                {
                    value = ValueGetter(arg);
                }
                catch (Exception e)
                {
                    value = fallback;
                    exception = e;
                }
            }

            return new ValueAccessResult(lh.Records, exception);
        }

        public ValueAccessResult SetValue(TValue value, TArg arg)
        {
            ValueAccesHelper.EnsureValueAccessor(ValueSetter);

            var lh = ValueAccesHelper.PrepareLogHandler();
            Exception exception = null;

            using (new LogHandlerScope(lh))
            {
                try
                {
                    ValueSetter(value, arg);
                }
                catch (Exception e)
                {
                    exception = e;
                }
            }

            return new ValueAccessResult(lh.Records, exception);
        }
    }

    public class ValueAccesHelper<TArg1, TArg2, TValue>
    {
        public Func<TArg1, TArg2, TValue> ValueGetter;
        public Action<TValue, TArg1, TArg2> ValueSetter;

        public ValueAccesHelper() { }
        public ValueAccesHelper(Func<TArg1, TArg2, TValue> valueGetter) : this(valueGetter, null) { }
        public ValueAccesHelper(Action<TValue, TArg1, TArg2> valueSetter) : this(null, valueSetter) { }

        public ValueAccesHelper(Func<TArg1, TArg2, TValue> valueGetter, Action<TValue, TArg1, TArg2> valueSetter)
        {
            ValueGetter = valueGetter;
            ValueSetter = valueSetter;
        }

        public ValueAccessResult GetValue(TArg1 arg1, TArg2 arg2, out TValue value, TValue fallback = default(TValue))
        {
            ValueAccesHelper.EnsureValueAccessor(ValueGetter);

            var lh = ErrorRecordsLogHandler.Default;
            lh.ClearRecords();
            Exception exception = null;

            using (new LogHandlerScope(lh))
            {
                try
                {
                    value = ValueGetter(arg1, arg2);
                }
                catch (Exception e)
                {
                    value = fallback;
                    exception = e;
                }
            }

            return new ValueAccessResult(lh.Records, exception);
        }

        public ValueAccessResult SetValue(TValue value, TArg1 arg1, TArg2 arg2)
        {
            ValueAccesHelper.EnsureValueAccessor(ValueSetter);

            var lh = ValueAccesHelper.PrepareLogHandler();
            Exception exception = null;

            using (new LogHandlerScope(lh))
            {
                try
                {
                    ValueSetter(value, arg1, arg2);
                }
                catch (Exception e)
                {
                    exception = e;
                }
            }

            return new ValueAccessResult(lh.Records, exception);
        }
    }

    internal static class ValueAccesHelper
    {
        public static void EnsureValueAccessor<TGetter>(TGetter accessor)
        {
            if (accessor == null)
                throw new InvalidOperationException("Attempt to get value while accessor is null.");
        }

        public static ErrorRecordsLogHandler PrepareLogHandler()
        {
            var lh = ErrorRecordsLogHandler.Default;
            lh.ClearRecords();
            return lh;
        }
    }
}