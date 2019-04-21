using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShuHai
{
    using UObject = UnityEngine.Object;

    public class StreamLogHandler : ILogHandler, IDisposable
    {
        public static readonly StreamLogHandler Default = new StreamLogHandler();

        /// <summary>
        ///     Recreate the internal log stream.
        ///     This means that the logged strings are all cleared and new logs starts from fresh empty string.
        /// </summary>
        /// <returns>The current <see cref="StreamLogHandler" /> instance.</returns>
        public StreamLogHandler Reset()
        {
            stream = new MemoryStream();
            return this;
        }

        public void Dispose() { stream.Dispose(); }

        #region Write Log

        public bool AutoLineFeed = true;

        public void LogFormat(LogType logType, UObject context, string format, params object[] args)
        {
            WriteToStream(string.Format(format, args));
        }

        public void LogException(Exception exception, UObject context) { WriteToStream(exception.Message); }

        #endregion Write Log

        #region Read Log

        public int ReadPosition { get { return readPosition; } set { readPosition = value.Clamp(0, Length - 1); } }

        /// <summary>
        ///     Reads log string from <see cref="ReadPosition" /> to end of the log stream.
        /// </summary>
        public string ReadLog() { return ReadLog(Length - ReadPosition); }

        /// <summary>
        ///     Reads a specified maximum of characters from the log stream.
        /// </summary>
        /// <param name="length">Maximum characters to read.</param>
        public string ReadLog(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "Length less than zero.");

            if (ReadPosition >= Length || length == 0)
                return string.Empty;

            if (ReadPosition > Length - length)
                length = Length - ReadPosition;

            using (new StreamPositionScope(stream, ReadPosition))
            {
                var buffer = new byte[length];
                stream.Read(buffer, ReadPosition, length);
                return Encoding.UTF8.GetString(buffer);
            }
        }

        private int readPosition;

        #endregion Read Log

        #region Stream

        public int Length { get { return checked((int)stream.Length); } }

        private MemoryStream stream = new MemoryStream();

        private void WriteToStream(string str)
        {
            if (AutoLineFeed && stream.Position > 0)
                stream.Write(new[] { (byte)'\n' }, 0, 1);

            var bytes = Encoding.UTF8.GetBytes(str);
            stream.Write(bytes, 0, bytes.Length);
        }

        #endregion Stream
    }
}