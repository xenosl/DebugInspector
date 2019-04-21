using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     DIGUI is short for DebugInspectorGUI.
    ///     Provides support for drawing custom GUI elements for Debug Inspector.
    /// </summary>
    public static class DIGUI
    {
        #region Foldout Toggle

        /// <summary>
        ///     Width of the foldout toggle rect, also width of the foldout texture.
        /// </summary>
        public const int FoldoutToggleWidth = 13;

        public static Rect GetFoldoutToggleRect()
        {
            return GUILayoutUtility.GetRect(
                FoldoutToggleWidth, EditorGUIUtility.singleLineHeight,
                DIGUIStyles.FoldoutToggle, GUILayout.Width(FoldoutToggleWidth));
        }

        public static bool FoldoutToggle(bool foldout, bool indented)
        {
            var rect = GetFoldoutToggleRect();
            if (indented)
                rect.x += EditorGUIEx.Indent;
            return EditorGUI.Toggle(rect, foldout, DIGUIStyles.FoldoutToggle);
        }

        public static bool FoldoutToggle(Rect rect, bool foldout)
        {
            return EditorGUI.Toggle(rect, foldout, DIGUIStyles.FoldoutToggle);
        }

        #endregion

        #region Error Toggle

        public static readonly Color ErrorToggleOffBgColor = Colors.Silver;
        public static readonly Color ErrorToggleOnBgColor = Colors.New(248, 248, 248);

        public static bool ErrorToggle(bool value)
        {
            var rect = GetErrorIconRect();
            return ErrorToggle(rect, value);
        }

        public static bool ErrorToggle(Rect rect, bool value)
        {
            using (new GUIEx.ColoredBackgroundScope(value ? ErrorToggleOnBgColor : ErrorToggleOffBgColor))
                return EditorGUI.Toggle(rect, value, DIGUIStyles.ErrorIcon);
        }

        #endregion

        #region Error Button

        public static bool ErrorButton(string tooltip)
        {
            var rect = GetErrorIconRect();
            var content = errorButtonContent ?? (errorButtonContent = new GUIContent());
            content.tooltip = tooltip;
            return GUI.Button(rect, content, DIGUIStyles.ErrorIcon);
        }

        private static GUIContent errorButtonContent;

        #endregion

        public static Rect GetErrorIconRect()
        {
            int width = DIIcons.Error.width;
            return GUILayoutUtility.GetRect(
                width, EditorGUIUtility.singleLineHeight,
                DIGUIStyles.ErrorIcon, GUILayout.Width(width));
        }

        #region Go Up Button

        public const int GoUpButtonWidth = 16;

        public static Rect GetGoUpButtonRect()
        {
            return GUILayoutUtility.GetRect(
                GoUpButtonWidth, EditorGUIUtility.singleLineHeight,
                DIGUIStyles.GoUpButton, GUILayout.Width(GoUpButtonWidth));
        }

        public static bool GoUpButton(Rect rect)
        {
            //var rect = GetGoUpButtonRect();
            var content = GUIContents.Temp1(DIIcons.TurnUp);
            return GUI.Button(rect, content, DIGUIStyles.GoUpButton);
        }

        #endregion

        public static bool PageFlipButton(GUIContent content)
        {
            return GUILayout.Button(content, DIGUIStyles.PageFlipButton, DIGUILayoutOptions.PageFlipButton);
        }

        public static bool SettingsButton()
        {
            return GUILayout.Button(DIGUIContents.SettingsButton,
                DIGUIStyles.SettingsButton, DIGUILayoutOptions.SettingsButton);
        }
    }
}