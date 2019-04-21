using System.Reflection;
using System.Text;

namespace ShuHai
{
    public static class MethodBaseExtensions
    {
        /// <summary>
        ///     Get a string representing the method.
        /// </summary>
        /// <param name="self">The method instance.</param>
        /// <param name="methodFullName">Indiciates whether to use full name of the method.</param>
        /// <param name="parameterTypesFullName">
        ///     <see langword="null" /> to omit the parameter types; otherwise <see langword="true" /> to use the full name of the
        ///     parameter types or <see langword="false" /> to use simple name of the parameter types.
        /// </param>
        /// <param name="withGenericParameterTypes">
        ///     <see langword="true" /> to append generic parameter types; otherwise, <see langword="false" /> to omit generic
        ///     parameter types.
        /// </param>
        public static string ToString(this MethodBase self, bool methodFullName,
            bool? parameterTypesFullName = false, bool withGenericParameterTypes = true)
        {
            Ensure.Argument.NotNull(self, "self");

            var builder = new StringBuilder();
            MethodBaseInternal.ToString(self, builder,
                methodFullName, parameterTypesFullName, withGenericParameterTypes);
            return builder.ToString();
        }
    }

    internal static class MethodBaseInternal
    {
        public static StringBuilder ToString(MethodBase method, StringBuilder builder,
            bool methodFullName, bool? parameterTypesFullName = false, bool withGenericParameterTypes = true)
        {
            if (methodFullName)
            {
                var dt = method.DeclaringType;
                builder.Append(dt.FullName).Append('.');
            }
            builder.Append(method.Name);

            if (withGenericParameterTypes && method.IsGenericMethod)
            {
                builder.Append('<');
                foreach (var gt in method.GetGenericArguments())
                    builder.Append(gt.Name).Append(", ");
                builder.RemoveTail(2); // Remove last ", "
                builder.Append('>');
            }

            var parameters = method.GetParameters();
            if (parameterTypesFullName != null && parameters.Length > 0)
            {
                builder.Append('(');
                foreach (var pi in parameters)
                {
                    var pt = pi.ParameterType;
                    if (pi.IsIn)
                        builder.Append("in ");
                    if (pi.IsOut)
                        builder.Append("out ");
                    if (pt.IsByRef && !pi.IsIn && !pi.IsOut)
                        builder.Append("ref ");
                    if (pt.IsByRef)
                        pt = pt.GetElementType();
                    builder.Append(pt.GetName(parameterTypesFullName.Value)).Append(", ");
                }
                builder.RemoveTail(2); // Remove last ", "
                builder.Append(')');
            }

            return builder;
        }
    }
}