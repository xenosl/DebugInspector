using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShuHai
{
    public static class SceneExtensions
    {
        #region Finds

        public static T FindComponent<T>(this Scene self, Func<T, bool> match = null)
            where T : Component
        {
            Ensure.Argument.Satisfy(self.isLoaded, "self", "Scene is not loaded");

            var transforms = self.GetRootGameObjects().Select(o => o.transform);
            foreach (var t in transforms)
            {
                var c = t.FindComponentInChildren(match);
                if (c != null)
                    return c;
            }
            return null;
        }

        public static List<T> FindComponents<T>(this Scene self, Func<T, bool> match = null)
            where T : Component
        {
            Ensure.Argument.Satisfy(self.isLoaded, "self", "Scene is not loaded");

            var result = new List<T>();
            FindComponents(self, result, match);
            return result;
        }

        public static void FindComponents<T>(
            this Scene self, ICollection<T> result, Func<T, bool> match = null)
            where T : Component
        {
            Ensure.Argument.Satisfy(self.isLoaded, "self", "Scene is not loaded");

            self.GetRootGameObjects().Select(o => o.transform)
                .ForEach(t => t.FindComponentsInChildren(result, match));
        }

        public static GameObject FindGameObject(this Scene self, Predicate<GameObject> match)
        {
            Ensure.Argument.Satisfy(self.isLoaded, "self", "Scene is not loaded");
            Ensure.Argument.NotNull(match, "match");

            var transforms = self.GetRootGameObjects().Select(o => o.transform);
            foreach (var t in transforms)
            {
                var obj = t.FindGameObjectInChildren(match);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        public static List<GameObject> FindGameObjects(this Scene self, Predicate<GameObject> match)
        {
            Ensure.Argument.Satisfy(self.isLoaded, "self", "Scene is not loaded");
            Ensure.Argument.NotNull(match, "match");

            var transforms = self.GetRootGameObjects().Select(o => o.transform);
            var result = new List<GameObject>();
            foreach (var t in transforms)
                t.FindGameObjectsInChildren(result, match);
            return result;
        }

        public static List<T> FindComponentsInChildren<T>(
            this GameObject self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            return self.transform.FindComponentsInChildren(match, findInSelf);
        }

        public static void FindComponentsInChildren<T>(this GameObject self,
            ICollection<T> result, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            self.transform.FindComponentsInChildren(result, match, findInSelf);
        }

        #endregion Finds
    }
}