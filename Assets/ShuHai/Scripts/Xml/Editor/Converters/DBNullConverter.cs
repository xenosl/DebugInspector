using System;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    public class DBNullConverter : XConverter
    {
        protected override int? GetPriorityImpl(Type type) { return type == typeof(DBNull) ? 0 : (int?)null; }

        #region Object To Element

        protected override void PopulateElementAttributes(
            XElement element, Type declareType, object obj, XConvertSettings settings)
        {
            element.Add(XConvertUtil.CreateTypeAttribute(declareType, settings.AssemblyNameStyle));
        }

        protected override void PopulateElementValue(
            XElement element, Type declareType, object obj, XConvertSettings settings) { }

        protected override void PopulateElementChildren(
            XElement element, Type declareType, object obj, XConvertSettings settings) { }

        protected override void ToElementArgumentsCheck(Type declareType, object obj)
        {
            if (declareType != typeof(DBNull))
                throw new ArgumentException("DBNull type expected.", "declareType");
            if (!Equals(obj, DBNull.Value))
                throw new ArgumentException("DBNull value expected.", "obj");
        }

        #endregion
    }
}