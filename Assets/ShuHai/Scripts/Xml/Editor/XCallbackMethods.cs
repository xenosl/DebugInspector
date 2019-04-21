using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai.Xml
{
    public class XCallbackMethods
    {
        public static BindingFlags MethodSearchFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public XCallbackMethods(Type objType)
        {
            Ensure.Argument.NotNull(objType, "objType");

            attributeTypeToMethods = objType.GetMethods(MethodSearchFlags)
                .Select(m => Tuple.Create(m.GetCustomAttribute<XConvertCallbackAttribute>(true), m))
                .Where(p => p.Item1 != null)
                .GroupBy(p => p.Item1, p => p.Item2)
                .ToDictionary(g => g.Key.GetType(), g => g.ToArray());
        }

        public void Call<TAttribute>(object obj)
            where TAttribute : XConvertCallbackAttribute
        {
            Call(typeof(TAttribute), obj);
        }

        public void Call(Type attributeType, object obj)
        {
            Ensure.Argument.NotNull(attributeType, "attributeType");

            var method = Get(attributeType).FirstOrDefault();
            if (method == null)
                return;

            if (!method.IsStatic)
                Ensure.Argument.NotNull(obj, "obj");

            method.Invoke(obj, null);
        }

        public IEnumerable<MethodInfo> Get(Type attributeType)
        {
            Ensure.Argument.NotNull(attributeType, "attributeType");

            MethodInfo[] methods;
            if (attributeTypeToMethods.TryGetValue(attributeType, out methods))
                return methods;
            return Enumerable.Empty<MethodInfo>();
        }

        private readonly Dictionary<Type, MethodInfo[]> attributeTypeToMethods;
    }
}