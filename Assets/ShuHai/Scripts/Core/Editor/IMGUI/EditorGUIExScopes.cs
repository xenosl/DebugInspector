using System;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static partial class EditorGUIEx
    {
        /// <summary>
        ///     Similar to <see cref="EditorGUI.DisabledScope" /> except this one is not affected by current state of
        ///     <see cref="GUI.enabled" />.
        /// </summary>
        public struct DisabledScope : IDisposable
        {
            public readonly bool ReservedDisabled;

            public DisabledScope(bool scopedDisabled)
            {
                ReservedDisabled = !GUI.enabled;
                GUI.enabled = !scopedDisabled;

                disposed = false;
            }

            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;

                GUI.enabled = !ReservedDisabled;
            }

            private bool disposed;
        }

        /// <remarks>
        ///     This class existed for following reasons:
        ///     1. EditorGUI.IndentLevelScope dosen't exist before Unity-2017.1.
        ///     2. EditorGUI.IndentLevelScope is a class and causes heap allocation.
        /// </remarks>
        public struct IndentLevelScope : IDisposable
        {
            public readonly int IndentOffset;

            public IndentLevelScope(int indentOffset)
            {
                IndentOffset = indentOffset;

                EditorGUI.indentLevel += IndentOffset;

                disposed = false;
            }

            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;

                EditorGUI.indentLevel -= IndentOffset;
            }

            private bool disposed;
        }
    }
}