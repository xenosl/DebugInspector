using System;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    using UObject = UnityEngine.Object;

    /// <summary>
    ///     Extra features for drawing IMGUI.
    /// </summary>
    public static partial class EditorGUIEx
    {
        #region Sizes

        /// <summary>
        ///     Actual indent size.
        /// </summary>
        public static float Indent { get { return EditorGUI.indentLevel * IndentPerLevel; } }

        /// <summary>
        ///     Unit size of every <see cref="EditorGUI.indentLevel" />.
        /// </summary>
        public static readonly float IndentPerLevel =
            CommonMethodsEmitter.CreateStaticFieldGetter<EditorGUI, float>("kIndentPerLevel")();

        #endregion Sizes

        #region Fields

        #region Object

        /// <summary>
        ///     Generic version of <see cref="EditorGUILayout.ObjectField(UObject, Type, bool, GUILayoutOption[])" />.
        /// </summary>
        /// <typeparam name="T"> Type of the object to draw. </typeparam>
        public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UObject
        {
            return (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options);
        }

        /// <summary>
        ///     Generic version of
        ///     <see cref="EditorGUILayout.ObjectField(string, UObject, Type, bool, GUILayoutOption[])" />.
        /// </summary>
        /// <typeparam name="T"> Type of the object to draw. </typeparam>
        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UObject
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
        }

        /// <summary>
        ///     Generic version of
        ///     <see cref="EditorGUILayout.ObjectField(GUIContent, UObject, Type, bool, GUILayoutOption[])" />.
        /// </summary>
        /// <typeparam name="T"> Type of the object to draw. </typeparam>
        public static T ObjectField<T>(
            GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UObject
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
        }

        /// <summary>
        ///     Same as <see cref="ObjectField{T}(T, bool, GUILayoutOption[])" />, except that scene objects is not allowed.
        /// </summary>
        public static T AssetObjectField<T>(T obj, params GUILayoutOption[] options)
            where T : UObject
        {
            return ObjectField(obj, false, options);
        }

        /// <summary>
        ///     Same as <see cref="ObjectField{T}(string, T, bool, GUILayoutOption[])" />, except that scene objects is not
        ///     allowed.
        /// </summary>
        public static T AssetObjectField<T>(string label, T obj, params GUILayoutOption[] options)
            where T : UObject
        {
            return ObjectField(label, obj, false, options);
        }

        /// <summary>
        ///     Same as <see cref="ObjectField{T}(GUIContent, T, bool, GUILayoutOption[])" />, except that scene objects is
        ///     not allowed.
        /// </summary>
        public static T AssetObjectField<T>(GUIContent label, T obj, params GUILayoutOption[] options)
            where T : UObject
        {
            return ObjectField(label, obj, false, options);
        }

        #endregion Object

        #region Enum

        #region Popup

        public static T EnumPopup<T>(T value, params GUILayoutOption[] options)
            where T : struct
        {
            EnsureEnumType<T>();
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumPopup(EnumCast.ToEnum(value), options));
        }

        public static T EnumPopup<T>(string label, T value, params GUILayoutOption[] options)
            where T : struct
        {
            EnsureEnumType<T>();
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumPopup(label, EnumCast.ToEnum(value), options));
        }

        public static T EnumPopup<T>(GUIContent label, T value, params GUILayoutOption[] options)
            where T : struct
        {
            EnsureEnumType<T>();
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumPopup(label, EnumCast.ToEnum(value), options));
        }

        #endregion Popup

        #region Flags

        public static T EnumFlagsField<T>(T value, params GUILayoutOption[] options)
            where T : struct
        {
            EnsureEnumType<T>();
#if UNITY_2017_3_OR_NEWER
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumFlagsField(EnumCast.ToEnum(value), options));
#else
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumMaskField(EnumCast.ToEnum(value), options));
#endif
        }

        public static T EnumFlagsField<T>(string label, T value, params GUILayoutOption[] options)
            where T : struct
        {
            EnsureEnumType<T>();
#if UNITY_2017_3_OR_NEWER
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumFlagsField(label, EnumCast.ToEnum(value), options));
#else
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumMaskField(label, EnumCast.ToEnum(value), options));
#endif
        }

        public static T EnumFlagsField<T>(GUIContent label, T value, params GUILayoutOption[] options)
            where T : struct
        {
            EnsureEnumType<T>();
#if UNITY_2017_3_OR_NEWER
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumFlagsField(label, EnumCast.ToEnum(value), options));
#else
            return EnumCast.FromEnum<T>(EditorGUILayout.EnumMaskField(label, EnumCast.ToEnum(value), options));
#endif
        }

        #endregion Flags

        private static void EnsureEnumType<T>()
            where T : struct
        {
            Ensure.Argument.Satisfy(typeof(T).IsEnum, "value", "Enum type expected.");
        }

        #endregion Enum

        #region Label

        public static void MinWidthLabelField(string label, float redundantWidth = 2)
        {
            MinWidthLabelField(label, EditorStyles.label, redundantWidth);
        }

        public static void MinWidthLabelField(string label, GUIStyle style, float redundantWidth = 2)
        {
            style = style ?? EditorStyles.label;
            var width = CalcMinWidth(style, label, redundantWidth);
            EditorGUILayout.LabelField(label, GUILayout.Width(width));
        }

        #endregion Label

        #region Text

        public const int MaxTextLength = 120;

        public static string TextField(string text, params GUILayoutOption[] options)
        {
            return GUILayout.TextField(CutOffTextIfTooLong(text), options);
        }

        public static string TextField(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.TextField(CutOffTextIfTooLong(text), style, options);
        }

        public static string TextField(string label, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            text = GUILayout.TextField(CutOffTextIfTooLong(text), options);
            EditorGUILayout.EndHorizontal();
            return text;
        }

        public static string TextField(GUIContent label, string text, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            text = GUILayout.TextField(CutOffTextIfTooLong(text), options);
            EditorGUILayout.EndHorizontal();
            return text;
        }

        private static GUIContent CutOffTextIfTooLong(GUIContent content)
        {
            return CutOffIfTooLong(content, MaxTextLength);
        }

        private static string CutOffTextIfTooLong(string str) { return CutOffIfTooLong(str, MaxTextLength); }

        #endregion Text

        #region Number

        public static int MinSizeDelayedIntField(int value, float redundantWidth = 2, float redundantHeight = 1)
        {
            return MinSizeDelayedIntField(value, EditorStyles.numberField, redundantWidth, redundantHeight);
        }

        public static int MinSizeDelayedIntField(
            int value, GUIStyle style, float redundantWidth = 2, float redundantHeight = 1)
        {
            style = style ?? EditorStyles.numberField;
            var size = CalcMinSize(style, value.ToString(), redundantWidth, redundantHeight);
            return EditorGUILayout.DelayedIntField(value, style, GUILayout.Width(size.x), GUILayout.Height(size.y));
        }

        public static int MinWidthDelayedIntField(int value, float redundantWidth = 2)
        {
            return MinWidthDelayedIntField(value, EditorStyles.numberField, redundantWidth);
        }

        /// <summary>
        ///     Make a delayed int field for entering integers with minimum width that can contain the entered value.
        /// </summary>
        /// <param name="value"> The value to edit. </param>
        /// <param name="style">
        ///     The <see cref="GUIStyle" /> used to make the field. <see cref="EditorStyles.numberField" /> is used if its
        ///     <see langword="null" />.
        /// </param>
        /// <param name="redundantWidth"> Extra width to make the field look good. </param>
        /// <returns>
        ///     The value entered by the user. Note that the return value will not change until the user has pressed enter or focus
        ///     is moved away from the int field.
        /// </returns>
        public static int MinWidthDelayedIntField(int value, GUIStyle style, float redundantWidth = 2)
        {
            style = style ?? EditorStyles.numberField;
            var width = CalcMinWidth(style, value.ToString(), redundantWidth);
            return EditorGUILayout.DelayedIntField(value, style, GUILayout.Width(width));
        }

        #endregion Number

        #endregion Fields

        #region Button

        public static bool ImageButton(Texture2D image) { return ImageButton(image, image.width, image.height); }

        public static bool ImageButton(Texture2D image, float size) { return ImageButton(image, size, size); }

        public static bool ImageButton(Texture2D image, Vector2 size) { return ImageButton(image, size.x, size.y); }

        public static bool ImageButton(Texture2D image, float width, float height)
        {
            var rect = GUILayoutUtility.GetRect(width, height, GUILayout.Width(width), GUILayout.Height(height));
            return GUI.Button(rect, GUIContents.Temp1(image), EditorStylesEx.ImageButton);
        }

        #endregion Button

        #region Utilities

        private static GUIContent CutOffIfTooLong(GUIContent content, int maxLength)
        {
            content.text = CutOffIfTooLong(content.text, maxLength);
            content.tooltip = CutOffIfTooLong(content.tooltip, maxLength);
            return content;
        }

        private static string CutOffIfTooLong(string str, int maxLength)
        {
            if (str == null)
                return null;
            if (str.Length > maxLength)
                str = str.Substring(0, maxLength);
            return str;
        }

        private static Vector2 CalcMinSize(
            GUIStyle style, string str, float redundantWidth = 0, float redundantHeight = 0)
        {
            var size = style.CalcSize(GUIContents.Temp1(str));
            size.x += redundantWidth;
            size.y += redundantHeight;
            return size;
        }

        private static float CalcMinWidth(GUIStyle style, string str, float redundant = 0)
        {
            float min, max;
            style.CalcMinMaxWidth(GUIContents.Temp1(str), out min, out max);
            return min + redundant;
        }

        #endregion Utilities
    }
}