using System;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    public class TypeConverter : XConverter
    {
        protected override int? GetPriorityImpl(Type type)
        {
            var root = typeof(Type);
            if (!root.IsAssignableFrom(type))
                return null;

            if (type == root)
                return 0;

            var fullName = type.FullName;
            if (fullName == "System.Reflection.TypeInfo")
                return 0;
            if (fullName == "System.MonoType" || // Actual type in Mono
                fullName == "System.RuntimeType") // Actual type in .Net CoreFX
                return 1;

            return null;
        }

        protected override void PopulateElementValue(
            XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            element.Value = obj != null
                ? XConvertUtil.TypeNameOf((Type)obj, settings.AssemblyNameStyle)
                : null;
        }

        protected override object CreateObject(Type objType, XElement element, XConvertSettings settings)
        {
            return element.IsEmpty ? null : TypeCache.GetType(element.Value, true);
        }
    }
}