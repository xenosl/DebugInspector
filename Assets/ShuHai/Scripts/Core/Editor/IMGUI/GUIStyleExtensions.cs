using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static class GUIStyleExtensions
    {
        public static void CopyTo(this GUIStyleState self, GUIStyleState other)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(other, "other");

            other.textColor = self.textColor;
            other.background = self.background;

//#if UNITY_5_4_OR_NEWER
//            var sbg = self.scaledBackgrounds;
//            other.scaledBackgrounds = sbg != null ? (Texture2D[])sbg.Clone() : null;
//#endif
        }
    }
}