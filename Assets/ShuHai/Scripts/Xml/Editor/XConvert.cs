using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    public static class XConvert
    {
        #region Element

        /// <summary>
        ///     Convert an object to xml element.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        /// <param name="obj">The object to convert.</param>
        /// <param name="settings">
        ///     Convert settings used during the convertion. <see cref="XConvertSettings.Default" /> will be used if the value is
        ///     <see langword="null" />.
        /// </param>
        /// <returns>An instance of <see cref="XElement" /> that represents <paramref name="obj" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is <see langword="null" />.</exception>
        public static XElement ToElement(string name, object obj, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(obj, "obj");

            settings = settings ?? XConvertSettings.Default;

            var type = obj.GetType();
            var converter = settings.FindConverter(type);
            return converter.ToElement(name, type, obj, settings);
        }

        public static T ToObject<T>(XElement element, XConvertSettings settings = null)
        {
            return (T)ToObject(typeof(T), element, settings);
        }

        public static object ToObject(XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, "element");

            var objType = XConvertUtil.ParseType(element);
            Ensure.Argument.NotNull(objType, "element");

            return ToObject(objType, element, settings);
        }

        /// <summary>
        ///     Convert xml element to its representing object.
        /// </summary>
        /// <param name="objType">Type of the result object.</param>
        /// <param name="element">The xml elemnt to convert.</param>
        /// <param name="settings">
        ///     Convert settings used during the convertion. <see cref="XConvertSettings.Default" /> will be used if the value is
        ///     <see langword="null" />.
        /// </param>
        /// <returns>Object that converted from <paramref name="element" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="objType" /> or <paramref name="element" /> is <see langword="null" />.
        /// </exception>
        public static object ToObject(Type objType, XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(objType, "objType");
            Ensure.Argument.NotNull(element, "element");

            settings = settings ?? XConvertSettings.Default;

            var converter = settings.FindConverter(objType);
            return converter.ToObject(objType, element, settings);
        }

        public static void PopulateObject(object obj, XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(obj, "obj");
            Ensure.Argument.NotNull(element, "element");

            settings = settings ?? XConvertSettings.Default;

            var converter = settings.FindConverter(obj.GetType());
            converter.PopulateObject(obj, element, settings);
        }

        #endregion

        #region Document

        public static XDocument ToDocument(string name, object obj, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(name, "name");
            Ensure.Argument.NotNull(obj, "obj");

            var document = new XDocument();

            var element = ToElement(name, obj, settings);
            document.Add(element);

            return document;
        }

        public static XDocument ToDocument(
            IEnumerable<KeyValuePair<string, object>> objects, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(objects, "objects");

            var document = new XDocument();

            foreach (var kvp in objects)
                document.Add(ToElement(kvp.Key, kvp.Value, settings));

            return document;
        }

        public static object[] ToObjects(XDocument document, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(document, "document");
            return document.Elements().Select(e => ToObject(e, settings)).ToArray();
        }

        #endregion
    }
}