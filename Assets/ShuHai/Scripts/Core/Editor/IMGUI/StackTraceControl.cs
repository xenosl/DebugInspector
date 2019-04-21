using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public class StackTraceControl
    {
        public StackTraceControl() { }

        public StackTraceControl(StackTrace trace) { Trace = trace; }

        public void GUI()
        {
            if (Trace == null)
                return;

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var field in FrameFields)
                        field.GUI();
                }
                scrollPosition = scroll.scrollPosition;
            }
        }

        private Vector2 scrollPosition;

        #region Trace

        public StackTrace Trace
        {
            get { return trace; }
            set
            {
                if (value == trace)
                    return;
                trace = value;
                _frameFields = null;
            }
        }

        private StackTrace trace;

        #endregion Trace

        #region Frame Fields

        public int FrameFieldCount { get { return frameFields.Length; } }

        public IEnumerable<StackFrameField> FrameFields { get { return frameFields; } }

        public StackFrameField GetFrameField(int frameIndex)
        {
            if (Trace == null)
                throw new InvalidOperationException("StackTrace not set.");
            return frameFields[frameIndex];
        }

        private StackFrameField[] frameFields
        {
            get
            {
                if (_frameFields == null)
                {
                    int count = Trace.FrameCount;
                    _frameFields = new StackFrameField[count];
                    for (int i = 0; i < count; ++i)
                        _frameFields[i] = new StackFrameField(Trace.GetFrame(i));
                }
                return _frameFields;
            }
        }

        private StackFrameField[] _frameFields;

        #endregion Frame Fields
    }
}