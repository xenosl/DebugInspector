using System;
using System.Collections.Generic;

namespace ShuHai.DebugInspector.Editor
{
    public abstract class DITypeInfo
    {
        /// <summary>
        ///     Target type which current instance represents.
        /// </summary>
        public readonly Type Type;

        public readonly int DeriveDepth;

        public readonly GenericArguments GenericArguments;

        protected DITypeInfo(Type type)
        {
            Ensure.Argument.NotNull(type, "type");

            Type = type;
            DeriveDepth = Type.GetDeriveDepth();

            if (type.IsGenericType)
                GenericArguments = CreateGenericArguments(type);
        }

        protected virtual GenericArguments CreateGenericArguments(Type type) { return new GenericArguments(type); }

        #region Instances

        protected static T GetOrCreate<T>(Dictionary<Type, T> instances, Type type, Func<Type, T> creator)
            where T : DITypeInfo
        {
            T info;
            if (!instances.TryGetValue(type, out info))
            {
                info = creator(type);
                instances.Add(type, info);
            }
            return info;
        }

        #endregion
    }
}