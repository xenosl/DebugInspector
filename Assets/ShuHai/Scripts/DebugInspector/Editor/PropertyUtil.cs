using System;
using System.Collections.Generic;

namespace ShuHai.DebugInspector.Editor
{
    internal static class PropertyUtil
    {
        public static void SetValue<T>(ref T field, T value, Action<T> action, Action<T> notification)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            var old = field;
            field = value;

            if (action != null)
                action(old);

            if (notification != null)
                notification(old);
        }
    }
}