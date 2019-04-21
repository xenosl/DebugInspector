using UnityEngine;
using UnityEngine.Assertions;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class GameObjectDrawer<TOwner> : UnityObjectDrawer<TOwner, GameObject>
    {
        public override void ClearChildren()
        {
            base.ClearChildren();
            ComponentGroup = null;
        }

        protected override void ChildrenUpdateImpl()
        {
            base.ChildrenUpdateImpl();
            UpdateComponentGroupDrawer(TypedDrawingValue);
        }

        #region Component Group

        /// <summary>
        /// Group drawer that contains all child component drawers.
        /// </summary>
        public ComponentGroupDrawer ComponentGroup { get; private set; }

        private void UpdateComponentGroupDrawer(GameObject value)
        {
            if (value != null)
            {
                if (ComponentGroup == null)
                    ComponentGroup = Create<ComponentGroupDrawer>(null, this);

                ComponentGroup.UpdateChildren(value);

                //ComponentGroup.ClearChildren();

                //var components = value.GetComponents<Component>();
                //foreach (var c in components)
                //{
                //    var entry = new FixedValueEntry<GameObject, Component>(value, c);
                //    Create(entry, ComponentGroup, c.GetType().Name);
                //}
            }
            else
            {
                if (ComponentGroup == null)
                    return;

                ComponentGroup.Parent = null;
                ComponentGroup = null;
            }
        }

        #endregion
    }
}