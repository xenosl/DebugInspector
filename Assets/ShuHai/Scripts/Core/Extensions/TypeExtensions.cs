using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ShuHai
{
    public static class TypeExtensions
    {
        /// <summary>
        ///     Get a value indicating how much the specified type is derived from or parent to specified type.
        /// </summary>
        /// <param name="self"> From which the depth comes. </param>
        /// <param name="type"> The type from which the depth is calculated. </param>
        /// <returns>
        ///     A value indicating number of types in hierarchy between <paramref name="self" /> and <paramref name="type" /> if
        ///     they have relationship of inheriting; otherwise -1.
        /// </returns>
        public static int GetDeriveDepth(this Type self, Type type = null)
        {
            Ensure.Argument.NotNull(self, "self");

            type = type ?? typeof(object);

            if (self == type)
                return 0;
            if (self.IsSubclassOf(type))
                return GetDerivedDepthImpl(self, type);
            if (type.IsSubclassOf(self))
                return GetDerivedDepthImpl(type, self);
            return -1;
        }

        private static int GetDerivedDepthImpl(Type derived, Type @base)
        {
            int depth = 1;
            var t = derived.BaseType;
            while (t != @base)
            {
                depth++;
                t = t.BaseType;
            }

            return depth;
        }

        /// <summary>
        ///     Get all interfaces of specified type and filter them by removing types those derived from any other.
        /// </summary>
        /// <param name="self">The type instance that derived or implemented the interfaces.</param>
        /// <returns>
        ///     An array of <see cref="Type" /> instances that contains only most derived interfaces of <paramref name="self" />;
        ///     or an empty array if no interfaces are implemented or inherited by <paramref name="self" />.
        /// </returns>
        public static Type[] GetMostDerivedInterfaces(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");

            var interfaces = self.GetInterfaces();
            return interfaces.Where(i => !interfaces.Any(ii => i != ii && i.IsAssignableFrom(ii))).ToArray();
            //return interfaces.Except(interfaces.SelectMany(t => t.GetInterfaces())).ToArray();
        }

        public static bool IsPublicOrNestedPublic(this Type self) { return self.IsPublic || self.IsNestedPublic; }

        //public static bool IsInternal(this Type self) { return self.Attributes.HasFlag(TypeAttributes.NestedAssembly); }

        //// only nested types can be declared "protected"
        //public static bool IsProtected(this Type self) { return self.IsNestedFamily; }

        //// only nested types can be declared "private"
        //public static bool IsPrivate(this Type self) { return self.IsNestedPrivate; }

        #region Member Getters

        /// <summary>
        ///     Searches for the default constructor.
        /// </summary>
        /// <param name="self">The <see cref="Type" /> instance to search.</param>
        /// <returns>
        ///     A <see cref="ConstructorInfo" /> object representing the default constructor if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        public static ConstructorInfo GetDefaultConstructor(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");

            return self.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null);
        }

        /// <summary>
        ///     Searches for the specified field that matches certain conditions.
        /// </summary>
        /// <param name="self">The <see cref="Type" /> instance to search.</param>
        /// <param name="name">Name of the field.</param>
        /// <param name="isStatic"> Whether the field is from instance or static. </param>
        /// <param name="includeNonPublic"> Whether to search non-public fields. </param>
        /// <returns>
        ///     An object representing the field that matches the specified requirements, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        public static FieldInfo GetField(
            this Type self, string name, bool isStatic, bool includeNonPublic = true)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.GetField(name, BindingFlagsForGettingMember(isStatic, includeNonPublic));
        }

        /// <summary>
        ///     Searches for the specified property that matches certain conditions.
        /// </summary>
        /// <param name="self"> The <see cref="Type" /> instance to search. </param>
        /// <param name="name"> Name of the property. </param>
        /// <param name="isStatic"> Whether the property is from instance or static. </param>
        /// <param name="includeNonPublic"> Whether to search non-public fields. </param>
        /// <returns>
        ///     An object representing the property that matches the specified requirements, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        public static PropertyInfo GetProperty(
            this Type self, string name, bool isStatic, bool includeNonPublic = true)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.GetProperty(name, BindingFlagsForGettingMember(isStatic, includeNonPublic));
        }

        /// <summary>
        ///     Searches for the specified method that matches certain conditions.
        /// </summary>
        /// <param name="self"> The <see cref="Type" /> instance to search. </param>
        /// <param name="name"> Name of the method. </param>
        /// <param name="isStatic"> Whether the method is from instance or static. </param>
        /// <param name="includeNonPublic"> Whether to search non-public fields. </param>
        /// <returns>
        ///     An object representing the method that matches the specified requirements, if found; otherwise,
        ///     <see langword="null" />.
        /// </returns>
        public static MethodInfo GetMethod(this Type self, string name, bool isStatic, bool includeNonPublic = true)
        {
            Ensure.Argument.NotNull(self, "self");
            var bindingFlags = BindingFlagsForGettingMember(isStatic, includeNonPublic);
            return self.GetMethod(name, bindingFlags);
        }

        public static MethodInfo GetMethod(this Type self, string name,
            bool isStatic, bool includeNonPublic, params Type[] parameterTypes)
        {
            Ensure.Argument.NotNull(self, "self");
            var bindingFlags = BindingFlagsForGettingMember(isStatic, includeNonPublic);
            return self.GetMethod(name, bindingFlags, null, parameterTypes, null);
        }

        private static BindingFlags BindingFlagsForGettingMember(bool isStatic, bool includeNonPublic)
        {
            var bindingAttr = BindingFlags.Public;
            bindingAttr |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
            bindingAttr |= includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default;
            return bindingAttr;
        }

        #endregion

        #region Related Type Enumeration

        /// <summary>
        ///     Enumerate all base types of <paramref name="self" />.
        /// </summary>
        /// <param name="self"> Base type of which to enumerate. </param>
        /// <remarks>
        ///     Order of the enumeration is from <paramref name="self" /> to <see cref="object" />.
        /// </remarks>
        public static IEnumerable<Type> EnumBaseTypes(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");

            var t = self.BaseType;
            while (t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }

        /// <summary>
        ///     Enumerate all base types of specified type and the type itself.
        /// </summary>
        /// <param name="self"> Most derived type in the class hierarchy of the enumeration. </param>
        /// <remarks>
        ///     Order of the enumeration is from <paramref name="self" /> to <see cref="object" />.
        /// </remarks>
        public static IEnumerable<Type> EnumHierarchicTypesAsMostDerived(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");

            var t = self;
            while (t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }

        /// <summary>
        ///     Similar with <see cref="EnumDerivedTypes(Type, IEnumerable{Type})" />, but search types from
        ///     assemblies.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchAssemblies">
        ///     Where derived types come from. Assembly of <paramref name="self" /> is used if null.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        public static IEnumerable<Type> EnumDerivedTypes(this Type self, params Assembly[] searchAssemblies)
        {
            return EnumTypes(EnumDerivedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     Enumerate derived types of <paramref name="self" /> in <paramref name="searchTypes" />.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchTypes"> Where derived types come from. </param>
        /// <returns> Derived types of <paramref name="self" />. </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        /// </exception>
        public static IEnumerable<Type> EnumDerivedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return searchTypes.Where(t => t.IsSubclassOf(self)).Distinct();
        }

        /// <summary>
        ///     Similar with <see cref="EnumDirectDerivedTypes(Type, IEnumerable{Type})" />, but search types
        ///     from assemblies.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchAssemblies">
        ///     Where derived types come from.
        ///     Assembly of <paramref name="self" /> is used if null.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        public static IEnumerable<Type> EnumDirectDerivedTypes(this Type self, params Assembly[] searchAssemblies)
        {
            return EnumTypes(EnumDirectDerivedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     Enumerate directly inherited derived types of <paramref name="self" />.
        /// </summary>
        /// <param name="self"> Base type to enumerate. </param>
        /// <param name="searchTypes"> Where derived types come from. </param>
        /// <returns> Directly inherited derived types of <paramref name="self" />. </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        /// </exception>
        public static IEnumerable<Type> EnumDirectDerivedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return EnumDerivedTypes(self, searchTypes).Where(t => t.BaseType == self).Distinct();
        }

        /// <summary>
        ///     Similar with <see cref="EnumHierarchicTypesAsRoot(Type, IEnumerable{Type})" />, but search types from specified
        ///     assembly list.
        /// </summary>
        /// <param name="self"> Root type for enumerating. </param>
        /// <param name="searchAssemblies">
        ///     Where derived types come from.
        ///     Assembly of <paramref name="self" /> is used if null.
        /// </param>
        public static IEnumerable<Type> EnumHierarchicTypesAsRoot(this Type self, params Assembly[] searchAssemblies)
        {
            return EnumTypes(EnumHierarchicTypesAsRoot, self, searchAssemblies);
        }

        /// <summary>
        ///     Enumerate all derived types of <paramref name="self" /> and <paramref name="self" />.
        /// </summary>
        /// <param name="self"> Root type for enumerating. </param>
        /// <param name="searchTypes"> From where derived types come from. </param>
        /// <returns>
        ///     A enumerable collection that contains <paramref name="self" /> and derived types of
        ///     <paramref name="self" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        /// </exception>
        public static IEnumerable<Type> EnumHierarchicTypesAsRoot(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return self.ToEnumerable().Concat(EnumDerivedTypes(self, searchTypes)).Distinct();
        }

        /// <summary>
        ///     Similar with <see cref="EnumConstructedTypes(Type, IEnumerable{Type})" />, but search types from
        ///     assemblies.
        /// </summary>
        /// <param name="self"> Generic type from which the result types are constructed. </param>
        /// <param name="searchAssemblies">
        ///     Where constructed types come from.
        ///     Assembly of <paramref name="self" /> is used if null.
        /// </param>
        public static IEnumerable<Type> EnumConstructedTypes(this Type self, params Assembly[] searchAssemblies)
        {
            return EnumTypes(EnumConstructedTypes, self, searchAssemblies);
        }

        /// <summary>
        ///     All constructed types of <paramref name="self" />.
        /// </summary>
        /// <param name="self"> Generic type from which the result types are constructed. </param>
        /// <param name="searchTypes"> Where constructed types come from. </param>
        /// <returns> Types constructed from <paramref name="self" />. </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="searchTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="searchTypes" /> is empty.
        ///     <paramref name="self" /> is not generic type definition.
        /// </exception>
        public static IEnumerable<Type> EnumConstructedTypes(this Type self, IEnumerable<Type> searchTypes)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.Satisfy(self.IsGenericTypeDefinition, "type", "Generic type definition expected.");
            Ensure.Argument.NotNullOrEmpty(searchTypes, "searchTypes");
            return searchTypes.Where(t => t.IsClosedConstructedGenericTypeOf(self)).Distinct();
        }

        private static IEnumerable<Type> EnumTypes(
            Func<Type, IEnumerable<Type>, IEnumerable<Type>> method, Type type, Assembly[] assemblies)
        {
            if (CollectionUtil.IsNullOrEmpty(assemblies))
                assemblies = new[] { type.Assembly };
            return method(type, assemblies.Where(a => a != null).Distinct().SelectMany(a => a.GetTypes()));
        }

        #endregion Related Type Enumeration

        /// <summary>
        ///     Mock keyword <see langword="is" /> for <see cref="Type" />.
        /// </summary>
        /// <param name="self"> The type to check. </param>
        /// <param name="type"> The target type to compare. </param>
        /// <returns>
        ///     A value indicating whether <paramref name="self" /> equals <paramref name="type" /> or <paramref name="self" /> is
        ///     subclass of <paramref name="type" />.
        /// </returns>
        public static bool Is(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(type, "type");
            return self == type || self.IsSubclassOf(type);
        }

        /// <summary>
        ///     Get a value indicates whether the current type is closed constructed generic type.
        /// </summary>
        /// <param name="self"> Current type to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is a closed constructed generic type; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        /// <remarks>
        ///     Closed constructed generic type means that all type parameters of the generic type have been replaced by
        ///     specific types.
        /// </remarks>
        public static bool IsClosedConstructedGenericType(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.IsGenericType && !self.ContainsGenericParameters;
        }

        /// <summary>
        ///     Get a value indicates whether the current type is closed constructed generic type from a specified generic
        ///     type definition.
        /// </summary>
        /// <param name="self"> The current type to check. </param>
        /// <param name="type"> The generic type definition to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is closed constructed from <paramref name="type" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="self" /> or <paramref name="type" /> is null.
        /// </exception>
        public static bool IsClosedConstructedGenericTypeOf(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(type, "type");
            return self.IsClosedConstructedGenericType() && self.GetGenericTypeDefinition() == type;
        }

        /// <summary>
        ///     Get a value indicates whether the current type is a partial constructed generic type.
        /// </summary>
        /// <param name="self"> The type to check. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is a partial constructed generic type; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        /// <remarks>
        ///     Partial constructed generic type means that part of the type parameters of the generic type have been
        ///     replaced by specific types, but others remained as generic type parameter.
        /// </remarks>
        public static bool IsPartialConstructedGenericType(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.IsGenericType && !self.IsGenericTypeDefinition && self.ContainsGenericParameters;
        }

        /// <summary>
        ///     Get a value indicates whether the current type is a partial constructed generic type from a specified
        ///     generic type definition.
        /// </summary>
        /// <param name="self"> The current type to check. </param>
        /// <param name="type"> The generic type definition to compare. </param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="self" /> is partial constructed from <paramref name="type" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsPartialConstructedGenericTypeOf(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(type, "type");
            return self.IsPartialConstructedGenericType() && self.GetGenericTypeDefinition() == type;
        }

        /// <summary>
        ///     Similar to <see cref="Type.IsSubclassOf(Type)" /> but also available on generic types.
        /// </summary>
        /// <param name="self">Current type instance.</param>
        /// <param name="type">The type to compare with the current type.</param>
        /// <returns>
        ///     <see langword="true" /> if the current Type derives from <paramref name="type" /> or its generic type definition;
        ///     otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="type" />
        ///     and the current Type are equal.
        /// </returns>
        public static bool IsBroadlySubclassOf(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, "self");

            if (type == null || self == type)
                return false;

            if (!type.IsGenericType && !self.IsGenericType)
                return self.IsSubclassOf(type);

            type = type.GetGenericTypeDefinition();
            self = self.BaseType;
            while (self != typeof(object) && self != null)
            {
                var t = self.IsGenericType ? self.GetGenericTypeDefinition() : self;
                if (t == type)
                    return true;
                self = self.BaseType;
            }
            return false;
        }

        /// <summary>
        ///     Get a value indicating whether <paramref name="self" /> is anonymous type.
        /// </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="self" /> is null. </exception>
        public static bool IsAnonymous(this Type self)
        {
            Ensure.Argument.NotNull(self, "self");

            return Attribute.IsDefined(self, typeof(CompilerGeneratedAttribute), false)
                && self.IsGenericType && self.Name.Contains("AnonymousType")
                && (self.Name.StartsWith("<>") || self.Name.StartsWith("VB$"))
                && (self.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static string GetName(this Type self, bool fullName)
        {
            Ensure.Argument.NotNull(self, "self");

            return self.IsGenericParameter ? self.Name // Generic parameter dosen't have a full name.
                : (fullName ? self.FullName : self.Name);
        }
    }
}