using System;
using System.Reflection;

namespace ShuHai.DebugInspector.Editor
{
    internal static class MemberInfoUtil
    {
        public static Type MemberTypeOf(MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.FieldType;
            var property = member as PropertyInfo;
            if (property != null)
                return property.PropertyType;
            var evt = member as EventInfo;
            if (evt != null)
                return evt.EventHandlerType;

            throw new NotSupportedException(
                string.Format(@"Get member type for ""{0}"" is not supported", member.GetType()));
        }
    }
}