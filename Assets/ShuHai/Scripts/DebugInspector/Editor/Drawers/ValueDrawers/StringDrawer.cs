using System;
using ShuHai.Editor.IMGUI;

namespace ShuHai.DebugInspector.Editor
{
    public class StringDrawer<TOwner> : ValueDrawer<TOwner, String>
    {
        protected override void ValueGUIImpl() { typedDrawnValue = EditorGUIEx.TextField(TypedDrawingValue); }
    }
}