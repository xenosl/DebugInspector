using System.Collections.Generic;

namespace ShuHai
{
    public struct HashCode
    {
        public static implicit operator int(HashCode hc) { return hc.Value; }

        public int Value;

        public HashCode Add<T>(T obj, IEqualityComparer<T> comparer = null)
        {
            Value = Combine(Value, Get(obj, comparer));
            return this;
        }

        public static HashCode New<T>(T obj, IEqualityComparer<T> comparer = null)
        {
            return new HashCode { Value = Get(obj, comparer) };
        }

        #region Utilities

        public static int Calculate<T1, T2>(T1 v1, T2 v2) { return Combine(Get(v1), Get(v2)); }

        public static int Calculate<T1, T2, T3>(T1 v1, T2 v2, T3 v3) { return Combine(Get(v1), Get(v2), Get(v3)); }

        public static int Calculate<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
        {
            return Combine(Get(v1), Get(v2), Get(v3), Get(v4));
        }

        public static int Calculate<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            return Combine(Get(v1), Get(v2), Get(v3), Get(v4), Get(v5));
        }

        public static int Calculate<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
        {
            return Combine(Get(v1), Get(v2), Get(v3), Get(v4), Get(v5), Get(v6));
        }

        public static int Calculate<T>(IEnumerable<T> collection)
        {
            if (CollectionUtil.IsNullOrEmpty(collection))
                return 0;

            var list = collection as IList<T>;
            if (list != null)
            {
                int hash = Get(list[0]);
                for (int i = 1; i < list.Count; ++i)
                    hash = Combine(hash, Get(list[i]));
                return hash;
            }
            else
            {
                int hash = 0;
                foreach (var item in collection)
                    hash = Combine(hash, Get(item));
                return hash;
            }
        }

        public static int Get<T>(T value, IEqualityComparer<T> comparer = null)
        {
            return typeof(T).IsValueType
                ? GetImpl(value, comparer)
                : ((object)value == null ? 0 : GetImpl(value, comparer));
        }

        private static int GetImpl<T>(T value, IEqualityComparer<T> comparer = null)
        {
            return comparer != null ? comparer.GetHashCode(value) : value.GetHashCode();
        }

        #region Combine

        public static int Combine(int h1, int h2) { return ((h1 << 5) + h1) ^ h2; }

        public static int Combine(int h1, int h2, int h3) { return Combine(Combine(h1, h2), h3); }

        public static int Combine(int h1, int h2, int h3, int h4) { return Combine(Combine(h1, h2, h3), h4); }

        public static int Combine(int h1, int h2, int h3, int h4, int h5)
        {
            return Combine(Combine(h1, h2, h3, h4), h5);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return Combine(Combine(h1, h2, h3, h4, h5), h6);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return Combine(Combine(h1, h2, h3, h4, h5, h6), h7);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return Combine(Combine(h1, h2, h3, h4, h5, h6, h7), h8);
        }

        #endregion Combine

        #endregion Utilities
    }
}