using System;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    /// <summary>
    ///     Provides common functionalities for primitive type convertion.
    /// </summary>
    internal abstract class PrimitiveConverter : XConverter
    {
        public abstract Type ConvertType { get; }

        protected sealed override int? GetPriorityImpl(Type type)
        {
            return type == ConvertType ? int.MaxValue : (int?)null;
        }

        protected override object CreateObject(Type objType, XElement element, XConvertSettings settings)
        {
            return ParseValue(element.Value);
        }

        protected abstract object ParseValue(string value);
    }

    internal sealed class BooleanConverter : PrimitiveConverter
    {
        public static readonly BooleanConverter Instance = new BooleanConverter();

        public override Type ConvertType { get { return typeof(Boolean); } }

        protected override object ParseValue(string value) { return Boolean.Parse(value); }

        private BooleanConverter() { }
    }

    internal sealed class CharConverter : PrimitiveConverter
    {
        public static readonly CharConverter Instance = new CharConverter();

        public override Type ConvertType { get { return typeof(Char); } }

        protected override object ParseValue(string value) { return Char.Parse(value); }

        private CharConverter() { }
    }

    internal sealed class SByteConverter : PrimitiveConverter
    {
        public static readonly SByteConverter Instance = new SByteConverter();

        public override Type ConvertType { get { return typeof(SByte); } }

        protected override object ParseValue(string value) { return SByte.Parse(value); }

        private SByteConverter() { }
    }

    internal sealed class ByteConverter : PrimitiveConverter
    {
        public static readonly ByteConverter Instance = new ByteConverter();

        public override Type ConvertType { get { return typeof(Byte); } }

        protected override object ParseValue(string value) { return Byte.Parse(value); }

        private ByteConverter() { }
    }

    internal sealed class Int16Converter : PrimitiveConverter
    {
        public static readonly Int16Converter Instance = new Int16Converter();

        public override Type ConvertType { get { return typeof(Int16); } }

        protected override object ParseValue(string value) { return Int16.Parse(value); }

        private Int16Converter() { }
    }

    internal sealed class UInt16Converter : PrimitiveConverter
    {
        public static readonly UInt16Converter Instance = new UInt16Converter();

        public override Type ConvertType { get { return typeof(UInt16); } }

        protected override object ParseValue(string value) { return UInt16.Parse(value); }

        private UInt16Converter() { }
    }

    internal sealed class Int32Converter : PrimitiveConverter
    {
        public static readonly Int32Converter Instance = new Int32Converter();

        public override Type ConvertType { get { return typeof(Int32); } }

        protected override object ParseValue(string value) { return Int32.Parse(value); }

        private Int32Converter() { }
    }

    internal sealed class UInt32Converter : PrimitiveConverter
    {
        public static readonly UInt32Converter Instance = new UInt32Converter();

        public override Type ConvertType { get { return typeof(UInt32); } }

        protected override object ParseValue(string value) { return UInt32.Parse(value); }

        private UInt32Converter() { }
    }

    internal sealed class Int64Converter : PrimitiveConverter
    {
        public static readonly Int64Converter Instance = new Int64Converter();

        public override Type ConvertType { get { return typeof(Int64); } }

        protected override object ParseValue(string value) { return Int64.Parse(value); }

        private Int64Converter() { }
    }

    internal sealed class UInt64Converter : PrimitiveConverter
    {
        public static readonly UInt64Converter Instance = new UInt64Converter();

        public override Type ConvertType { get { return typeof(UInt64); } }

        protected override object ParseValue(string value) { return UInt64.Parse(value); }

        private UInt64Converter() { }
    }

    internal sealed class SingleConverter : PrimitiveConverter
    {
        public static readonly SingleConverter Instance = new SingleConverter();

        public override Type ConvertType { get { return typeof(Single); } }

        protected override object ParseValue(string value) { return Single.Parse(value); }

        private SingleConverter() { }
    }

    internal sealed class DoubleConverter : PrimitiveConverter
    {
        public static readonly DoubleConverter Instance = new DoubleConverter();

        public override Type ConvertType { get { return typeof(Double); } }

        protected override object ParseValue(string value) { return Double.Parse(value); }

        private DoubleConverter() { }
    }

    internal sealed class IntPtrConverter : PrimitiveConverter
    {
        public static readonly IntPtrConverter Instance = new IntPtrConverter();

        public override Type ConvertType { get { return typeof(IntPtr); } }

        protected override object ParseValue(string value)
        {
            return IntPtr.Size == 4
                ? (IntPtr)Int32.Parse(value)
                : (IntPtr)Int64.Parse(value);
        }

        private IntPtrConverter() { }
    }

    internal sealed class UIntPtrConverter : PrimitiveConverter
    {
        public static readonly UIntPtrConverter Instance = new UIntPtrConverter();

        public override Type ConvertType { get { return typeof(UIntPtr); } }

        protected override object ParseValue(string value)
        {
            return UIntPtr.Size == 4
                ? (UIntPtr)UInt32.Parse(value)
                : (UIntPtr)UInt64.Parse(value);
        }

        private UIntPtrConverter() { }
    }
}