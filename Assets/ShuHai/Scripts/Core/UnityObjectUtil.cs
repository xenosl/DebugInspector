using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShuHai
{
    using UObject = UnityEngine.Object;

    public static class UnityObjectUtil
    {
        /// <summary>
        ///     Deal with Unity built-in "null" Object
        /// </summary>
        public static bool IsNull(UObject self) { return self == null || self.Equals(null); }

        public static T FindUniqueSceneComponent<T>() where T : Component { return FindUniqueSceneComponent<T>(null); }

        public static T FindUniqueSceneComponent<T>(Func<T, bool> match) where T : Component
        {
            var list = FindSceneComponents(match);
            if (list.Count > 1)
                throw new InvalidOperationException("Multiple component found.");
            if (list.Count == 0)
                return null;
            return list[0];
        }

        /// <summary>
        ///     Find components of type <typeparamref name="T" /> that match specified condition
        ///     <paramref name="match" /> in opened scenes.
        /// </summary>
        /// <typeparam name="T"> Component type to search. </typeparam>
        /// <param name="match"> Condition to match. </param>
        /// <returns> List contains result components or empty if not found. </returns>
        public static List<T> FindSceneComponents<T>(Func<T, bool> match = null) where T : Component
        {
            var result = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;
                scene.FindComponents(result, match);
            }
            return result;
        }

        public static void Destroy(UObject obj)
        {
            if (obj is Transform)
                throw new ArgumentException("Unable to destroy Transform", "obj");

            if (IsNull(obj))
                return;

            if (Application.isPlaying)
                UObject.Destroy(obj);
            else
                UObject.DestroyImmediate(obj);
        }

        public static void Destroy<T>(ref T obj) where T : UObject
        {
            Destroy(obj);
            obj = null;
        }
    }
}