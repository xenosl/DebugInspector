using System.IO;
using UnityEngine;

namespace ShuHai.Editor
{
    /// <summary>
    ///     Rooted paths to current unity project.
    /// </summary>
    public static class ProjectPaths
    {
        #region Paths

        /// <summary>
        ///     Root directory path of current project.
        /// </summary>
        public static string Root { get { return root ?? (root = Path.GetDirectoryName(Assets)); } }

        /// <summary>
        ///     The rooted path of "Assets" folder in project root directory.
        /// </summary>
        public static string Assets { get { return Application.dataPath; } }

        /// <summary>
        ///     The rooted path of "Library" folder in project root directory.
        /// </summary>
        public static string Library { get { return library ?? (library = Root + "/Library"); } }

        /// <summary>
        ///     The rooted path of "Library/ScriptAssemblies" folder in project root directory.
        /// </summary>
        public static string ScriptAssemblies
        {
            get { return scriptAssemblies ?? (scriptAssemblies = Library + "/ScriptAssemblies"); }
        }

        /// <summary>
        ///     The rooted path of "ProjectSettings" folder in project root directory.
        /// </summary>
        public static string Settings { get { return settings ?? (settings = Root + "/ProjectSettings"); } }

        /// <summary>
        ///     The rooted path of "Temp" folder in project root directory.
        /// </summary>
        public static string Temp { get { return temp ?? (temp = Root + "/Temp"); } }

        private static string root;
        private static string library;
        private static string scriptAssemblies;
        private static string settings;
        private static string temp;

        #endregion Paths

        #region Utilities

        /// <summary>
        ///     Get asset path of specified (rooted) path.
        /// </summary>
        public static string AssetOf(string path)
        {
            Ensure.Argument.NotNull(path, "path");

            path = PathUtil.Normalize(path);

            if (path.StartsWith("Assets"))
                return path;

            if (path.StartsWith(Assets))
                return path.Remove(0, Root.Length + 1);

            return string.Empty;
        }

        #endregion Utilities
    }
}