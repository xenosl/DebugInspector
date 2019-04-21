using System;
using ShuHai.Editor;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    using GUIStyleGetter = Func<GUIStyle>;

    public static class EditorStylesEx
    {
        #region Non-Public Styles Of EditorStyles

        #region Search Field

        public static GUIStyle ToolbarSearchField { get { return toolbarSearchFieldGetter(); } }
        public static GUIStyle ToolbarSearchFieldPopup { get { return toolbarSeachTextFieldPopupGetter(); } }
        public static GUIStyle ToolbarSearchFieldCancelButton { get { return toolbarSearchFieldCancelButtonGetter(); } }

        public static GUIStyle ToolbarSearchFieldCancelButtonEmpty
        {
            get { return toolbarSearchFieldCancelButtonEmptyGetter(); }
        }

        public static GUIStyle SearchField { get { return searchFieldGetter(); } }
        public static GUIStyle SearchFieldCancelButton { get { return searchFieldCancelButtonGetter(); } }
        public static GUIStyle SearchFieldCancelButtonEmpty { get { return searchFieldCancelButtonEmptyGetter(); } }

        private static readonly GUIStyleGetter toolbarSearchFieldGetter = CreateStyleGetter("toolbarSearchField");

        private static readonly GUIStyleGetter toolbarSeachTextFieldPopupGetter =
            CreateStyleGetter("toolbarSearchFieldPopup");

        private static readonly GUIStyleGetter toolbarSearchFieldCancelButtonGetter =
            CreateStyleGetter("toolbarSearchFieldCancelButton");

        private static readonly GUIStyleGetter toolbarSearchFieldCancelButtonEmptyGetter =
            CreateStyleGetter("toolbarSearchFieldCancelButtonEmpty");

        private static readonly GUIStyleGetter searchFieldGetter =
            CreateStyleGetter("searchField");

        private static readonly GUIStyleGetter searchFieldCancelButtonGetter =
            CreateStyleGetter("searchFieldCancelButton");

        private static readonly GUIStyleGetter searchFieldCancelButtonEmptyGetter =
            CreateStyleGetter("searchFieldCancelButtonEmpty");

        #endregion Search Field

        private static GUIStyleGetter CreateStyleGetter(string name)
        {
            return CommonMethodsEmitter.CreateStaticPropertyGetter<EditorStyles, GUIStyle>(name);
        }

        #endregion Non-Public Styles Of EditorStyles

        #region Custom Styles

        #region Image Button

        public static GUIStyle ImageButton { get { return imageButton.Value; } }

        private static readonly Lazy<GUIStyle> imageButton = new Lazy<GUIStyle>(CreateImageButton);

        private static GUIStyle CreateImageButton()
        {
            return new GUIStyle
            {
                name = "ImageButton",
                imagePosition = ImagePosition.ImageOnly,
                padding = { top = 2, bottom = 2, left = 2, right = 2 }
            };
        }

        #endregion Image Button

        #region Hexadecimal Toggle

        public static GUIStyle HexadecimalToggle { get { return hexadecimalToggle.Value; } }

        private static readonly Lazy<GUIStyle> hexadecimalToggle = new Lazy<GUIStyle>(CreateHexadecimalToggle);

        private static GUIStyle CreateHexadecimalToggle()
        {
            Texture2D imageOn = Icons.HexadecimalOn, imageOff = Icons.HexadecimalOff;
            var size = EditorGUIUtility.singleLineHeight - 2;
            return new GUIStyle("BypassToggle")
            {
                name = "HexadecimalToggle",
                imagePosition = ImagePosition.ImageOnly,
                fixedWidth = size,
                fixedHeight = size,
                normal = { background = imageOff },
                onNormal = { background = imageOn },
                hover = { background = imageOff },
                onHover = { background = imageOn },
                active = { background = imageOff },
                onActive = { background = imageOn },
                focused = { background = imageOff },
                onFocused = { background = imageOn }
            };
        }

        #endregion Hexadecimal Toggle

        #endregion Custom Styles
    }
}