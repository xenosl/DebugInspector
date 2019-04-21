using System;

namespace ShuHai.DebugInspector.Editor
{
    internal static class DIEnsure
    {
        public static class Argument
        {
            public static void ValidParamForConstructing(Type type, string name)
            {
                if (!type.IsValidParamForConstructing())
                    throw new ArgumentException("Type is invalid as generic type parameter.", name);
            }
        }
    }
}