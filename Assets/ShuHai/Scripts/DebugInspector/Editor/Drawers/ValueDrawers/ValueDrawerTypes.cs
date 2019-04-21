using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    using TypeTuple = Tuple<Type, Type>;

    public static class ValueDrawerTypes
    {
        /// <summary>
        ///     Root type of all value drawers.
        /// </summary>
        public static readonly Type Root = typeof(ValueDrawer);

        #region Generic Definitions

        /// <summary>
        ///     Root generic type definition of value drawers.
        /// </summary>
        public static readonly Type RootDefinition = typeof(ValueDrawer<,>);

        /// <summary>
        ///     A Collection contains all generic type definitions of <see cref="ValueDrawer" />'s derived type.
        /// </summary>
        public static IEnumerable<Type> Definitions
        {
            get
            {
                if (definitions == null)
                    InitDefinitions();
                return definitions;
            }
        }

        private static Type[] definitions;

        private static void InitDefinitions(params Assembly[] searchAssemblies)
        {
            definitions = searchAssemblies
                .Where(a => a != null).Distinct() // Search assemblies.
                .SelectMany(a => a.GetTypes()) // Search types.
                .Where(t => t.IsSubclassOf(Root) && !t.IsAbstract && t.IsGenericTypeDefinition)
                .ToArray();
        }

        private static void InitDefinitions()
        {
            InitDefinitions(typeof(ValueDrawer).Assembly, Assemblies.UserEditor, Assemblies.PluginEditor);
        }

        #endregion Generic Definitions

        #region Built Types

        public static Type GetOrBuild(Type ownerType, Type valueType, bool useRootAsDefault = true)
        {
            Ensure.Argument.NotNull(ownerType, "ownerType");
            EnsureArgumentSpecificType(ownerType, "ownerType");
            Ensure.Argument.NotNull(valueType, "valueType");
            EnsureArgumentSpecificType(valueType, "valueType");

            var key = new TypeTuple(ownerType, valueType);
            Type builtType;
            if (!builtTypes.TryGetValue(key, out builtType))
            {
                var drawerType = FindByValueType(valueType);
                if (drawerType != null)
                {
                    if (drawerType.IsGenericType && drawerType.ContainsGenericParameters)
                    {
                        var builder = ValueDrawerTypeBuilder.Get(drawerType);
                        Type constructedDrawerType;
                        if (builder.TryBuild(ownerType, valueType, out constructedDrawerType))
                        {
                            builtType = constructedDrawerType;
                        }
                        else if (useRootAsDefault)
                        {
                            builtType = RootDefinition.MakeGenericType(ownerType, valueType);
                        }
                    }
                    else
                    {
                        builtType = drawerType;
                    }
                }
                else if (useRootAsDefault)
                {
                    builtType = RootDefinition.MakeGenericType(ownerType, valueType);
                }

                if (builtType != null)
                    builtTypes.Add(key, builtType);
            }

            return builtType;
        }

        private static readonly Dictionary<TypeTuple, Type> builtTypes = new Dictionary<TypeTuple, Type>();

        private static void EnsureArgumentSpecificType(Type type, string argName)
        {
            if (type.IsGenericType && type.ContainsGenericParameters)
                throw new ArgumentException("Constructed generic type expected.", argName);
        }

        #endregion Built Types

        #region Value To Drawer Type Mapping

        public static void AddValueToDrawerMapping(Type valueType, Type drawerType)
        {
            Ensure.Argument.NotNull(valueType, "valueType");

            if (valueType.IsGenericType)
                valueType = valueType.GetGenericTypeDefinition();

            valueToDrawer.Add(valueType, drawerType);
        }

        /// <summary>
        ///     Mapping from value type to drawer type or drawer generic type definition.
        /// </summary>
        private static readonly Dictionary<Type, Type> valueToDrawer = new Dictionary<Type, Type>();

        private static Type FindByValueType(Type valueType)
        {
            Ensure.Argument.NotNull(valueType, "valueType");

            //var drawers = CollectByValueType(valueType);
            //using (new DrawerTypeComparer.Scope(valueType))
            //    return drawers.OrderByDescending(t => t, DrawerTypeComparer.Default).FirstOrDefault();

            return CollectOrderedByValueType(valueType).FirstOrDefault();
        }

        private static Type[] CollectOrderedByValueType(Type valueType)
        {
            var drawers = CollectByValueType(valueType).ToArray();
            using (new DrawerTypeComparer.Scope(valueType, true))
                Array.Sort(drawers, DrawerTypeComparer.Default);
            return drawers;
        }

        private static HashSet<Type> CollectByValueType(Type valueType)
        {
            var drawers = new HashSet<Type>();

            // Collect available drawer types in derive hierarchy.
            var t = valueType;
            do
            {
                var drawer = GetByValueTypeImpl(t);
                if (drawer != null)
                    drawers.Add(drawer);
                t = t.BaseType;
            } while (t != null);

            // Collect available drawer types in implemented interfaces.
            CollectFromInterfacesByValueType(drawers, valueType);

            return drawers;
        }

        private static void CollectFromInterfacesByValueType(HashSet<Type> drawers, Type targetType)
        {
            var interfaces = TypeCache.Get(targetType).MostDerivedInterfaces;
            foreach (var i in interfaces)
            {
                var drawer = GetByValueTypeImpl(i);
                if (drawer != null)
                    drawers.Add(drawer);
                CollectFromInterfacesByValueType(drawers, i);
            }
        }

        private static Type GetByValueTypeImpl(Type valueType)
        {
            Type drawer;
            if (valueType.IsGenericType)
            {
                Ensure.Argument.Satisfy(!valueType.ContainsGenericParameters, "valueType",
                    "Specific generic type expected.");

                drawer = valueToDrawer.GetValue(valueType.GetGenericTypeDefinition());
            }
            else
            {
                drawer = valueToDrawer.GetValue(valueType);
            }
            return drawer;
        }

        private static void InitValueToDrawerMapping()
        {
            AddValueToDrawerMapping(typeof(Enum), typeof(EnumDrawer<,>));

            foreach (var def in Definitions)
            {
                var typeInfo = ValueDrawerTypeInfo.Get(def);

                var value = typeInfo.GenericArguments.Value;
                if (!value.IsGenericParameter && (value.IsGenericType || !value.ContainsGenericParameters))
                    AddValueToDrawerMapping(value, def);

                var valueConstraints = typeInfo.ConstraintsOnValue;
                if (valueConstraints != null && valueConstraints.TypeCount == 1)
                {
                    var constraintType = valueConstraints.GetType(0);
                    if (constraintType != typeof(ValueType) && constraintType != typeof(Enum))
                        AddValueToDrawerMapping(constraintType, def);
                }
            }

            //UnityEngine.Debug.Log(StringUtil.Join(valueToDrawer, p => string.Format("{0}, {1}", p.Key, p.Value), "\n"));
        }

        #endregion Value To Drawer Type Mapping

        [InitializeOnLoadMethod]
        private static void Initialize() { InitValueToDrawerMapping(); }

        //[MenuItem("Debug/Try Drawer Type Mapping")]
        //private static void Try()
        //{
        //    var drawers = CollectOrderedByValueType(typeof(UnityEngine.MeshFilter));
        //    UnityEngine.Debug.Log(StringUtil.Join(drawers, "\n"));
        //}
    }
}