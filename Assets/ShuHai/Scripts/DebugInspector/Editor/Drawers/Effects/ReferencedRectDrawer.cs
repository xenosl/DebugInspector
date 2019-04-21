using System;
using System.Collections.Generic;
using ShuHai.Editor.IMGUI;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    internal class ReferencedRectDrawer
    {
        public enum VisibleState
        {
            Visible,
            Hidden,
            ControlByAnimation
        }

        public VisibleState Visibility = VisibleState.Visible;

        public Color Color
        {
            get { return color; }
            set
            {
                if (value == color)
                    return;
                color = value;
                animationDirty = true;
            }
        }

        private Color color = Colors.Yellow;

        public void GUI()
        {
            switch (Visibility)
            {
                case VisibleState.Visible:
                    GUIImpl(Color);
                    break;
                case VisibleState.Hidden:
                    // Draw Nothing.
                    break;
                case VisibleState.ControlByAnimation:
                    Animation.Update();
                    GUIImpl(Animation.Color);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GUIImpl(Color color)
        {
            if (RectsGUI(color) && BeginRect != EndRect)
                ReferencePolylineGUI(color);
        }

        #region Rects

        public Rect BeginRect { get { return beginRect; } set { ChangeRect(ref beginRect, value); } }

        public Rect EndRect { get { return endRect; } set { ChangeRect(ref endRect, value); } }

        private Rect beginRect, endRect;

        private const float MinRectArea = 0.1f;

        private void ChangeRect(ref Rect field, Rect value)
        {
            if (value == field)
                return;
            field = value;
            referencePolylineDirty = true;
        }

        private bool RectsGUI(Color color)
        {
            bool drawn = false;
            if (BeginRect.GetArea() > MinRectArea)
            {
                Graph.Rect(BeginRect, color);
                drawn = true;
            }
            if (EndRect.GetArea() > MinRectArea)
            {
                Graph.Rect(EndRect, color);
                drawn = true;
            }
            return drawn;
        }

        #endregion Rects

        #region Reference Polyline

        private List<Vector2> ReferencePolyline { get { return UpdateReferencePolyline(); } }

        private readonly List<Vector2> referencePolyline = new List<Vector2>();

        private bool referencePolylineDirty = true;

        private List<Vector2> UpdateReferencePolyline()
        {
            if (!referencePolylineDirty)
                return referencePolyline;

            referencePolyline.Clear();

            var xMin = BeginRect.xMin + 7;
            var endRectCenterY = endRect.center.y;
            referencePolyline.Add(new Vector2(xMin, BeginRect.yMax));
            referencePolyline.Add(new Vector2(xMin, endRectCenterY));

            referencePolyline.Add(new Vector2(xMin, endRectCenterY));
            referencePolyline.Add(new Vector2(EndRect.xMin, endRectCenterY));

            referencePolylineDirty = false;
            return referencePolyline;
        }

        private void ReferencePolylineGUI(Color color) { Graph.Polyline(ReferencePolyline, color); }

        #endregion Reference Polyline

        #region Animation

        public ColorAnimation Animation { get { return UpdateAnimation(); } }

        private readonly ColorAnimation animation = ColorAnimation.Create();

        private bool animationDirty = true;

        private const float AnimationDuration = 4;
        private const float AnimationDurationOnConstant = 2;

        private ColorAnimation UpdateAnimation()
        {
            if (!animationDirty)
                return animation;

            animation.Duration = AnimationDuration;
            animation.AlphaCurve = CreateEaseAnimationCurve();
            animation.RedCurve = AnimationCurveUtil.Constant(0, AnimationDuration, Color.r);
            animation.GreenCurve = AnimationCurveUtil.Constant(0, AnimationDuration, Color.g);
            animation.BlueCurve = AnimationCurveUtil.Constant(0, AnimationDuration, Color.b);

            animationDirty = false;
            return animation;
        }

        private static AnimationCurve CreateEaseAnimationCurve()
        {
            var curve = new AnimationCurve();
            curve.AddKey(0, 1);
            curve.AddKey(AnimationDurationOnConstant, 1);
            curve.AddKey(AnimationDuration, 0);
            return curve;
        }

        #endregion Animation
    }
}