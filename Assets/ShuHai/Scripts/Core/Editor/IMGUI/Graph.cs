using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    /// <summary>
    ///     Draw simple graphs on Unity's GUI.
    /// </summary>
    public static class Graph
    {
        /// <summary>
        ///     Color used when drawing if color argument is omitted.
        /// </summary>
        public static readonly Color DefaultColor = Colors.DimGray;

        /// <summary>
        ///     Cross size used when calling <see cref="Cross(Vector2, Color)" />.
        /// </summary>
        public const float DefaultCrossSize = 12f;

        #region Materials

        /// <summary>
        ///     Material used when drawing.
        ///     <see cref="DefaultMaterial" /> is used if null.
        /// </summary>
        public static Material Material;

        /// <summary>
        ///     Material used if <see cref="Material" /> is null when drawing.
        /// </summary>
        public static Material DefaultMaterial
        {
            get
            {
                return defaultMaterial ? defaultMaterial
                    : (defaultMaterial = new Material(Shader.Find("Hidden/Internal-Colored")));
            }
        }

        private static Material defaultMaterial;

        #endregion Materials

        public static void Line(Vector2 from, Vector2 to) { Line(from, to, DefaultColor); }

        /// <summary>
        ///     Draw a line on GUI.
        /// </summary>
        /// <param name="from"> Start position of the line. </param>
        /// <param name="to"> End position of the line. </param>
        /// <param name="color"> Color of the line. </param>
        public static void Line(Vector2 from, Vector2 to, Color color)
        {
            if (!PrepareDraw())
                return;

            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(from.x, from.y, 0);
            GL.Vertex3(to.x, to.y, 0);
            GL.End();

            FinishDraw();
        }

        public static void Polyline(IEnumerable<Vector2> points, Color color)
        {
            if (!PrepareDraw())
                return;

            GL.Begin(GL.LINES);
            GL.Color(color);

            var list = points as IList<Vector2>;
            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                    GL.Vertex3(list[i].x, list[i].y, 0);
            }
            else
            {
                foreach (var p in points)
                    GL.Vertex3(p.x, p.y, 0);
            }

            GL.End();

            FinishDraw();
        }

        public static void Cross(Vector2 position) { Cross(position, DefaultColor); }

        public static void Cross(Vector2 position, Color color) { Cross(position, DefaultCrossSize, color); }

        /// <summary>
        ///     Draw a cross on GUI.
        /// </summary>
        /// <param name="position"> Position of the cross. </param>
        /// <param name="size"> Size of the cross. </param>
        /// <param name="color"> Color of the cross. </param>
        public static void Cross(Vector2 position, float size, Color color)
        {
            if (!PrepareDraw())
                return;

            GL.Begin(GL.LINES);
            GL.Color(color);

            float hs = size / 2;
            GL.Vertex3(position.x - hs, position.y, 0);
            GL.Vertex3(position.x + hs, position.y, 0);
            GL.Vertex3(position.x, position.y - hs, 0);
            GL.Vertex3(position.x, position.y + hs, 0);

            GL.End();

            FinishDraw();
        }

        //public static void Triangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        //{
        //    if (!PrepareDraw())
        //        return;

        //    GL.Begin(GL.TRIANGLES);
        //    GL.Color(color);
        //    GL.Vertex3(p1.x, p1.x, 0);
        //    GL.Vertex3(p2.x, p2.x, 0);
        //    GL.Vertex3(p3.x, p3.x, 0);
        //    GL.End();

        //    FinishDraw();
        //}

        public static void Rect(Rect rect) { Rect(rect, DefaultColor); }

        /// <summary>
        ///     Draw a rectangle on GUI.
        /// </summary>
        /// <param name="rect"> Position and size of the rectangle. </param>
        /// <param name="color">Color of the frame.</param>
        /// <param name="solid">Indicates whether the rectangle is solid.</param>
        public static void Rect(Rect rect, Color color, bool solid = false)
        {
            if (!PrepareDraw())
                return;

            GL.Begin(solid ? GL.QUADS : GL.LINES);
            GL.Color(color);
            if (solid)
            {
                GL.Vertex3(rect.xMin, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMax, 0);
                GL.Vertex3(rect.xMin, rect.yMax, 0);
            }
            else
            {
                // Bottom-Left to Bottom-Right
                GL.Vertex3(rect.xMin, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMin, 0);
                // Bottom-Right to Top-Right
                GL.Vertex3(rect.xMax, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMax, 0);
                // Top-Right to Top-Left
                GL.Vertex3(rect.xMax, rect.yMax, 0);
                GL.Vertex3(rect.xMin, rect.yMax, 0);
                // Top-Left to Bottom-Left
                GL.Vertex3(rect.xMin, rect.yMax, 0);
                GL.Vertex3(rect.xMin, rect.yMin, 0);
            }
            GL.End();

            FinishDraw();
        }

        private static bool canDraw { get { return Event.current.type == EventType.Repaint; } }

        private static Material usingMaterial { get { return Material ? Material : DefaultMaterial; } }

        private static bool PrepareDraw()
        {
            if (!canDraw)
                return false;

            GL.PushMatrix();

            GL.Clear(true, false, Color.gray);
            usingMaterial.SetPass(0);

            return true;
        }

        private static void FinishDraw() { GL.PopMatrix(); }
    }
}