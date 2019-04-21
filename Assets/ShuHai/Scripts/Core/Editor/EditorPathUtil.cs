using System.IO;

namespace ShuHai.Editor
{
    public static class EditorPathUtil
    {
        /// <summary>
        ///     Convert an asset path to rooted path.
        /// </summary>
        /// <param name="assetPath"> Asset path to convert. </param>
        /// <returns> Equivalent rooted path to <paramref name="assetPath" />. </returns>
        /// <remarks>
        ///     This method does not check whether the argument <paramref name="assetPath" /> is a valid asset path, so if
        ///     it is not valid then the result path is invalid as well.
        /// </remarks>
        public static string ToRooted(string assetPath)
        {
            Ensure.Argument.NotNull(assetPath, "assetPath");
            return PathUtil.Normalize(Path.Combine(ProjectPaths.Root, assetPath));
        }
    }
}