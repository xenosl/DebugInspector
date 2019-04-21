using System;

namespace ShuHai
{
    public class TargetTypeAttribute : Attribute
    {
        public Type Value;

        public TargetTypeAttribute(Type value) { Value = value; }
    }
}