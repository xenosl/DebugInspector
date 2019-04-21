using System;
using System.Linq;
using System.Text;

namespace ShuHai.DebugInspector.Editor
{
    internal static class DITypeExtensions
    {
        /// <summary>
        /// Get a value indicates whether <paramref name="self"/> is a valid type parameter for constructing a generic
        /// type.
        /// This can be used to check whether arguments of <see cref="Type.MakeGenericType(Type[])"/> is valid.
        /// </summary>
        /// <param name="self"> Type to check. </param>
        /// <returns>
        /// True if <paramref name="self"/> is valid as generic type parameter for constructing a generic type;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="self"/> is null. </exception>
        public static bool IsValidParamForConstructing(this Type self)
        {
            Ensure.Argument.NotNull(self, "type");
            return !self.IsPointer
                && !self.IsByRef
                && self != typeof(void);
        }

        /// <summary>
        /// Determines whether <paramref name="self"/> is subclass of the generic definition <paramref name="self"/>.
        /// </summary>
        /// <param name="self"> The type to check. </param>
        /// <param name="def"> The generic type definition to compare with. </param>
        /// <returns>
        /// True if the current Type derives from <paramref name="def"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="self"/> or <paramref name="def"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="def"/> is not generic type definition.
        /// </exception>
        public static bool IsSubclassOfGenericTypeDefinition(this Type self, Type def)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(def, "def");
            Ensure.Argument.Satisfy(def.IsGenericTypeDefinition, "generic", "Generic type definition expected.");

            var t = self.BaseType;
            while (t != null && t != typeof(object))
            {
                var toCheck = t.IsGenericType ? t.GetGenericTypeDefinition() : t;
                if (toCheck == def)
                    return true;
                t = t.BaseType;
            }
            return false;
        }

        public static bool IsAssignableFromGenericTypeDefinition(this Type self, Type type)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(type, "type");

            // Non-generic type does not need to compare with generic type definition.
            if (!self.IsGenericType)
                return self.IsAssignableFrom(type);

            // If generic type in parent satisfy the condition, derived type satisfy as well.
            if (!type.IsGenericType)
            {
                var b = type;
                while (b != null)
                {
                    if (b.IsGenericType)
                        return IsAssignableFromGenericTypeDefinition(self, b);
                    b = b.BaseType;
                }
                // A non-generic type can't assign to a generic type.
                return false;
            }

            var selfDef = self.GetGenericTypeDefinition();
            var t = type;
            while (t != null)
            {
                if (t.GetGenericTypeDefinition() == selfDef)
                    return true;

                var interfaces = t.GetInterfaces();
                if (interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == selfDef))
                    return true;

                t = t.BaseType;
            }

            return false;
        }

        public static string GetReadableName(this Type self)
        {
            if (self.IsGenericType)
            {
                var builder = new StringBuilder(self.Name.RemoveTail(2))
                    .Append('<')
                    .AppendJoin(self.GetGenericArguments(), GetReadableName)
                    .Append('>');
                return builder.ToString();
            }
            else
            {
                return self.GetName(false);
            }
        }
    }
}