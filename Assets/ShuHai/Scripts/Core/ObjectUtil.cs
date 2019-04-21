using System;

namespace ShuHai
{
    public static class ObjectUtil
    {
        public static Type GetType(object obj) { return obj != null ? obj.GetType() : null; }
    }
}