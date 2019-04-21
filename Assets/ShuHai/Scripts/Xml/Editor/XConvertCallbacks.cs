using System;
using System.Collections.Generic;

namespace ShuHai.Xml
{
    public static class XConvertCallbacks
    {
        public static XCallbackMethods GetMethods(Type objType)
        {
            Ensure.Argument.NotNull(objType, "type");
            Ensure.Argument.Satisfy(!objType.ContainsGenericParameters, "type",
                "Unable to get certain callback methods for an open constructed generic type.");

            XCallbackMethods methods;
            if (!typeToMethods.TryGetValue(objType, out methods))
            {
                methods = new XCallbackMethods(objType);
                typeToMethods.Add(objType, methods);
            }
            return methods;
        }

        /// <summary>
        ///     Mapping from object type to its callback methods.
        /// </summary>
        private static readonly Dictionary<Type, XCallbackMethods>
            typeToMethods = new Dictionary<Type, XCallbackMethods>();
    }
}