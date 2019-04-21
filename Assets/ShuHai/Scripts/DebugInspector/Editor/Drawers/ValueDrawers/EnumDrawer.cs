using System;
using ShuHai.Editor.IMGUI;

namespace ShuHai.DebugInspector.Editor
{
    public class EnumDrawer<TOwner, TValue> : ValueDrawer<TOwner, TValue>
        where TValue : struct
    {
        protected override void ValueGUIImpl()
        {
            if (isFlags)
                TypedDrawingValue = EditorGUIEx.EnumFlagsField(TypedDrawingValue);
            else
                typedDrawnValue = EditorGUIEx.EnumPopup(TypedDrawingValue);
        }

        private static readonly bool isFlags = typeof(TValue).IsDefined(typeof(FlagsAttribute), false);
    }
}