using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Represents <see cref="GroupDrawer" /> that contains mappings from specific type to <see cref="ValueDrawer" />
    ///     children.
    /// </summary>
    /// <typeparam name="T">The mapping type.</typeparam>
    public class MappingValueGroupDrawer<T> : GroupDrawer
    {
        public int MappedCount { get { return mappedToChild.Count; } }
        public IEnumerable<ValueDrawer> MappedChildren { get { return mappedToChild.Values; } }

        public IEnumerable<KeyValuePair<T, ValueDrawer>> MappedToChild { get { return mappedToChild; } }
        public IEnumerable<KeyValuePair<ValueDrawer, T>> ChildToMapped { get { return childToMapped; } }

        public ValueDrawer GetChild(T mapped)
        {
            Ensure.Argument.NotNull(mapped, "mapped");
            return mappedToChild.GetValue(mapped);
        }

        public T GetMapped(ValueDrawer child)
        {
            Ensure.Argument.NotNull(child, "child");
            return childToMapped.GetValue(child);
        }

        private readonly Dictionary<T, ValueDrawer> mappedToChild = new Dictionary<T, ValueDrawer>();
        private readonly Dictionary<ValueDrawer, T> childToMapped = new Dictionary<ValueDrawer, T>();

        protected bool AddMapping(T toMap, ValueDrawer child, bool replaceIfExisted = false)
        {
            ValueDrawer existedChild;
            if (mappedToChild.TryGetValue(toMap, out existedChild))
            {
                if (child == existedChild)
                    return false;

                if (!replaceIfExisted)
                {
                    throw new InvalidOperationException(string.Format(
                        @"Member ""{0}"" mapping to child ""{1}"" already existed.",
                        toMap, child.HierarchicalName));
                }
            }

            child.Parent = this;
            mappedToChild[toMap] = child;
            childToMapped[child] = toMap;

            return true;
        }

        #region Remove Mapping

        protected bool RemoveMapping(T member) { return RemoveMapping(mappedToChild, childToMapped, member, true); }

        protected bool RemoveMapping(ValueDrawer child)
        {
            // Maybe the child is added from parent without mapping to a member, so reverse mapping is not required.
            return RemoveMapping(childToMapped, mappedToChild, child, false);
        }

        private static bool RemoveMapping<T1, T2>(
            IDictionary<T1, T2> mainDict, IDictionary<T2, T1> reverseDict, T1 mainKey, bool reverseMappingRequired)
        {
            T2 mainValue;
            if (!mainDict.TryGetValue(mainKey, out mainValue))
                return false;

            mainDict.Remove(mainKey);
            bool reverseRemoved = reverseDict.Remove(mainValue);
            if (reverseMappingRequired)
                Assert.IsTrue(reverseRemoved);
            return true;
        }

        #endregion Remove Mapping

        protected override void OnChildRemoved(Drawer child)
        {
            var vdc = child as ValueDrawer;
            if (vdc != null)
                RemoveMapping(vdc);

            base.OnChildRemoved(child);
        }
    }
}