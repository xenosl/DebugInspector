using System;
using System.Collections.Generic;

namespace ShuHai.Editor
{
    using CreateMethod = Func<object>;

    public static class ObjectFactory
    {
        public static object Create(Type type)
        {
            Ensure.Argument.NotNull(type, "type");
            Ensure.Argument.Satisfy(!type.IsAbstract, "type", "Unable to create instance of abstract type.");
            Ensure.Argument.Satisfy(!type.ContainsGenericParameters, "type",
                "Unable to create instance of type which contains generic parameter.");

            CreateMethod method;
            if (!newMethods.TryGetValue(type, out method))
            {
                method = CommonMethodsEmitter.CreateNew(type);
                newMethods.Add(type, method);
            }
            return method();
        }

        private static readonly Dictionary<Type, CreateMethod> newMethods = new Dictionary<Type, CreateMethod>();
    }
}