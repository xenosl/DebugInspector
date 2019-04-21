using UnityEngine;

namespace ShuHai
{
    public sealed class ColorAnimation : CurveAnimation
    {
        public new static ColorAnimation Create() { return CreateInstance<ColorAnimation>(); }

        public AnimationCurve AlphaCurve { get { return Curve; } set { Curve = value; } }
        public AnimationCurve RedCurve;
        public AnimationCurve GreenCurve;
        public AnimationCurve BlueCurve;

        public Color Color { get { return color; } }

        private Color color;

        protected override void UpdateImpl()
        {
            base.UpdateImpl();

            color.a = Value;
            color.r = Evaluate(RedCurve, color.r);
            color.g = Evaluate(GreenCurve, color.g);
            color.b = Evaluate(BlueCurve, color.b);
        }
    }
}