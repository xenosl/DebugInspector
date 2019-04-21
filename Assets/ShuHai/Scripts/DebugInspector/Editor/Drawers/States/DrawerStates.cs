using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;

namespace ShuHai.DebugInspector.Editor
{
    public sealed class DrawerStates
    {
        #region Singleton

        public static readonly DrawerStates Instance = new DrawerStates();

        private DrawerStates() { }

        #endregion

        #region States

        public static void Set(string name, DrawerState state) { Instance.states[name] = state; }

        public static DrawerState Get(string name) { return Instance.states.GetValue(name); }

        //[MenuItem("Debug/Clear Drawer States")]
        public static void Clear() { Instance.states.Clear(); }

        [XmlIgnore]
        private Dictionary<string, DrawerState> states
        {
            get { return _states ?? (_states = new Dictionary<string, DrawerState>()); }
        }

        private Dictionary<string, DrawerState> _states;

        #endregion

        #region Persistence

        public const string Filename = "DrawerStates";
        public const string FileVersion = "1.0";

        public static void Save() { Persistency.AsyncSave(Instance, Filename, FileVersion); }

        [InitializeOnLoadMethod]
        public static void Load() { Persistency.Load(Instance, Filename, FileVersion, false); }

        #endregion
    }
}