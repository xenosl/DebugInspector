using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI.Controls
{
    public class Label : Text
    {
        #region Constructors

        public Label() : this(typeof(Label).Name) { }

        public Label(string text) : base(text) { selectable = false; }

        #endregion Constructors

        #region Options

        /// <summary>
        ///     Determines whether the text is editable. The value is always <see langword="false" /> for <see cref="Label" />,
        ///     and setting the value takes no effect.
        /// </summary>
        public override bool TextEditable { get { return false; } set { } }

        /// <summary>
        ///     Determines wheter the text is selectable. The value is always <see langword="false" /> for <see cref="Label" />,
        ///     and setting the value takes no effect.
        /// </summary>
        public override bool TextSelectable { get { return false; } set { } }

        #endregion Options

        #region Events

        protected override bool HandleMouseDown(Event evt) { return false; }
        protected override bool HandleMouseUp(Event evt) { return false; }

        protected override bool HandleKeyDown(Event evt) { return false; }

        #endregion Events

        #region Style

        protected override GUIStyle InitializeStyle()
        {
            return new GUIStyle(EditorStyles.label)
            {
                name = GetType().Name,
                alignment = TextAnchor.MiddleLeft
            };
        }

        #endregion Style
    }
}