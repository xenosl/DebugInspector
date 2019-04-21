using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    public class TypeCache
    {
        public readonly Type Type;

        /// <summary>
        ///     All derived types of current instance.
        /// </summary>
        public IEnumerable<Type> DerivedTypes
        {
            get { return derivedTypes ?? (derivedTypes = Type.EnumDerivedTypes(SearchAssemblies).ToArray()); }
        }

        /// <summary>
        ///     Derived types directly inherit from current instance.
        /// </summary>
        public IEnumerable<Type> DirectDerivedTypes
        {
            get
            {
                return directDerivedTypes ??
                    (directDerivedTypes = Type.EnumDirectDerivedTypes(SearchAssemblies).ToArray());
            }
        }

        public IEnumerable<Type> MostDerivedInterfaces
        {
            get { return mostDerivedInterfaces ?? (mostDerivedInterfaces = Type.GetMostDerivedInterfaces()); }
        }

        /// <summary>
        ///     All constructed types of this type.
        ///     Only works if this type is a generic type definition.
        /// </summary>
        public IEnumerable<Type> ConstructedTypes
        {
            get
            {
                return constructedTypes ?? (constructedTypes = Type.EnumConstructedTypes(SearchAssemblies).ToArray());
            }
        }

        public Assembly[] SearchAssemblies;

        public void Clear()
        {
            derivedTypes = null;
            directDerivedTypes = null;
            constructedTypes = null;
        }

        private Type[] derivedTypes;
        private Type[] directDerivedTypes;
        private Type[] mostDerivedInterfaces;
        private Type[] constructedTypes;

        private TypeCache(Type type)
        {
            Type = type;

            SearchAssemblies = DefaultSearchAssemblies
                .Concat(type.Assembly.ToEnumerable())
                .Distinct()
                .ToArray();

            instances.Add(type, this);
        }

        #region TypeCache Instances

        public static implicit operator TypeCache(Type type) { return Get(type); }
        public static implicit operator Type(TypeCache cache) { return cache.Type; }

        public static TypeCache Get<T>() { return Get(typeof(T)); }

        public static TypeCache Get(Type type)
        {
            if (type == null)
                return null;

            TypeCache ex;
            if (!instances.TryGetValue(type, out ex))
                ex = new TypeCache(type);
            return ex;
        }

        private static readonly Dictionary<Type, TypeCache> instances = new Dictionary<Type, TypeCache>();

        #endregion

        #region Types By Name

        public static Assembly[] DefaultSearchAssemblies =
        {
            Assemblies.Mscorlib,
            Assemblies.UserRuntime,
            Assemblies.UnityEngine,
            Assemblies.PluginRuntime,
#if UNITY_EDITOR
            Assemblies.UserEditor,
            Assemblies.UnityEditor,
            Assemblies.PluginEditor,
#endif
        };

        public static Type GetType(string name, bool throwOnError, params Assembly[] searchAssemblies)
        {
            var type = GetType(name, searchAssemblies);
            if (type == null && throwOnError)
                throw new TypeLoadException(string.Format("Type \"{0}\" load failed.", name));
            return type;
        }

        public static Type GetType(string name, params Assembly[] searchAssemblies)
        {
            Ensure.Argument.NotNullOrEmpty(name, "name");

            if (CollectionUtil.IsNullOrEmpty(searchAssemblies))
                searchAssemblies = DefaultSearchAssemblies;

            Type type;
            if (!typesByName.TryGetValue(name, out type))
            {
                type = MakeType(TypeName.Get(name), searchAssemblies);
                if (type != null)
                    typesByName.Add(name, type);
            }
            return type;
        }

        private static readonly Dictionary<string, Type> typesByName = new Dictionary<string, Type>();

        private static Type MakeType(TypeName name, params Assembly[] searchAssemblies)
        {
            Type type = null;

            string typeName = name.DeclareName, asmName = name.AssemblyName;
            if (!name.IsGeneric)
            {
                if (string.IsNullOrEmpty(asmName))
                {
                    type = FindType(typeName, searchAssemblies);
                }
                else
                {
                    var asm = Assembly.Load(asmName);
                    type = asm != null ? asm.GetType(typeName) : null;
                }
            }
            else
            {
                var typeDef = FindType(typeName, searchAssemblies);
                if (typeDef != null)
                {
                    var argTypes = name.GenericArguments.Select(n => MakeType(n, searchAssemblies)).ToArray();
                    type = argTypes.All(t => t != null) ? typeDef.MakeGenericType(argTypes) : null;
                }
            }

            if (type != null)
            {
                if (name.IsArray)
                {
                    type = name.ArrayRanks.Aggregate(type,
                        (current, rank) => (rank == 1 ? current.MakeArrayType() : current.MakeArrayType(rank)));
                }
            }

            return type;
        }

        private static Type FindType(string name, params Assembly[] searchAssemblies)
        {
            foreach (var assembly in searchAssemblies)
            {
                if (assembly == null)
                    continue;
                var type = assembly.GetType(name);
                if (type != null)
                    return type;
            }
            return null;
        }

        #endregion
    }
}