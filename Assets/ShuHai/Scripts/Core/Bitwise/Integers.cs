using System;

namespace ShuHai.Bitwise
{
    public static class Integers
    {
        #region SByte

        public static bool HasFlag(this SByte self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this SByte self, Byte flag) { return (self & flag) == flag; }

        #endregion

        #region Byte

        public static bool HasFlag(this Byte self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Byte self, Byte flag) { return (self & flag) == flag; }

        #endregion

        #region Int16

        public static bool HasFlag(this Int16 self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int16 self, Int16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int16 self, Byte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int16 self, UInt16 flag) { return (self & flag) == flag; }

        #endregion

        #region UInt16

        public static bool HasFlag(this UInt16 self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt16 self, Int16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt16 self, Byte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt16 self, UInt16 flag) { return (self & flag) == flag; }

        #endregion

        #region Int32

        public static bool HasFlag(this Int32 self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int32 self, Int16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int32 self, Int32 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int32 self, Byte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int32 self, UInt16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int32 self, UInt32 flag) { return (self & flag) == flag; }

        #endregion

        #region UInt32

        public static bool HasFlag(this UInt32 self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt32 self, Int16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt32 self, Int32 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt32 self, Byte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt32 self, UInt16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt32 self, UInt32 flag) { return (self & flag) == flag; }

        #endregion

        #region Int64

        public static bool HasFlag(this Int64 self, SByte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int64 self, Int16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int64 self, Int32 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int64 self, Int64 flag) { return (self & flag) == flag; }

        public static bool HasFlag(this Int64 self, Byte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int64 self, UInt16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this Int64 self, UInt32 flag) { return (self & flag) == flag; }

        public static bool HasFlag(this Int64 self, UInt64 flag)
        {
            var sflag = unchecked((Int64)flag);
            return (self & sflag) == sflag;
        }

        #endregion

        #region UInt64

        public static bool HasFlag(this UInt64 self, SByte flag)
        {
            var uflag = unchecked((Byte)flag);
            return (self & uflag) == uflag;
        }

        public static bool HasFlag(this UInt64 self, Int16 flag)
        {
            var uflag = unchecked((UInt16)flag);
            return (self & uflag) == uflag;
        }

        public static bool HasFlag(this UInt64 self, Int32 flag)
        {
            var uflag = unchecked((UInt32)flag);
            return (self & uflag) == uflag;
        }

        public static bool HasFlag(this UInt64 self, Int64 flag)
        {
            var uflag = unchecked((UInt64)flag);
            return (self & uflag) == uflag;
        }

        public static bool HasFlag(this UInt64 self, Byte flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt64 self, UInt16 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt64 self, UInt32 flag) { return (self & flag) == flag; }
        public static bool HasFlag(this UInt64 self, UInt64 flag) { return (self & flag) == flag; }

        public static UInt64 AddFlag(this UInt64 self, SByte flag) { return self | unchecked((Byte)flag); }
        public static UInt64 AddFlag(this UInt64 self, Int16 flag) { return self | unchecked((UInt16)flag); }
        public static UInt64 AddFlag(this UInt64 self, Int32 flag) { return self | unchecked((UInt32)flag); }
        public static UInt64 AddFlag(this UInt64 self, Int64 flag) { return self | unchecked((UInt64)flag); }
        public static UInt64 AddFlag(this UInt64 self, Byte flag) { return self | flag; }
        public static UInt64 AddFlag(this UInt64 self, UInt16 flag) { return self | flag; }
        public static UInt64 AddFlag(this UInt64 self, UInt32 flag) { return self | flag; }
        public static UInt64 AddFlag(this UInt64 self, UInt64 flag) { return self | flag; }

        public static UInt64 RemoveFlag(this UInt64 self, SByte flag) { return self & unchecked((Byte)~flag); }
        public static UInt64 RemoveFlag(this UInt64 self, Int16 flag) { return self & unchecked((UInt16)~flag); }
        public static UInt64 RemoveFlag(this UInt64 self, Int32 flag) { return self & unchecked((UInt32)~flag); }
        public static UInt64 RemoveFlag(this UInt64 self, Int64 flag) { return self & unchecked((UInt64)~flag); }
        public static UInt64 RemoveFlag(this UInt64 self, Byte flag) { return self & unchecked((Byte)~flag); }
        public static UInt64 RemoveFlag(this UInt64 self, UInt16 flag) { return self & unchecked((UInt16)~flag); }
        public static UInt64 RemoveFlag(this UInt64 self, UInt32 flag) { return self & ~flag; }
        public static UInt64 RemoveFlag(this UInt64 self, UInt64 flag) { return self & ~flag; }

        #endregion
    }
}