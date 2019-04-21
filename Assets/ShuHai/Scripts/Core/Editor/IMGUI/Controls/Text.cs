using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI.Controls
{
    public class Text : ContentControl
    {
        #region Constructors

        public Text() : this(typeof(Text).Name) { }

        public Text(string text) { content.text = text; }

        #endregion Constructors

        #region Options

        /// <summary>
        ///     Determines whether the text is editable.
        /// </summary>
        public virtual bool TextEditable { get { return textEditable; } set { textEditable = value; } }

        /// <summary>
        ///     Determines wheter the text is selectable.
        /// </summary>
        public virtual bool TextSelectable { get { return textSelectable; } set { textSelectable = value; } }

        public TextAnchor Alignment { get { return style.alignment; } set { style.alignment = value; } }

        public TextClipping Clipping { get { return style.clipping; } set { style.clipping = value; } }

        public bool WordWrap { get { return style.wordWrap; } set { style.wordWrap = value; } }

        public bool RichText { get { return style.richText; } set { style.richText = value; } }

        private bool textEditable = true;
        private bool textSelectable = true;

        #endregion Options

        public int CursorIndex { get { return cursorIndex; } set { cursorIndex = value.Clamp(0, TextLength); } }

        private int cursorIndex;

        #region Events

        protected override bool HandleMouseDown(Event evt)
        {
            CursorIndex = style.GetCursorStringIndex(ActualRect, content, evt.mousePosition);
            return true;
        }

        protected override bool HandleMouseUp(Event evt) { return true; }

        protected override bool HandleKeyDown(Event evt)
        {
            if (!HasKeyboardFocus)
                return false;

            return true;
        }

        protected override void HandleRepaint(Event evt)
        {
            base.HandleRepaint(evt);

            if (TextEditable)
                style.DrawCursor(ActualRect, content, ID, CursorIndex);
        }

        #endregion Events

        #region Focus

        protected override FocusType focusType { get { return TextEditable ? FocusType.Keyboard : FocusType.Passive; } }

        #endregion Focus

        #region Style

        protected override GUIStyle InitializeStyle()
        {
            return new GUIStyle(EditorStyles.textField)
            {
                name = GetType().Name,
            };
        }

        #endregion Style
    }
}