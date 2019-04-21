using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShuHai.DebugInspector.Editor
{
    public abstract class Matrix4x4MemberEntry<TValue> : PropertyEntry<Matrix4x4, TValue, PropertyInfo>
    {
        public override TValue Value
        {
            get
            {
                Matrix4x4MemberEntryHelper.ThrowOnInvalidTRS(Owner);
                return base.Value;
            }
            set
            {
                Matrix4x4MemberEntryHelper.ThrowOnInvalidTRS(Owner);
                base.Value = value;
            }
        }

        protected Matrix4x4MemberEntry(Matrix4x4 owner, PropertyInfo info) : base(owner, info) { }
    }

    public class Matrix4x4RotationEntry : Matrix4x4MemberEntry<Quaternion>
    {
        public Matrix4x4RotationEntry(Matrix4x4 owner) : base(owner, Property) { }

        private static readonly PropertyInfo Property = typeof(Matrix4x4).GetProperty("rotation", false, false);
    }

    public class Matrix4x4LossyScaleEntry : Matrix4x4MemberEntry<Vector3>
    {
        public Matrix4x4LossyScaleEntry(Matrix4x4 owner) : base(owner, Property) { }

        private static readonly PropertyInfo Property = typeof(Matrix4x4).GetProperty("lossyScale", false, false);
    }

    internal static class Matrix4x4MemberEntryHelper
    {
        public static void ThrowOnInvalidTRS(Matrix4x4 owner)
        {
#if UNITY_2017_2_OR_NEWER
            if (!owner.ValidTRS())
                throw new AssertionException("Assertion failed on expression: 'ValidTRS()'", null);
#endif
        }
    }
}