using System.Reflection;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class MeshFilterDrawer<TOwner> : ComponentDrawer<TOwner, MeshFilter>
    {
        protected override IValueEntry CreateMemberGroupChildValueEntry(object value, MemberInfo member)
        {
            switch (member.Name)
            {
                case "mesh":
                    return new MeshFilterMeshEntry((MeshFilter)value);
                default:
                    return base.CreateMemberGroupChildValueEntry(value, member);
            }
        }
    }
}