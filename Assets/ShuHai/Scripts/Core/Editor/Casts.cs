using System;

namespace ShuHai.Editor
{
    public static class Cast<TIn, TOut>
    {
        public static readonly Converter<TIn, TOut> ChangeType = CommonMethodsEmitter.CreateCast<TIn, TOut>();
    }

    public static class EnumCast
    {
        public static Enum ToEnum<T>(T value) { return Impl<T>.ToEnum(value); }
        public static T FromEnum<T>(Enum value) { return Impl<T>.FromEnum(value); }

        public static SByte ToSByte<T>(T value) { return Impl<T>.ToSByte(value); }
        public static Byte ToByte<T>(T value) { return Impl<T>.ToByte(value); }
        public static Int16 ToInt16<T>(T value) { return Impl<T>.ToInt16(value); }
        public static UInt16 ToUInt16<T>(T value) { return Impl<T>.ToUInt16(value); }
        public static Int32 ToInt32<T>(T value) { return Impl<T>.ToInt32(value); }
        public static UInt32 ToUInt32<T>(T value) { return Impl<T>.ToUInt32(value); }
        public static Int64 ToInt64<T>(T value) { return Impl<T>.ToInt64(value); }
        public static UInt64 ToUInt64<T>(T value) { return Impl<T>.ToUInt64(value); }

        private static class Impl<T>
        {
            public static readonly Converter<T, Enum> ToEnum = CommonMethodsEmitter.CreateCast<T, Enum>();
            public static readonly Converter<Enum, T> FromEnum = CommonMethodsEmitter.CreateCast<Enum, T>();

            public static readonly Converter<T, SByte> ToSByte = CommonMethodsEmitter.CreateCast<T, SByte>();
            public static readonly Converter<T, Byte> ToByte = CommonMethodsEmitter.CreateCast<T, Byte>();
            public static readonly Converter<T, Int16> ToInt16 = CommonMethodsEmitter.CreateCast<T, Int16>();
            public static readonly Converter<T, UInt16> ToUInt16 = CommonMethodsEmitter.CreateCast<T, UInt16>();
            public static readonly Converter<T, Int32> ToInt32 = CommonMethodsEmitter.CreateCast<T, Int32>();
            public static readonly Converter<T, UInt32> ToUInt32 = CommonMethodsEmitter.CreateCast<T, UInt32>();
            public static readonly Converter<T, Int64> ToInt64 = CommonMethodsEmitter.CreateCast<T, Int64>();
            public static readonly Converter<T, UInt64> ToUInt64 = CommonMethodsEmitter.CreateCast<T, UInt64>();

            static Impl()
            {
                var type = typeof(T);
                if (!type.IsEnum)
                    throw new NotSupportedException("enum type expected.");
            }
        }
    }
}