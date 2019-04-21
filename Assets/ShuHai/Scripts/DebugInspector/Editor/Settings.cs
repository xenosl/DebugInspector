using System;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    public enum DrawerUpdateMode
    {
        TimeInterval,
        Manual
    }

    [Flags]
    public enum VisibleMemberTypes
    {
        Field = 1,
        Property = 2,
        All = Field | Property
    }

    [Flags]
    public enum AccessModifiers
    {
        None = 0,
        Public = 0x1,
        Private = 0x2,
        Protected = 0x4,
        Internal = 0x8,
        ProtectedInternal = 0x10,
        PrivateProtected = 0x20,
        All = Public | Private | Protected | Internal | ProtectedInternal | PrivateProtected
    }

    public sealed class Settings
    {
        #region Singleton

        public static readonly Settings Instance = new Settings();

        private Settings() { }

        #endregion

        #region Options

        #region Hierarchy Loop

        [XmlIgnore]
        public static int MaxHierarchyLoopCount
        {
            get { return Instance.maxHierarchyLoopCount; }
            set
            {
                value = Mathf.Clamp(value, 1, 8);
                PropertyUtil.SetValue(ref Instance.maxHierarchyLoopCount, value, null, null);
            }
        }

        private int maxHierarchyLoopCount = 2;

        #endregion Hierarchy Loop

        #region Update

        public const float DefaultDrawerUpdateInterval = 0f;
        public const float MaxDrawerUpdateInterval = 10;

        public static event Action<DrawerUpdateMode> DrawerUpdateModeChanged;
        public static event Action<float> DrawerUpdateFrameIntervalChanged;

        [XmlIgnore]
        public static DrawerUpdateMode DrawerUpdateMode
        {
            get { return Instance.drawerUpdateMode; }
            set { PropertyUtil.SetValue(ref Instance.drawerUpdateMode, value, null, DrawerUpdateModeChanged); }
        }

        [XmlIgnore]
        public static float DrawerUpdateTimeInterval
        {
            get { return Instance.drawerUpdateTimeInterval; }
            set
            {
                value = Mathf.Clamp(value, 0, MaxDrawerUpdateInterval);
                PropertyUtil.SetValue(ref Instance.drawerUpdateTimeInterval, value,
                    null, DrawerUpdateFrameIntervalChanged);
            }
        }

        private DrawerUpdateMode drawerUpdateMode = DrawerUpdateMode.TimeInterval;
        private float drawerUpdateTimeInterval = DefaultDrawerUpdateInterval;

        #endregion Update

        #region Visible Members

        public static event Action<VisibleMemberTypes> VisibleMemberTypesChanged;

        [XmlIgnore]
        public static VisibleMemberTypes VisibleMemberTypes
        {
            get { return Instance.visibleMemberTypes; }
            set { PropertyUtil.SetValue(ref Instance.visibleMemberTypes, value, null, VisibleMemberTypesChanged); }
        }

        private VisibleMemberTypes visibleMemberTypes = VisibleMemberTypes.All;

        #endregion

        #region Member Visiblity By Modifier

        public static event Action<AccessModifiers> VisiblityByAccessModifiersChanged;

        [XmlIgnore]
        public static AccessModifiers VisiblityByAccessModifiers
        {
            get { return Instance.visiblityByAccessModifiers; }
            set
            {
                PropertyUtil.SetValue(ref Instance.visiblityByAccessModifiers, value,
                    null, VisiblityByAccessModifiersChanged);
            }
        }

        private AccessModifiers visiblityByAccessModifiers = AccessModifiers.All;

        #endregion

        #region Search

        #region Depth

        public const int UnlimitedSearchDepth = int.MaxValue;
        public const int MinSearchDepth = 1;
        public const int DefaultSearchDepth = 10;

        /// <summary>
        /// Determine how deep the search is applied in the object hierarchy
        /// </summary>
        [XmlIgnore]
        public static int SearchDepth
        {
            get { return Instance.searchDepth; }
            set { Instance.searchDepth = Mathf.Clamp(value, MinSearchDepth, int.MaxValue); }
        }

        private int searchDepth = DefaultSearchDepth;

        #endregion Depth

        //[XmlIgnore]
        //public static bool SearchEnumerables
        //{
        //    get { return Instance.searchEnumerables; }
        //    set { Instance.searchEnumerables = value; }
        //}

        //private bool searchEnumerables;

        [XmlIgnore]
        public static bool DeepSearch { get { return Instance.deepSearch; } set { Instance.deepSearch = value; } }

        private bool deepSearch;

        #endregion Search

        #region Page

        public const int MinItemCountPerPage = 3;
        public const int MaxItemCountPerPage = 100;
        public const int DefaultItemCountPerPage = 10;

        public const int DefaultMaxItemCountToEnumerate = 1000;

        public static event Action<int> MaxEnumerateCountChanged;
        public static event Action<int> ItemCountPerPageChanged;

        [XmlIgnore]
        public static int MaxEnumerateCount
        {
            get { return Instance.maxEnumerateCount; }
            set { PropertyUtil.SetValue(ref Instance.maxEnumerateCount, value, null, MaxEnumerateCountChanged); }
        }

        [XmlIgnore]
        public static int ItemCountPerPage
        {
            get { return Instance.itemCountPerPage; }
            set { PropertyUtil.SetValue(ref Instance.itemCountPerPage, value, null, ItemCountPerPageChanged); }
        }

        private int maxEnumerateCount = DefaultMaxItemCountToEnumerate;
        private int itemCountPerPage = DefaultItemCountPerPage;

        #endregion Page

        #endregion Options

        #region Persistence

        public const string Filename = "DrawerSettings";
        public const string FileVersion = "1.0";

        public static void Save() { Persistency.Save(Instance, Filename, FileVersion); }
        public static void Load() { Persistency.Load(Instance, Filename, FileVersion, false); }

        [InitializeOnLoadMethod]
        private static void LoadOnEditorStartup() { Load(); }

        #endregion
    }
}