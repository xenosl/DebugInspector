using System;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    internal sealed class StringConverter : XConverter
    {
        protected override int? GetPriorityImpl(Type type) { return type == typeof(String) ? int.MaxValue : (int?)null; }

        protected override object CreateObject(Type objType, XElement element, XConvertSettings settings)
        {
            return element.IsEmpty ? null : element.Value;
        }
    }
}