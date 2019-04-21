using ShuHai.Editor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Provides access of icons used on GUI of Debug Inspector.
    /// </summary>
    public static class DIIcons
    {
        public static Texture2D Info { get { return Icons.GetOrLoadBuiltin(ref info, "console.infoicon.sml"); } }
        private static Texture2D info;

        public static Texture2D Error { get { return Icons.GetOrLoadBuiltin(ref error, "console.erroricon.sml"); } }
        private static Texture2D error;

        public static Texture2D Settings { get { return GetOrLoad(ref settings, "Settings.png"); } }
        private static Texture2D settings;

        public static Texture2D Previous { get { return GetOrLoad(ref previous, "Previous.png"); } }
        private static Texture2D previous;

        public static Texture2D Next { get { return GetOrLoad(ref next, "Next.png"); } }
        private static Texture2D next;

        public static Texture2D TurnUp { get { return GetOrLoad(ref turnUp, "TurnUp.png"); } }
        private static Texture2D turnUp;

        private static Texture2D GetOrLoad(ref Texture2D field, string filename)
        {
            return Icons.GetOrLoad(ref field, "DebugInspector", filename);
        }
    }
}