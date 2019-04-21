using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor
{
    public static class Icons
    {
        public static Texture2D Open { get { return GetOrLoad(ref open, "Open.png"); } }
        private static Texture2D open;

        public static Texture2D Copy { get { return GetOrLoad(ref copy, "Copy.png"); } }
        private static Texture2D copy;

        public static Texture2D HexadecimalOn { get { return GetOrLoad(ref hexadecimalOn, "HexadecimalOn.png"); } }
        private static Texture2D hexadecimalOn;

        public static Texture2D HexadecimalOff { get { return GetOrLoad(ref hexadecimalOff, "HexadecimalOff.png"); } }
        private static Texture2D hexadecimalOff;

        /// <summary>
        ///     Get icon texture stored in specified field if exist; or load icon texture with specified name and assigned it to
        ///     the specified field.
        /// </summary>
        /// <param name="field">Field to get.</param>
        /// <param name="name">Name of the icon texture.</param>
        /// <returns>A <see cref="Texture2D" /> reference which named <paramref name="name" />.</returns>
        public static Texture2D GetOrLoadBuiltin(ref Texture2D field, string name)
        {
            if (!field)
                field = (Texture2D)EditorGUIUtility.Load(name);
            return field;
        }

        public static Texture2D GetOrLoad(ref Texture2D field, string subFolder, string filename)
        {
            return Assets.GetOrLoad(ref field, "Icons/" + subFolder + '/' + filename);
        }

        private static Texture2D GetOrLoad(ref Texture2D field, string filename)
        {
            return GetOrLoad(ref field, "Core", filename);
        }
    }
}