using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class MemberGroupDrawer : MappingValueGroupDrawer<MemberInfo>
    {
        private MemberGroupDrawer() { Name = "Members"; }

        #region Value Access

        internal void OnChildValueSet(ValueDrawer child, ValueAccessResult result)
        {
            if (Parent == null)
                return;
            if (!Parent.ValueEntry.TypeOfValue.IsValueType)
                return;
            if (GetMapped(child) == null)
                return;

            var ownerValue = child.ValueEntry.Owner;
            Parent.SetValue(ownerValue, true, true);

            foreach (var otherChild in MappedChildren)
            {
                if (otherChild != child)
                    otherChild.ValueEntry.Owner = ownerValue;
            }
        }

        #endregion Value Access

        #region Parents

        public new ValueDrawer Parent { get { return (ValueDrawer)base.Parent; } set { base.Parent = value; } }

        protected override void BeforeChangeParent(Drawer newParent)
        {
            if (newParent != null && !(newParent is ValueDrawer))
                throw new ArgumentException("ValueDrawer expected.", "newParent");

            base.BeforeChangeParent(newParent);
        }

        #endregion Parents

        #region Children

        public Type ValueType { get; private set; }

        #region SetChildren

        public void SetChildren(Type valueType)
        {
            Ensure.Argument.NotNull(valueType, "valueType");

            ValueType = valueType;
            SetChildrenImpl(GetMembers(ValueType), false);
        }

        public void SetChildren(Type valueType, IEnumerable<MemberInfo> members)
        {
            Ensure.Argument.NotNull(valueType, "valueType");

            if (CollectionUtil.IsNullOrEmpty(members))
                return;

            ValueType = valueType;
            SetChildrenImpl(members, true);
        }

        private void SetChildrenImpl(IEnumerable<MemberInfo> members, bool verifyReflectedType)
        {
            foreach (var member in members)
            {
                if (verifyReflectedType && !member.ReflectedType.IsAssignableFrom(ValueType))
                {
                    var msg = string.Format(
                        @"Member of ""{0}"" expected, got ""{1}"".",
                        ValueType, member.ReflectedType);
                    throw new ArgumentException(msg, "members");
                }

                var child = GetChild(member);
                if (child != null)
                    continue;

                var childName = member.Name;
                var childState = DrawerStates.Get(HierarchicalName + '.' + childName);
                child = ValueDrawer.Create(
                    member.DeclaringType, MemberInfoUtil.MemberTypeOf(member), childName, null, childState);
                Assert.IsTrue(AddMapping(member, child), string.Format(
                    @"Failed to add member mapping from ""{0}"" to ""{1}"".", member, child.HierarchicalName));
            }
        }

        #endregion SetChildren

        public void UpdateChildrenValueEntry(object value, Func<object, MemberInfo, IValueEntry> creator)
        {
            Ensure.Argument.NotNull(value, "value");

            var valueType = value.GetType();
            Ensure.Argument.Satisfy(ValueType.IsAssignableFrom(valueType), "value",
                string.Format(@"Instance of ""{0}"" expected, got ""{1}"".", ValueType, valueType));

            foreach (var kvp in MappedToChild)
            {
                var member = kvp.Key;
                var drawer = kvp.Value;
                var valueEntry = creator(value, member);
                drawer.ValueEntry = valueEntry;
            }
        }

        #endregion Children

        #region Members

        public static MemberInfo[] GetMembers(Type type)
        {
            return type != null
                ? type.GetMembers(memberSearchFlags).Where(FilterMember).ToArray()
                : ArrayUtil.Empty<MemberInfo>();
        }

        private const BindingFlags DefaultMemberSearchFlags
            = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly BindingFlags memberSearchFlags = DefaultMemberSearchFlags;

        private static MemberTypes memberSearchTypes
        {
            get
            {
                var flags = Settings.VisibleMemberTypes;
                MemberTypes types = 0;
                if (flags.HasFlag(VisibleMemberTypes.Field))
                    types |= MemberTypes.Field;
                if (flags.HasFlag(VisibleMemberTypes.Property))
                    types |= MemberTypes.Property;
                return types;
            }
        }

        private static bool FilterMember(MemberInfo member)
        {
            if (!member.MemberType.HasAnyFlag(memberSearchTypes))
                return false;

            if (member.IsDefined(typeof(ObsoleteAttribute)))
                return false;
            if (member.IsDefined(typeof(CompilerGeneratedAttribute)))
                return false;

            var field = member as FieldInfo;
            if (field != null)
            {
                if (!field.FieldType.IsValidParamForConstructing())
                    return false;
                if (!FilterMemberByAccessModifier(field))
                    return false;
                return true;
            }

            var property = member as PropertyInfo;
            if (property != null)
            {
                if (!property.CanRead)
                    return false;
                if (!property.PropertyType.IsValidParamForConstructing())
                    return false;
                if (property.GetIndexParameters().Length > 0)
                    return false;
                if (!FilterMemberByAccessModifier(property.GetGetMethod(true)))
                    return false;
                return true;
            }

            //var evt = member as EventInfo;
            //if (evt != null)
            //    return true;

            return false;
        }

        private static bool FilterMemberByAccessModifier(FieldInfo member)
        {
            var flags = Settings.VisiblityByAccessModifiers;
            if (member.IsPublic)
                return flags.HasFlag(AccessModifiers.Public);
            if (member.IsFamily)
                return flags.HasFlag(AccessModifiers.Protected);
            if (member.IsPrivate)
                return flags.HasFlag(AccessModifiers.Private);
            if (member.IsAssembly)
                return flags.HasFlag(AccessModifiers.Internal);
            if (member.IsFamilyOrAssembly)
                return flags.HasFlag(AccessModifiers.ProtectedInternal);
            if (member.IsFamilyAndAssembly)
                return flags.HasFlag(AccessModifiers.PrivateProtected);
            return false;
        }

        private static bool FilterMemberByAccessModifier(MethodInfo member)
        {
            var flags = Settings.VisiblityByAccessModifiers;
            if (member.IsPublic)
                return flags.HasFlag(AccessModifiers.Public);
            if (member.IsFamily)
                return flags.HasFlag(AccessModifiers.Protected);
            if (member.IsPrivate)
                return flags.HasFlag(AccessModifiers.Private);
            if (member.IsAssembly)
                return flags.HasFlag(AccessModifiers.Internal);
            if (member.IsFamilyOrAssembly)
                return flags.HasFlag(AccessModifiers.ProtectedInternal);
            if (member.IsFamilyAndAssembly)
                return flags.HasFlag(AccessModifiers.PrivateProtected);
            return false;
        }

        #endregion Members
    }
}