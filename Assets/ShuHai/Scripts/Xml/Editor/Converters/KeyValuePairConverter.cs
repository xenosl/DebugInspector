using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai.Xml
{
    public class KeyValuePairConverter : ObjectConverter
    {
        protected override int? GetPriorityImpl(Type type)
        {
            if (!type.IsGenericType)
                return null;
            return type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>) ? 0 : (int?)null;
        }

        protected override bool FilterMember(MemberInfo member)
        {
            if (!base.FilterMember(member))
                return false;
            return member is FieldInfo;
        }
    }
}