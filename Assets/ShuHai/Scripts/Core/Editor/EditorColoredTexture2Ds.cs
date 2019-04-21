using UnityEngine;

namespace ShuHai.Editor
{
    public static class EditorColoredTexture2Ds
    {
        public static Texture2D DefaultBackground
        {
            get { return ColoredTexture2Ds.GetOrCreate(ref defaultBackground, EditorColors.DefaultBackground); }
        }

        public static Texture2D BrightBackground
        {
            get { return ColoredTexture2Ds.GetOrCreate(ref brightBackground, EditorColors.BrightBackground); }
        }

        public static Texture2D FocusedSelected
        {
            get { return ColoredTexture2Ds.GetOrCreate(ref focusedSelected, EditorColors.FocusedSelected); }
        }

        public static Texture2D UnfocusedSelected
        {
            get { return ColoredTexture2Ds.GetOrCreate(ref unfocusedSelected, EditorColors.UnfocusedSelected); }
        }

        private static Texture2D defaultBackground;
        private static Texture2D brightBackground;
        private static Texture2D focusedSelected;
        private static Texture2D unfocusedSelected;
    }
}