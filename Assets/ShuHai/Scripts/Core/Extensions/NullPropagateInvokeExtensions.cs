using System;

namespace ShuHai
{
    /// <summary>
    ///     Poor workaround for null-propagation operator in C# 6.
    /// </summary>
    public static class NullPropagateInvokeExtensions
    {
        #region Actions

        public static void NPInvoke(this Action self)
        {
            if (self != null)
                self();
        }

        public static void NPInvoke<T>(this Action<T> self, T arg)
        {
            if (self != null)
                self(arg);
        }

        public static void NPInvoke<T1, T2>(this Action<T1, T2> self, T1 a1, T2 a2)
        {
            if (self != null)
                self(a1, a2);
        }

        public static void NPInvoke<T1, T2, T3>(this Action<T1, T2, T3> self, T1 a1, T2 a2, T3 a3)
        {
            if (self != null)
                self(a1, a2, a3);
        }

        public static void NPInvoke<T1, T2, T3, T4>(
            this Action<T1, T2, T3, T4> self, T1 a1, T2 a2, T3 a3, T4 a4)
        {
            if (self != null)
                self(a1, a2, a3, a4);
        }

        //public static void NPInvoke<T1, T2, T3, T4, T5>(
        //    this Action<T1, T2, T3, T4, T5> self, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        //{
        //    if (self != null)
        //        self(a1, a2, a3, a4, a5);
        //}

        //public static void NPInvoke<T1, T2, T3, T4, T5, T6>(
        //    this Action<T1, T2, T3, T4, T5, T6> self, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        //{
        //    if (self != null)
        //        self(a1, a2, a3, a4, a5, a6);
        //}

        #endregion Actions

        #region Funcs

        public static TRet NPInvoke<TRet>(this Func<TRet> self, TRet fallback = default(TRet))
        {
            return self != null ? self() : fallback;
        }

        public static TRet NPInvoke<TArg, TRet>(
            this Func<TArg, TRet> self, TArg arg, TRet fallback = default(TRet))
        {
            return self != null ? self(arg) : fallback;
        }

        public static TRet NPInvoke<TArg1, TArg2, TRet>(
            this Func<TArg1, TArg2, TRet> self,
            TArg1 a1, TArg2 a2, TRet fallback = default(TRet))
        {
            return self != null ? self(a1, a2) : fallback;
        }

        public static TRet NPInvoke<TArg1, TArg2, TArg3, TRet>(
            this Func<TArg1, TArg2, TArg3, TRet> self,
            TArg1 a1, TArg2 a2, TArg3 a3, TRet fallback = default(TRet))
        {
            return self != null ? self(a1, a2, a3) : fallback;
        }

        public static TRet NPInvoke<TArg1, TArg2, TArg3, TArg4, TRet>(
            this Func<TArg1, TArg2, TArg3, TArg4, TRet> self,
            TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4, TRet fallback = default(TRet))
        {
            return self != null ? self(a1, a2, a3, a4) : fallback;
        }

        //public static TRet NPInvoke<TArg1, TArg2, TArg3, TArg4, TArg5, TRet>(
        //    this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TRet> self,
        //    TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4, TArg5 a5, TRet fallback = default(TRet))
        //{
        //    return self != null ? self(a1, a2, a3, a4, a5) : fallback;
        //}

        //public static TRet NPInvoke<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRet>(
        //    this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRet> self,
        //    TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4, TArg5 a5, TArg6 a6, TRet fallback = default(TRet))
        //{
        //    return self != null ? self(a1, a2, a3, a4, a5, a6) : fallback;
        //}

        #endregion Funcs

        public static bool NPInvoke<TRet>(this Predicate<TRet> self, TRet arg, bool fallback = false)
        {
            return self != null ? self(arg) : fallback;
        }
    }
}