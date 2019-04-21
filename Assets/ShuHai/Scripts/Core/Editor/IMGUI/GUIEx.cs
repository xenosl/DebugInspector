using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public static partial class GUIEx
    {
        #region Graphs

        /// <summary>
        /// Default color for line painting.
        /// This value is used if the color argument of the line drawing method call is omitted.
        /// </summary>
        public static readonly Color LineColor = Colors.DimGray;

        public static void HorizontalLine()
        {
            HorizontalLine(LineColor);
        }

        public static void HorizontalLine(Color color)
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            HorizontalLine(lastRect.x, lastRect.yMax, lastRect.width, color);
        }

        public static void HorizontalLine(float length)
        {
            HorizontalLine(length, LineColor);
        }

        public static void HorizontalLine(float length, Color color)
        {
            var rect = GUILayoutUtility.GetRect(length, 0);
            HorizontalLine(rect.x, rect.center.y, length, color);
        }

        public static void HorizontalLine(float x, float y, float length)
        {
            HorizontalLine(x, y, length, LineColor);
        }

        /// <summary>
        /// Draw a horizontal line at specified position with specified length and color.
        /// </summary>
        /// <param name="x"> The x coordinate of the line start position. </param>
        /// <param name="y"> The y coordinate of the line start position. </param>
        /// <param name="length"> Length of the line. </param>
        /// <param name="color"> Color of the line. </param>
        public static void HorizontalLine(float x, float y, float length, Color color)
        {
            Graph.Line(new Vector2(x, y), new Vector2(x + length, y), color);
        }

        public static void LastRectFrame()
        {
            LastRectFrame(Graph.DefaultColor);
        }

        public static void LastRectFrame(Color color)
        {
            var rect = GUILayoutUtility.GetLastRect();
            Graph.Rect(rect, color);
        }

        #endregion Graphs
    }
}