using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public class TabBar<TTabData>
    {
        public TabBar(IEnumerable<string> tabs, int selected = 0)
        {
            AddTabs(tabs);
            SelectedTabIndex = selected;
        }

        public TabBar(IEnumerable<Tab> tabs, int selected = 0)
        {
            AddTabs(tabs);
            SelectedTabIndex = selected;
        }

        public void GUI()
        {
            if (CollectionUtil.IsNullOrEmpty(tabs))
                return;

            SelectedTabIndex = GUILayout.Toolbar(SelectedTabIndex, guiOptions);
        }

        private GUIContent[] guiOptions
        {
            get { return _guiOptions ?? (_guiOptions = tabs.Select(t => t.Content).ToArray()); }
        }

        private GUIContent[] _guiOptions;

        #region Tabs

        #region Selected

        public event Action<int> SelectedTabChanged;

        public int SelectedTabIndex
        {
            get { return Index.Clamp(selectedTabIndex, TabCount); }
            set
            {
                value = Index.Clamp(value, TabCount);
                if (value == selectedTabIndex)
                    return;
                int oldIndex = selectedTabIndex;
                selectedTabIndex = value;
                SelectedTabChanged.NPInvoke(oldIndex);
            }
        }

        public Tab SelectedTab
        {
            get { return Index.IsValid(selectedTabIndex, TabCount) ? tabs[SelectedTabIndex] : null; }
        }

        public string SelectedTabText { get { return tabs[SelectedTabIndex].Text; } }

        private int selectedTabIndex = Index.Invalid;

        #endregion

        public int TabCount { get { return tabs.Count; } }

        public void AddTab(string tab) { AddTab(new Tab(tab)); }

        public void AddTab(Tab tab)
        {
            tabs.Add(tab);
            _guiOptions = null;
        }

        public void AddTabs(IEnumerable<string> tabs) { AddTabs(tabs.Select(t => new Tab(t))); }

        public void AddTabs(IEnumerable<Tab> tabs)
        {
            this.tabs.AddRange(tabs);
            _guiOptions = null;
        }

        public void RemoveTab(Tab tab)
        {
            if (tabs.Remove(tab))
                _guiOptions = null;
        }

        public Tab GetTab(int index) { return tabs[index]; }

        private readonly List<Tab> tabs = new List<Tab>();

        public sealed class Tab : IEquatable<Tab>
        {
            public string Text { get { return Content.text; } set { Content.text = value; } }
            public Texture Image { get { return Content.image; } set { Content.image = value; } }
            public string Tooltip { get { return Content.tooltip; } set { Content.tooltip = value; } }

            public readonly GUIContent Content;

            public TTabData Data;

            public Tab(string text, Texture2D image = null, string tooltip = null)
            {
                Content = new GUIContent(text, image, tooltip);
                Data = default(TTabData);

                hashCode = Content.GetHashCode();
            }

            #region Equality

            public bool Equals(Tab other) { return other != null && Equals(Content, other.Content); }

            public override bool Equals(object obj) { return Equals(obj as Tab); }

            public override int GetHashCode() { return hashCode; }

            private readonly int hashCode;

            #endregion
        }

        #endregion Options
    }
}