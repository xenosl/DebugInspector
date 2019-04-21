using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    using UObject = UnityEngine.Object;

    public class InspectorWindow : EditorWindow
    {
        public const string Title = "Debug Inspector";

        [MenuItem("Window/" + Title)]
        public static void Open() { Get().Show(); }

        public new static void Close() { ((EditorWindow)Get()).Close(); }

        private static InspectorWindow Get() { return GetWindow<InspectorWindow>(Title); }

        private void OnDestroy() { DrawerStates.Save(); }

        private void OnEnable()
        {
            InitSettings();
            UpdateDrawerInstances();
            InitSearchFilter();
        }

        private void OnDisable()
        {
            DeinitSearchFilter();
            DestroyDrawerInstances();
            DeinitSettings();
        }

        //private void OnFocus()
        //{
        //    //UpdateDrawers();
        //}

        private readonly FrameCounter frameCounter = new FrameCounter();

        private void Update()
        {
            if (!frameCounter.Update())
                return;

            if (!searching) // Avoid value change during search (This may cause problems).
                DrawersUpdateOnUpdate();

            Repaint();
        }

        private void OnSelectionChange() { UpdateDrawerInstances(); }

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                searchFilter.SearchTextGUI();
                SettingsGUI();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawersGUI();
            DrawersUpdateGUI();
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            SearchProgressGUI();

            SearchInputUpdate();
        }

        private void SettingsGUI()
        {
            if (DIGUI.SettingsButton())
                SettingsWindow.Open();
        }

        #region Search Filter

        private readonly DrawerSearchFilter searchFilter = new DrawerSearchFilter();

        private bool searching;

        private DrawerUpdateParameters drawerSearchUpdateParameters;

        private void InitSearchFilter()
        {
            searchFilter.SearchTextChanging += OnSearchTextChanging;
            searchFilter.SearchTextApplying += OnSearchTextApplying;
            searchFilter.SearchTextApplied += OnSearchTextApplied;

            drawerSearchUpdateParameters = new DrawerUpdateParameters
            {
                MaxDepth = Settings.SearchDepth,
                DeepUpdate = true,
                ChildrenUpdatePrecondition = ChildrenUpdatePreconditionDuringSearch
            };
        }

        private void DeinitSearchFilter()
        {
            EndSearch();

            drawerSearchUpdateParameters = null;

            searchFilter.SearchTextApplied -= OnSearchTextApplied;
            searchFilter.SearchTextApplying -= OnSearchTextApplying;
            searchFilter.SearchTextChanging -= OnSearchTextChanging;
        }

        private void BeginSearch()
        {
            if (searching)
                throw new InvalidOperationException("Last search is not done.");

            // Temporary store states.
            foreach (var drawer in AllDrawers)
                drawer.SaveState(DrawerActionScope.Self);

            // Hide all drawers in the beginning. (Show filtered only.)
            foreach (var drawer in AllDrawers)
                drawer.SelfVisible = false;

            // Setup search filter.
            searchFilter.ResultDrawerAdded += OnSearchResultAdded;
            searchFilter.AddTargetDrawers(RootDrawers, DrawerActionScope.SelfAndChildrenInHierarchy, false);

            // Register event for up coming drawers.
            foreach (var drawer in RootDrawers)
                drawer.ChildAddedInHierarchy += OnChildAddedInHierarchyDuringSearch;

            // Update drawers.
            drawerSearchUpdateParameters.DeepUpdate = Settings.DeepSearch;
            DrawersUpdateImmediate(true, drawerSearchUpdateParameters);

            searching = true;
        }

        private Drawer[] searchResultDrawers;

        private void PrepareEndSearch() { searchResultDrawers = searchFilter.ResultDrawers.ToArray(); }

        private void EndSearch()
        {
            if (!searching)
                return;

            searching = false;

            // Stop updating.
            StopDrawersAsyncUpdate();
            // Cancal registerations.
            foreach (var drawer in RootDrawers)
                drawer.ChildAddedInHierarchy -= OnChildAddedInHierarchyDuringSearch;

            // Cleanup search filter.
            searchFilter.ClearTargetDrawers();
            searchFilter.ResultDrawerAdded -= OnSearchResultAdded;
            searchFilter.SearchText = string.Empty;

            foreach (var drawer in AllDrawers)
                drawer.ResetVisiblity(DrawerActionScope.Self);
            foreach (var drawer in searchResultDrawers)
                drawer.ExpandToSelf();
        }

        private void UpdateSearch() { }

        private void SearchInputUpdate()
        {
            if (focusedWindow != this)
                return;

            if (!searching)
                return;

            var evt = Event.current;
            if (!evt.isKey)
                return;

            if (evt.type == EventType.KeyUp && evt.keyCode == KeyCode.Escape)
                searchFilter.SearchText = string.Empty;
        }

        private bool ChildrenUpdatePreconditionDuringSearch(Drawer drawer) { return true; }

        private void OnSearchTextChanging(string newText)
        {
            if (!searching)
                return;
            if (string.IsNullOrEmpty(newText))
                return;

            StopDrawersAsyncUpdate();

            foreach (var drawer in RootDrawers)
            {
                drawer.SelfVisible = false;
                drawer.ChildrenVisible = false;
            }
        }

        private void OnSearchTextApplying(string oldText, string newText)
        {
            var oldTextEmpty = string.IsNullOrEmpty(oldText);
            var newTextEmpty = string.IsNullOrEmpty(newText);

            if (oldTextEmpty && !newTextEmpty)
                BeginSearch();
            else if (!oldTextEmpty && newTextEmpty)
                PrepareEndSearch();
        }

        private void OnSearchTextApplied(string oldText)
        {
            var oldTextEmpty = string.IsNullOrEmpty(oldText);
            var newTextEmpty = string.IsNullOrEmpty(searchFilter.SearchText);

            if (!oldTextEmpty && newTextEmpty)
                EndSearch();
            else
                UpdateSearch();
        }

        private void OnSearchResultAdded(Drawer drawer) { drawer.ExpandToSelf(); }

        private Drawer lastAddedChildDuringSearch;

        private void OnChildAddedInHierarchyDuringSearch(Drawer child)
        {
            child.SelfVisible = false;
            searchFilter.AddTargetDrawer(child, DrawerActionScope.Self, true);

            lastAddedChildDuringSearch = child;
        }

        private void SearchProgressGUI()
        {
            if (RootValueDrawers.Any(d => d.IsAsyncUpdating))
            {
                var name = lastAddedChildDuringSearch != null ? lastAddedChildDuringSearch.HierarchicalName : "";
                EditorGUILayout.LabelField("Searching... " + name, EditorStyles.miniLabel);
            }
        }

        #endregion Search Filter

        #region Drawers

        private readonly GUILayoutOption[] drawersGUIOptions = new GUILayoutOption[1];

        private void DrawersGUI()
        {
            int count = RootValueDrawerCount;
            if (count == 0)
                return;

            var width = position.width;
            drawersGUIOptions[0] = GUILayout.MaxWidth(width);
            for (int i = 0; i < count; ++i)
            {
                var drawer = rootValueDrawers[i];
                drawer.GUI(null, drawersGUIOptions);

                // Draw separate line.
                if (i < count - 1)
                    GUIEx.HorizontalLine(position.width);
            }
        }

        #region Update

        private void DrawersUpdateOnUpdate()
        {
            if (Settings.DrawerUpdateMode == DrawerUpdateMode.TimeInterval)
                DrawersTimeIntervalUpdate();
            DrawersUpdateIfNeeded(false, DrawerUpdateParameters.Default);
        }

        private void DrawersUpdateGUI()
        {
            if (Settings.DrawerUpdateMode != DrawerUpdateMode.Manual)
                return;
            if (RootValueDrawerCount == 0)
                return;

            if (GUILayout.Button("Update"))
                drawersNeedUpdate = true;
        }

        private double nextDrawerUpdateTime;

        private void DrawersTimeIntervalUpdate()
        {
            if (frameCounter.Time < nextDrawerUpdateTime)
                return;

            drawersNeedUpdate = true;

            nextDrawerUpdateTime = Math.Max(
                nextDrawerUpdateTime + Settings.DrawerUpdateTimeInterval, frameCounter.Time);
        }

        private bool drawersNeedUpdate = true;

        private void DrawersUpdateImmediate(bool async, DrawerUpdateParameters parameters)
        {
            DrawersUpdate(async, parameters);
            drawersNeedUpdate = false;
        }

        private void DrawersUpdateIfNeeded(bool async, DrawerUpdateParameters parameters)
        {
            if (!drawersNeedUpdate)
                return;
            DrawersUpdate(async, parameters);
            drawersNeedUpdate = false;
        }

        private void DrawersUpdate(bool async, DrawerUpdateParameters parameters)
        {
            for (int i = 0; i < RootValueDrawerCount; ++i)
            {
                var drawer = rootValueDrawers[i];
                if (async)
                    drawer.AsyncUpdate(parameters);
                else
                    drawer.Update(parameters);
            }
        }

        private void StopDrawersAsyncUpdate()
        {
            for (int i = 0; i < RootValueDrawerCount; ++i)
                rootValueDrawers[i].StopAsyncUpdate();
        }

        #endregion Update

        #region Instances

        public const int MaxDrawerCount = 8;

        public int RootValueDrawerCount { get { return rootValueDrawers.Count; } }
        public IEnumerable<ValueDrawer> RootValueDrawers { get { return rootValueDrawers; } }

        public IEnumerable<Drawer> RootDrawers { get { return RootValueDrawers.Cast<Drawer>(); } }

        public IEnumerable<Drawer> AllDrawers
        {
            get { return RootDrawers.Concat(RootDrawers.SelectMany(d => d.ChildrenInHierarchy)); }
        }

        private List<ValueDrawer> rootValueDrawers = new List<ValueDrawer>();

        private void UpdateDrawerInstances() { UpdateDrawerInstances(Selection.objects); }

        private void UpdateDrawerInstances(IEnumerable<UObject> objects)
        {
            EndSearch();

            DestroyDrawerInstances();

            var targetObjects = objects.Where(o => o).Take(MaxDrawerCount).ToArray();
            rootValueDrawers = targetObjects
                .Select((obj, index) => CreateDrawer(targetObjects, obj, index))
                .ToList();

            DrawersUpdateImmediate(false, DrawerUpdateParameters.Default);
        }

        private void DestroyDrawerInstances()
        {
            if (RootValueDrawerCount == 0)
                return;

            for (int i = RootValueDrawerCount - 1; i >= 0; --i)
            {
                var drawer = rootValueDrawers[i];
                drawer.StopAsyncUpdate();
                drawer.SaveState(DrawerActionScope.SelfAndChildrenInHierarchy);
                //Drawer.Destroy(drawer);   
            }
            rootValueDrawers.Clear();
        }

        private ValueDrawer CreateDrawer(UObject[] objects, UObject obj, int index)
        {
            var valueEntry = SelectionEntryFactory.Create(obj.GetType(), objects, index);
            var drawer = ValueDrawer.Create(valueEntry.TypeOfOwner, valueEntry.TypeOfValue, objects[index].name);
            drawer.ValueEntry = valueEntry;
            drawer.SelfVisible = true;
            drawer.ChildrenVisible = true;
            return drawer;
        }

        #endregion Instances

        #endregion Drawers

        #region Settings

        private void InitSettings()
        {
            Settings.VisibleMemberTypesChanged += OnVisibleMemberTypesChanged;
            Settings.VisiblityByAccessModifiersChanged += OnVisiblityByAccessModifiersChanged;
        }

        private void DeinitSettings()
        {
            Settings.VisiblityByAccessModifiersChanged -= OnVisiblityByAccessModifiersChanged;
            Settings.VisibleMemberTypesChanged -= OnVisibleMemberTypesChanged;
        }

        private void OnVisibleMemberTypesChanged(VisibleMemberTypes old) { OnMemberVisiblityChanged(); }

        private void OnVisiblityByAccessModifiersChanged(AccessModifiers oldFlags) { OnMemberVisiblityChanged(); }

        private void OnMemberVisiblityChanged()
        {
            var objects = RootValueDrawers
                .Select(d => d.DrawingValue)
                .Where(v => v is UObject)
                .Cast<UObject>();
            UpdateDrawerInstances(objects.ToArray());
        }

        #endregion
    }
}