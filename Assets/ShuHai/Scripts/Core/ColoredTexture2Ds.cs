using UnityEngine;

namespace ShuHai
{
    public static class ColoredTexture2Ds
    {
        public const int Size = 4;

        public static Texture2D GetOrCreate(ref Texture2D tex, Color color) { return tex ?? (tex = Create(color)); }

        public static Texture2D Create(Color color)
        {
            var tex = new Texture2D(Size, Size);
            tex.Fill(color);
            tex.Apply();
            return tex;
        }
    }
}