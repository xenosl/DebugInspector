using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Cached <see cref="GUILayoutOption" /> arrays for using as parameters. This avoid heap allocation when using
    ///     <see cref="GUILayoutOption" />s.
    /// </summary>
    public static class DIGUILayoutOptions
    {
        public static readonly GUILayoutOption[] SettingsButton = { GUILayout.Width(16), GUILayout.Height(16) };
        public static readonly GUILayoutOption[] PageFlipButton = { GUILayout.Width(24), GUILayout.Height(16) };
    }
}