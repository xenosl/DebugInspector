using System.Reflection;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class Matrix4x4Drawer<TOwner> : ValueDrawer<TOwner, Matrix4x4>
    {
        protected override IValueEntry CreateMemberGroupChildValueEntry(object value, MemberInfo member)
        {
            switch (member.Name)
            {
                case "rotation":
                    return new Matrix4x4RotationEntry((Matrix4x4)value);
                case "lossyScale":
                    return new Matrix4x4LossyScaleEntry((Matrix4x4)value);
                default:
                    return base.CreateMemberGroupChildValueEntry(value, member);
            }
        }
    }
}