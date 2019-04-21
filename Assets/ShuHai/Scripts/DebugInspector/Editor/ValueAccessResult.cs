using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Result when getting/setting value of <see cref="ValueEntry{TOwner, TValue}" />.
    /// </summary>
    public struct ValueAccessResult
    {
        public static bool IsNullOrFailed(ValueAccessResult? result) { return result == null || !result.Value.Succeed; }

        /// <summary>
        ///     <see cref="Succeed" />.
        /// </summary>
        public static implicit operator bool(ValueAccessResult result) { return result.Succeed; }

        /// <summary>
        ///     Indicates whether the access succeed.
        /// </summary>
        public bool Succeed { get { return !HasErrorLog && Exception == null; } }

        /// <summary>
        ///     Indicates whether value is got or set.
        /// </summary>
        /// <remarks>
        ///     Only exceptions prevent access methods from getting or setting value.
        ///     Error logs does not corrupt the execution sequence, so the value is accessed (that is able to get or already set)
        ///     if <see cref="Exception" /> is <see langword="null" />.
        /// </remarks>
        public bool ValueAccessed { get { return Exception == null; } }

        /// <summary>
        ///     Is there any error log exist.
        /// </summary>
        public bool HasErrorLog { get { return !CollectionUtil.IsNullOrEmpty(ErrorLogs); } }

        /// <summary>
        ///     Errors logged when accessing value.
        /// </summary>
        public List<LogInfo> ErrorLogs;

        /// <summary>
        ///     Exception thrown when accessing value.
        /// </summary>
        public Exception Exception;

        public ValueAccessResult(IEnumerable<LogInfo> errorLogs, Exception exception)
        {
            ErrorLogs = errorLogs.ToList();
            Exception = exception;
        }
    }
}