using System.IO;
using ShuHai.Editor;

namespace ShuHai.DebugInspector.Editor
{
    public static class DIProjectPaths
    {
        public static readonly string DirectoryName = typeof(DIProjectPaths).Namespace.Split('.')[1];

        private static readonly string Suffix = '/' + DirectoryName;

        #region Library

        /// <summary>
        ///     The path of "Library/<see cref="ShuHai" />/<see cref="DebugInspector" />" folder in project root directory.
        /// </summary>
        public static string Library
        {
            get
            {
                if (library == null)
                    library = ShuHaiProjectPaths.Library + Suffix;
                return library;
            }
        }

        public static void CreateLibraryFolder() { Directory.CreateDirectory(Library); }

        private static string library;

        #endregion

        #region Settings

        /// <summary>
        ///     The path of "Settings/<see cref="ShuHai" />/<see cref="DebugInspector" />" folder in project root directory.
        /// </summary>
        public static string Settings
        {
            get
            {
                if (settings == null)
                    settings = ShuHaiProjectPaths.Settings + Suffix;
                return settings;
            }
        }

        public static void CreateSettingsFolder() { Directory.CreateDirectory(Settings); }

        private static string settings;

        #endregion

        #region Temp

        /// <summary>
        ///     The path of "Temp/<see cref="ShuHai" />/<see cref="DebugInspector" />" folder in project root directory.
        /// </summary>
        public static string Temp
        {
            get
            {
                if (temp == null)
                    temp = ShuHaiProjectPaths.Temp + Suffix;
                return temp;
            }
        }

        public static void CreateTempFolder() { Directory.CreateDirectory(Temp); }

        private static string temp;

        #endregion
    }
}