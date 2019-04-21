using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI.Controls
{
    public enum ToggleState
    {
        Checked,
        Unchecked,
        Indeterminate
    }

    public class Toggle : ContentControl
    {
        #region State

        public ToggleState State { get { return state; } set { state = value; } }

        public bool IsChecked
        {
            get { return State == ToggleState.Checked; }
            set { State = value ? ToggleState.Checked : ToggleState.Unchecked; }
        }

        private ToggleState state;

        #endregion State

        #region Events

        protected override bool HandleMouseDown(Event evt)
        {
            IsChecked = !IsChecked;
            return true;
        }

        protected override bool HandleMouseUp(Event evt) { return true; }

        //protected override void HandleRepaint(Event evt)
        //{
        //    base.HandleRepaint(evt);
        //}

        #endregion Events

        #region Focus

        protected override FocusType focusType { get { return FocusType.Keyboard; } }

        #endregion Focus

        #region Style

        protected override GUIStyle InitializeStyle()
        {
            return new GUIStyle(EditorStyles.toggle)
            {
                name = GetType().Name,
            };
        }

        #endregion Style
    }
}