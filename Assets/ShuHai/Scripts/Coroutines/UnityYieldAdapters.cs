using System;
using UnityEngine;

namespace ShuHai.Coroutines
{
    [TargetTypes(typeof(WaitForSeconds), typeof(WaitForSecondsRealtime))]
    internal class WaitForSecondsAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            if (yieldObject is WaitForSeconds)
            {
                var seconds = GetFloatField(typeof(WaitForSeconds), "m_Seconds", yieldObject);
                return new WaitSeconds(seconds) { TimeSource = () => Time.time };
            }

            if (yieldObject is WaitForSecondsRealtime)
            {
                var stopTime = GetFloatField(typeof(WaitForSecondsRealtime), "waitTime", yieldObject);
                return new WaitSeconds(stopTime - Time.realtimeSinceStartup)
                {
                    TimeSource = () => Time.realtimeSinceStartup
                };
            }

            throw new NotSupportedException("Yield type not supported.");
        }

        private static float GetFloatField(Type type, string name, object obj)
        {
            return (float)type.GetField(name, false).GetValue(obj);
        }
    }

    // TODO: Compatible with old unity versions (which WWW not inherited CustomYieldInstruction).
    //[TargetType(typeof(WWW))]
    //internal class WWWAdapter : IYieldAdapter
    //{
    //    public IYield ToYield(object yieldObject) { return new WaitWWW((WWW)yieldObject); }
    //}

    [TargetType(typeof(AsyncOperation))]
    internal class AsyncOperationAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject) { return new WaitAsyncOperation((AsyncOperation)yieldObject); }
    }

    [TargetType(typeof(UnityEngine.WaitWhile))]
    internal class WaitWhileAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            var field = typeof(UnityEngine.WaitWhile).GetField("m_Predicate", false);
            return new WaitWhile((Func<bool>)field.GetValue(yieldObject));
        }
    }

    [TargetType(typeof(UnityEngine.WaitUntil))]
    internal class WaitUntilAdapter : IYieldAdapter
    {
        public IYield ToYield(object yieldObject)
        {
            var field = typeof(UnityEngine.WaitUntil).GetField("m_Predicate", false);
            return new WaitUntil((Func<bool>)field.GetValue(yieldObject));
        }
    }
}