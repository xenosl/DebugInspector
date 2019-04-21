using UnityEngine;

namespace ShuHai
{
    public static class Texture2DExtensions
    {
        public static void Fill(this Texture2D self, Color color)
        {
            int w = self.width, h = self.height;
            for (int x = 0; x < w; ++x)
            for (int y = 0; y < h; ++y)
                self.SetPixel(x, y, color);
        }
    }
}