using System;
using System.Diagnostics;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Information of one log message.
    /// </summary>
    public struct LogInfo
    {
        /// <summary>
        ///     Type of the log.
        /// </summary>
        public LogType Type;

        /// <summary>
        ///     Object to which the log applies.
        /// </summary>
        public object Context;

        /// <summary>
        ///     Message of the log.
        /// </summary>
        public string Message;

        public StackTrace StackTrace;

        public LogInfo(LogType type, object context, string message, StackTrace stackTrace = null)
        {
            Type = type;
            Context = context;
            Message = message;
            StackTrace = stackTrace;
        }

        public LogInfo(LogType type, string message, StackTrace stackTrace = null)
            : this(type, null, message, stackTrace) { }

        public LogInfo(Exception exception, object context = null)
            : this(LogType.Exception, context, exception.Message, new StackTrace(exception, true)) { }

        public override string ToString() { return Type + ": " + Message; }
    }
}