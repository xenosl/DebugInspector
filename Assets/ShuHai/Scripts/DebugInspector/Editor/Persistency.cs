using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using ShuHai.Xml;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Provides utitlies for persisting object in "Library/DebugInspector" folder as xml file.
    /// </summary>
    public static class Persistency
    {
        #region Persist

        #region Save

        public static void Save(object instance, string filename, string fileVersion)
        {
            Save(instance, string.Empty, filename, fileVersion);
        }

        public static void Save(object instance, string directory, string filename, string fileVersion)
        {
            var path = PrepareSavePath(instance, directory, filename, fileVersion);
            ToDocument(instance, fileVersion).Save(path);
        }

        #region Async

        public static void AsyncSave(object instance, string filename, string fileVersion)
        {
            AsyncSave(instance, string.Empty, filename, fileVersion);
        }

        public static void AsyncSave(object instance, string directory, string filename, string fileVersion)
        {
            var path = PrepareSavePath(instance, directory, filename, fileVersion);
            new Thread(() => AsyncSaveImpl(path, instance, fileVersion)) { IsBackground = true }.Start();
        }

        private static readonly object asyncSaveLock = new object();

        private static void AsyncSaveImpl(string path, object instance, string fileVersion)
        {
            lock (asyncSaveLock)
                ToDocument(instance, fileVersion).Save(path);
        }

        #endregion Async

        private static string PrepareSavePath(object instance, string directory, string filename, string fileVersion)
        {
            Ensure.Argument.NotNull(instance, "instance");
            Ensure.Argument.NotNullOrEmpty(filename, "filename");
            Ensure.Argument.NotNullOrEmpty(fileVersion, "fileVersion");

            var path = GetPath(directory, filename);
            var dir = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);
            return path;
        }

        private static XDocument ToDocument(object instance, string fileVersion)
        {
            return new XDocument(
                new XDeclaration(fileVersion, "utf-8", "yes"),
                XConvert.ToElement("Instance", instance));
        }

        #endregion Save

        #region Load

        public static bool Load(object instance, string filename, string fileVersion, bool throwOnError = true)
        {
            return Load(instance, string.Empty, filename, fileVersion, throwOnError);
        }

        public static bool Load(object instance,
            string directory, string filename, string fileVersion, bool throwOnError = true)
        {
            Ensure.Argument.NotNull(instance, "instance");
            Ensure.Argument.NotNullOrEmpty(filename, "filename");
            Ensure.Argument.NotNullOrEmpty(fileVersion, "fileVersion");

            try
            {
                var path = GetPath(directory, filename);
                var doc = XDocument.Load(path);

                var version = doc.Declaration.Version;
                if (version != fileVersion)
                {
                    if (throwOnError)
                    {
                        throw new InvalidOperationException(string.Format(
                            "Serialzied version({0}) not match({1}).", version, fileVersion));
                    }
                    return false;
                }

                XConvert.PopulateObject(instance, doc.Root);
            }
            catch (Exception)
            {
                if (throwOnError)
                    throw;
                return false;
            }

            return true;
        }

        #endregion Load

        #endregion

        #region Path 

        public const string FileExtension = ".xml";

        public static string GetPath(string filename) { return GetPath(string.Empty, filename); }

        public static string GetPath(string directory, string filename)
        {
            Ensure.Argument.NotNullOrEmpty(filename, "filename");
            if (!string.IsNullOrEmpty(directory))
                Ensure.Argument.Satisfy(!Path.IsPathRooted(directory), "directory", "Non-rooted path expected.");

            directory = directory ?? string.Empty;
            return PathUtil.Normalize(DIProjectPaths.Library + '/' + directory + '/' + filename + FileExtension);
        }

        #endregion
    }
}