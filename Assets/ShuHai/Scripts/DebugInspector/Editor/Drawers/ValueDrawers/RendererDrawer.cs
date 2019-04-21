using System.Reflection;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class RendererDrawer<TOwner, TRenderer> : ComponentDrawer<TOwner, TRenderer>
        where TRenderer : Renderer
    {
        protected override IValueEntry CreateMemberGroupChildValueEntry(object value, MemberInfo member)
        {
            switch (member.Name)
            {
                case "material":
                    return new RendererMaterialEntry<Renderer>((Renderer)value);
                case "materials":
                    return new RendererMaterialsEntry<Renderer>((Renderer)value);
                default:
                    return base.CreateMemberGroupChildValueEntry(value, member);
            }
        }
    }
}