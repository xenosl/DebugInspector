using System;
using UnityEngine;

namespace ShuHai.DebugInspector.Editor
{
    /// <summary>
    ///     Represents a value in inspector and its context.
    /// </summary>
    public interface IValueEntry
    {
        /// <summary>
        ///     Owner value of <see cref="Value" />. If this entry represents a member of an object, this is the object.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        ///     Actual value this entry owns.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        ///     Type of <see cref="Owner" />
        /// </summary>
        Type TypeOfOwner { get; }

        /// <summary>
        ///     Type of <see cref="Value" />
        /// </summary>
        Type TypeOfValue { get; }

        /// <summary>
        ///     Is <see cref="Value" /> readable.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        ///     Is <see cref="Value" /> writable.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        ///     Get value with exception and error logs caught.
        /// </summary>
        /// <param name="value">Result value.</param>
        /// <param name="handleErrorLogs">
        ///     Is error logs intercepted when getting value. If true, errors logs (those <see cref="LogType" /> is
        ///     <see cref="LogType.Error" />, <see cref="LogType.Assert" /> or <see cref="LogType.Exception" />) will not log
        ///     to editor console window, but add to <see cref="ValueAccessResult.ErrorLogs" />.
        /// </param>
        /// <param name="catchException">
        ///     Is exception been caught when getting value. If true, any exception thrown when getting value will be caught
        ///     by this method and put it to <see cref="ValueAccessResult.Exception" />.
        /// </param>
        /// <returns> A structure contains error logs and exception those caught during getting value. </returns>
        ValueAccessResult GetValue(out object value, bool handleErrorLogs, bool catchException);

        /// <summary>
        ///     Set value with exception and error logs caught.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="handleErrorLogs">
        ///     Is error logs intercepted when getting value. If true, errors logs (those <see cref="LogType" /> is
        ///     <see cref="LogType.Error" />, <see cref="LogType.Assert" /> or <see cref="LogType.Exception" />) will not log
        ///     to editor console window, but add to <see cref="ValueAccessResult.ErrorLogs" />.
        /// </param>
        /// <param name="catchException">
        ///     Is exception been caught when getting value. If true, any exception thrown when getting value will be caught
        ///     by this method and put it to <see cref="ValueAccessResult.Exception" />.
        /// </param>
        /// <returns> A structure contains error logs and exception those caught during setting value. </returns>
        ValueAccessResult SetValue(object value, bool handleErrorLogs, bool catchException);
    }

    /// <summary>
    ///     Represents the strongly typed <see cref="IValueEntry" />.
    /// </summary>
    public interface IValueEntry<T> : IValueEntry
    {
        new T Value { get; set; }

        ValueAccessResult GetValue(out T value, bool handleErrorLogs, bool catchException);
        ValueAccessResult SetValue(T value, bool handleErrorLogs, bool catchException);
    }

    /// <summary>
    ///     Represents the strongly typed <see cref="IValueEntry" />.
    /// </summary>
    public interface IValueEntry<TOwner, TValue> : IValueEntry<TValue>
    {
        new TOwner Owner { get; set; }
    }
}