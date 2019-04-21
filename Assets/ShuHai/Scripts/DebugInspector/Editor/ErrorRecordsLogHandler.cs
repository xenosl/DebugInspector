using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    using UObject = UnityEngine.Object;

    internal class ErrorRecordsLogHandler : ILogHandler
    {
        public static readonly ErrorRecordsLogHandler Default = new ErrorRecordsLogHandler();

        /// <summary>
        ///     Log handler used when non-error log coming.
        ///     <see cref="DebugEx.UnityLogHandler" /> will be used if this value is <see langword="null" />.
        /// </summary>
        public ILogHandler NonErrorLogHandler = DebugEx.UnityLogHandler;

        public void LogFormat(LogType logType, UObject context, string format, params object[] args)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Record(new LogInfo(logType, context, string.Format(format, args), new StackTrace(1, true)));
                    break;
                default:
                    if (NonErrorLogHandler == null)
                        DebugEx.UnityLogHandler.LogFormat(logType, context, format, args);
                    else
                        NonErrorLogHandler.LogFormat(logType, context, format, args);
                    break;
            }
        }

        public void LogException(Exception exception, UObject context) { Record(new LogInfo(exception, context)); }

        #region Records

        public int MaxRecords = 1000;

        public IEnumerable<LogInfo> Records { get { return records; } }

        public void ClearRecords() { records.Clear(); }

        private Queue<LogInfo> records = new Queue<LogInfo>();

        private void Record(LogInfo info)
        {
            records.Enqueue(info);

            while (records.Count > MaxRecords)
                records.Dequeue();
        }

        #endregion Records
    }
}