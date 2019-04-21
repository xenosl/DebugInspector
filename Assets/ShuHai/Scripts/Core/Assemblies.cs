using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ShuHai
{
    /// <summary>
    ///     A shortcut class for accessing various <see cref="Assembly" /> in unity project.
    /// </summary>
    public static class Assemblies
    {
        public const string UserRuntimeName = "Assembly-CSharp";
        public const string PluginRuntimeName = "Assembly-CSharp-firstpass";
        public const string UserEditorName = "Assembly-CSharp-Editor";
        public const string PluginEditorName = "Assembly-CSharp-Editor-firstpass";

        public static readonly Assembly Mscorlib = typeof(object).Assembly;
#if NETFX_CORE
        public static readonly Assembly UnityEngine = typeof(UnityEngine.Object).GetTypeInfo().Assembly;
#else
        public static readonly Assembly UnityEngine = typeof(UnityEngine.Object).Assembly;
#endif
        public static readonly Assembly UserRuntime = Find(UserRuntimeName);
        public static readonly Assembly PluginRuntime = Find(PluginRuntimeName);

#if UNITY_EDITOR
        public static readonly Assembly UnityEditor = typeof(UnityEditor.Editor).Assembly;
        public static readonly Assembly UserEditor = Find(UserEditorName);
        public static readonly Assembly PluginEditor = Find(PluginEditorName);
#endif

        /// <summary>
        ///     Enumeration of all loaded assemblies.
        /// </summary>
        public static IEnumerable<Assembly> All { get { return all; } }

        /// <summary>
        ///     Number of loaded assemblies.
        /// </summary>
        public static int Count { get { return all.Length; } }

        public static Assembly Find(string name) { return all.FirstOrDefault(a => a.GetName().Name == name); }

        /// <summary>
        ///     Searches for an element that matches the condition defined by <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate"> Condition to match. </param>
        /// <returns>
        ///     The first element that matches the condition defined by <paramref name="predicate" />, if found;
        ///     otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        public static Assembly Find(Func<Assembly, bool> predicate)
        {
            Ensure.Argument.NotNull(predicate, "predicate");
            return all.FirstOrDefault(predicate);
        }

        private static Assembly[] all
        {
            get
            {
                if (_all == null)
                    Reload();
                return _all;
            }
        }

        private static Assembly[] _all;

        /// <remarks>
        ///     Note that this method may called twice if <see cref="all" /> is accessed before all assembly is loaded. The
        ///     first one is called when accessing <see cref="all" /> and the second is here after all assemblies are loaded.
        ///     This behaviour is intended to make sure that all assemblies are loaded.
        /// </remarks>
        [RuntimeInitializeOnLoadMethod]
        private static void Reload() { _all = AppDomain.CurrentDomain.GetAssemblies(); }
    }
}