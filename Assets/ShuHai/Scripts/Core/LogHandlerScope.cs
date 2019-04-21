using System;
using UnityEngine;

namespace ShuHai
{
    public struct LogHandlerScope : IDisposable
    {
        public readonly ILogHandler ScopedHandler;
        public readonly ILogHandler ReservedHandler;

        public LogHandlerScope(ILogHandler scopedHandler)
        {
            ScopedHandler = scopedHandler;

            ReservedHandler = DebugEx.UnityLogHandler;
            DebugEx.UnityLogHandler = scopedHandler;

            initialized = true;
        }

        public void Dispose()
        {
            ValueScopeUtil.Deinitialize(ref initialized);

            DebugEx.UnityLogHandler = ReservedHandler;
        }

        private bool initialized;
    }
}