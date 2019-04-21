using System;

namespace ShuHai
{
    public static class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 item1) { return new Tuple<T1>(item1); }

        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return new Tuple<T1, T2>(item1, item2); }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        //public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        //{
        //    return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        //}

        //public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        //{
        //    return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        //}

        //public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        //{
        //    return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        //}
    }

    [Serializable]
    public class Tuple<T1> : IEquatable<Tuple<T1>>
    {
        public T1 Item1;

        public Tuple(T1 item1) { Item1 = item1; }

        #region Equality

        public static bool operator ==(Tuple<T1> l, Tuple<T1> r) { return Equals(l, r); }
        public static bool operator !=(Tuple<T1> l, Tuple<T1> r) { return !Equals(l, r); }

        public override bool Equals(object obj) { return ((IEquatable<Tuple<T1>>)this).Equals(obj as Tuple<T1>); }

        bool IEquatable<Tuple<T1>>.Equals(Tuple<T1> other) { return other != null && Equals(Item1, other.Item1); }

        public override int GetHashCode() { return HashCode.Get(Item1); }

        #endregion Equality

        public override string ToString() { return "(" + Item1 + ")"; }
    }

    [Serializable]
    public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
    {
        public T1 Item1;
        public T2 Item2;

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        #region Equality

        public static bool operator ==(Tuple<T1, T2> l, Tuple<T1, T2> r) { return Equals(l, r); }
        public static bool operator !=(Tuple<T1, T2> l, Tuple<T1, T2> r) { return !Equals(l, r); }

        public override bool Equals(object obj)
        {
            return ((IEquatable<Tuple<T1, T2>>)this).Equals(obj as Tuple<T1, T2>);
        }

        bool IEquatable<Tuple<T1, T2>>.Equals(Tuple<T1, T2> other)
        {
            return other != null
                && Equals(Item1, other.Item1)
                && Equals(Item2, other.Item2);
        }

        public override int GetHashCode() { return HashCode.Calculate(Item1, Item2); }

        #endregion Equality

        public override string ToString() { return "(" + Item1 + ", " + Item2 + ")"; }
    }

    [Serializable]
    public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        #region Equality

        public static bool operator ==(Tuple<T1, T2, T3> l, Tuple<T1, T2, T3> r) { return Equals(l, r); }
        public static bool operator !=(Tuple<T1, T2, T3> l, Tuple<T1, T2, T3> r) { return !Equals(l, r); }

        public override bool Equals(object obj)
        {
            return ((IEquatable<Tuple<T1, T2, T3>>)this).Equals(obj as Tuple<T1, T2, T3>);
        }

        bool IEquatable<Tuple<T1, T2, T3>>.Equals(Tuple<T1, T2, T3> other)
        {
            return other != null
                && Equals(Item1, other.Item1)
                && Equals(Item2, other.Item2)
                && Equals(Item3, other.Item3);
        }

        public override int GetHashCode() { return HashCode.Calculate(Item1, Item2, Item3); }

        #endregion Equality

        public override string ToString() { return "(" + Item1 + ", " + Item2 + ", " + Item3 + ")"; }
    }

    [Serializable]
    public class Tuple<T1, T2, T3, T4> : IEquatable<Tuple<T1, T2, T3, T4>>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        #region Equality

        public static bool operator ==(Tuple<T1, T2, T3, T4> l, Tuple<T1, T2, T3, T4> r) { return Equals(l, r); }
        public static bool operator !=(Tuple<T1, T2, T3, T4> l, Tuple<T1, T2, T3, T4> r) { return !Equals(l, r); }

        public override bool Equals(object obj)
        {
            return ((IEquatable<Tuple<T1, T2, T3, T4>>)this).Equals(obj as Tuple<T1, T2, T3, T4>);
        }

        bool IEquatable<Tuple<T1, T2, T3, T4>>.Equals(Tuple<T1, T2, T3, T4> other)
        {
            return other != null
                && Equals(Item1, other.Item1)
                && Equals(Item2, other.Item2)
                && Equals(Item3, other.Item3)
                && Equals(Item4, other.Item4);
        }

        public override int GetHashCode() { return HashCode.Calculate(Item1, Item2, Item3, Item4); }

        #endregion Equality

        public override string ToString() { return "(" + Item1 + ", " + Item2 + ", " + Item3 + ", " + Item4 + ")"; }
    }
}