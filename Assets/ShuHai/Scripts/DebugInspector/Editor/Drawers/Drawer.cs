using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShuHai.Editor;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    using SHCoroutine = Coroutines.Coroutine;

    /// <summary>
    ///     Root type for all drawers.
    /// </summary>
    [DrawerStateType(typeof(DrawerState))]
    public abstract class Drawer
    {
        #region Create

        public static event Action<Drawer> Created;

        public static T Create<T>(string name = null, Drawer parent = null, DrawerState state = null)
            where T : Drawer
        {
            return (T)Create(typeof(T), name, parent, state);
        }

        public static Drawer Create(Type type, string name = null, Drawer parent = null, DrawerState state = null)
        {
            var drawer = CreateImpl(type);
            drawer.Construct(name, parent);
            drawer.ApplyInitState(state);
            drawer.NotifyCreated();
            return drawer;
        }

        protected static Drawer CreateImpl(Type type)
        {
            Ensure.Argument.NotNull(type, "type");
            Ensure.Argument.Is<Drawer>(type, "type");
            return (Drawer)ObjectFactory.Create(type);
        }

        protected Drawer() { Root = this; }

        protected virtual void Construct(string name, Drawer parent)
        {
            if (name != null)
                Name = name;
            Parent = parent;
        }

        protected void ApplyInitState(DrawerState state)
        {
            if (state != null)
                ApplyState(state);
            else
                ApplySavedOrDefaultState();
        }

        protected void NotifyCreated() { Created.NPInvoke(this); }

        #endregion Create

        #region Names

        /// <summary>
        ///     Name of current instance.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (value == name)
                    return;
                name = value;
                MarkHierarchicalNameDirty();
            }
        }

        private string name = string.Empty;

        /// <summary>
        ///     Hierarchy name combined by parent <see cref="Name" /> and current instance <see cref="Name" /> with ".".
        ///     e.g. if <see cref="Name" /> is "hideFlags", the value may be "Camera.Members.gameObject.hideFlags".
        /// </summary>
        public string HierarchicalName
        {
            get
            {
                if (!hierarchicalNameDirty)
                    return hierarchicalName;

                var builder = new StringBuilder(Name);
                foreach (var parent in Parents)
                    builder.InsertHead('.').InsertHead(parent.Name);
                hierarchicalName = builder.ToString();
                hierarchicalNameDirty = false;

                return hierarchicalName;
            }
        }

        private string hierarchicalName;

        private bool hierarchicalNameDirty = true;

        private void MarkHierarchicalNameDirty()
        {
            hierarchicalNameDirty = true;

            foreach (var child in ChildrenInHierarchy)
                child.hierarchicalNameDirty = true;
        }

        #endregion

        #region Hierarchy

        public int DepthInHierarchy { get; private set; }

        private void UpdateDepthInHierarchy()
        {
            DepthInHierarchy = 0;
            var p = parent;
            while (p != null)
            {
                DepthInHierarchy++;
                p = p.parent;
            }
        }

        public Drawer[] ListByScope(DrawerActionScope scope) { return EnumerateByScope(scope).ToArray(); }

        /// <summary>
        ///     Enumerate related drawers of current instance by specific scope.
        /// </summary>
        /// <param name="scope">Scope that indicates what drawers to enumerate.</param>
        /// <returns>An enumerable collection that enumerate all drawers refered by specific scope.</returns>
        public IEnumerable<Drawer> EnumerateByScope(DrawerActionScope scope)
        {
            switch (scope)
            {
                case DrawerActionScope.Self:
                    return this.ToEnumerable();
                case DrawerActionScope.Children:
                    return Children;
                case DrawerActionScope.ChildrenInHierarchy:
                    return ChildrenInHierarchy;
                case DrawerActionScope.SelfAndChildren:
                    return this.ToEnumerable().Concat(Children);
                case DrawerActionScope.SelfAndChildrenInHierarchy:
                    return this.ToEnumerable().Concat(ChildrenInHierarchy);
                default:
                    throw new ArgumentOutOfRangeException("scope", scope, null);
            }
        }

        #region Parents

        /// <summary>
        ///     Occurs before parent of current instance has been changed.
        ///     The <see cref="Drawer" /> argument refers to the new parent after change.
        /// </summary>
        public event Action<Drawer> ParentChanging;

        /// <summary>
        ///     Occurs after parent of current instance has been changed.
        ///     The <see cref="Drawer" /> argument refers to the old parent before change.
        /// </summary>
        public event Action<Drawer> ParentChanged;

        public Drawer Root { get; private set; }

        public bool IsRoot { get { return this == Root; } }

        public Drawer Parent
        {
            get { return parent; }
            set { ChangeParent(value, value != null ? value.ChildCount : 0); }
        }

        public IEnumerable<Drawer> Parents
        {
            get
            {
                var p = parent;
                while (p != null)
                {
                    yield return p;
                    p = p.parent;
                }
            }
        }

        public bool ChangeParent(Drawer newParent, int indexInNewParent)
        {
            var oldParent = parent;
            if (oldParent != null && oldParent.childrenEnumerating)
            {
                var msg = string.Format(
                    "Attempt to remove child '{0}' from '{1}' while its children enumerating.",
                    HierarchicalName, oldParent.HierarchicalName);
                throw new InvalidOperationException(msg);
            }

            if (newParent != null)
            {
                Ensure.Argument.IsValidIndex(indexInNewParent, "indexInParent", newParent.ChildCount + 1);

                if (newParent.childrenEnumerating)
                {
                    var msg = string.Format(
                        "Attempt to add child '{0}' to '{1}' while its children enumerating.",
                        HierarchicalName, newParent.HierarchicalName);
                    throw new InvalidOperationException(msg);
                }
            }

            if (newParent == parent)
                return false;

            // Notifications for public.
            ParentChanging.NPInvoke(newParent);

            // Notifications for deriveds.
            BeforeChangeParent(newParent);

            parent = newParent;

            // Update children list
            if (oldParent != null)
                oldParent.children.Remove(this);
            if (newParent != null)
            {
                var childList = newParent.children;
                childList.Insert(indexInNewParent, this);
            }

            UpdateDepthInHierarchy();

            Root = newParent != null ? newParent.Root : this;

            if (oldParent != null)
                oldParent.MarkHierarchicalNameDirty();
            MarkHierarchicalNameDirty();

            // Notifications for deriveds.
            AfterChangeParent(oldParent);
            if (oldParent != null)
                oldParent.OnChildRemoved(this);
            if (newParent != null)
                newParent.OnChildAdded(this);

            // Notifications for public.
            ParentChanged.NPInvoke(oldParent);
            if (oldParent != null)
                oldParent.NotifyChildRemoved(this);
            if (newParent != null)
                newParent.NotifyChildAdded(this);

            return true;
        }

        public bool IsParentOf(Drawer drawer)
        {
            Ensure.Argument.NotNull(drawer, "drawer");
            return drawer.IsChildOf(this);
        }

        public bool IsChildOf(Drawer drawer)
        {
            Ensure.Argument.NotNull(drawer, "drawer");
            return Parents.Contains(drawer);
        }

        private Drawer parent;

        protected virtual void BeforeChangeParent(Drawer newParent) { }
        protected virtual void AfterChangeParent(Drawer oldParent) { }

        #endregion

        #region Children

        /// <summary>
        ///     Occurs after a child drawer is added to children list of current instance.
        /// </summary>
        public event Action<Drawer> ChildAdded;

        /// <summary>
        ///     Occurs after a child drawer is added to current instance's children list or its parents' children list.
        /// </summary>
        public event Action<Drawer> ChildAddedInHierarchy;

        /// <summary>
        ///     Occurs after a child drawer is removed from children list of current instance.
        /// </summary>
        public event Action<Drawer> ChildRemoved;

        /// <summary>
        ///     Occurs after a child drawer is removed from current instance's children list or its parents' children list.
        /// </summary>
        public event Action<Drawer> ChildRemovedInHierarchy;

        /// <summary>
        ///     Number of child drawers.
        /// </summary>
        public int ChildCount { get { return children.Count; } }

        /// <summary>
        ///     Enumeration of child drawers.
        /// </summary>
        public IEnumerable<Drawer> Children
        {
            get
            {
                childrenEnumerating = true;
                foreach (var child in children)
                    yield return child;
                childrenEnumerating = false;
            }
        }

        /// <summary>
        ///     Enumeration of all child drawers in hierarchy.
        /// </summary>
        public IEnumerable<Drawer> ChildrenInHierarchy
        {
            get
            {
                foreach (var child in Children)
                {
                    yield return child;
                    foreach (var childOfChild in child.ChildrenInHierarchy)
                        yield return childOfChild;
                }
            }
        }

        /// <summary>
        ///     First child drawer in the children list.
        /// </summary>
        public Drawer FirstChild { get { return ChildCount > 0 ? children[0] : null; } }

        /// <summary>
        ///     Last child drawer in the children list.
        /// </summary>
        public Drawer LastChild { get { return ChildCount > 0 ? children[ChildCount - 1] : null; } }

        /// <summary>
        ///     Replace child drawer at specific index.
        /// </summary>
        public Drawer ReplaceChild(int index, Drawer child)
        {
            Ensure.Argument.IsValidIndex(index, "index", ChildCount);
            Ensure.Argument.NotNull(child, "child");
            Ensure.Argument.Satisfy(child.Parent != this, "child", "Unable to change child index.");

            var old = children[index];
            if (old == child)
                return old;

            old.Parent = null;
            child.ChangeParent(this, index);

            return old;
        }

        /// <summary>
        ///     Remove all child drawers.
        /// </summary>
        public virtual void ClearChildren()
        {
            while (LastChild != null)
                LastChild.Parent = null;
        }

        /// <summary>
        ///     Get child drawer at specific index.
        /// </summary>
        /// <param name="index"> The zero-based index of the drawer to get. </param>
        /// <returns>The child drawer instance at specified index.</returns>
        public Drawer GetChild(int index) { return children.ElementAtOrDefault(index); }

        private readonly List<Drawer> children = new List<Drawer>();

        protected bool childrenEnumerating;

        protected virtual void OnChildAdded(Drawer child) { }

        protected virtual void OnChildRemoved(Drawer child) { }

        private void NotifyChildAdded(Drawer child)
        {
            ChildAdded.NPInvoke(child);

            ChildAddedInHierarchy.NPInvoke(child);
            foreach (var parent in Parents)
                parent.ChildAddedInHierarchy.NPInvoke(child);
        }

        private void NotifyChildRemoved(Drawer child)
        {
            ChildRemoved.NPInvoke(child);

            ChildRemovedInHierarchy.NPInvoke(child);
            foreach (var parent in Parents)
                parent.ChildRemovedInHierarchy.NPInvoke(child);
        }

        #endregion Children

        #region Siblings

        public int SiblingCount { get { return Parent != null ? Parent.ChildCount - 1 : 0; } }

        public IEnumerable<Drawer> Siblings
        {
            get
            {
                if (Parent != null)
                {
                    foreach (var drawer in Parent.Children)
                    {
                        if (drawer != this)
                            yield return drawer;
                    }
                }
            }
        }

        public Drawer GetSibling(int index)
        {
            if (Parent == null)
                return null;
            var drawer = Parent.GetChild(index);
            return drawer != this ? drawer : null;
        }

        #endregion Siblings

        #endregion Hierarchy

        #region Visiblity

        public const bool DefaultSelfVisible = true;
        public const bool DefaultChildrenVisible = false;

        /// <summary>
        ///     Indicates whether self part of this drawer is visible on GUI.
        /// </summary>
        public virtual bool SelfVisible { get { return selfVisible; } set { selfVisible = value; } }

        public bool SelfVisibleInHierarchy { get { return SelfVisible && allChildrenVisibleOfParents; } }

        /// <summary>
        ///     Indicates whether children is visible on GUI.
        /// </summary>
        public virtual bool ChildrenVisible
        {
            get { return childrenVisible; }
            set
            {
                if (value == childrenVisible)
                    return;
                childrenVisible = value;
                OnChildrenVisibilityChanged();
            }
        }

        private bool selfVisible = DefaultSelfVisible;
        private bool childrenVisible = DefaultChildrenVisible;

        public bool ChildrenVisibleInHierarchy { get { return ChildrenVisible && allChildrenVisibleOfParents; } }

        public void ResetVisiblity(DrawerActionScope scope, bool bySavedStates = true)
        {
            if (scope == DrawerActionScope.Self)
            {
                ResetVisibility(bySavedStates); // Avoid heap alloc from EnumerateByScope.
            }
            else
            {
                foreach (var drawer in EnumerateByScope(scope))
                    drawer.ResetVisibility(bySavedStates);
            }
        }

        public void ExpandToSelf()
        {
            SelfVisible = true;
            foreach (var p in Parents)
            {
                if (!(p is GroupDrawer))
                    p.SelfVisible = true;
                p.ChildrenVisible = true;
            }
        }

        protected virtual void ResetVisibility(bool bySavedStates)
        {
            var state = bySavedStates ? DrawerStates.Get(HierarchicalName) : null;
            SelfVisible = state != null ? state.SelfVisible : DefaultSelfVisible;
            ChildrenVisible = state != null ? state.ChildrenVisible : IsRoot;
        }

        protected virtual void OnChildrenVisibilityChanged()
        {
            if (!IsAsyncUpdating && ChildrenVisibleInHierarchy)
                scheduledGUIActions.ScheduleAction(() => EachChildUpdate(DrawerUpdateParameters.Default));
        }

        private bool allChildrenVisibleOfParents
        {
            get
            {
                var p = parent;
                while (p != null)
                {
                    if (!p.ChildrenVisible)
                        return false;
                    p = p.parent;
                }
                return true;
            }
        }

        #endregion Visiblity

        #region Update

        public void Update() { Update(DrawerUpdateParameters.Default); }

        public void Update(DrawerUpdateParameters parameters)
        {
            if (DepthInHierarchy > parameters.MaxDepth)
                return;

            SelfUpdate();

            if (CanChildrenUpdate(parameters))
            {
                ChildrenUpdate();

                if (CanEachChildUpdate(parameters.DeepUpdate))
                    EachChildUpdate(parameters);
            }
        }

        protected virtual void SelfUpdate() { }

        protected virtual void ChildrenUpdate() { }

        private bool CanChildrenUpdate(DrawerUpdateParameters parameters)
        {
            if (DepthInHierarchy > parameters.MaxDepth)
                return false;

            var precondition = parameters.ChildrenUpdatePrecondition;
            if (precondition != null && !precondition(this))
                return false;

            return true;
        }

        private bool CanEachChildUpdate(bool deepUpdate)
        {
            if (!deepUpdate && !ChildrenVisible)
                return false;
            return ChildCount > 0;
        }

        private void EachChildUpdate(DrawerUpdateParameters parameters)
        {
            foreach (var child in Children)
                child.Update(parameters);
        }

        #region Async

        public static event Action<Drawer> AsyncUpdateDone;

        public bool IsAsyncUpdating { get { return RootAsyncUpdateRoutine != null; } }

        public SHCoroutine RootAsyncUpdateRoutine
        {
            get
            {
                if (asyncUpdateRoutine != null)
                    return asyncUpdateRoutine;

                var p = parent;
                while (p != null)
                {
                    if (p.asyncUpdateRoutine != null)
                        return p.asyncUpdateRoutine;
                    p = p.parent;
                }
                return null;
            }
        }

        public void AsyncUpdate(int executeMultiplier = 1, Action done = null)
        {
            AsyncUpdate(DrawerUpdateParameters.Default, executeMultiplier, done);
        }

        public void AsyncUpdate(DrawerUpdateParameters parameters, int executeMultiplier = 1, Action done = null)
        {
            if (DepthInHierarchy > parameters.MaxDepth)
                return;

            StopAsyncUpdate();

            asyncUpdateDone = done;
            asyncUpdateRoutine = CreateAsyncUpdateImplRoutine(this, parameters, executeMultiplier);
            asyncUpdateRoutine.Start();
        }

        public void StopAsyncUpdate()
        {
            if (asyncUpdateRoutine != null)
                asyncUpdateRoutine.Stop();
        }

        public void AsyncUpdatePaused(bool paused)
        {
            if (asyncUpdateRoutine != null)
                asyncUpdateRoutine.SelfPaused = paused;
        }

        private SHCoroutine asyncUpdateRoutine;
        private Action asyncUpdateDone;

        private IEnumerator AsyncUpdateImpl(DrawerUpdateParameters parameters, int executeMultiplier = 1)
        {
            if (DepthInHierarchy > parameters.MaxDepth)
                yield break;

            SelfUpdate();
            yield return null;

            if (CanChildrenUpdate(parameters))
            {
                ChildrenUpdate();

                if (CanEachChildUpdate(parameters.DeepUpdate))
                {
                    yield return new SHCoroutine(
                        AsyncEachChildUpdateImpl(parameters, executeMultiplier),
                        HierarchicalName + "+AsyncEachChildUpdateImpl");
                }
            }
        }

        private IEnumerator AsyncEachChildUpdateImpl(DrawerUpdateParameters parameters, int executeMultiplier = 1)
        {
            foreach (var child in Children)
                yield return CreateAsyncUpdateImplRoutine(child, parameters, executeMultiplier);
        }

        private void OnAsyncUpdateDone()
        {
            asyncUpdateRoutine = null;

            asyncUpdateDone.NPInvoke();
            asyncUpdateDone = null;

            AsyncUpdateDone.NPInvoke(this);
        }

        private static SHCoroutine CreateAsyncUpdateImplRoutine(
            Drawer drawer, DrawerUpdateParameters parameters, int executeMultiplier)
        {
            return new SHCoroutine(
                drawer.AsyncUpdateImpl(parameters, executeMultiplier),
                drawer.HierarchicalName + "+AsyncUpdateImpl", executeMultiplier, drawer.OnAsyncUpdateDone);
        }

        #endregion Async

        #endregion Update

        #region GUI

        public void GUI(GUIStyle style, params GUILayoutOption[] options)
        {
            if (Event.current.type == EventType.Layout)
            {
                scheduledGUIActions.ExecuteScheduledActions();
                ApplyGUIData();
            }

            try
            {
                GUIImpl(style);
            }
            catch (Exception e)
            {
                var drawSelf = SelfVisible;
                if (drawSelf)
                    GUIBegin(style);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(Name);
                    exceptionContent.text = e.GetType().Name;
                    exceptionContent.tooltip = e.Message;
                    EditorGUILayout.LabelField(e.Message, DIGUIStyles.ExceptionLabel);
                }

                if (drawSelf)
                    GUIEnd();
            }
        }

        private readonly GUIContent exceptionContent = new GUIContent();

        protected virtual void GUIImpl(GUIStyle style, params GUILayoutOption[] options)
        {
            var drawSelf = SelfVisible;
            if (drawSelf)
                GUIBegin(style, options);

            if (drawSelf)
                SelfGUI();

            if (ChildrenVisible)
            {
                ChildrenGUI();
                EachChildGUI();
            }

            if (drawSelf)
                GUIEnd();
        }

        /// <summary>
        ///     Draw contents of this drawer.
        /// </summary>
        protected abstract void SelfGUI();

        /// <summary>
        ///     Draw contents of child drawers.
        /// </summary>
        protected virtual void ChildrenGUI() { }

        private void GUIBegin(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            if (DepthInHierarchy > 0)
                GUILayout.Space(EditorGUIEx.IndentPerLevel);

            EditorGUILayout.BeginVertical(style ?? GUIStyle.none);
        }

        private void GUIEnd()
        {
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void EachChildGUI()
        {
            for (int i = 0; i < ChildCount; ++i)
                children[i].GUI(null);
        }

        protected virtual void ApplyGUIData() { }

        protected readonly ScheduledActions scheduledGUIActions = new ScheduledActions();

        #endregion GUI

        #region State

        public DrawerState CreateState()
        {
            var type = GetStateType();
            var state = (DrawerState)ObjectFactory.Create(type);
            PopulateState(state);
            return state;
        }

        public void SaveState(DrawerActionScope scope)
        {
            foreach (var drawer in EnumerateByScope(scope))
                DrawerStates.Set(drawer.HierarchicalName, drawer.CreateState());
        }

        protected virtual void PopulateState(DrawerState state)
        {
            state.SelfVisible = SelfVisible;
            state.ChildrenVisible = ChildrenVisible;
        }

        private Type GetStateType()
        {
            DrawerStateTypeAttribute attr = null;
            var type = GetType();
            while (type != typeof(object))
            {
                attr = type.GetCustomAttribute<DrawerStateTypeAttribute>();
                if (attr != null)
                    break;
                type = type.BaseType;
            }
            return attr != null ? attr.Value : null;
        }

        #region Apply

        public bool ApplySavedState()
        {
            var state = DrawerStates.Get(HierarchicalName);
            if (state == null)
                return false;
            ApplyState(state);
            return true;
        }

        public virtual void ApplyState(DrawerState state)
        {
            SelfVisible = state.SelfVisible;
            ChildrenVisible = state.ChildrenVisible;
        }

        public virtual void ApplyDefaultState()
        {
            SelfVisible = DefaultSelfVisible;
            ChildrenVisible = DefaultChildrenVisible;
        }

        public void ApplySavedOrDefaultState()
        {
            if (!ApplySavedState())
                ApplyDefaultState();
        }

        public void ApplyStateOrDefault(DrawerState state)
        {
            if (state != null)
                ApplyState(state);
            else
                ApplyDefaultState();
        }

        #endregion Apply

        #endregion States

        #region Debug

        protected virtual void DebugGUI()
        {
            if (GUILayout.Button(DIGUIContents.DebugInfoButton, DIGUIStyles.InfoButton, GUILayout.Width(20)))
                PrintDebugInfo();
        }

        public void PrintDebugInfo() { Debug.Log(BuildDebugInfo()); }

        public string BuildDebugInfo() { return BuildDebugInfo(new StringBuilder()).ToString(); }

        protected virtual StringBuilder BuildDebugInfo(StringBuilder builder)
        {
            if (builder.Length > 0)
                builder.Append('\n');

            builder.Append("Type: ").Append(GetType().GetReadableName());

            return builder;
        }

        #endregion Debug
    }
}