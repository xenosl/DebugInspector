using ShuHai.Editor.IMGUI;

namespace ShuHai.DebugInspector.Editor
{
    using UObject = UnityEngine.Object;

    public class UnityObjectDrawer<TOwner, TValue> : ValueDrawer<TOwner, TValue>
        where TValue : UObject
    {
        protected override void ValueGUIImpl() { typedDrawnValue = EditorGUIEx.ObjectField(TypedDrawingValue, true); }

        protected override bool childrenAvailable
        {
            get
            {
                return !UnityObjectUtil.IsNull(TypedDrawingValue)
                    && base.childrenAvailable;
            }
        }
    }
}