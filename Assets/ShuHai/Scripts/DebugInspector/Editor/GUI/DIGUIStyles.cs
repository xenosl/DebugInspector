using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     <see cref="GUIStyle" />s for Debug Inspector.
    ///     The name "DI" is short for Debug Inspector.
    /// </summary>
    internal static class DIGUIStyles
    {
        #region Exception Label

        public static GUIStyle ExceptionLabel { get { return exceptionLabel.Value; } }

        private static readonly Lazy<GUIStyle> exceptionLabel = new Lazy<GUIStyle>(CreateExceptionLabel);

        private static GUIStyle CreateExceptionLabel()
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                name = "ExceptionLabel",
                normal = { textColor = Colors.Red }
            };
            return style;
        }

        #endregion

        #region Foldout Toggle

        public static GUIStyle FoldoutToggle { get { return foldoutToggle.Value; } }

        private static readonly Lazy<GUIStyle> foldoutToggle = new Lazy<GUIStyle>(CreateFoldoutToggle);

        private static GUIStyle CreateFoldoutToggle()
        {
            var builtinFoldout = EditorStyles.foldout;
            var style = new GUIStyle(EditorStyles.toggle) { name = "FoldoutToggle" };

            ZeroHorizontal(style.margin);
            ZeroHorizontal(style.padding);

            builtinFoldout.normal.CopyTo(style.normal);
            builtinFoldout.onNormal.CopyTo(style.onNormal);
            builtinFoldout.active.CopyTo(style.active);
            builtinFoldout.onActive.CopyTo(style.onActive);
            builtinFoldout.focused.CopyTo(style.focused);
            builtinFoldout.onFocused.CopyTo(style.onFocused);

            return style;
        }

        #endregion

        #region Error Icon

        public static GUIStyle ErrorIcon { get { return errorIcon.Value; } }

        private static readonly Lazy<GUIStyle> errorIcon = new Lazy<GUIStyle>(CreateErrorIcon);

        private static GUIStyle CreateErrorIcon()
        {
            var style = new GUIStyle(EditorStyles.toggle) { name = "ErrorIcon" };

            ZeroHorizontal(style.margin);
            ZeroHorizontal(style.padding);

            style.normal.background = DIIcons.Error;
            style.onNormal.background = DIIcons.Error;
            style.active.background = null;
            style.onActive.background = null;
            style.focused.background = null;
            style.onFocused.background = null;

            return style;
        }

        #endregion

        #region Settings Button

        public static GUIStyle SettingsButton { get { return settingsButton.Value; } }

        private static readonly Lazy<GUIStyle> settingsButton = new Lazy<GUIStyle>(CreateSettingsButton);

        private static GUIStyle CreateSettingsButton()
        {
            var style = new GUIStyle { name = "SettingsButton" };

            var padding = style.padding;
            padding.top = 3;
            padding.bottom = 2;

            return style;
        }

        #endregion

        #region Go Up Button

        public static GUIStyle GoUpButton { get { return goUpButton.Value; } }

        private static readonly Lazy<GUIStyle> goUpButton = new Lazy<GUIStyle>(CreateGoUpButton);

        private static GUIStyle CreateGoUpButton()
        {
            var style = new GUIStyle { name = "GoUpButton" };

            var margin = style.margin;
            ZeroHorizontal(margin);

            var padding = style.padding;
            padding.top = 2;
            ZeroHorizontal(style.padding);

            return style;
        }

        #endregion

        #region Page Flip Button

        public static GUIStyle PageFlipButton { get { return pageFlipButton.Value; } }

        private static readonly Lazy<GUIStyle> pageFlipButton = new Lazy<GUIStyle>(CreatePageFlipButton);

        private static GUIStyle CreatePageFlipButton()
        {
            var style = new GUIStyle(EditorStyles.miniButton) { name = "PageFlipButton" };

            var padding = style.padding;
            padding.top = 4;
            padding.bottom = 4;

            return style;
        }

        #endregion

        #region Info Button

        public static GUIStyle InfoButton { get { return infoButton.Value; } }

        private static readonly Lazy<GUIStyle> infoButton = new Lazy<GUIStyle>(CreateInfoButton);

        private static GUIStyle CreateInfoButton()
        {
            var style = new GUIStyle { name = "InfoButton" };

            var padding = style.padding;
            padding.top = 2;
            padding.bottom = 1;

            return style;
        }

        #endregion

        #region Collection Box

        public static GUIStyle CollectionBox { get { return collectionBox.Value; } }

        private static readonly Lazy<GUIStyle> collectionBox = new Lazy<GUIStyle>(CreateCollectionBox);

        private static GUIStyle CreateCollectionBox()
        {
            var box = EditorStyles.helpBox;
            var style = new GUIStyle { name = "CollectionBox" };

            ZeroHorizontal(style.margin);
            ZeroHorizontal(style.padding);

            box.normal.CopyTo(style.normal);
            box.border.CopyTo(style.border);

            return style;
        }

        #endregion

        #region Utilities

        private static void Zero(RectOffset offset)
        {
            ZeroHorizontal(offset);
            ZeroVertical(offset);
        }

        private static void ZeroHorizontal(RectOffset offset)
        {
            offset.left = 0;
            offset.right = 0;
        }

        private static void ZeroVertical(RectOffset offset)
        {
            offset.top = 0;
            offset.bottom = 0;
        }

        #endregion
    }
}