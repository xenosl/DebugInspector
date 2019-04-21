using System;

namespace ShuHai
{
    public class TargetTypesAttribute : Attribute
    {
        public Type[] Values;

        public TargetTypesAttribute(params Type[] values) { Values = values; }
    }
}