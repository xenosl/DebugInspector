using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static class GUILayoutOptionExtensions
    {
        public static void SetValue(this GUILayoutOption self, object value) { valueSetter(ref self, value); }

        public static object GetValue(this GUILayoutOption self) { return valueGetter(ref self); }

        private static readonly ThisValueSetter<GUILayoutOption, object> valueSetter =
            CommonMethodsEmitter.CreateInstanceFieldSetter<GUILayoutOption, object>("value");

        private static readonly ThisValueGetter<GUILayoutOption, object> valueGetter =
            CommonMethodsEmitter.CreateInstanceFieldGetter<GUILayoutOption, object>("value");
    }
}