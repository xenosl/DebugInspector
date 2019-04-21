using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static class GUIContents
    {
        #region Temp Instances

        public static GUIContent Temp1(string text) { return Temp(temp1, text); }
        public static GUIContent Temp1(string text, Texture image) { return Temp(temp1, text, image); }
        public static GUIContent Temp1(string text, string tooltip) { return Temp(temp1, text, tooltip); }
        public static GUIContent Temp1(Texture image) { return Temp(temp1, image); }
        public static GUIContent Temp1(Texture image, string tooltip) { return Temp(temp1, image, tooltip); }

        /// <summary>
        ///     Gets a temporary <see cref="GUIContent" /> with specified parameters.
        /// </summary>
        /// <param name="text">The text string for the <see cref="GUIContent" />.</param>
        /// <param name="image">The image texture for the <see cref="GUIContent" />.</param>
        /// <param name="tooltip">The tooltip string for the <see cref="GUIContent" />.</param>
        /// <returns>
        ///     A temporary <see cref="GUIContent" /> instance with specified <paramref name="text" />, <paramref name="image" />
        ///     and <paramref name="tooltip" />.
        /// </returns>
        public static GUIContent Temp1(string text, Texture image, string tooltip)
        {
            return Temp(temp1, text, image, tooltip);
        }

        public static GUIContent Temp2(string text) { return Temp(temp2, text); }
        public static GUIContent Temp2(string text, string tooltip) { return Temp(temp2, text, tooltip); }
        public static GUIContent Temp2(string text, Texture image) { return Temp(temp2, text, image); }
        public static GUIContent Temp2(Texture image) { return Temp(temp2, image); }
        public static GUIContent Temp2(Texture image, string tooltip) { return Temp(temp2, image, tooltip); }

        /// <summary>
        ///     Similar with <see cref="Temp1(string,Texture,string)" /> but a different instance.
        /// </summary>
        public static GUIContent Temp2(string text, Texture image, string tooltip)
        {
            return Temp(temp2, text, image, tooltip);
        }

        private static readonly GUIContent temp1 = new GUIContent(string.Empty);
        private static readonly GUIContent temp2 = new GUIContent(string.Empty);

        private static GUIContent Temp(GUIContent content, string text) { return Temp(content, text, null, null); }

        private static GUIContent Temp(GUIContent content, Texture image) { return Temp(content, null, image, null); }

        private static GUIContent Temp(GUIContent content, Texture image, string tooltip)
        {
            return Temp(content, null, image, tooltip);
        }

        private static GUIContent Temp(GUIContent content, string text, string tooltip)
        {
            return Temp(content, text, null, tooltip);
        }

        private static GUIContent Temp(GUIContent content, string text, Texture image)
        {
            return Temp(content, text, image, null);
        }

        private static GUIContent Temp(GUIContent content, string text, Texture image, string tooltip)
        {
            content.text = text;
            content.tooltip = tooltip;
            content.image = image;
            return content;
        }

        #endregion
    }
}