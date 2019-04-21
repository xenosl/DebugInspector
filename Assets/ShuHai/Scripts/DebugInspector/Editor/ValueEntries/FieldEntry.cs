using System;
using System.Reflection;
using ShuHai.Editor;

namespace ShuHai.DebugInspector.Editor
{
    public class FieldEntry<TOwner, TValue, TMemberInfo> : MemberEntry<TOwner, TValue, TMemberInfo>
        where TMemberInfo : FieldInfo
    {
        public FieldEntry(TOwner owner, TMemberInfo info)
            : base(owner, info)
        {
            valueGetter = CommonMethodsEmitter.CreateInstanceFieldGetter<TOwner, TValue>(MemberInfo);
            valueSetter = CommonMethodsEmitter.CreateInstanceFieldSetter<TOwner, TValue>(MemberInfo);
        }

        public override ValueAccessResult GetValue(out TValue value, bool handleErrorLogs, bool catchException)
        {
            var result = new ValueAccessResult();
            if (Owner == null)
            {
                value = default(TValue);
                result.Exception = new NullReferenceException();
            }
            else
            {
                value = Value;
            }
            return result;
        }

        public override ValueAccessResult SetValue(TValue value, bool handleErrorLogs, bool catchException)
        {
            var result = new ValueAccessResult();
            if (Owner == null)
                result.Exception = new NullReferenceException();
            else
                Value = value;
            return result;
        }
    }
}