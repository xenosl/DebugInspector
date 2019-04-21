using UnityEngine;

namespace ShuHai
{
    public static class AnimationCurveUtil
    {
        /// <remarks>
        ///     The only reason this method exist is because AnimationCurve.Constant dosn't exist before Unity-2017.3.
        /// </remarks>
        public static AnimationCurve Constant(float timeStart, float timeEnd, float value)
        {
            return AnimationCurve.Linear(timeStart, value, timeEnd, value);
        }
    }
}