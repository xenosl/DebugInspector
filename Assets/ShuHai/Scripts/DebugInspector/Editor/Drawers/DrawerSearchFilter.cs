using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public class DrawerSearchFilter
    {
        public DrawerSearchFilter() { InitializeSearchText(); }

        #region Search Contents

        [Flags]
        public enum ContentFlags
        {
            Name = 0x1,
            Type = 0x2,
            All = Name | Type
        }

        public ContentFlags SearchContents
        {
            get { return searchContents; }
            set { PropertyUtil.SetValue(ref searchContents, value, OnSearchContentsChanged, null); }
        }

        private ContentFlags searchContents = ContentFlags.Name;

        private readonly EnumFlagsField<ContentFlags> searchContentsField = new EnumFlagsField<ContentFlags>();

        private void OnSearchContentsChanged(ContentFlags oldContents)
        {
            UpdateResultDrawers();
            searchContentsField.Value = SearchContents;
        }

        #endregion Search Contents

        #region Search Text

        public event Action<string> SearchTextChanging;

        public event Action<string, string> SearchTextApplying;
        public event Action<string> SearchTextApplied;

        public float SearchTextApplyDelay
        {
            get { return (float)(searchTextApplyTimer.Interval / 1000); }
            set { searchTextApplyTimer.Interval = value * 1000; }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (value == searchText)
                    return;

                SearchTextChanging.NPInvoke(value);

                searchText = value ?? string.Empty;
                if (searchText != string.Empty)
                {
                    searchTextApplyTimer.Stop();
                    searchTextApplyTimer.Start();
                }
                else // searchText == string.Empty, then search done, apply change immediately.
                {
                    ApplySearchTextChange();
                }
            }
        }

        public void SearchTextGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            SearchText = GUILayout.TextField(SearchText, EditorStylesEx.ToolbarSearchField);

            var searchTextExist = !string.IsNullOrEmpty(SearchText);
            var cancelButtonStyle = searchTextExist
                ? EditorStylesEx.ToolbarSearchFieldCancelButton : EditorStylesEx.ToolbarSearchFieldCancelButtonEmpty;
            if (GUILayout.Button(GUIContent.none, cancelButtonStyle) && searchTextExist)
                SearchText = string.Empty;

            EditorGUILayout.EndHorizontal();
        }

        private string searchText = string.Empty;

        private string lastAppliedSearchText = string.Empty;
        private readonly Timer searchTextApplyTimer = new Timer(1000) { AutoReset = false };

        private void InitializeSearchText() { searchTextApplyTimer.Elapsed += ApplySearchTextChange; }

        private void ApplySearchTextChange()
        {
            if (lastAppliedSearchText == searchText)
                return;

            SearchTextApplying.NPInvoke(lastAppliedSearchText, searchText);

            if (!string.IsNullOrEmpty(searchText))
                UpdateResultDrawers();
            else
                ClearResultDrawers();

            SearchTextApplied.NPInvoke(lastAppliedSearchText);

            lastAppliedSearchText = searchText;
        }

        #endregion Search Text

        #region Options

        [Flags]
        public enum FilterStringOptions
        {
            IgnoreCase = 0x1,
            MatchWholeWord = 0x2
        }

        public FilterStringOptions FilterTextOptions = FilterStringOptions.IgnoreCase;

        #endregion Options

        #region Filter Targets

        public int TargetDrawerCount { get { return targetDrawers.Count; } }

        public IEnumerable<Drawer> TargetDrawers
        {
            get
            {
                targetDrawerIterating = true;
                foreach (var d in targetDrawers)
                    yield return d;
                targetDrawerIterating = false;
                ApplyTargetDrawersToBeAdded();
            }
        }

        public void ClearTargetDrawers() { targetDrawers.Clear(); }

        private readonly HashSet<Drawer> targetDrawers = new HashSet<Drawer>();

        private bool targetDrawerIterating;

        #region Add

        public void AddTargetDrawer(Drawer drawer, DrawerActionScope scope, bool updateResult)
        {
            Ensure.Argument.NotNull(drawer, "drawer");
            AddTargetDrawersWithIteratingCheck(drawer.EnumerateByScope(scope), updateResult);
        }

        public void AddTargetDrawers(IEnumerable<Drawer> drawers, DrawerActionScope scope, bool updateResult)
        {
            foreach (var d in drawers)
                AddTargetDrawer(d, scope, updateResult);
        }

        private readonly List<TargetDrawerAddParams> targetDrawersToBeAdd = new List<TargetDrawerAddParams>();

        private void AddTargetDrawerWithIteratingCheck(Drawer drawer, bool updateResult)
        {
            if (targetDrawerIterating)
                targetDrawersToBeAdd.Add(new TargetDrawerAddParams(drawer, updateResult));
            else
                AddTargetDrawerImpl(drawer, updateResult);
        }

        private void AddTargetDrawersWithIteratingCheck(IEnumerable<Drawer> drawers, bool updateResult)
        {
            if (targetDrawerIterating)
                targetDrawersToBeAdd.AddRange(drawers.Select(d => new TargetDrawerAddParams(d, updateResult)));
            else
                AddTargetDrawersImpl(drawers, updateResult);
        }

        private void ApplyTargetDrawersToBeAdded()
        {
            if (targetDrawersToBeAdd.Count == 0)
                return;

            foreach (var info in targetDrawersToBeAdd)
                AddTargetDrawerImpl(info.Instance, info.UpdateResult);

            targetDrawersToBeAdd.Clear();
        }

        private void AddTargetDrawerImpl(Drawer drawer, bool updateResult)
        {
            if (drawer.DepthInHierarchy > Settings.SearchDepth)
                return;

            if (!targetDrawers.Add(drawer))
                return;

            if (updateResult)
                UpdateFilterResultDrawersOnTargetAdded(drawer);
        }

        private void AddTargetDrawersImpl(IEnumerable<Drawer> drawers, bool updateResult)
        {
            foreach (var d in drawers)
                AddTargetDrawerImpl(d, updateResult);
        }

        private struct TargetDrawerAddParams
        {
            public Drawer Instance;
            public bool UpdateResult;

            public TargetDrawerAddParams(Drawer instance, bool updateResult)
            {
                Instance = instance;
                UpdateResult = updateResult;
            }
        }

        #endregion Add

        #endregion Filter Targets

        #region Filter Operations

        #region Filter Results

        public event Action<Drawer> ResultDrawerAdded;
        public event Action ResultDrawersCleared;

        public int ResultDrawerCount { get { return resultDrawers.Count; } }
        public IEnumerable<Drawer> ResultDrawers { get { return resultDrawers; } }

        public void UpdateResultDrawers()
        {
            ClearResultDrawers();
            FilterToResultDrawers(TargetDrawers);
        }

        private readonly HashSet<Drawer> resultDrawers = new HashSet<Drawer>();

        private void ClearResultDrawers()
        {
            resultDrawers.Clear();
            ResultDrawersCleared.NPInvoke();
        }

        private void FilterToResultDrawers(Drawer drawer)
        {
            if (Filter(drawer) && resultDrawers.Add(drawer))
                ResultDrawerAdded.NPInvoke(drawer);
        }

        private void FilterToResultDrawers(IEnumerable<Drawer> drawers) { drawers.ForEach(FilterToResultDrawers); }

        private void UpdateFilterResultDrawersOnTargetAdded(Drawer newTarget)
        {
            FilterToResultDrawers(newTarget);
            FilterToResultDrawers(newTarget.ChildrenInHierarchy);
        }

        #endregion Filter Results

        #region Filter Action

        public bool Filter(Drawer drawer)
        {
            if (string.IsNullOrEmpty(SearchText))
                return true;

            if (drawer is GroupDrawer)
                return false;

            if (SearchContents.HasFlag(ContentFlags.Name))
                return Filter(drawer.Name);

            if (SearchContents.HasFlag(ContentFlags.Type))
            {
                var valueDrawer = drawer as ValueDrawer;
                if (valueDrawer == null)
                    return false;
                var drawingValue = valueDrawer.DrawingValue;
                if (drawingValue == null)
                    return false;
                return Filter(drawingValue.GetType().Name);
            }

            return false;
        }

        private bool Filter(string text)
        {
            var comparison = FilterTextOptions.HasFlag(FilterStringOptions.IgnoreCase)
                ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return FilterTextOptions.HasFlag(FilterStringOptions.MatchWholeWord)
                ? text.Equals(SearchText, comparison)
                : text.Contains(SearchText, comparison);
        }

        #endregion Filter Action

        #endregion Filter Operations
    }
}