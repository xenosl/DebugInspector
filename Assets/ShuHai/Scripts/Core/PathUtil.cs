using System;
using System.IO;
using System.Text;

namespace ShuHai
{
    public static class PathUtil
    {
        /// <summary>
        ///     See <see cref="Path.DirectorySeparatorChar" />, but this one is specialized for Unity.
        /// </summary>
        public static readonly char DirectorySeparatorChar = '/';

        /// <summary>
        ///     See <see cref="Path.AltDirectorySeparatorChar" />, but this one is specialized for Unity.
        /// </summary>
        public static readonly char AltDirectorySeparatorChar = '\\';

        /// <summary>
        ///     Convert all backslashes('\') to forward slashes('/') and remove all redundant slashes.
        /// </summary>
        /// <param name="path"> The path to convert. </param>
        /// <returns> A normalized path which is ready to use in Unity. </returns>
        public static string Normalize(string path)
        {
            Ensure.Argument.NotNull(path, "path");

            var sb = new StringBuilder(path.Length);
            var len = path.Length;
            for (int i = 0; i < len; ++i)
            {
                var c = path[i];
                if (c == AltDirectorySeparatorChar)
                    c = DirectorySeparatorChar;
                int sbl = sb.Length;
                if (c == DirectorySeparatorChar && sbl > 0 && sb[sbl - 1] == DirectorySeparatorChar)
                    continue;
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Get the filename or directory name of the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to obtain the name.</param>
        public static string NameOf(string path)
        {
            var trimmedPath = path.Trim(separators);
            int index = trimmedPath.LastIndexOfAny(separators);
            if (index < 0)
            {
                if (trimmedPath.Length > 0 && trimmedPath[trimmedPath.Length - 1] == VolumeSeparatorChar)
                    return string.Empty;
                return trimmedPath;
            }
            return trimmedPath.Substring(index + 1);
        }

        /// <summary>
        ///     Get the parent directory name of the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to obtain the name.</param>
        public static string ParentOf(string path)
        {
            int index = path.Trim(separators).LastIndexOfAny(separators);
            if (index >= 0)
            {
                if (index > 0 && path[index - 1] == VolumeSeparatorChar)
                    index--;
                return path.Remove(index);
            }
            return string.Empty;
        }

        private static readonly char[] separators = { DirectorySeparatorChar, AltDirectorySeparatorChar };

        // Codes that copy from .Net reference source with slightly modification for use in Unity.
        // This is mainly for the method <see cref="Combine(string[])" />, since .Net3.5 which Unity is using does not
        // contain this method.

        #region Copy From System.IO.Path

        public static readonly char VolumeSeparatorChar = Path.VolumeSeparatorChar;

        public static string Combine(string path1, string path2)
        {
            if (path1 == null || path2 == null)
                throw new ArgumentNullException(path1 == null ? "path1" : "path2");

            CheckInvalidPathChars(path1);
            CheckInvalidPathChars(path2);
            return CombineNoChecks(path1, path2);
        }

        public static string Combine(string path1, string path2, string path3)
        {
            if (path1 == null || path2 == null || path3 == null)
            {
                throw new ArgumentNullException(
                    path1 == null ? "path1" : path2 == null ? "path2" : "path3");
            }
            CheckInvalidPathChars(path1);
            CheckInvalidPathChars(path2);
            CheckInvalidPathChars(path3);
            return CombineNoChecks(CombineNoChecks(path1, path2), path3);
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            if (path1 == null || path2 == null || path3 == null || path4 == null)
            {
                throw new ArgumentNullException(
                    path1 == null ? "path1" : path2 == null ? "path2" : path3 == null ? "path3" : "path4");
            }
            CheckInvalidPathChars(path1);
            CheckInvalidPathChars(path2);
            CheckInvalidPathChars(path3);
            CheckInvalidPathChars(path4);
            return CombineNoChecks(CombineNoChecks(CombineNoChecks(path1, path2), path3), path4);
        }

        public static string Combine(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            int finalSize = 0;
            int firstComponent = 0;

            // We have two passes, the first calcuates how large a buffer to allocate and does some precondition
            // checks on the paths passed in.  The second actually does the combination.
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null)
                    throw new ArgumentNullException("paths");

                if (paths[i].Length == 0)
                    continue;

                CheckInvalidPathChars(paths[i]);

                if (Path.IsPathRooted(paths[i]))
                {
                    firstComponent = i;
                    finalSize = paths[i].Length;
                }
                else
                {
                    finalSize += paths[i].Length;
                }

                char ch = paths[i][paths[i].Length - 1];
                if (ch != DirectorySeparatorChar && ch != AltDirectorySeparatorChar && ch != VolumeSeparatorChar)
                    finalSize++;
            }

            var finalPath = new StringBuilder(finalSize);
            for (int i = firstComponent; i < paths.Length; i++)
            {
                if (paths[i].Length == 0)
                    continue;

                if (finalPath.Length == 0)
                {
                    finalPath.Append(paths[i]);
                }
                else
                {
                    char ch = finalPath[finalPath.Length - 1];
                    if (ch != DirectorySeparatorChar && ch != AltDirectorySeparatorChar && ch != VolumeSeparatorChar)
                    {
                        finalPath.Append(DirectorySeparatorChar);
                    }

                    finalPath.Append(paths[i]);
                }
            }
            return finalPath.ToString();
        }

        private static string CombineNoChecks(string path1, string path2)
        {
            if (path2.Length == 0)
                return path1;
            if (path1.Length == 0)
                return path2;
            if (Path.IsPathRooted(path2))
                return path2;

            char ch = path1[path1.Length - 1];
            if (ch != DirectorySeparatorChar && ch != AltDirectorySeparatorChar && ch != VolumeSeparatorChar)
                return path1 + DirectorySeparatorChar + path2;
            return path1 + path2;
        }

        private static void CheckInvalidPathChars(string path, bool checkAdditional = false)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (HasIllegalCharacters(path, checkAdditional))
                throw new ArgumentException("Path contains illegal character(s)");
        }

        private static bool HasIllegalCharacters(string path, bool checkAdditional)
        {
            var platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
            {
                if (path.Length >= 2 && path[0] == '\\' && path[1] == '\\')
                    return true;
            }

            for (int i = 0; i < path.Length; i++)
            {
                int c = path[i];

                // Note: This list is duplicated in static char[] InvalidPathChars
                if (c == '\"' || c == '<' || c == '>' || c == '|' || c < 32)
                    return true;

                // used only by FileIOPermission, FileStream.Init, and AppDomainSet.ManifestFilePath
                if (checkAdditional && (c == '?' || c == '*'))
                    return true;
            }
            return false;
        }

        #endregion Copy From System.IO.Path
    }
}