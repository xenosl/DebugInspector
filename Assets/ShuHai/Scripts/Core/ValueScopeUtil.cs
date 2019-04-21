using System;

namespace ShuHai
{
    internal static class ValueScopeUtil
    {
        public static void Deinitialize(ref bool initialized)
        {
            if (!initialized)
            {
                throw new InvalidOperationException(
                    "Unable to dispose while not initialized." +
                    " Don't create with default constructor since it dosen't initialize the scope." +
                    "(Defined a default constructor for a struct is not allowed)");
            }
            initialized = false;
        }
    }
}