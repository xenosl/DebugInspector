using System;
using System.IO;

namespace ShuHai
{
    public struct StreamPositionScope : IDisposable
    {
        public readonly Stream ScopedStream;
        public readonly long ReservedPosition;

        public StreamPositionScope(Stream stream, int position)
        {
            ScopedStream = stream;

            ReservedPosition = stream.Position;
            stream.Position = position;

            initialized = true;
        }

        public void Dispose()
        {
            ValueScopeUtil.Deinitialize(ref initialized);

            ScopedStream.Position = ReservedPosition;
        }

        private bool initialized;
    }
}