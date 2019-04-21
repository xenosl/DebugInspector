using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ShuHai.Editor.IMGUI.Controls
{
    using EventHandler = Action<Control, Event>;

    /// <summary>
    ///     Base type for all GUI controls.
    /// </summary>
    public abstract class Control
    {
        public int ID
        {
            get
            {
                if (id == 0)
                    id = GUIUtility.GetControlID(RuntimeHelpers.GetHashCode(this), focusType);
                return id;
            }
        }

        private int id;

        public void GUI()
        {
            HandleRect();
            HandleEvents();

            DebugGUI();
        }

        #region Options

        /// <summary>
        ///     Indicates whether the current instance is selectable.
        ///     Only selectable <see cref="Control" /> is able to capture focus.
        /// </summary>
        public virtual bool Selectable { get { return selectable; } set { selectable = value; } }

        protected bool selectable = true;

        #endregion Options

        #region Focus

        public event Action<Control> CaptureMouseFocus;
        public event Action<Control> LoseMouseFocus;

        public event Action<Control> CaptureKeyboardFocus;
        public event Action<Control> LoseKeyboardFocus;

        //public bool IsMouseHover { get; private set; }

        public bool HasMouseFocus
        {
            get { return GUIUtility.hotControl == ID; }
            set
            {
                if (HasMouseFocus && !value)
                {
                    GUIUtility.hotControl = 0;
                    LoseMouseFocus.NPInvoke(this);
                }
                else if (!HasMouseFocus && value)
                {
                    GUIUtility.hotControl = ID;
                    CaptureMouseFocus.NPInvoke(this);
                }
            }
        }

        public bool HasKeyboardFocus
        {
            get { return GUIUtility.keyboardControl == ID; }
            set
            {
                if (HasKeyboardFocus && !value)
                {
                    GUIUtility.keyboardControl = 0;
                    LoseKeyboardFocus.NPInvoke(this);
                }
                else if (!HasKeyboardFocus && value)
                {
                    GUIUtility.keyboardControl = ID;
                    CaptureKeyboardFocus.NPInvoke(this);
                }
            }
        }

        protected abstract FocusType focusType { get; }

        #endregion Focus

        #region Rect

        public bool LayoutByUnity = true;

        /// <summary>
        ///     Position and size of current instance. Applied when <see cref="LayoutByUnity" /> is <see langword="false" />.
        /// </summary>
        public Rect ExpectedRect = new Rect(0, 0, 100, 100);

        public Rect ActualRect { get; private set; }

        protected virtual Rect GetRectByUnityLayout()
        {
            return GUILayoutUtility.GetRect(ExpectedRect.width, ExpectedRect.height);
        }

        private void HandleRect()
        {
            if (LayoutByUnity)
            {
                var evtType = Event.current.type;
                var rect = GetRectByUnityLayout();
                if (evtType != EventType.Layout && evtType != EventType.Used)
                    ActualRect = rect;
            }
            else
            {
                ActualRect = ExpectedRect;
            }
        }

        private bool MouseInActualRect(Event evt) { return ActualRect.Contains(evt.mousePosition); }

        #endregion Rect

        #region Events

        public event EventHandler MouseDown;
        public event EventHandler MouseUp;

        public event EventHandler MouseDoubleClick;
        public event EventHandler MouseTripleClick;

        //public event EventHandler MouseEnter;
        //public event EventHandler MouseLeave;
        //public event EventHandler MouseHover;

        public event EventHandler KeyDown;
        public event EventHandler KeyUp;

        protected virtual void HandleEvents()
        {
            var evt = Event.current;

            bool done = false;
            switch (evt.type)
            {
                case EventType.MouseDown:
                    var mouseInside = MouseInActualRect(evt);
                    if (Selectable)
                    {
                        HasMouseFocus = mouseInside;
                        HasKeyboardFocus = mouseInside;
                    }
                    if (mouseInside)
                    {
                        int cc = evt.clickCount;
                        done = HandleMouseDown(evt);
                        if (cc == 2)
                            HandleMouseDoubleClick(evt);
                        else if (cc == 3)
                            HandleMouseTripleClick(evt);

                        MouseDown.NPInvoke(this, evt);
                        if (cc == 2)
                            MouseDoubleClick.NPInvoke(this, evt);
                        else if (cc == 3)
                            MouseTripleClick.NPInvoke(this, evt);
                    }
                    break;

                case EventType.MouseUp:
                    if (HasMouseFocus)
                    {
                        done = HandleMouseUp(evt);
                        HasMouseFocus = false;
                        MouseUp.NPInvoke(this, evt);
                    }
                    break;

                // Only available when EditorWindow.wantsMouseMove set to true in EditorWindow.
                case EventType.MouseMove:
                    //var hoverOnLastEvent = IsMouseHover;
                    //IsMouseHover = MouseInActualRect(evt);
                    //if (!hoverOnLastEvent && IsMouseHover)
                    //{
                    //    done = HandleMouseEnter(evt);
                    //    MouseEnter.NPInvoke(this, evt);
                    //}
                    //else if (hoverOnLastEvent && !IsMouseHover)
                    //{
                    //    done = HandleMouseLeave(evt);
                    //    MouseLeave.NPInvoke(this, evt);
                    //}
                    //else if (IsMouseHover)
                    //{
                    //    done = HandleMouseHover(evt);
                    //    MouseHover.NPInvoke(this, evt);
                    //}
                    break;

                case EventType.MouseDrag:
                    done = HandleMouseDrag(evt);
                    break;
                case EventType.KeyDown:
                    if (HasKeyboardFocus)
                    {
                        done = HandleKeyDown(evt);
                        KeyDown.NPInvoke(this, evt);
                    }
                    break;
                case EventType.KeyUp:
                    if (HasKeyboardFocus)
                    {
                        done = HandleKeyUp(evt);
                        KeyUp.NPInvoke(this, evt);
                    }
                    break;
                case EventType.ScrollWheel:
                    if (MouseInActualRect(evt))
                        done = HandleScrollWheel(evt);
                    break;
                case EventType.Repaint:
                    HandleRepaint(evt);
                    break;
                case EventType.Layout:
                    HandleLayout(evt);
                    break;
                case EventType.DragUpdated:
                    done = HandleDragUpdated(evt);
                    break;
                case EventType.DragPerform:
                    done = HandleDragPerform(evt);
                    break;
                case EventType.DragExited:
                    done = HandleDragExited(evt);
                    break;
                case EventType.Ignore:
                    done = HandleIgnore(evt);
                    break;
                case EventType.Used:
                    HandleUsed(evt);
                    break;
                case EventType.ValidateCommand:
                    done = HandleValidateCommand(evt);
                    break;
                case EventType.ExecuteCommand:
                    done = HandleExecuteCommand(evt);
                    break;
                case EventType.ContextClick:
                    done = HandleContextClick(evt);
                    break;
                case EventType.MouseEnterWindow:
                    done = HandleMouseEnterWindow(evt);
                    break;
                case EventType.MouseLeaveWindow:
                    done = HandleMouseLeaveWindow(evt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (done)
                evt.Use();
        }

        protected virtual bool HandleMouseDown(Event evt) { return false; }
        protected virtual bool HandleMouseUp(Event evt) { return false; }

        protected virtual void HandleMouseDoubleClick(Event evt) { }
        protected virtual void HandleMouseTripleClick(Event evt) { }

        //protected virtual bool HandleMouseEnter(Event evt) { return false; }
        //protected virtual bool HandleMouseLeave(Event evt) { return false; }
        //protected virtual bool HandleMouseHover(Event evt) { return false; }

        protected virtual bool HandleMouseDrag(Event evt) { return false; }

        protected virtual bool HandleKeyDown(Event evt) { return false; }
        protected virtual bool HandleKeyUp(Event evt) { return false; }

        protected virtual bool HandleScrollWheel(Event evt) { return false; }

        protected abstract void HandleRepaint(Event evt);

        protected virtual void HandleLayout(Event evt) { }

        protected virtual bool HandleDragUpdated(Event evt) { return false; }
        protected virtual bool HandleDragPerform(Event evt) { return false; }
        protected virtual bool HandleDragExited(Event evt) { return false; }

        protected virtual bool HandleIgnore(Event evt) { return false; }
        protected virtual void HandleUsed(Event evt) { }

        protected virtual bool HandleValidateCommand(Event evt) { return false; }
        protected virtual bool HandleExecuteCommand(Event evt) { return false; }

        protected virtual bool HandleContextClick(Event evt) { return false; }

        protected virtual bool HandleMouseEnterWindow(Event evt) { return false; }
        protected virtual bool HandleMouseLeaveWindow(Event evt) { return false; }

        #endregion Events

        #region Debug

        public bool ShowRectFrame;

        public Color RectFrameColor = Colors.GreenYellow;

        private void DrawRectFrame()
        {
            if (!ShowRectFrame)
                return;
            if (Event.current.type != EventType.Repaint)
                return;
            Graph.Rect(ActualRect, RectFrameColor);
        }

        private void DebugGUI() { DrawRectFrame(); }

        #endregion Debug
    }
}