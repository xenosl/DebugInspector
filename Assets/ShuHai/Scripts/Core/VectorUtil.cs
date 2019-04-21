using System;
using UnityEngine;

namespace ShuHai
{
    public static class VectorUtil
    {
        #region Verfication

        public static bool IsNaN(Vector2 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y);
        }

        public static bool IsNaN(Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }

        public static bool IsNaN(Vector4 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsNaN(v.w);
        }

        public static bool IsInfinity(Vector2 v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y);
        }

        public static bool IsInfinity(Vector3 v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
        }

        public static bool IsInfinity(Vector4 v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y)
                || float.IsInfinity(v.z) || float.IsInfinity(v.w);
        }

        public static bool IsValid(Vector2 v)
        {
            return !(IsNaN(v) || IsInfinity(v));
        }

        public static bool IsValid(Vector3 v)
        {
            return !(IsNaN(v) || IsInfinity(v));
        }

        public static bool IsValid(Vector4 v)
        {
            return !(IsNaN(v) || IsInfinity(v));
        }

        #endregion Verfication

        #region Geometry Calculation

        public static float Cross(Vector2 first, Vector2 second)
        {
            return first.x * second.y - first.y * second.x;
        }

        public static Vector2 Project(Vector2 vec, Vector2 on)
        {
            return on * Vector2.Dot(vec, on) / Vector2.Dot(on, on);
        }

        public static Vector2 InverseScale(Vector2 value, Vector2 scale)
        {
            if (scale.x != 0) scale.x = 1 / scale.x;
            if (scale.y != 0) scale.y = 1 / scale.y;
            return Vector2.Scale(value, scale);
        }

        public static Vector3 InverseScale(Vector3 value, Vector3 scale)
        {
            if (scale.x != 0) scale.x = 1 / scale.x;
            if (scale.y != 0) scale.y = 1 / scale.y;
            if (scale.z != 0) scale.z = 1 / scale.z;
            return Vector3.Scale(value, scale);
        }

        #endregion Geometry Calculation

        #region Field Getter/Setter

        // Get/Set field as method. Convenient in case where method is required.
        public static float GetX(this Vector2 self) { return self.x; }
        public static float GetY(this Vector2 self) { return self.y; }
        public static void SetX(this Vector2 self, float value) { self.x = value; }
        public static void SetY(this Vector2 self, float value) { self.y = value; }

        public static float GetX(this Vector3 self) { return self.x; }
        public static float GetY(this Vector3 self) { return self.y; }
        public static float GetZ(this Vector3 self) { return self.z; }
        public static void SetX(this Vector3 self, float value) { self.x = value; }
        public static void SetY(this Vector3 self, float value) { self.y = value; }
        public static void SetZ(this Vector3 self, float value) { self.z = value; }

        #endregion Field Getter/Setter

        #region Field Iteration

        public static void ForEach(this Vector2 self, Action<int, float> action)
        {
            for (int i = 0; i < 2; ++i)
                action(i, self[i]);
        }

        public static void ForEach(this Vector2 self, Action<float> action)
        {
            for (int i = 0; i < 2; ++i)
                action(self[i]);
        }

        public static void ForEach(this Vector3 self, Action<int, float> action)
        {
            for (int i = 0; i < 3; ++i)
                action(i, self[i]);
        }

        public static void ForEach(this Vector3 self, Action<float> action)
        {
            for (int i = 0; i < 3; ++i)
                action(self[i]);
        }

        public static void ForEach(this Vector4 self, Action<int, float> action)
        {
            for (int i = 0; i < 4; ++i)
                action(i, self[i]);
        }

        public static void ForEach(this Vector4 self, Action<float> action)
        {
            for (int i = 0; i < 4; ++i)
                action(self[i]);
        }

        #endregion Field Iteration
    }
}