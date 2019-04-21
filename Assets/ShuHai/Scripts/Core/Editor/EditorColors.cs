using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor
{
    public static class EditorColors
    {
        /// <summary>
        ///     Default background of empty <see cref="EditorWindow" />.
        /// </summary>
        public static readonly Color DefaultBackground = Colors.New(194, 194, 194);

        /// <summary>
        ///     Brighter background of editor windows.
        ///     Example: Console, Test Runner.
        /// </summary>
        public static readonly Color BrightBackground = Colors.New(222, 222, 222);

        /// <summary>
        ///     Background of selected object in focused window.
        /// </summary>
        public static readonly Color FocusedSelected = Colors.New(65, 125, 231);

        /// <summary>
        ///     Background of selected object in unfocused window.
        /// </summary>
        public static readonly Color UnfocusedSelected = Colors.New(143, 143, 143);
    }
}