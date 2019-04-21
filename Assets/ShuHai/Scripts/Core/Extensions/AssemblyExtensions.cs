using System.Linq;
using System.Reflection;

namespace ShuHai
{
    public static class AssemblyExtensions
    {
        public static bool IsReferencedUnityEditorAssembly(this Assembly self)
        {
            //var editorAssemblyName = Assemblies.UnityEditor.GetName().Name; // Not available on WINDOWS_UWP
            const string editorAssemblyName = "UnityEditor";
            return self.GetName().Name == editorAssemblyName
                || self.GetReferencedAssemblies().Any(n => n.Name == editorAssemblyName);
        }
    }
}