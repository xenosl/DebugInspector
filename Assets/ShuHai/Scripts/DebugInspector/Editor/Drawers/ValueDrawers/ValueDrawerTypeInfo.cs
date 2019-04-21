using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class ValueDrawerTypeInfo : DITypeInfo
    {
        /// <summary>
        ///     <see cref="ValueDrawerTypeInfo" /> for generic type definition of current instance if <see cref="Type" /> is
        ///     generic type.
        /// </summary>
        public readonly ValueDrawerTypeInfo GenericDefinitionInfo;

        public new ValueDrawerGenericArguments GenericArguments
        {
            get { return (ValueDrawerGenericArguments)base.GenericArguments; }
        }

        public readonly GenericParameterConstraints ConstraintsOnOwner;
        public readonly GenericParameterConstraints ConstraintsOnValue;

        public readonly bool IsInterfaceDrawer;

        private ValueDrawerTypeInfo(Type type)
            : base(type)
        {
            Ensure.Argument.NotNull(type, "type");
            Ensure.Argument.Is(type, "type", ValueDrawerTypes.Root);

            if (Type.IsGenericType)
                GenericDefinitionInfo = Type.IsGenericTypeDefinition ? this : Get(Type.GetGenericTypeDefinition());

            var argsForConstraints = GenericDefinitionInfo != null
                ? GenericDefinitionInfo.GenericArguments : GenericArguments;
            if (argsForConstraints.Owner.IsGenericParameter)
                ConstraintsOnOwner = new GenericParameterConstraints(argsForConstraints.Owner);
            if (argsForConstraints.Value.IsGenericParameter)
                ConstraintsOnValue = new GenericParameterConstraints(argsForConstraints.Value);

            if (ConstraintsOnValue != null)
            {
                IsInterfaceDrawer = ConstraintsOnValue.TypeCount > 0
                    && ConstraintsOnValue.Types.All(t => t.IsInterface);
            }
        }

        protected override GenericArguments CreateGenericArguments(Type type)
        {
            return new ValueDrawerGenericArguments(type);
        }

        #region ToString

        public string ToString(bool fullname)
        {
            var selfType = GetType();
            return string.Format("{0}({1}{2})",
                fullname ? selfType.FullName : selfType.Name,
                fullname ? Type.FullName : Type.Name,
                GenericArguments.ToString(fullname, true));
        }

        public override string ToString() { return ToString(false); }

        #endregion ToString

        #region Instances

        public static ValueDrawerTypeInfo Get(Type drawerType)
        {
            Ensure.Argument.NotNull(drawerType, "drawerType");
            Ensure.Argument.Is(drawerType, "drawerType", ValueDrawerTypes.Root);
            return GetOrCreate(instances, drawerType, Create);
        }

        private static ValueDrawerTypeInfo Create(Type type) { return new ValueDrawerTypeInfo(type); }

        private static readonly Dictionary<Type, ValueDrawerTypeInfo>
            instances = new Dictionary<Type, ValueDrawerTypeInfo>();

        #endregion Instances
    }
}