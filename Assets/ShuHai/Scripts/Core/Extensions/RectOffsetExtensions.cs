using UnityEngine;

namespace ShuHai
{
    public static class RectOffsetExtensions
    {
        public static void CopyTo(this RectOffset self, RectOffset other)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(other, "other");

            other.left = self.left;
            other.right = self.right;
            other.top = self.top;
            other.bottom = self.bottom;
        }
    }
}