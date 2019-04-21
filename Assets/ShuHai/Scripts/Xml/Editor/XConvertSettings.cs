using System;
using System.Runtime.Serialization.Formatters;

namespace ShuHai.Xml
{
    public class XConvertSettings
    {
        public static readonly XConvertSettings Default = new XConvertSettings();

        /// <summary>
        ///     Controls how assembly names are written during converting.
        /// </summary>
        public FormatterAssemblyStyle? AssemblyNameStyle = FormatterAssemblyStyle.Simple;

        /// <summary>
        ///     Controls whether converts includes type information as xml attribute during converting from object to xml object.
        /// </summary>
        public TypeAttributeHandling TypeAttributeHandling;

        #region Converters

        /// <summary>
        ///     Converters that are used during converting.
        /// </summary>
        public ConverterCollection Converters;

        /// <summary>
        ///     Get converter instance from <see cref="Converters" /> list by convert type, or default converter if the instance
        ///     does not exist.
        /// </summary>
        /// <param name="convertType">Type to convert.</param>
        /// <returns>
        ///     <see cref="XConverter" /> instance that converts objects of type <paramref name="convertType" /> in
        ///     <see cref="Converters" /> list or default converter if not found.
        /// </returns>
        public XConverter FindConverter(Type convertType)
        {
            return Converters != null
                ? Converters.FindByConvertType(convertType) ?? Xml.Converters.FindDefault(convertType)
                : Xml.Converters.FindDefault(convertType);
        }

        #endregion
    }

    public enum TypeAttributeHandling
    {
        /// <summary>
        ///     Only write type attribute if needed during converting.
        /// </summary>
        Auto,

        /// <summary>
        ///     Write type attribute for reference types.
        /// </summary>
        Objects,

        /// <summary>
        ///     Write type attribute for all types.
        /// </summary>
        All
    }
}