using System;
using System.Linq;
using System.Reflection;

namespace ShuHai.Xml
{
    /// <summary>
    ///     Provides access to built-in converter instances.
    /// </summary>
    public static class Converters
    {
        #region Defaults

        public static XConverter FindDefault(Type type)
        {
            Ensure.Argument.NotNull(type, "type");
            return defaults.FindByConvertType(type);
        }

        private static readonly ConverterCollection defaults = new ConverterCollection();

        private static void InitDefaults()
        {
            // Primitives
            defaults.Add(BooleanConverter.Instance);
            defaults.Add(CharConverter.Instance);
            defaults.Add(SByteConverter.Instance);
            defaults.Add(ByteConverter.Instance);
            defaults.Add(Int16Converter.Instance);
            defaults.Add(UInt16Converter.Instance);
            defaults.Add(Int32Converter.Instance);
            defaults.Add(UInt32Converter.Instance);
            defaults.Add(Int64Converter.Instance);
            defaults.Add(UInt64Converter.Instance);
            defaults.Add(SingleConverter.Instance);
            defaults.Add(DoubleConverter.Instance);
            defaults.Add(IntPtrConverter.Instance);
            defaults.Add(UIntPtrConverter.Instance);

            // Objects
            var objectConverters = typeof(XConverter)
                .EnumDerivedTypes(Assembly.GetExecutingAssembly())
                .Where(c => !c.IsAbstract && !c.IsSubclassOf(typeof(PrimitiveConverter)))
                .Select(Activator.CreateInstance)
                .Cast<XConverter>();
            defaults.AddRange(objectConverters);
        }

        #endregion

        static Converters() { InitDefaults(); }
    }
}