using System.Reflection;
using ShuHai.Editor;

namespace ShuHai.DebugInspector.Editor
{
    public class PropertyEntry<TOwner, TValue, TMemberInfo> : MemberEntry<TOwner, TValue, TMemberInfo>
        where TMemberInfo : PropertyInfo
    {
        public override bool CanRead { get { return MemberInfo.CanRead; } }
        public override bool CanWrite { get { return MemberInfo.CanWrite; } }

        public PropertyEntry(TOwner owner, TMemberInfo info)
            : base(owner, info)
        {
            if (info.CanRead)
                valueGetter = CommonMethodsEmitter.CreateInstancePropertyGetter<TOwner, TValue>(info);
            if (info.CanWrite)
                valueSetter = CommonMethodsEmitter.CreateInstancePropertySetter<TOwner, TValue>(info);
        }
    }
}