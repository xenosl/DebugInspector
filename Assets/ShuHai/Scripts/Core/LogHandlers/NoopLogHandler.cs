using System;
using UnityEngine;

namespace ShuHai
{
    using UObject = UnityEngine.Object;

    /// <summary>
    ///     Represents log handler that logs nothing.
    /// </summary>
    public sealed class NoopLogHandler : ILogHandler
    {
        #region Singleton

        public static readonly NoopLogHandler Default = new NoopLogHandler();

        private NoopLogHandler() { }

        #endregion

        public void LogException(Exception exception, UObject context) { }

        public void LogFormat(LogType logType, UObject context, string format, params object[] args) { }
    }
}