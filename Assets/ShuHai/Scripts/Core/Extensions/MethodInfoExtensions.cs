using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShuHai
{
    public static class MethodInfoExtensions
    {
        public static bool IsProtected(this MethodInfo self)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.IsFamily || self.IsFamilyOrAssembly;
        }

        public static bool IsInternal(this MethodInfo self)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.IsAssembly || self.IsFamilyOrAssembly;
        }

        /// <summary>
        ///     Get a string representing the method.
        /// </summary>
        /// <param name="self">The method instance.</param>
        /// <param name="methodFullName">Indiciates whether to use full name of the method.</param>
        /// <param name="returnTypeFullName">
        ///     <see langword="null" /> to omit the return type; otherwise <see langword="true" /> to use the full name of the
        ///     return type or <see langword="false" /> to use simple name of the return type.
        /// </param>
        /// <param name="parameterTypesFullName">
        ///     <see langword="null" /> to omit the parameter types; otherwise <see langword="true" /> to use the full name of the
        ///     parameter types or <see langword="false" /> to use simple name of the parameter types.
        /// </param>
        /// <param name="withGenericParameterTypes">
        ///     <see langword="true" /> to append generic parameter types; otherwise, <see langword="false" /> to omit generic
        ///     parameter types.
        /// </param>
        public static string ToString(this MethodInfo self, bool methodFullName,
            bool? returnTypeFullName = false, bool? parameterTypesFullName = false,
            bool withGenericParameterTypes = true)
        {
            Ensure.Argument.NotNull(self, "self");

            var builder = new StringBuilder();

            if (returnTypeFullName != null)
            {
                var rt = self.ReturnType;
                builder.Append(returnTypeFullName.Value ? rt.FullName : rt.Name).Append(' ');
            }

            MethodBaseInternal.ToString(self, builder,
                methodFullName, parameterTypesFullName, withGenericParameterTypes);

            return builder.ToString();
        }

        #region Signature Comparison

        public static bool SignatureEquals(this MethodInfo self, MethodInfo other)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(other, "other");

            return self.CallingConvention == other.CallingConvention
                && self.ReturnType == other.ReturnType
                && self.GetParameters().SequenceEqual(other.GetParameters(), ParameterInfoComparer.Instance);
        }

        private sealed class ParameterInfoComparer : IEqualityComparer<ParameterInfo>
        {
            public static readonly ParameterInfoComparer Instance = new ParameterInfoComparer();

            private ParameterInfoComparer() { }

            public bool Equals(ParameterInfo l, ParameterInfo r)
            {
                Type lt = l.ParameterType, rt = r.ParameterType;
                if (lt.IsGenericParameter && rt.IsGenericParameter)
                {
                    throw new NotImplementedException();
                }
                if (!lt.IsGenericParameter && !rt.IsGenericParameter)
                {
                    return l.Position == r.Position && l.Attributes == r.Attributes && lt == rt;
                }
                return false;
            }

            public int GetHashCode(ParameterInfo obj)
            {
                return HashCode.Calculate(obj.Position, obj.ParameterType, obj.Attributes);
            }
        }

        #endregion Signature Comparison
    }
}