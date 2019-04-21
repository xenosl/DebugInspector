using System;

namespace ShuHai.DebugInspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerStateTypeAttribute : TargetTypeAttribute
    {
        public DrawerStateTypeAttribute(Type value) : base(value) { }
    }
}