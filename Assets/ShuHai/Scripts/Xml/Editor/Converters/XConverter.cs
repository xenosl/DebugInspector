using System;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    public abstract class XConverter
    {
        public int? GetPriority(Type type)
        {
            Ensure.Argument.NotNull(type, "type");

            if (type.ContainsGenericParameters)
                return null;

            return GetPriorityImpl(type);
        }

        protected abstract int? GetPriorityImpl(Type type);

        public bool CanConvert(Type type) { return GetPriorityImpl(type).HasValue; }

        #region Object To Element

        /// <summary>
        ///     Convert specified object to <see cref="XElement" />.
        /// </summary>
        /// <param name="name">Name of the converted <see cref="XElement" />.</param>
        /// <param name="declareType">Declare type of the converting object.</param>
        /// <param name="obj">Object to convert.</param>
        /// <param name="settings">
        ///     Convert settings used during the convertion. <see cref="XConvertSettings.Default" /> will be used if the value is
        ///     <see langword="null" />.
        /// </param>
        /// <returns>A <see cref="XElement" /> instance converted from <paramref name="obj" />.</returns>
        public XElement ToElement(string name, Type declareType, object obj, XConvertSettings settings)
        {
            Ensure.Argument.NotNullOrEmpty(name, "name");
            ToElementArgumentsCheck(declareType, obj);

            settings = settings ?? XConvertSettings.Default;

            ExecuteCallback<OnConvertingToElementAttribute>(declareType, obj);

            var element = new XElement(name);
            PopulateElement(element, declareType, obj, settings);
            return element;
        }

        /// <summary>
        ///     Populate contents of specified <see cref="XElement" /> instance from specified object.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> instance to populate.</param>
        /// <param name="declareType">Declaring type of the specified object.</param>
        /// <param name="obj">The object that contains the contents to populate from.</param>
        /// <param name="settings">
        ///     Convert settings used when populating. <see cref="XConvertSettings.Default" /> will be used if the value is
        ///     <see langword="null" />.
        /// </param>
        public void PopulateElement(XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(element, "element");
            ToElementArgumentsCheck(declareType, obj);

            settings = settings ?? XConvertSettings.Default;

            PopulateElementAttributes(element, declareType, obj, settings);
            PopulateElementValue(element, declareType, obj, settings);
            PopulateElementChildren(element, declareType, obj, settings);

            ExecuteCallback<OnConvertedToElementAttribute>(declareType, obj);
        }

        protected virtual void PopulateElementAttributes(
            XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            var actualType = obj != null ? obj.GetType() : declareType;

            var typeAttribute = CreateTypeAttribute(declareType, actualType,
                settings.TypeAttributeHandling, settings.AssemblyNameStyle);
            if (typeAttribute != null)
                element.Add(typeAttribute);
        }

        protected virtual void PopulateElementValue(
            XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            element.Value = obj != null ? obj.ToString() : null;
        }

        protected virtual void PopulateElementChildren(
            XElement element, Type declareType, object obj, XConvertSettings settings) { }

        protected virtual XAttribute CreateTypeAttribute(Type declareType, Type actualType,
            TypeAttributeHandling attributeHandling, FormatterAssemblyStyle? assemblyNameStyle)
        {
            XAttribute typeAttribute = null;
            switch (attributeHandling)
            {
                case TypeAttributeHandling.Auto:
                    if (actualType != declareType)
                        typeAttribute = XConvertUtil.CreateTypeAttribute(actualType, assemblyNameStyle);
                    break;
                case TypeAttributeHandling.Objects:
                    if (!actualType.IsValueType)
                        typeAttribute = XConvertUtil.CreateTypeAttribute(actualType, assemblyNameStyle);
                    break;
                case TypeAttributeHandling.All:
                    typeAttribute = XConvertUtil.CreateTypeAttribute(actualType, assemblyNameStyle);
                    break;
            }
            return typeAttribute;
        }

        protected virtual void ToElementArgumentsCheck(Type declareType, object obj)
        {
            Ensure.Argument.NotNull(declareType, "declareType");
            EnsureArgumentCanConvert(declareType, "declareType");

            var actualType = obj != null ? obj.GetType() : null;
            if (actualType != null)
                Ensure.Argument.Is(actualType, "obj", declareType);
        }

        #endregion

        #region Element To Object

        public T ToObject<T>(XElement element, XConvertSettings settings)
        {
            return (T)ToObject(typeof(T), element, settings);
        }

        /// <summary>
        ///     Assume specified xml element represents an object, convert the xml element to the object instance.
        /// </summary>
        /// <param name="element">The xml element that contains the contents of the object.</param>
        /// <param name="settings">
        ///     Convert settings used during the convert. <see cref="XConvertSettings.Default" /> is used if the value is
        ///     <see langword="null" />.
        /// </param>
        /// <returns>Object that converted from <paramref name="element" />.</returns>
        public object ToObject(XElement element, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(element, "element");

            var objType = XConvertUtil.ParseType(element);
            Ensure.Argument.NotNull(objType, "element", "Object type not found.");

            return ToObject(objType, element, settings);
        }

        /// <summary>
        ///     Create object of specified type, then populate contents of specified xml element to the created object.
        /// </summary>
        /// <param name="objType">The type of object to create.</param>
        /// <param name="element">The xml element that contains the contents to populate.</param>
        /// <param name="settings">
        ///     Convert settings used during the convert. <see cref="XConvertSettings.Default" /> is used if the value is
        ///     <see langword="null" />.
        /// </param>
        /// <returns>Object of type <paramref name="objType" /> that holds contents of <paramref name="element" />.</returns>
        public object ToObject(Type objType, XElement element, XConvertSettings settings)
        {
            ToObjectArgumentsCheck(objType, element);

            settings = settings ?? XConvertSettings.Default;

            var obj = CreateObject(objType, element, settings);
            ExecuteCallback<OnConvertingToObjectAttribute>(objType, obj);
            if (obj != null)
                PopulateObjectImpl(obj, element, settings);
            ExecuteCallback<OnConvertedToObjectAttribute>(objType, obj);
            return obj;
        }

        /// <summary>
        ///     Populate the contents of specified xml element to an object.
        /// </summary>
        /// <param name="obj">The object to populate.</param>
        /// <param name="element">The xml element that contains the contents of the object to populate.</param>
        /// <param name="settings">
        ///     Convert settings used during the convert. <see cref="XConvertSettings.Default" /> is used if the value is
        ///     <see langword="null" />.
        /// </param>
        public void PopulateObject(object obj, XElement element, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(obj, "obj");
            ToObjectArgumentsCheck(obj.GetType(), element);

            settings = settings ?? XConvertSettings.Default;

            var objType = obj.GetType();
            ExecuteCallback<OnConvertingToObjectAttribute>(objType, obj);
            PopulateObjectImpl(obj, element, settings);
            ExecuteCallback<OnConvertedToObjectAttribute>(objType, obj);
        }

        protected virtual object CreateObject(Type objType, XElement element, XConvertSettings settings)
        {
            return element.IsEmpty && !element.Elements().Any() ? null : Activator.CreateInstance(objType, true);
        }

        protected virtual void PopulateObjectImpl(object obj, XElement element, XConvertSettings settings) { }

        protected virtual void ToObjectArgumentsCheck(Type objType, XElement element)
        {
            Ensure.Argument.NotNull(element, "element");
            Ensure.Argument.NotNull(objType, "objType");

            var savedType = XConvertUtil.ParseType(element);
            if (savedType != null)
            {
                Ensure.Argument.Is(objType, "objType", savedType);
                EnsureArgumentCanConvert(savedType, "element");
            }
            else
            {
                EnsureArgumentCanConvert(objType, "objType");
            }
        }

        #endregion

        private void EnsureArgumentCanConvert(Type arg, string name)
        {
            Ensure.Argument.Satisfy(CanConvert(arg), name,
                string.Format("Unable to convert type '{0}'.", arg));
        }

        private void ExecuteCallback<TAttribute>(Type declareType, object obj)
            where TAttribute : XConvertCallbackAttribute
        {
            var type = obj != null ? obj.GetType() : declareType;
            var methods = XConvertCallbacks.GetMethods(type);
            methods.Call<TAttribute>(obj);
        }
    }
}