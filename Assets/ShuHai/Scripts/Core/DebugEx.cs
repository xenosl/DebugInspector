using UnityEngine;

namespace ShuHai
{
    public static class DebugEx
    {
        public static ILogger UnityLogger
        {
            get
            {
#if UNITY_2017_1_OR_NEWER
                return Debug.unityLogger;
#else
                return Debug.logger;
#endif
            }
        }

        public static ILogHandler UnityLogHandler
        {
            get { return UnityLogger.logHandler; }
            set { UnityLogger.logHandler = value; }
        }
    }
}