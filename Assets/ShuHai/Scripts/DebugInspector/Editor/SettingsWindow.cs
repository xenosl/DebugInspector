using System;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    using AccessModifiersField = EnumFlagsField<AccessModifiers>;
    using MemberTypeFlagsField = EnumFlagsField<VisibleMemberTypes>;

    public sealed class SettingsWindow : EditorWindow
    {
        public const string Title = "Settings";

        public static void Open() { GetWindow<SettingsWindow>(Title).Show(); }

        public new static void Close() { ((EditorWindow)GetWindow<SettingsWindow>(Title)).Close(); }

        private void OnEnable() { }

        private void OnDestroy() { Settings.Save(); }

        private void OnGUI()
        {
            OptionsGUI("Visible Members", ref VisibleMembersFoldout, VisibleMembersGUI);
            OptionsGUI("Update Settings", ref UpdateSettingsFoldout, UpdateSettingsGUI);
            OptionsGUI("Search Settings", ref SearchSettingsFoldout, SearchSettingsGUI);
        }

        private static void OptionsGUI(string name, ref bool foldout, Action contentsGUI)
        {
            foldout = EditorGUILayout.Foldout(foldout, name);
            if (!foldout)
                return;

            using (new EditorGUIEx.IndentLevelScope(1))
                contentsGUI();
        }

        #region Options 

        #region Visible Members

        public bool VisibleMembersFoldout = true;

        private readonly MemberTypeFlagsField VisibleMemberTypesField =
            new MemberTypeFlagsField("Types", VisibleMemberTypes.All);

        private readonly AccessModifiersField VisiblityByAccessModifiersField =
            new AccessModifiersField("Access Modifiers", AccessModifiers.All);

        private void VisibleMembersGUI()
        {
            Settings.VisibleMemberTypes = VisibleMemberTypesField.GUI();
            Settings.VisiblityByAccessModifiers = VisiblityByAccessModifiersField.GUI();
        }

        #endregion Visible Members

        #region Update

        public bool UpdateSettingsFoldout = true;

        private static readonly GUIContent dataUpdateModeContent =
            new GUIContent("Data Update Mode", "Determines how the data is updated.");

        private static readonly GUIContent timeIntervalContent = new GUIContent("Time Interval",
            "Determines the time interval between each data update. " +
            "If the value is less than time interval between each frame, updates occurs every frame");

        private void UpdateSettingsGUI()
        {
            var updateMode = Settings.DrawerUpdateMode;
            updateMode = EditorGUIEx.EnumPopup(dataUpdateModeContent, updateMode);
            switch (updateMode)
            {
                case DrawerUpdateMode.TimeInterval:
                    Settings.DrawerUpdateTimeInterval = EditorGUILayout
                        .FloatField(timeIntervalContent, Settings.DrawerUpdateTimeInterval);
                    break;
            }
            Settings.DrawerUpdateMode = updateMode;
        }

        #endregion Update

        #region Search

        public bool SearchSettingsFoldout = true;

        private void SearchSettingsGUI()
        {
            SearchDepthGUI();
            //SearchCollectionsGUI();
            DeepSearchGUI();
        }

        private static readonly GUIContent searchDepthContent = new GUIContent("Search Depth",
            "Determine how deep the search is applied in the object hierarchy");

        private void SearchDepthGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                int depth = Settings.SearchDepth;
                depth = EditorGUILayout.IntField(searchDepthContent, depth);
                if (depth == Settings.UnlimitedSearchDepth)
                    EditorGUILayout.LabelField(searchDepthContent, "(Unlimited)");
                Settings.SearchDepth = depth;
            }
        }

        //private void SearchCollectionsGUI()
        //{
        //    Settings.SearchEnumerables = EditorGUILayout.Toggle("Search Enumerables", Settings.SearchEnumerables);
        //}

        private static readonly GUIContent deepSearchContent = new GUIContent("Deep Search",
            "If checked, perform asynchronous action to search the whole hierarchy deep to primitive type; " +
            "otherwise, only search initialized Debug Inspector objects. " +
            "Initialized objects are objects that ever displayed on GUI " +
            "(Objects are not initialized until you click the triangle icon before it)");

        private void DeepSearchGUI()
        {
            Settings.DeepSearch = EditorGUILayout.Toggle(deepSearchContent, Settings.DeepSearch);
        }

        #endregion Search

        #endregion Options 
    }
}