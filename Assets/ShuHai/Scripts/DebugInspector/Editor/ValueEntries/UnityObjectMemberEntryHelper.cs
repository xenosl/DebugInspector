using System;
using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    using UObject = UnityEngine.Object;

    internal static class UnityObjectMemberEntryHelper
    {
        public static void ThrowOnInaccessible(UObject obj, string memberName, string fallbackMemberName)
        {
            if (!EditorApplication.isPlaying)
                ThrowInaccessible(memberName, fallbackMemberName);

            var prefabType = PrefabUtility.GetPrefabType(obj);
            if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
                ThrowInaccessible(memberName, fallbackMemberName);
        }

        private static void ThrowInaccessible(string memberName, string fallbackMemberName)
        {
            var msg = string.Format(
                "Not allowed to access {0} during edit mode. Please use {1} instead.",
                memberName, fallbackMemberName);
            throw new InvalidOperationException(msg);
        }
    }
}