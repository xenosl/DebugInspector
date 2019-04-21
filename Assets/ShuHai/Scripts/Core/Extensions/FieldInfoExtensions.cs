using System.Reflection;

namespace ShuHai
{
    public static class FieldInfoExtensions
    {
        public static bool IsProtected(this FieldInfo self)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.IsFamily || self.IsFamilyOrAssembly;
        }

        public static bool IsInternal(this FieldInfo self)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.IsAssembly || self.IsFamilyOrAssembly;
        }
    }
}