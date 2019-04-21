using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Represents the constraints of a generic type parameter.
    /// </summary>
    public sealed class GenericParameterConstraints : IEquatable<GenericParameterConstraints>
    {
        /// <summary>
        ///     The generic type parameter that limited by the constraints of current instance.
        /// </summary>
        public readonly Type Parameter;

        /// <summary>
        ///     See <see cref="Type.GenericParameterAttributes" />.
        /// </summary>
        public readonly GenericParameterAttributes Attributes;

        /// <summary>
        ///     Number of constraint types.
        /// </summary>
        public int TypeCount { get { return types.Length; } }

        /// <summary>
        ///     Constraint types of <see cref="Parameter" />.
        /// </summary>
        public IEnumerable<Type> Types { get { return types; } }

        public GenericParameterConstraints(Type parameter)
        {
            Ensure.Argument.NotNull(parameter, "parameter");
            Ensure.Argument.Satisfy(parameter.IsGenericParameter, "parameter", "Generic type parameter expected.");

            Parameter = parameter;
            Attributes = Parameter.GenericParameterAttributes;
            types = Parameter.GetGenericParameterConstraints();

            // Hash code is constant since this class is immutable.
            hashCode = InitHashCode();
        }

        public Type GetType(int index) { return types[index]; }

        private readonly Type[] types;

        #region Match

        /// <summary>
        ///     Check whether the specified type match the constraints of current instance.
        /// </summary>
        /// <param name="type"> The type to check. </param>
        /// <returns>
        ///     <see langword="true" /> if the <paramref name="type" /> match the constraints of current instance; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool Match(Type type) { return Match(type, Attributes, types); }

        /// <summary>
        ///     Check whether the specified type match the given generic parameter's constraints.
        /// </summary>
        /// <param name="type"> The type to check. </param>
        /// <param name="parameter"> The generic parameter that limits by the constraints to match. </param>
        /// <returns>
        ///     <see langword="true" /> if the <paramref name="type" /> match the constraints of <paramref name="parameter" />;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool Match(Type type, Type parameter)
        {
            Ensure.Argument.NotNull(parameter, "parameter");
            Ensure.Argument.Satisfy(parameter.IsGenericParameter, "parameter", "Generic parameter expected.");
            return Match(type, parameter.GenericParameterAttributes, parameter.GetGenericParameterConstraints());
        }

        /// <summary>
        ///     Check whether the specified type match the constraints.
        /// </summary>
        /// <param name="type"> The type to check. </param>
        /// <param name="attributes"> The generic parameter attributes to match. </param>
        /// <param name="typeConstraints"> The constraint type list to match. </param>
        /// <returns>
        ///     <see langword="true" /> if the <paramref name="type" /> match the constraints; otherwise, <see langword="false" />.
        /// </returns>
        public static bool Match(Type type, GenericParameterAttributes attributes, Type[] typeConstraints)
        {
            Ensure.Argument.NotNull(type, "type");
            Ensure.Argument.Satisfy(!type.ContainsGenericParameters, "type",
                "The type to check must be a specific type.");

            if (attributes.HasAnyFlag(GenericParameterAttributes.VarianceMask))
                throw new NotSupportedException("Match constraints with variance is not supported.");

            if (attributes.HasAnyFlag(GenericParameterAttributes.SpecialConstraintMask))
            {
                if (attributes.HasFlag((Enum)GenericParameterAttributes.NotNullableValueTypeConstraint)) // struct
                {
                    if (!type.IsValueType)
                        return false;
                }
                else if (attributes.HasFlag((Enum)GenericParameterAttributes.ReferenceTypeConstraint)) // class
                {
                    if (type.IsValueType)
                        return false;
                }

                if (attributes.HasFlag((Enum)GenericParameterAttributes.DefaultConstructorConstraint)) // new()
                {
                    if (!type.IsValueType)
                    {
                        if (type.GetDefaultConstructor() == null)
                            return false;
                    }
                }
            }

            if (typeConstraints.Length > 0)
            {
                foreach (var constraint in typeConstraints)
                {
                    if (!MatchConstraintType(type, constraint))
                        return false;
                }
            }

            return true;
        }

        private static bool MatchConstraintType(Type type, Type constraint)
        {
            if (constraint.ContainsGenericParameters)
            {
                if (constraint.IsGenericParameter)
                {
                    if (!Match(type, constraint))
                        return false;
                }
                else
                {
                    // ContainsGenericParameters && !IsGenericParameter && !IsGenericType
                    // This situation doesn't exist.
                    Assert.IsTrue(constraint.IsGenericType);

                    if (!constraint.IsAssignableFromGenericTypeDefinition(type))
                        return false;

                    if (!MatchArguments(type, constraint))
                    {
                        bool anyInterfaceMatch = false;
                        var typeInterfaces = type.GetInterfaces();
                        foreach (var i in typeInterfaces)
                        {
                            if (!i.IsGenericType)
                                continue;

                            if (MatchArguments(i, constraint))
                            {
                                anyInterfaceMatch = true;
                                break;
                            }
                        }
                        if (!anyInterfaceMatch)
                            return false;
                    }
                }
            }
            else
            {
                if (!constraint.IsAssignableFrom(type))
                    return false;
            }
            return true;
        }

        private static bool MatchArguments(Type type, Type constraint)
        {
            var constraintArgs = constraint.GetGenericArguments();
            var typeArgs = type.GetGenericArguments();

            int argCount = constraintArgs.Length;
            if (typeArgs.Length != argCount)
                return false;

            for (int i = 0; i < argCount; ++i)
            {
                if (!MatchConstraintType(typeArgs[i], constraintArgs[i]))
                    return false;
            }
            return true;
        }

        #endregion Match

        #region Equality Comparison

        public static bool operator ==(GenericParameterConstraints l, GenericParameterConstraints r)
        {
            return Equals(l, r);
        }

        public static bool operator !=(GenericParameterConstraints l, GenericParameterConstraints r)
        {
            return !Equals(l, r);
        }

        public bool Equals(GenericParameterConstraints other)
        {
            if (other == null)
                return false;

            for (int i = 0; i < TypeCount; ++i)
            {
                if (types[i] != other.types[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj) { return Equals(obj as GenericParameterConstraints); }

        public override int GetHashCode() { return hashCode; }

        private readonly int hashCode;

        private int InitHashCode()
        {
            unchecked
            {
                int h = 17;
                h = h * 23 + Attributes.GetHashCode();
                h = h * 23 + HashCode.Calculate(types);
                return h;
            }
        }

        #endregion Equality Comparison
    }
}