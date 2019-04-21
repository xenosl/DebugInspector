using System;
using System.Runtime.Serialization.Formatters;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    internal static class XConvertUtil
    {
        public static readonly string DefaultTypeAttributeName = typeof(Type).Name;

        public const string NullTypeName = "null";

        public static XAttribute CreateTypeAttribute(string name, Type type, FormatterAssemblyStyle? assemblyNameStyle)
        {
            return new XAttribute(name, TypeNameOf(type, assemblyNameStyle));
        }

        public static XAttribute CreateTypeAttribute(Type type, FormatterAssemblyStyle? assemblyNameStyle)
        {
            return CreateTypeAttribute(DefaultTypeAttributeName, type, assemblyNameStyle);
        }

        public static Type ParseType(XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            return ParseType(attribute);
        }

        public static Type ParseType(XElement element) { return ParseType(element, DefaultTypeAttributeName); }

        public static Type ParseType(XAttribute attribute)
        {
            return attribute != null ? TypeCache.GetType(attribute.Value) : null;
        }

        public static string TypeNameOf(Type type, FormatterAssemblyStyle? assemblyNameStyle)
        {
            return TypeName.Get(type).ToString(assemblyNameStyle);
        }
    }
}