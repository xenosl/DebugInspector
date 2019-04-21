using System.IO;
using UnityEditor;

namespace ShuHai.Editor
{
    using UObject = UnityEngine.Object;

    /// <summary>
    ///     Provides access for internal assets of <see cref="ShuHai" /> project.
    /// </summary>
    public static class Assets
    {
        /// <summary>
        ///     Root folder path of the <see cref="ShuHai" /> assets.
        /// </summary>
        public static string RootPath
        {
            get
            {
                if (rootPath != null)
                    return rootPath;
                rootPath = "Assets/" + typeof(Assets).Namespace.Split('.')[0];
                return rootPath;
            }
        }

        private static string rootPath;

        /// <summary>
        ///     Load asset in <see cref="ShuHai" /> folder.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">
        ///     Path of the asset to load.
        ///     Note that the path is relative to <see cref="RootPath" />, for example: "SomeFolder/SomeImage.png" actually
        ///     loads asset at <see cref="RootPath" /> + "/SomeFolder/SomeImage.png".
        /// </param>
        /// <returns>The asset at given <paramref name="path" />.</returns>
        public static T Load<T>(string path)
            where T : UObject
        {
            Ensure.Argument.NotNull(path, "path");

            var assetPath = PathUtil.Normalize(Path.Combine(RootPath, path));
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        /// <summary>
        ///     Get asset reference if exists or load if not.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="storeField"> Where the asset reference is stored.</param>
        /// <param name="path">Path of the asset to load.</param>
        /// <returns><paramref name="storeField" /> or asset at given <paramref name="path" />.</returns>
        public static T GetOrLoad<T>(ref T storeField, string path)
            where T : UObject
        {
            if (!storeField)
                storeField = Load<T>(path);
            return storeField;
        }
    }
}