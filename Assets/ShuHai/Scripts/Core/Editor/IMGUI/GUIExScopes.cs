using System;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static partial class GUIEx
    {
        #region Colored

        public struct ColoredScope : IDisposable
        {
            public readonly Color ScopedColor;
            public readonly Color ReservedColor;

            public ColoredScope(Color scopedColor)
            {
                ScopedColor = scopedColor;

                ReservedColor = GUI.color;
                GUI.color = scopedColor;

                disposed = false;
            }

            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;

                GUI.color = ReservedColor;
            }

            private bool disposed;
        }

        public struct ColoredContentScope : IDisposable
        {
            public readonly Color ScopedColor;
            public readonly Color ReservedColor;

            public ColoredContentScope(Color scopedColor)
            {
                ScopedColor = scopedColor;

                ReservedColor = GUI.contentColor;
                GUI.contentColor = scopedColor;

                disposed = false;
            }

            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;

                GUI.contentColor = ReservedColor;
            }

            private bool disposed;
        }

        public class ColoredBackgroundScope : IDisposable
        {
            public readonly Color ScopedColor;
            public readonly Color ReservedColor;

            public ColoredBackgroundScope(Color scopedColor)
            {
                ScopedColor = scopedColor;

                ReservedColor = GUI.backgroundColor;
                GUI.backgroundColor = scopedColor;

                disposed = false;
            }

            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;

                GUI.backgroundColor = ReservedColor;
            }

            private bool disposed;
        }

        #endregion Colored
    }
}