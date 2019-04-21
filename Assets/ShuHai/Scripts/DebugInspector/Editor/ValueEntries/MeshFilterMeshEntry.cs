using System.Reflection;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class MeshFilterMeshEntry : PropertyEntry<MeshFilter, Mesh, PropertyInfo>
    {
        public override Mesh Value
        {
            get
            {
                UnityObjectMemberEntryHelper.ThrowOnInaccessible(Owner, "MeshFilter.mesh", "MeshFilter.sharedMesh");
                return base.Value;
            }
            set
            {
                UnityObjectMemberEntryHelper.ThrowOnInaccessible(Owner, "MeshFilter.mesh", "MeshFilter.sharedMesh");
                base.Value = value;
            }
        }

        public MeshFilterMeshEntry(MeshFilter owner) : base(owner, Property) { }

        private static readonly PropertyInfo Property = typeof(MeshFilter).GetProperty("mesh", false, false);
    }
}