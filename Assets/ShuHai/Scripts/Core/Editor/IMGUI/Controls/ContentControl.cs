using UnityEngine;

namespace ShuHai.Editor.IMGUI.Controls
{
    public abstract class ContentControl : Control
    {
        #region Constructors

        protected ContentControl() { _style = new Lazy<GUIStyle>(InitializeStyle); }

        #endregion Constructors

        #region Content

        public int TextLength { get { return Text != null ? Text.Length : 0; } }
        public virtual string Text { get { return content.text; } set { content.text = value; } }

        public virtual Texture Image { get { return content.image; } set { content.image = value; } }

        public int TooltipLength { get { return Tooltip != null ? Tooltip.Length : 0; } }
        public virtual string Tooltip { get { return content.tooltip; } set { content.tooltip = value; } }

        protected readonly GUIContent content = new GUIContent();

        #endregion Content

        #region Content Rect Options

        public RectOffset Padding { get { return style.padding; } set { style.padding = value; } }
        public RectOffset Overflow { get { return style.overflow; } set { style.overflow = value; } }
        public RectOffset BackgroundBorder { get { return style.border; } set { style.border = value; } }

        #endregion Content Rect Options

        #region Style

        protected GUIStyle style { get { return _style.Value; } }

        private readonly Lazy<GUIStyle> _style;

        protected virtual GUIStyle InitializeStyle() { return new GUIStyle { name = GetType().Name }; }

        #endregion Style

        #region Rect

        protected override Rect GetRectByUnityLayout() { return GUILayoutUtility.GetRect(content, style); }

        #endregion Rect

        #region Events

        protected override void HandleRepaint(Event evt) { style.Draw(ActualRect, content, ID); }

        #endregion Events
    }
}