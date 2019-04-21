using UnityEngine;

namespace ShuHai
{
    public static class Vectors
    {
        public static readonly Vector2 NaN2 = new Vector2(float.NaN, float.NaN);
        public static readonly Vector3 NaN3 = new Vector3(float.NaN, float.NaN, float.NaN);
        public static readonly Vector4 NaN4 = new Vector4(float.NaN, float.NaN, float.NaN, float.NaN);

        public static readonly Vector2 Min2 = new Vector2(float.MinValue, float.MinValue);
        public static readonly Vector3 Min3 = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        public static readonly Vector2 Max2 = new Vector2(float.MaxValue, float.MaxValue);
        public static readonly Vector3 Max3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public static readonly Vector2 Zero2 = new Vector2(0, 0);
        public static readonly Vector3 Zero3 = new Vector3(0, 0, 0);

        public static readonly Vector2 Half2 = new Vector2(0.5f, 0.5f);
        public static readonly Vector3 Half3 = new Vector3(0.5f, 0.5f, 0.5f);

        public static readonly Vector2 One2 = new Vector2(1, 1);
        public static readonly Vector3 One3 = new Vector3(1, 1, 1);
    }
}