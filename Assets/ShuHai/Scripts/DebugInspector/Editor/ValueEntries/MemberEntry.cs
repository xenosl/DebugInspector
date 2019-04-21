using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShuHai.Editor;
using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    public abstract class MemberEntry<TOwner, TValue, TMemberInfo> : ValueEntry<TOwner, TValue>
        where TMemberInfo : MemberInfo
    {
        public override TValue Value
        {
            get
            {
                if (valueGetter == null)
                    throw new InvalidOperationException("Value getter not set.");
                return valueGetter(ref owner);
            }
            set
            {
                if (valueSetter == null)
                    throw new InvalidOperationException("Value setter not set.");
                valueSetter(ref owner, value);
            }
        }

        protected ThisValueGetter<TOwner, TValue> valueGetter;
        protected ThisValueSetter<TOwner, TValue> valueSetter;

        public readonly TMemberInfo MemberInfo;

        public MemberEntry(TOwner owner, TMemberInfo memberInfo)
            : base(owner)
        {
            Ensure.Argument.NotNull(memberInfo, "memberInfo");
            MemberInfo = memberInfo;
        }
    }

    /// <summary>
    ///     Provides support for type mapping of <see cref="ValueEntry{TOwner, TValue}" /> and creation of
    ///     <see cref="ValueEntry{TOwner, TValue}" />.
    /// </summary>
    [InitializeOnLoad]
    public static class MemberEntryFactory
    {
        public static IValueEntry Create(Type ownerType, object owner, MemberInfo memberInfo)
        {
            Ensure.Argument.NotNull(ownerType, "ownerType");
            Ensure.Argument.NotNull(memberInfo, "memberInfo");

            var valueType = MemberInfoUtil.MemberTypeOf(memberInfo);
            var valueEntryType = MakeEntryType(ownerType, valueType, memberInfo.GetType());
            if (valueEntryType == null)
                return null;

            return (IValueEntry)Activator.CreateInstance(valueEntryType, owner, memberInfo);
        }

        public static IValueEntry Create<TOwner>(TOwner owner, MemberInfo memberInfo)
        {
            return Create(typeof(TOwner), owner, memberInfo);
        }

        #region Type Map

        public static Type MakeEntryType(Type ownerType, Type valueType, Type memberInfoType)
        {
            Ensure.Argument.NotNull(memberInfoType, "memberInfoType");
            DIEnsure.Argument.ValidParamForConstructing(ownerType, "ownerType");
            DIEnsure.Argument.ValidParamForConstructing(valueType, "valueType");
            DIEnsure.Argument.ValidParamForConstructing(memberInfoType, "memberInfoType");
            Ensure.Argument.Is<MemberInfo>(memberInfoType, "memberInfoType");

            var valueEntryTypeDef = GetEntryTypeDefByInfoType(memberInfoType);
            if (valueEntryTypeDef == null)
                return null;
            return valueEntryTypeDef.MakeGenericType(ownerType, valueType, memberInfoType);
        }

        public static Type GetEntryTypeDefByInfoType(Type memberInfoType)
        {
            Ensure.Argument.NotNull(memberInfoType, "memberInfoType");
            Ensure.Argument.Is<MemberInfo>(memberInfoType, "memberInfoType");

            var mit = memberInfoType;
            while (mit != typeof(MemberInfo) && mit != null)
            {
                Type valueEntryTypeDef;
                if (infoTypeToEntryTypeDef.TryGetValue(mit, out valueEntryTypeDef))
                    return valueEntryTypeDef;
                mit = mit.BaseType;
            }
            return null;
        }

        private static Dictionary<Type, Type> infoTypeToEntryTypeDef;

        private static readonly Type RootEntryTypeDef = typeof(MemberEntry<,,>);

        private static void InitTypeMap(params Assembly[] searchAssemblies)
        {
            var entryTypeInfos = searchAssemblies
                .Where(a => a != null).Distinct() // Search assemblies.
                .SelectMany(a => a.GetTypes()) // Search types.
                .Where(t => t.IsSubclassOfGenericTypeDefinition(RootEntryTypeDef) && !t.IsAbstract);
                //.Select(ValueEntryTypeInfo.Get); // Entry types.
            //.ToDictionary(t => t.GetGenericArguments()[2].GetGenericParameterConstraints()[0], t => t);

            infoTypeToEntryTypeDef = new Dictionary<Type, Type>();
            foreach (var type in entryTypeInfos)
            {
                var ga = type.GetGenericArguments();
                if (ga.Length == 3)
                {
                    var c = ga[2].GetGenericParameterConstraints()[0];
                    if (c.IsSubclassOf(typeof(MemberInfo)))
                        infoTypeToEntryTypeDef.Add(c, type);
                }
            }
        }

        #endregion Type Map

        static MemberEntryFactory()
        {
            InitTypeMap(RootEntryTypeDef.Assembly, Assemblies.UserEditor, Assemblies.PluginEditor);
        }
    }
}