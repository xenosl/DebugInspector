using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public static class DIGUIContents
    {
        public static GUIContent SettingsButton { get { return GetOrCreate(ref settingsButton, DIIcons.Settings); } }
        private static GUIContent settingsButton;

        public static GUIContent PrevButton { get { return GetOrCreate(ref prevButton, DIIcons.Previous); } }
        private static GUIContent prevButton;

        public static GUIContent NextButton { get { return GetOrCreate(ref nextButton, DIIcons.Next); } }
        private static GUIContent nextButton;

        public static GUIContent DebugInfoButton
        {
            get { return GetOrCreate(ref debugInfoButton, DIIcons.Info, "Print Debug Info"); }
        }

        private static GUIContent debugInfoButton;

        public static GUIContent GetOrCreate(ref GUIContent storeField, string text)
        {
            return GetOrCreate(ref storeField, text, null, string.Empty);
        }

        public static GUIContent GetOrCreate(ref GUIContent storeField, Texture2D image)
        {
            return GetOrCreate(ref storeField, string.Empty, image, string.Empty);
        }

        public static GUIContent GetOrCreate(ref GUIContent storeField, Texture2D image, string tooltip)
        {
            return GetOrCreate(ref storeField, string.Empty, image, tooltip);
        }

        public static GUIContent GetOrCreate(ref GUIContent storeField, string text, Texture2D image, string tooltip)
        {
            if (storeField == null)
                storeField = new GUIContent(text, image, tooltip);
            return storeField;
        }
    }
}