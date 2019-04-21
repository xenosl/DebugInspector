using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ShuHai.Xml
{
    public class ObjectConverter : XConverter
    {
        protected override int? GetPriorityImpl(Type type)
        {
            return type.IsPrimitive ? (int?)null : -type.GetDeriveDepth();
        }

        #region Settings

        /// <summary>
        ///     Controls how missing member is processed during converting.
        /// </summary>
        public MissingMemberHandling MissingMemberHandling;

        #endregion

        #region Object To Element

        protected override void PopulateElementValue(
            XElement element, Type declareType, object obj, XConvertSettings settings) { }

        protected override void PopulateElementChildren(
            XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            if (obj == null)
                return;

            var actualType = obj.GetType();
            Ensure.Argument.Is(actualType, "obj", declareType);

            foreach (var members in EnumMembers(obj))
            {
                var memberCount = members.Count();
                foreach (var member in members)
                {
                    Type declareMemberType = null;
                    Type actualMemberType = null;
                    object value = null;

                    var field = member as FieldInfo;
                    if (field != null)
                    {
                        declareMemberType = field.FieldType;
                        value = field.GetValue(obj);
                        if (value != null)
                            actualMemberType = value.GetType();
                    }

                    var property = member as PropertyInfo;
                    if (property != null && property.CanRead)
                    {
                        declareMemberType = property.PropertyType;
                        value = property.GetValue(obj, null);
                        if (value != null)
                            actualMemberType = value.GetType();
                    }

                    var memberType = actualMemberType ?? declareMemberType;
                    var converter = settings.FindConverter(memberType);

                    var name = memberCount > 1 ? GetHierachicalMemberName(actualType, member) : member.Name;
                    var memberElement = converter.ToElement(name, declareMemberType, value, settings);
                    element.Add(memberElement);
                }
            }
        }

        protected override XAttribute CreateTypeAttribute(Type declareType, Type actualType,
            TypeAttributeHandling attributeHandling, FormatterAssemblyStyle? assemblyNameStyle)
        {
            XAttribute typeAttribute = null;
            switch (attributeHandling)
            {
                case TypeAttributeHandling.Auto:
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

        private string GetHierachicalMemberName(Type objType, MemberInfo member)
        {
            string name = member.Name;
            var declareType = member.DeclaringType;
            while (objType != declareType)
            {
                name = "base." + name;
                objType = objType.BaseType;
            }
            return name;
        }

        #endregion

        #region Element To Object

        protected override void PopulateObjectImpl(object obj, XElement element, XConvertSettings settings)
        {
            var objType = obj.GetType();
            var membersDict = GetMembers(obj);
            foreach (var memberElement in element.Elements())
            {
                string name;
                MemberInfo member;
                GetMemberInfo(objType, membersDict, memberElement, out name, out member);
                if (member == null)
                {
                    switch (MissingMemberHandling)
                    {
                        case MissingMemberHandling.Ignore:
                            continue;
                        case MissingMemberHandling.ThrowException:
                            throw new MissingMemberException(objType.FullName, name);
                        default:
                            continue;
                    }
                }

                var declareMemberType = GetDeclareMemberType(member);
                var actualMemberType = XConvertUtil.ParseType(memberElement);
                if (actualMemberType != null)
                {
                    if (actualMemberType != declareMemberType && !actualMemberType.IsSubclassOf(declareMemberType))
                    {
                        throw new XmlException(string.Format(
                            "Object(of type {0}) member type({1}) from xml does not match with its declare type({2}) of the object.",
                            objType, actualMemberType, declareMemberType));
                    }
                }

                var usingMemberType = actualMemberType ?? declareMemberType;
                var converter = settings.FindConverter(usingMemberType);
                var value = converter.ToObject(usingMemberType, memberElement, settings);
                SetMemberValue(member, obj, value);
            }
        }

        private static void GetMemberInfo(Type objType,
            IDictionary<string, MemberInfo[]> membersDict, XElement memberElement,
            out string name, out MemberInfo member)
        {
            var nameArray = memberElement.Name.LocalName.Split('.');
            var nameArrayLen = nameArray.Length;
            name = nameArray[nameArrayLen - 1];
            member = null;

            var members = membersDict.GetValue(name);
            if (members != null)
            {
                Type declareObjType = objType;
                for (int i = 1; i < nameArrayLen; ++i)
                    declareObjType = declareObjType.BaseType;
                member = members.FirstOrDefault(m => m.DeclaringType == declareObjType) ?? members[0];
            }
        }

        private static Type GetDeclareMemberType(MemberInfo member)
        {
            Type type = null;

            var field = member as FieldInfo;
            if (field != null)
                type = field.FieldType;

            var property = member as PropertyInfo;
            if (property != null)
                type = property.PropertyType;

            return type;
        }

        private static void SetMemberValue(MemberInfo member, object obj, object value)
        {
            var field = member as FieldInfo;
            if (field != null)
                field.SetValue(obj, value);

            var property = member as PropertyInfo;
            if (property != null)
                property.SetValue(obj, value, null);
        }

        #endregion

        #region Members

        protected const BindingFlags MemberSearchFlags
            = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected Dictionary<string, MemberInfo[]> GetMembers(object obj)
        {
            return EnumMembers(obj).ToDictionary(g => g.Key, g => g.ToArray());
        }

        protected virtual IEnumerable<IGrouping<string, MemberInfo>> EnumMembers(object obj)
        {
            return obj.GetType().GetMembers(MemberSearchFlags)
                .Where(FilterMember)
                .GroupBy(m => m.Name);
        }

        protected virtual bool FilterMember(MemberInfo member)
        {
            if (!member.MemberType.HasAnyFlag(MemberTypes.Field | MemberTypes.Property))
                return false;

            if (member.IsDefined(typeof(XmlIgnoreAttribute)))
                return false;

            var field = member as FieldInfo;
            if (field != null)
                return true;

            var property = member as PropertyInfo;
            if (property != null)
            {
                if (!property.CanRead || !property.CanWrite)
                    return false;
                if (property.GetIndexParameters().Length > 0)
                    return false;
                return true;
            }

            return false;
        }

        #endregion
    }

    public enum MissingMemberHandling
    {
        /// <summary>
        ///     Ignore the missing memmber.
        /// </summary>
        Ignore,

        /// <summary>
        ///     Thorw an exception on missing member.
        /// </summary>
        ThrowException
    }
}