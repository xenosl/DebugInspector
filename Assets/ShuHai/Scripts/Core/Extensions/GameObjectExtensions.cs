using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai
{
    public static class GameObjectExtensions
    {
        #region Hierarchy

        public static GameObject FindOrCreateChild(this GameObject self, string name)
        {
            Ensure.Argument.NotNull(self, "self");
            return self.transform.FindOrCreateChild(name).gameObject;
        }

        public static void DestroyChildren(this GameObject self, Action<GameObject> beforeDestroy = null)
        {
            Ensure.Argument.NotNull(self, "self");

            Action<Transform> bd = null;
            if (beforeDestroy != null)
                bd = t => beforeDestroy(t.gameObject);
            self.transform.DestroyChildren(bd);
        }

        public static void DestroyChildren(this GameObject self, Func<GameObject, bool> predicate)
        {
            Ensure.Argument.NotNull(self, "self");

            Func<Transform, bool> p = null;
            if (predicate != null)
                p = (t) => predicate(t.gameObject);
            self.transform.DestroyChildren(p);
        }

        #endregion Hierarchy

        #region Components

        public static T GetComponentInParent<T>(this GameObject self, bool includeSelf)
            where T : Component
        {
            return self.transform.GetComponentInParent<T>(includeSelf);
        }

        public static Component GetComponentInParent(this GameObject self, Type type, bool includeSelf)
        {
            return self.transform.GetComponentInParent(type, includeSelf);
        }

        public static T FindComponentInChildren<T>(
            this GameObject self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            return self.transform.FindComponentInChildren(match, findInSelf);
        }

        #endregion Components

        #region GameObject

        public static GameObject FindGameObjectInChildren(
            this GameObject self, Predicate<GameObject> match, bool findInSelf = true)
        {
            return self.transform.FindGameObjectInChildren(match, findInSelf);
        }

        public static List<GameObject> FindGameObjectsInChildren(
            this GameObject self, Predicate<GameObject> match, bool findInSelf = true)
        {
            return self.transform.FindGameObjectsInChildren(match, findInSelf);
        }

        public static void FindGameObjectsInChildren(this GameObject self,
            Predicate<GameObject> match, ICollection<GameObject> result, bool findInSelf = true)
        {
            self.transform.FindGameObjectsInChildren(result, match, findInSelf);
        }

        #endregion GameObject
    }
}