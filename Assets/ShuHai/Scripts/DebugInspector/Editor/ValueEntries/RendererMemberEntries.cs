using System.Reflection;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class RendererMaterialEntry<TOwner> : PropertyEntry<TOwner, Material, PropertyInfo>
        where TOwner : Renderer
    {
        public override Material Value
        {
            get
            {
                UnityObjectMemberEntryHelper.ThrowOnInaccessible(
                    Owner, "Renderer.material", "Renderer.sharedMaterial");
                return base.Value;
            }
            set
            {
                UnityObjectMemberEntryHelper.ThrowOnInaccessible(
                    Owner, "Renderer.material", "Renderer.sharedMaterial");
                base.Value = value;
            }
        }

        public RendererMaterialEntry(TOwner owner) : base(owner, Property) { }

        private static readonly PropertyInfo Property = typeof(TOwner).GetProperty("material", false, false);
    }

    public class RendererMaterialsEntry<TOwner> : PropertyEntry<TOwner, Material[], PropertyInfo>
        where TOwner : Renderer
    {
        public override Material[] Value
        {
            get
            {
                UnityObjectMemberEntryHelper.ThrowOnInaccessible(
                    Owner, "Renderer.materials", "Renderer.sharedMaterials");
                return base.Value;
            }
            set
            {
                UnityObjectMemberEntryHelper.ThrowOnInaccessible(
                    Owner, "Renderer.materials", "Renderer.sharedMaterials");
                base.Value = value;
            }
        }

        public RendererMaterialsEntry(TOwner owner) : base(owner, Property) { }

        private static readonly PropertyInfo Property = typeof(TOwner).GetProperty("materials", false, false);
    }
}