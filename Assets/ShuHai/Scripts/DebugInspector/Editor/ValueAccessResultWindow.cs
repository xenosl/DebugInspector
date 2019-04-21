using System;
using System.Collections.Generic;
using System.Diagnostics;
using ShuHai.Editor;
using ShuHai.Editor.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    using TabBar = TabBar<ValueAccessResult>;
    using ValueAccessResultComparer = EqualityComparer<ValueAccessResult>;

    public sealed class ValueAccessResultWindow : EditorWindow
    {
        #region Window

        public const string Title = "Value Access Result";

        public static ValueAccessResultWindow Open()
        {
            var instance = Get();
            instance.Show();
            return instance;
        }

        public new static void Close() { ((EditorWindow)Get()).Close(); }

        private static ValueAccessResultWindow Get() { return GetWindow<ValueAccessResultWindow>(true, Title); }

        private void HandleEvents()
        {
            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.MouseUp:
                    Repaint();
                    break;
            }
        }

        #region Messages

        private void OnEnable() { EnableTabBar(); }

        private void OnDisable() { DisableTabBar(); }

        private void OnGUI()
        {
            //var bgRect = new Rect(Vector2.zero, maxSize);
            //GUI.DrawTexture(bgRect, EditorColoredTexture2Ds.BrightBackground, ScaleMode.StretchToFill);

            ExecuteScheduledGUIActions();
            TabsGUI();

            HandleEvents();
        }

        #endregion Messages

        #endregion Window

        #region Scheduled Actions

        private readonly ScheduledActions scheduledGUIActions = new ScheduledActions();

        private void ExecuteScheduledGUIActions()
        {
            if (Event.current.type == EventType.Layout)
                scheduledGUIActions.ExecuteScheduledActions();
        }

        #endregion Scheduled Actions

        #region Tabs

        private const int GetTabIndex = 0;
        private const string GetTabText = "get";
        private const int SetTabIndex = 1;
        private const string SetTabText = "set";

        private TabBar.Tab getTab { get { return tabBar.GetTab(GetTabIndex); } }
        private TabBar.Tab setTab { get { return tabBar.GetTab(SetTabIndex); } }

        private readonly TabBar tabBar = new TabBar(new[] { new TabBar.Tab(GetTabText), new TabBar.Tab(SetTabText) });

        private void EnableTabBar() { tabBar.SelectedTabChanged += OnSelectedTabChanged; }

        private void DisableTabBar() { tabBar.SelectedTabChanged -= OnSelectedTabChanged; }

        private void TabsGUI()
        {
            tabBar.GUI();
            LogEntriesGUI();
        }

        private void OnSelectedTabChanged(int oldIndex) { UpdateLogEntries(); }

        #region Data

        public ValueAccessResult GetResult { get { return getTab.Data; } set { ChangeTabData(GetTabIndex, value); } }
        public ValueAccessResult SetResult { get { return setTab.Data; } set { ChangeTabData(SetTabIndex, value); } }

        private void ChangeTabData(int tabIndex, ValueAccessResult value)
        {
            var tab = tabBar.GetTab(tabIndex);
            if (ValueAccessResultComparer.Default.Equals(value, tab.Data))
                return;

            tab.Data = value;

            if (tabIndex == tabBar.SelectedTabIndex)
                UpdateLogEntries(tab.Data);
        }

        #endregion Data

        #endregion Tabs

        #region Log Entries

        public int LogEntryCount { get { return logEntries.Count; } }
        public IEnumerable<LogEntry> LogEntries { get { return logEntries; } }

        public LogEntry GetLogEntry(int index) { return logEntries[index]; }

        private readonly List<LogEntry> logEntries = new List<LogEntry>();

        private void UpdateLogEntries() { UpdateLogEntries(tabBar.SelectedTab.Data); }

        private void UpdateLogEntries(ValueAccessResult valueAccessResult)
        {
            logEntries.Clear();

            var logInfos = valueAccessResult.ErrorLogs;
            if (!CollectionUtil.IsNullOrEmpty(logInfos))
            {
                foreach (var logInfo in logInfos)
                    logEntries.Add(new LogEntry(LogEntryCount, logInfo));
            }

            var exception = valueAccessResult.Exception;
            if (exception != null)
                logEntries.Add(new LogEntry(LogEntryCount, exception));
        }

        private Vector2 logEntriesScrollPosition;

        private void LogEntriesGUI()
        {
            var evt = Event.current;
            var evtType = evt.type;

            var options = GUILayoutOptions.Get(GUILayoutOptionType.MinHeight)
                .SetValue(GUILayoutOptionType.MinHeight, 40);
            using (var scroll = new EditorGUILayout.ScrollViewScope(logEntriesScrollPosition, options))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var entry in LogEntries)
                    {
                        bool selected = entry == SelectedLogEntry;
                        EditorGUILayout.LabelField(entry.Message, GetLogEntryStyle(selected));
                        var rect = GUILayoutUtility.GetLastRect();

                        int index = entry.Index;
                        if (evtType == EventType.MouseDown && rect.Contains(evt.mousePosition))
                        {
                            scheduledGUIActions.ScheduleAction(() => SelectedLogEntryIndex = index);
                            Repaint();
                        }
                    }
                }
                logEntriesScrollPosition = scroll.scrollPosition;
            }

            GUIEx.HorizontalLine(position.width);

            if (SelectedLogEntry != null)
                SelectedLogEntry.StackTraceControl.GUI();
        }

        #region Selected

        public int SelectedLogEntryIndex
        {
            get { return IsValidLogEntryIndex(selectedLogEntryIndex) ? selectedLogEntryIndex : Index.Invalid; }
            set { selectedLogEntryIndex = Index.Clamp(value, LogEntryCount); }
        }

        public LogEntry SelectedLogEntry
        {
            get
            {
                int index = SelectedLogEntryIndex;
                return IsValidLogEntryIndex(index) ? GetLogEntry(index) : null;
            }
        }

        private int selectedLogEntryIndex = Index.Invalid;

        private bool IsValidLogEntryIndex(int index) { return Index.IsValid(index, LogEntryCount); }

        #endregion Selected

        public class LogEntry
        {
            public readonly int Index;

            public readonly LogType Type;
            public readonly string Message;

            public readonly StackTraceControl StackTraceControl;

            public LogEntry(int index, LogInfo logInfo)
            {
                Index = index;
                Type = logInfo.Type;
                Message = logInfo.Message;
                StackTraceControl = new StackTraceControl(logInfo.StackTrace);
            }

            public LogEntry(int index, Exception exception)
            {
                Index = index;
                Type = LogType.Exception;
                Message = exception.Message;
                StackTraceControl = new StackTraceControl(new StackTrace(exception, true));
            }
        }

        #region Style

        private static GUIStyle logEntryStyle { get { return _logEntryStyle.Value; } }

        private static readonly Lazy<GUIStyle> _logEntryStyle = new Lazy<GUIStyle>(CreateLogEntryStyle);

        private static GUIStyle GetLogEntryStyle(bool selected)
        {
            logEntryStyle.normal.background = selected ? EditorColoredTexture2Ds.FocusedSelected : null;
            return logEntryStyle;
        }

        private static GUIStyle CreateLogEntryStyle()
        {
            return new GUIStyle(EditorStyles.label) { name = "ValueAccessResultWindow.logEntryStyle" };
        }

        #endregion Style

        #endregion Log Entries
    }
}