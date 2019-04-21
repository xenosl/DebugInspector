using System;
using UnityEngine;

namespace ShuHai
{
    public enum RGBAChannels
    {
        R,
        G,
        B,
        A
    }

    [Flags]
    public enum RGBAMasks
    {
        R = 0x1,
        G = 0x2,
        B = 0x4,
        A = 0x8
    }

    /// <summary>
    ///     Shortcut for web colors.
    ///     Color values from http://en.wikipedia.org/wiki/Web_colors
    /// </summary>
    public static class Colors
    {
        #region Pink colors

        public static readonly Color Pink = New(255, 192, 203);
        public static readonly Color LightPink = New(255, 182, 193);
        public static readonly Color HotPink = New(255, 105, 180);
        public static readonly Color DeepPink = New(255, 20, 147);
        public static readonly Color PaleVioletRed = New(219, 112, 147);
        public static readonly Color MediumVioletRed = New(199, 21, 133);

        #endregion Pink colors

        #region Red colors

        public static readonly Color LightSalmon = New(255, 160, 122);
        public static readonly Color Salmon = New(250, 128, 114);
        public static readonly Color DarkSalmon = New(233, 150, 122);
        public static readonly Color LightCoral = New(240, 128, 128);
        public static readonly Color IndianRed = New(205, 92, 92);
        public static readonly Color Crimson = New(220, 20, 60);
        public static readonly Color FireBrick = New(178, 34, 34);
        public static readonly Color DarkRed = New(139, 0, 0);
        public static readonly Color Red = New(255, 0, 0);

        #endregion Red colors

        #region Orange colors

        public static readonly Color OrangeRed = New(255, 69, 0);
        public static readonly Color Tomato = New(255, 99, 71);
        public static readonly Color Coral = New(255, 127, 80);
        public static readonly Color DarkOrange = New(255, 140, 0);
        public static readonly Color Orange = New(255, 165, 0);

        #endregion Orange colors

        #region Yellow colors

        public static readonly Color Yellow = New(255, 255, 0);
        public static readonly Color LightYellow = New(255, 255, 224);
        public static readonly Color LemonChiffon = New(255, 250, 205);
        public static readonly Color LightGoldenrodYellow = New(250, 250, 210);
        public static readonly Color PapayaWhip = New(255, 239, 213);
        public static readonly Color Moccasin = New(255, 228, 181);
        public static readonly Color PeachPuff = New(255, 218, 185);
        public static readonly Color PaleGoldenrod = New(238, 232, 170);
        public static readonly Color Khaki = New(240, 230, 140);
        public static readonly Color DarkKhaki = New(189, 183, 107);
        public static readonly Color Gold = New(255, 215, 0);

        #endregion Yellow colors

        #region Brown colors

        public static readonly Color Cornsilk = New(255, 248, 220);
        public static readonly Color BlanchedAlmond = New(255, 235, 205);
        public static readonly Color Bisque = New(255, 228, 196);
        public static readonly Color NavajoWhite = New(255, 222, 173);
        public static readonly Color Wheat = New(245, 222, 179);
        public static readonly Color BurlyWood = New(222, 184, 135);
        public static readonly Color Tan = New(210, 180, 140);
        public static readonly Color RosyBrown = New(188, 143, 143);
        public static readonly Color SandyBrown = New(244, 164, 96);
        public static readonly Color Goldenrod = New(218, 165, 32);
        public static readonly Color DarkGoldenrod = New(184, 134, 11);
        public static readonly Color Peru = New(205, 133, 63);
        public static readonly Color Chocolate = New(210, 105, 30);
        public static readonly Color SaddleBrown = New(139, 69, 19);
        public static readonly Color Sienna = New(160, 82, 45);
        public static readonly Color Brown = New(165, 42, 42);
        public static readonly Color Maroon = New(128, 0, 0);

        #endregion Brown colors

        #region Green colors

        public static readonly Color DarkOliveGreen = New(85, 107, 47);
        public static readonly Color Olive = New(128, 128, 0);
        public static readonly Color OliveDrab = New(107, 142, 35);
        public static readonly Color YellowGreen = New(154, 205, 50);
        public static readonly Color LimeGreen = New(50, 205, 50);
        public static readonly Color Lime = New(0, 255, 0);
        public static readonly Color LawnGreen = New(124, 252, 0);
        public static readonly Color Chartreuse = New(127, 255, 0);
        public static readonly Color GreenYellow = New(173, 255, 47);
        public static readonly Color SpringGreen = New(0, 255, 127);
        public static readonly Color MediumSpringGreen = New(0, 250, 154);
        public static readonly Color LightGreen = New(144, 238, 144);
        public static readonly Color PaleGreen = New(152, 251, 152);
        public static readonly Color DarkSeaGreen = New(143, 188, 143);
        public static readonly Color MediumSeaGreen = New(60, 179, 113);
        public static readonly Color SeaGreen = New(46, 139, 87);
        public static readonly Color ForestGreen = New(34, 139, 34);
        public static readonly Color Green = New(0, 128, 0);
        public static readonly Color DarkGreen = New(0, 100, 0);

        #endregion Green colors

        #region Cyan colors

        public static readonly Color Aqua = New(0, 255, 255);
        public static readonly Color Cyan = New(0, 255, 255);
        public static readonly Color LightCyan = New(224, 255, 255);
        public static readonly Color PaleTurquoise = New(175, 238, 238);
        public static readonly Color Aquamarine = New(127, 255, 212);
        public static readonly Color Turquoise = New(64, 224, 208);
        public static readonly Color MediumTurquoise = New(72, 209, 204);
        public static readonly Color DarkTurquoise = New(0, 206, 209);
        public static readonly Color LightSeaGreen = New(32, 178, 170);
        public static readonly Color CadetBlue = New(95, 158, 160);
        public static readonly Color DarkCyan = New(0, 139, 139);
        public static readonly Color Teal = New(0, 128, 128);

        #endregion Cyan colors

        #region Blue colors

        public static readonly Color LightSteelBlue = New(176, 196, 222);
        public static readonly Color PowderBlue = New(176, 224, 230);
        public static readonly Color LightBlue = New(173, 216, 230);
        public static readonly Color SkyBlue = New(135, 206, 235);
        public static readonly Color LightSkyBlue = New(135, 206, 250);
        public static readonly Color DeepSkyBlue = New(0, 191, 255);
        public static readonly Color DodgerBlue = New(30, 144, 255);
        public static readonly Color CornflowerBlue = New(100, 149, 237);
        public static readonly Color SteelBlue = New(70, 130, 180);
        public static readonly Color RoyalBlue = New(65, 105, 225);
        public static readonly Color Blue = New(0, 0, 255);
        public static readonly Color MediumBlue = New(0, 0, 205);
        public static readonly Color DarkBlue = New(0, 0, 139);
        public static readonly Color Navy = New(0, 0, 128);
        public static readonly Color MidnightBlue = New(25, 25, 112);

        #endregion Blue colors

        #region Purple/Violet/Magenta colors

        public static readonly Color Lavender = New(230, 230, 250);
        public static readonly Color Thistle = New(216, 191, 216);
        public static readonly Color Plum = New(221, 160, 221);
        public static readonly Color Violet = New(238, 130, 238);
        public static readonly Color Orchid = New(218, 112, 214);
        public static readonly Color Fuchsia = New(255, 0, 255);
        public static readonly Color Magenta = New(255, 0, 255);
        public static readonly Color MediumOrchid = New(186, 85, 211);
        public static readonly Color MediumPurple = New(147, 112, 219);
        public static readonly Color BlueViolet = New(138, 43, 226);
        public static readonly Color DarkViolet = New(148, 0, 211);
        public static readonly Color DarkOrchid = New(153, 50, 204);
        public static readonly Color DarkMagenta = New(139, 0, 139);
        public static readonly Color Purple = New(128, 0, 128);
        public static readonly Color Indigo = New(75, 0, 130);
        public static readonly Color DarkSlateBlue = New(72, 61, 139);
        public static readonly Color SlateBlue = New(106, 90, 205);
        public static readonly Color MediumSlateBlue = New(123, 104, 238);

        #endregion Purple/Violet/Magenta colors

        #region White colors

        public static readonly Color White = New(255, 255, 255);
        public static readonly Color Snow = New(255, 250, 250);
        public static readonly Color Honeydew = New(240, 255, 240);
        public static readonly Color MintCream = New(245, 255, 250);
        public static readonly Color Azure = New(240, 255, 255);
        public static readonly Color AliceBlue = New(240, 248, 255);
        public static readonly Color GhostWhite = New(248, 248, 255);
        public static readonly Color WhiteSmoke = New(245, 245, 245);
        public static readonly Color Seashell = New(255, 245, 238);
        public static readonly Color Beige = New(245, 245, 220);
        public static readonly Color OldLace = New(253, 245, 230);
        public static readonly Color FloralWhite = New(255, 250, 240);
        public static readonly Color Ivory = New(255, 255, 240);
        public static readonly Color AntiqueWhite = New(250, 235, 215);
        public static readonly Color Linen = New(250, 240, 230);
        public static readonly Color LavenderBlush = New(255, 240, 245);
        public static readonly Color MistyRose = New(255, 228, 225);

        #endregion White colors

        #region Gray/Black colors

        public static readonly Color Gainsboro = New(220, 220, 220);
        public static readonly Color LightGrey = New(211, 211, 211);
        public static readonly Color Silver = New(192, 192, 192);
        public static readonly Color DarkGray = New(169, 169, 169);
        public static readonly Color Gray = New(128, 128, 128);
        public static readonly Color DimGray = New(105, 105, 105);
        public static readonly Color LightSlateGray = New(119, 136, 153);
        public static readonly Color SlateGray = New(112, 128, 144);
        public static readonly Color DarkSlateGray = New(47, 79, 79);
        public static readonly Color Black = New(0, 0, 0);

        #endregion Gray/Black colors

        #region Utilities

        /// <summary>
        ///     Copy <paramref name="self" /> and set <paramref name="channel" /> to <paramref name="value" />,
        ///     then return the copied color.
        /// </summary>
        /// <param name="self">Color to copy.</param>
        /// <param name="channel">Channel to set.</param>
        /// <param name="value">Channel value to set.</param>
        /// <returns>
        ///     Color value with given <paramref name="channel" /> set to <paramref name="value" />
        /// </returns>
        /// <remarks>
        ///     This method is not named "SetChannel" since Color is a value type, <paramref name="self" /> is
        ///     passed by value, and setting its field by extension method is actually setting the field of
        ///     the copied Color value of <paramref name="self" />. In order to get this method work, it requires
        ///     the caller use its return value assign to which meant to be changed.
        /// </remarks>
        public static Color Channelled(this Color self, RGBAChannels channel, float value)
        {
            self[(int)channel] = value;
            return self;
        }

        public static float GetChannel(this Color self, RGBAChannels channel) { return self[(int)channel]; }

        public static Color AlphaBlend(Color src, Color dst)
        {
            float invSrcA = 1 - src.a;
            Color c = dst;
            c.a = src.a + dst.a * invSrcA;
            for (int i = 0; i < 3; ++i) // 0,1,2 => r,g,b
                c[i] = src[i] * src.a + dst[i] * invSrcA;
            //c[i] = src[i] * src.a + dst[i] * dst.a * invSrcA;
            return c;
        }

        public static Color New(byte r, byte g, byte b, byte a = 255)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        #endregion
    }
}