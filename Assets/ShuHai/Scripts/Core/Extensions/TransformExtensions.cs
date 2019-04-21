using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai
{
    public static class TransformExtensions
    {
        #region Position

        /// <summary>
        /// Set position without changing its z value.
        /// </summary>
        public static void SetPosition2D(this Transform self, Vector2 value)
        {
            self.position = new Vector3(value.x, value.y, self.position.z);
        }

        /// <summary>
        /// Set local position without changing its z value.
        /// </summary>
        public static void SetLocalPosition2D(this Transform self, Vector2 value)
        {
            self.localPosition = new Vector3(value.x, value.y, self.localPosition.z);
        }

        #endregion Position

        #region Scale

        public static void SetLocalScale(this Transform self, float value)
        {
            self.localScale = new Vector3(value, value, value);
        }

        public static void SetLossyScale(this Transform self, Vector2 value)
        {
            var parent = self.parent;
            if (parent == null)
            {
                self.localScale = value;
                return;
            }

            var lossyOfParents = Vector2.one;
            while (parent != null)
            {
                lossyOfParents = Vector2.Scale(lossyOfParents, parent.localScale);
                parent = parent.parent;
            }
            Vector3 scale = VectorUtil.InverseScale(value, lossyOfParents);
            scale.z = self.localScale.z;
            self.localScale = scale;
        }

        public static void SetLossyScale(this Transform self, Vector3 value)
        {
            var parent = self.parent;
            var lossyOfParents = Vector3.one;
            while (parent != null)
            {
                lossyOfParents = Vector3.Scale(lossyOfParents, parent.localScale);
                parent = parent.parent;
            }
            self.localScale = VectorUtil.InverseScale(value, lossyOfParents);
        }

        #endregion Scale

        #region Hierarchy

        public static Transform FindOrCreateChild(this Transform self, string name)
        {
            Ensure.Argument.NotNull(self, "self");

            var t = self.Find(name);
            if (t == null)
            {
                t = new GameObject(name).transform;
                t.parent = self;
            }
            return t;
        }

        public static void DestroyChildren(this Transform self, Action<Transform> beforeDestroy = null)
        {
            Ensure.Argument.NotNull(self, "self");

            while (self.childCount > 0)
            {
                var child = self.GetChild(0);
                beforeDestroy.NPInvoke(child);
                child.SetParent(null);
                UnityObjectUtil.Destroy(child.gameObject);
            }
        }

        public static void DestroyChildren(this Transform self, Func<Transform, bool> predicate)
        {
            Ensure.Argument.NotNull(self, "self");

            var destroyList = new List<Transform>();
            for (int i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (predicate.NPInvoke(child))
                    destroyList.Add(child);
            }
            for (int i = 0; i < destroyList.Count; ++i)
                UnityObjectUtil.Destroy(destroyList[i].gameObject);
        }

        #endregion Hierarchy

        #region Component

        /// <remarks>
        /// Almost same as Unity provided GetComponentInParent except this one works with prefabs.
        /// </remarks>
        public static T GetComponentInParent<T>(this Transform self, bool includeSelf)
            where T : Component
        {
            return (T)GetComponentInParent(self, typeof(T), includeSelf);
        }

        public static Component GetComponentInParent(this Transform self, Type type, bool includeSelf)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.Is<Component>(type, "type");

            if (!includeSelf)
                self = self.parent;
            while (self != null)
            {
                var component = self.GetComponent(type);
                if (component != null)
                    return component;
                self = self.parent;
            }
            return null;
        }

        public static T FindComponentInChildren<T>(
            this Transform self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, "self");

            if (findInSelf)
            {
                var c = FindComponent(self, match);
                if (c != null)
                    return c;
            }

            for (int i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                {
                    var cc = FindComponent(child, match);
                    if (cc != null)
                        return cc;
                }

                var ccc = FindComponentInChildren(child, match, findInSelf);
                if (ccc != null)
                    return ccc;
            }
            return null;
        }

        private static T FindComponent<T>(Transform transform, Func<T, bool> match)
            where T : Component
        {
            var components = transform.GetComponents<T>();
            return match != null ? components.FirstOrDefault(match) : components.FirstOrDefault();
        }

        public static List<T> FindComponentsInChildren<T>(
            this Transform self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            var result = new List<T>();
            FindComponentsInChildren(self, result, match, findInSelf);
            return result;
        }

        /// <remarks>Similar with GetComponentsInChildren when match is null.</remarks>
        public static void FindComponentsInChildren<T>(this Transform self,
            ICollection<T> result, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(result, "result");

            if (findInSelf)
                result.AddRange(FindComponents(self, match));

            for (int i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                    result.AddRange(FindComponents(child, match));
                FindComponentsInChildren(child, result, match, findInSelf);
            }
        }

        private static IEnumerable<T> FindComponents<T>(Transform transform, Func<T, bool> match)
            where T : Component
        {
            var components = transform.GetComponents<T>();
            return match != null ? components.Where(match) : components;
        }

        #endregion Component

        #region GameObject

        public static GameObject FindGameObjectInChildren(
            this Transform self, Predicate<GameObject> match, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(match, "match");

            if (findInSelf)
            {
                var selfObj = self.gameObject;
                if (match(selfObj))
                    return selfObj;
            }

            for (int i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                {
                    var childObj = child.gameObject;
                    if (match(childObj))
                        return childObj;
                }

                var obj = FindGameObjectInChildren(child, match, findInSelf);
                if (obj != null && match(obj))
                    return obj;
            }
            return null;
        }

        public static List<GameObject> FindGameObjectsInChildren(
            this Transform self, Predicate<GameObject> match, bool findInSelf = true)
        {
            var result = new List<GameObject>();
            FindGameObjectsInChildren(self, result, match, findInSelf);
            return result;
        }

        public static void FindGameObjectsInChildren(this Transform self,
            ICollection<GameObject> result, Predicate<GameObject> match, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, "self");
            Ensure.Argument.NotNull(match, "match");
            Ensure.Argument.NotNull(result, "result");

            if (findInSelf)
            {
                var selfObj = self.gameObject;
                if (match(selfObj))
                    result.Add(selfObj);
            }

            for (int i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                {
                    var obj = child.gameObject;
                    if (match(obj))
                        result.Add(obj);
                }
                FindGameObjectsInChildren(child, result, match, findInSelf);
            }
        }

        #endregion GameObject
    }
}