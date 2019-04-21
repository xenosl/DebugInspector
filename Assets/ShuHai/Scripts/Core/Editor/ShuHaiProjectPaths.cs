namespace ShuHai.Editor
{
    /// <summary>
    ///     Provides access of project paths that written in <see cref="ShuHai" /> <see langword="namespace" />.
    /// </summary>
    public static class ShuHaiProjectPaths
    {
        public static readonly string DirectoryName = typeof(ShuHaiProjectPaths).Namespace.Split('.')[0];

        /// <summary>
        ///     The path of "Assets/<see cref="ShuHai" />" folder in project directory.
        /// </summary>
        public static string Assets { get { return assets ?? (assets = ProjectPaths.Assets + Suffix); } }

        private static string assets;

        /// <summary>
        ///     The path of "Library/<see cref="ShuHai" />" folder in project root directory.
        /// </summary>
        public static string Library { get { return library ?? (library = ProjectPaths.Library + Suffix); } }

        private static string library;

        /// <summary>
        ///     The path of "ProjectSettings/<see cref="ShuHai" />" folder in project root directory.
        /// </summary>
        public static string Settings
        {
            get
            {
                if (settings == null)
                    settings = ProjectPaths.Settings + Suffix;
                return settings;
            }
        }

        private static string settings;

        /// <summary>
        ///     The path of "Temp/<see cref="ShuHai" />" folder in project root directory.
        /// </summary>
        public static string Temp
        {
            get
            {
                if (temp == null)
                    temp = ProjectPaths.Temp + Suffix;
                return temp;
            }
        }

        private static string temp;

        private static readonly string Suffix = '/' + DirectoryName;
    }
}