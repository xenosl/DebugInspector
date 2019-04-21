using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ShuHai
{
    public enum LazyExecutionMode
    {
        NotThreadSafe = 0,
        AllowMultipleThreadSafeExecution = 1,
        EnsureSingleThreadSafeExecution = 2
    }

    [Serializable]
    [ComVisible(false)]
    public class Lazy<T>
    {
        private T value;
        private bool inited;
        private LazyExecutionMode mode;
        private Func<T> valueFactory;
        private object monitor;

        public Lazy() : this(LazyExecutionMode.NotThreadSafe) { }

        public Lazy(Func<T> valueFactory) : this(valueFactory, LazyExecutionMode.NotThreadSafe) { }

        public Lazy(LazyExecutionMode mode)
        {
            this.mode = mode;
            if (mode != LazyExecutionMode.NotThreadSafe)
                monitor = new Object();
        }

        public Lazy(Func<T> valueFactory, LazyExecutionMode mode)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");
            this.valueFactory = valueFactory;
            this.mode = mode;
            if (mode != LazyExecutionMode.NotThreadSafe)
                monitor = new Object();
        }

        public override string ToString()
        {
            if (!IsValueCreated)
                return "Value is not created";
            return Value.ToString();
        }

        public T Value
        {
            get
            {
                if (inited)
                    return value;

                return InitValue();
            }
        }

        private T InitValue()
        {
            if (mode == LazyExecutionMode.NotThreadSafe)
            {
                value = CreateValue();
                inited = true;
            }
            else
            {
                // We treat AllowMultipleThread... as EnsureSingleThread...
                lock (monitor)
                {
                    if (inited)
                        return value;
                    T v = CreateValue();
                    value = v;
                    Thread.MemoryBarrier();
                    inited = true;
                }
            }

            return value;
        }

        private T CreateValue()
        {
            if (valueFactory == null)
            {
                try
                {
                    return Activator.CreateInstance<T>();
                }
                catch (MissingMethodException)
                {
                    throw new MissingMemberException(
                        "The lazily-initialized type does not have a public, parameterless constructor.");
                }
            }
            else
            {
                return valueFactory();
            }
        }

        public bool IsValueCreated { get { return inited; } }
    }
}