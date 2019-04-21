using UnityEngine;

namespace ShuHai
{
    public static class RectExtensions
    {
        public static float GetArea(this Rect self) { return self.width * self.height; }

        public static Rect Union(this Rect self, Rect other)
        {
            var rect = self;
            rect.xMin = Mathf.Min(self.xMin, other.xMin);
            rect.yMin = Mathf.Min(self.yMin, other.yMin);
            rect.xMax = Mathf.Max(self.xMax, other.xMax);
            rect.yMax = Mathf.Max(self.yMax, other.yMax);
            return rect;
        }
    }
}