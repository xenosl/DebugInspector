using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    using UObject = UnityEngine.Object;

    [DrawerStateType(typeof(ValueDrawerState))]
    public abstract class ValueDrawer : Drawer
    {
        #region Create

        public static ValueDrawer Create(Type ownerType, Type valueType,
            string name = null, Drawer parent = null, DrawerState state = null)
        {
            var drawer = CreateImpl(ownerType, valueType);
            drawer.Construct(name, parent);
            drawer.ApplyInitState(state);
            drawer.NotifyCreated();
            return drawer;
        }

        protected static ValueDrawer CreateImpl(Type ownerType, Type valueType, bool useRootAsDefault = true)
        {
            var type = ValueDrawerTypes.GetOrBuild(ownerType, valueType, useRootAsDefault);
            return (ValueDrawer)CreateImpl(type);
        }

        protected ValueDrawer()
        {
            TypeInfo = ValueDrawerTypeInfo.Get(GetType());
            RootValueDrawer = this;
            Name = TypeInfo.GenericArguments.Value.FullName;
        }

        #endregion

        /// <summary>
        ///     Type infomation of current instance.
        /// </summary>
        public readonly ValueDrawerTypeInfo TypeInfo;

        #region Update

        protected virtual bool childrenAvailable
        {
            get
            {
                if (DrawingValue == null)
                    return false;
                if (IsReferenceOfParent)
                    return false;
                if (ParentCountWithSameValueType + 1 >= Settings.MaxHierarchyLoopCount)
                    return false;
                return true;
            }
        }

        protected override void SelfUpdate()
        {
            base.SelfUpdate();
            UpdateDrawingValue();
        }

        protected override void ChildrenUpdate()
        {
            if (childrenAvailable)
                ChildrenUpdateImpl();
        }

        protected virtual void ChildrenUpdateImpl()
        {
            UpdateInterfaceDrawersIfNeeded();
            UpdateMemberGroup(DrawingValue);
        }

        #endregion Update

        #region Hierarchy

        public bool IsRootValueDrawer { get { return this == RootValueDrawer; } }

        /// <summary>
        ///     Mosted rooted <see cref="ValueDrawer" /> in drawer hierarchy of current instance.
        /// </summary>
        public ValueDrawer RootValueDrawer { get; private set; }

        public ValueDrawer ParentValueDrawer
        {
            get
            {
                var p = Parent;
                while (p != null)
                {
                    var pvd = p as ValueDrawer;
                    if (pvd != null)
                        return pvd;
                    p = p.Parent;
                }
                return null;
            }
        }

        public IEnumerable<ValueDrawer> ParentValueDrawers
        {
            get { return Parents.Where(p => p is ValueDrawer).Cast<ValueDrawer>(); }
        }

        protected override void AfterChangeParent(Drawer oldParent)
        {
            base.AfterChangeParent(oldParent);
            UpdateRootValueDrawer();
            UpdateReferencedParent();
            UpdateParentsWithSameValueType();
        }

        private void UpdateRootValueDrawer()
        {
            Drawer drawer = this;
            while (drawer != null)
            {
                var valueDrawer = drawer as ValueDrawer;
                if (valueDrawer != null)
                    RootValueDrawer = valueDrawer;

                drawer = drawer.Parent;
            }
        }

        #region Loop Handling

        #region Referenced Parent

        /// <summary>
        ///     Indicates whether current instance is a reference drawer that draws a reference value from one of its parent
        ///     drawer.
        /// </summary>
        public bool IsReferenceOfParent { get { return ReferencedParent != null; } }

        public ValueDrawer ReferencedParent { get; private set; }

        protected void UpdateReferencedParent()
        {
            var value = DrawingValue;
            if (value != null)
            {
                foreach (var parent in ParentValueDrawers)
                {
                    var pve = parent.ValueEntry;
                    if (pve == null)
                        continue;

                    if (ReferenceEquals(value, parent.DrawingValue) && DrawType == parent.DrawType)
                    {
                        ReferencedParent = parent;
                        break;
                    }
                }
            }
            else
            {
                ReferencedParent = null;
            }
        }

        #endregion Referenced Parent

        #region Drawing Value Type Loop

        public int ParentCountWithSameValueType { get { return parentsWithSameValueType.Count; } }
        public IEnumerable<ValueDrawer> ParentsWithSameValueType { get { return parentsWithSameValueType; } }

        private List<ValueDrawer> parentsWithSameValueType = new List<ValueDrawer>();

        private void UpdateParentsWithSameValueType()
        {
            var value = DrawingValue;
            if (value == null)
                return;

            var type = ObjectUtil.GetType(DrawingValue);
            parentsWithSameValueType = ParentValueDrawers
                .Where(d => ObjectUtil.GetType(d.DrawingValue) == type)
                .ToList();
        }

        #endregion Drawing Value Type Loop

        #endregion Loop Handling

        #endregion Hierarchy

        #region Value Access

        /// <summary>
        ///     Where the drawing value come from.
        /// </summary>
        public IValueEntry ValueEntry
        {
            get { return valueEntry; }
            set
            {
                if (value != null)
                {
                    Ensure.Argument.Satisfy(CanDraw(value), "value",
                        string.Format("Unable to draw '{0}' by '{1}'.", value, this));
                }

                if (Equals(valueEntry, value))
                    return;

                var old = valueEntry;
                valueEntry = value;

                OnValueEntryChanged(old);
            }
        }

        private IValueEntry valueEntry;

        protected virtual void OnValueEntryChanged(IValueEntry old) { UpdateDrawingValue(); }

        public virtual bool CanDraw(IValueEntry valueEntry)
        {
            Ensure.Argument.NotNull(valueEntry, "valueEntry");
            return true;
        }

        public virtual Type DrawType { get { return ValueEntry != null ? ValueEntry.TypeOfValue : null; } }

        /// <summary>
        ///     Whether the drawer support set value.
        ///     This property usually decides whether the GUI element is disabled.
        /// </summary>
        public bool CanSetValue
        {
            get
            {
                if (ValueEntry == null)
                    return false;
                if (!ValueEntry.CanWrite)
                    return false;

                var type = ValueEntry.TypeOfValue;
                if (type == null)
                    return false;

                return type.IsPrimitive || type.IsEnum
                    || type == typeof(string)
                    || typeof(UObject).IsAssignableFrom(type);
            }
        }

        public ValueAccessResult SetValue(object value, bool handleErrorLogs, bool catchException)
        {
            if (ValueEntry == null)
                throw new InvalidOperationException("Value entry does not exist.");

            var result = ValueEntry.SetValue(value, handleErrorLogs, catchException);
            AfterSetValue(result);
            return result;
        }

        protected void AfterSetValue(ValueAccessResult result)
        {
            var parentGroup = Parent as MemberGroupDrawer;
            if (parentGroup != null)
                parentGroup.OnChildValueSet(this, result);
        }

        #region Comaprison

        public IEqualityComparer ValueComparer;

        protected bool ValueEquals(object l, object r)
        {
            return ValueComparer != null ? ValueComparer.Equals(l, r) : Equals(l, r);
        }

        #endregion Comaprison

        #region Before Draw

        /// <summary>
        ///     The value that current instance is drawing.
        /// </summary>
        public virtual object DrawingValue
        {
            get { return drawingValue; }
            protected set
            {
                if (ValueEquals(drawingValue, value))
                    return;

                var old = drawingValue;
                drawingValue = value;
                OnDrawingValueChanged(old);
            }
        }

        private object drawingValue;

        protected bool drawingValueInitialized;
        protected ValueAccessResult? drawingValueGetResult;

        protected virtual void UpdateDrawingValue()
        {
            if (ValueEntry != null && ValueEntry.CanRead)
            {
                object value;
                drawingValueGetResult = ValueEntry.GetValue(out value, true, true);
                DrawingValue = value;
            }
            else
            {
                drawingValueGetResult = null;
                DrawingValue = null;
            }

            drawingValueInitialized = true;
        }

        protected virtual void OnDrawingValueChanged(object oldValue)
        {
            var newValue = DrawingValue;
            Type oldType = ObjectUtil.GetType(oldValue), newType = ObjectUtil.GetType(newValue);
            bool typeChanged = oldType != newType;

            if (typeChanged)
            {
                interfaceDrawersNeedUpdate = true;
                memberGroupInstancesNeedUpdate = true;
            }
            memberGroupContentsNeedUpdate = true;

            UpdateReferencedParent();
        }

        #endregion Before Draw

        #region After Draw

        protected virtual object drawnValue
        {
            get { return _drawnValue; }
            set
            {
                if (ValueEquals(_drawnValue, value))
                    return;
                _drawnValue = value;
                drawnValueNeedApply = true;
            }
        }

        private object _drawnValue;

        protected bool drawnValueNeedApply;

        protected ValueAccessResult? drawnValueSetResult;

        protected override void ApplyGUIData()
        {
            base.ApplyGUIData();
            ApplyDrawnValue();
        }

        protected virtual bool ApplyDrawnValue()
        {
            if (!drawnValueNeedApply || !CanSetValue)
                return false;

            drawnValueSetResult = SetValue(drawnValue, true, true);
            if (drawnValueSetResult.Value)
                UpdateDrawingValue();

            drawnValueNeedApply = false;
            return true;
        }

        #endregion After Draw

        #endregion Value Access

        #region GUI

        protected Rect FoldoutRect;
        protected Rect FieldRect;

        protected override void SelfGUI()
        {
            if (ValueEntry == null)
                return;

            using (new EditorGUILayout.HorizontalScope())
            {
                // Reserve area for foldout triangle.
                // Foldout is going to draw after value of this instance is drawn since its' visibility depends on
                // child count of this drawer, that is, depends on value drawn by this drawer and DrawSelf must be called
                // before drawing foldout triangle.
                FoldoutRect = DIGUI.GetFoldoutToggleRect();

                FieldGUI();

                if (ChildCount > 0 && !IsReferenceOfParent)
                    ChildrenVisible = DIGUI.FoldoutToggle(FoldoutRect, ChildrenVisible);
            }
            if (Event.current.type == EventType.Repaint)
                FieldRect = GUILayoutUtility.GetLastRect();

            ReferenceEmphasisGUI();
        }

        protected virtual void FieldGUI()
        {
            NameGUI();
            ValueGUI();
            ValueErrorGUI();
            ReferencedParentGUI();
        }

        #region Name

        protected readonly GUIContent nameGUIContent = new GUIContent();

        protected virtual void NameGUI(bool isPrefix = true)
        {
            nameGUIContent.text = Name;

            var actualType = DrawingValue != null ? DrawingValue.GetType() : ValueEntry.TypeOfValue;
            nameGUIContent.tooltip = actualType.ToString();

            if (isPrefix)
                EditorGUILayout.PrefixLabel(nameGUIContent);
            else
                EditorGUILayout.LabelField(nameGUIContent);
        }

        #endregion

        #region Value

        public bool ValueVisible = true;

        protected readonly GUIContent valueGUIContent = new GUIContent();

        protected virtual void ValueGUI()
        {
            if (!ValueVisible)
                return;

            if (drawingValueGetResult == null)
                return;

            var result = drawingValueGetResult.Value;
            if (result.ValueAccessed) // Value successfully got.
            {
                using (new EditorGUIEx.DisabledScope(!CanSetValue))
                    ValueGUIImpl();
            }
            else // If exception is thrown while getting value, the value is not available.
            {
                var e = result.Exception;
                valueGUIContent.text = e.GetType().Name;
                valueGUIContent.tooltip = e.Message;
                EditorGUILayout.LabelField(valueGUIContent, DIGUIStyles.ExceptionLabel);
            }
        }

        protected virtual void ValueGUIImpl()
        {
            valueGUIContent.text = DrawingValue != null ? DrawingValue.ToString() : "(null)";
            EditorGUILayout.LabelField(valueGUIContent);

            if (drawingValueInitialized)
                drawnValue = DrawingValue;
        }

        #endregion Value

        #region Value Access Error

        protected virtual void ValueErrorGUI()
        {
            if (!ValueAccessResult.IsNullOrFailed(drawingValueGetResult) ||
                !ValueAccessResult.IsNullOrFailed(drawnValueSetResult))
                return;

            if (DIGUI.ErrorButton(GetErrorButtonTooltip()))
            {
                var window = ValueAccessResultWindow.Open();
                window.GetResult = drawingValueGetResult ?? default(ValueAccessResult);
                window.SetResult = drawnValueSetResult ?? default(ValueAccessResult);
            }
        }

        private string GetErrorButtonTooltip() { return null; }

        #endregion Value Access Error

        #region Reference Target

        protected virtual void ReferencedParentGUI()
        {
            if (ValueEntry.TypeOfValue.IsValueType)
                return;
            if (!IsReferenceOfParent)
                return;

            if (DIGUI.GoUpButton(FoldoutRect))
                ReferenceEmphasis();
        }

        #endregion Reference Target

        #region Reference Emphasis

        public void ReferenceEmphasis()
        {
            if (!IsReferenceOfParent)
                return;

            referenceEmphasisDrawer.BeginRect = ReferencedParent.FieldRect;
            referenceEmphasisDrawer.EndRect = FieldRect;
            referenceEmphasisDrawer.Visibility = ReferencedRectDrawer.VisibleState.ControlByAnimation;
            referenceEmphasisDrawer.Animation.Play();
        }

        private readonly ReferencedRectDrawer referenceEmphasisDrawer = new ReferencedRectDrawer();

        private void ReferenceEmphasisGUI() { referenceEmphasisDrawer.GUI(); }

        #endregion Reference Emphasis

        #endregion GUI

        #region Children

        public override void ClearChildren()
        {
            base.ClearChildren();
            interfaceDrawers.Clear();
            MemberGroup = null;
        }

        #region Interface Drawers

        public int InterfaceDrawerCount { get { return interfaceDrawers.Count; } }

        public IEnumerable<KeyValuePair<Type, ValueDrawer>> InterfaceDrawers { get { return interfaceDrawers; } }

        public ValueDrawer GetInterfaceDrawer(Type interfaceType)
        {
            Ensure.Argument.Satisfy(interfaceType.IsInterface, "interfaceType", "Interface type expected.");
            return interfaceDrawers.GetValue(interfaceType);
        }

        protected bool interfaceDrawersNeedUpdate = true;

        protected void UpdateInterfaceDrawersIfNeeded()
        {
            if (!interfaceDrawersNeedUpdate)
                return;
            UpdateInterfaceDrawers();
            interfaceDrawersNeedUpdate = false;
        }

        protected void UpdateInterfaceDrawers()
        {
            interfaceDrawers.Clear();

            if (ValueEntry == null)
                return;

            var interfaceTypes = ValueEntry.TypeOfValue.GetMostDerivedInterfaces();
            if (interfaceTypes.Length == 0)
                return;

            var ownerType = ValueEntry.TypeOfOwner;
            foreach (var interfaceType in interfaceTypes)
            {
                var drawerType = ValueDrawerTypes.GetOrBuild(ownerType, interfaceType, false);
                if (drawerType == null)
                    continue;

                var drawer = (ValueDrawer)Create(drawerType, interfaceType.GetReadableName(), this);
                drawer.ValueEntry = ProxyValueEntryFactory.Create(ownerType, interfaceType, ValueEntry);
                drawer.ValueVisible = false;

                interfaceDrawers.Add(interfaceType, drawer);
            }
        }

        private readonly Dictionary<Type, ValueDrawer> interfaceDrawers = new Dictionary<Type, ValueDrawer>();

        #endregion Interface Drawers

        #region Member Group

        /// <summary>
        ///     Group drawer that contains all child member drawers.
        /// </summary>
        public MemberGroupDrawer MemberGroup { get; private set; }

        /// <summary>
        ///     Indicates whether the <see cref="MemberGroup" /> instance or its child instances need update.
        ///     If the value is set to <see langword="true" /> the <see cref="MemberGroup" /> instance and its child instances will
        ///     update in next <see cref="ChildrenUpdate" /> call.
        /// </summary>
        protected bool memberGroupInstancesNeedUpdate = true;

        /// <summary>
        ///     Indicates whether contents of the <see cref="MemberGroup" />'s children need update.
        /// </summary>
        protected bool memberGroupContentsNeedUpdate = true;

        protected void UpdateMemberGroup(object value)
        {
            if (memberGroupInstancesNeedUpdate)
            {
                var valueType = ObjectUtil.GetType(value);

                var members = SelectMembers(MemberGroupDrawer.GetMembers(valueType));
                if (!CollectionUtil.IsNullOrEmpty(members))
                {
                    if (MemberGroup == null)
                        MemberGroup = Create<MemberGroupDrawer>(null, this);
                    MemberGroup.SetChildren(valueType, members);
                }
                else
                {
                    if (MemberGroup == null)
                        return;
                    MemberGroup.Parent = null;
                    MemberGroup = null;
                }

                memberGroupInstancesNeedUpdate = false;
            }

            if (memberGroupContentsNeedUpdate)
            {
                if (MemberGroup != null)
                    MemberGroup.UpdateChildrenValueEntry(value, CreateMemberGroupChildValueEntry);
                memberGroupContentsNeedUpdate = false;
            }
        }

        protected virtual IValueEntry CreateMemberGroupChildValueEntry(object value, MemberInfo member)
        {
            return MemberEntryFactory.Create(member.DeclaringType, value, member);
        }

        /// <summary>
        ///     When overriden, select members that need to update by <see cref="MemberGroup" />.
        /// </summary>
        /// <param name="members">Members to selected.</param>
        protected virtual IEnumerable<MemberInfo> SelectMembers(MemberInfo[] members) { return members; }

        #endregion Member Group

        #endregion Children
    }
}