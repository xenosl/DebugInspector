using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class ComponentGroupDrawer : MappingValueGroupDrawer<Component>
    {
        private ComponentGroupDrawer() { Name = "Components"; }

        public GameObject GameObject { get; private set; }

        public void UpdateChildren(GameObject gameObject)
        {
            Ensure.Argument.NotNull(gameObject, "gameObject");

            GameObject = gameObject;
            UpdateChildrenImpl(GameObject.GetComponents<Component>(), false);
        }

        public void UpdateChildren(GameObject gameObject, IEnumerable<Component> components)
        {
            Ensure.Argument.NotNull(gameObject, "gameObject");

            GameObject = gameObject;
            UpdateChildrenImpl(components, true);
        }

        public void UpdateChildrenImpl(IEnumerable<Component> components, bool verifyGameObject)
        {
            int componentCount = components != null ? components.Count() : 0;

            // Remove redundant child drawers.
            while (ChildCount > componentCount)
                LastChild.Parent = null;

            if (componentCount > 0)
            {
                foreach (var component in components)
                {
                    if (verifyGameObject && component.gameObject != GameObject)
                    {
                        var msg = string.Format(
                            @"Components of ""{0}"" expected, got ""{1}"".",
                            GameObject, component.gameObject);
                        throw new ArgumentException(msg, "components");
                    }

                    var expectedChildType = ValueDrawerTypes.GetOrBuild(typeof(GameObject), component.GetType());
                    var child = GetChild(component);
                    if (child != null && child.GetType() != expectedChildType)
                    {
                        child.Parent = null;
                        child = null;
                    }
                    if (child == null)
                        child = (ValueDrawer)Create(expectedChildType, component.GetType().Name);
                    AddMapping(component, child, true);

                    child.ValueEntry = new FixedValueEntry<GameObject, Component>(GameObject, component);
                }
            }
            else
            {
                ClearChildren();
            }
        }
    }
}