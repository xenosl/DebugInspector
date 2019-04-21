using UnityEngine;

namespace ShuHai
{
    public static class RichTextUtil
    {
        public static string Bold(string text)
        {
            return "<b>" + text + "</b>";
        }

        public static string Italic(string text)
        {
            return "<i>" + text + "</i>";
        }

        public static string Size(string text, int size)
        {
            return string.Format("<size={0}>{1}</size>", size, text);
        }

        public static string Color(string text, Color color)
        {
            return string.Format("<color=#{0}>{1}<color>", ColorUtility.ToHtmlStringRGBA(color), text);
        }
    }
}