using System;
using System.Diagnostics;

namespace ShuHai
{
    /// <summary>
    ///     A alternative class to <see cref="System.Timers.Timer" /> that executed on main thread for using in Unity.
    /// </summary>
    public class Timer
    {
        public event Action Elapsed;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value == enabled)
                    return;

                enabled = value;

                if (enabled)
                {
                    update += Update;
                    stopwatch.Start();
                }
                else
                {
                    stopwatch.Reset();
                    update -= Update;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the interval, expressed in milliseconds, at which to raise the <see cref="Elapsed" /> event.
        /// </summary>
        public double Interval
        {
            get { return interval; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Invalid value: " + value);
                interval = value;
            }
        }

        public bool AutoReset = true;

        public Timer() : this(100) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class, and sets the <see cref="Interval" /> property to the
        ///     specified number of milliseconds.
        /// </summary>
        /// <param name="interval">The time, in milliseconds, between events.</param>
        public Timer(double interval) { Interval = interval; }

        public void Start() { Enabled = true; }

        public void Stop() { Enabled = false; }

        private bool enabled;
        private double interval;
        private readonly Stopwatch stopwatch = new Stopwatch();

        private void Update()
        {
            double elapsedSeconds = stopwatch.Elapsed.TotalMilliseconds;
            if (elapsedSeconds < Interval)
                return;

            Elapsed.NPInvoke();

            if (AutoReset)
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
            else
            {
                Enabled = false;
            }
        }

#if UNITY_EDITOR
        private static event UnityEditor.EditorApplication.CallbackFunction update
        {
            add { UnityEditor.EditorApplication.update += value; }
            remove { UnityEditor.EditorApplication.update -= value; }
        }
#else
        private static event Action update
        {
            add { Updater.UpdateEvent += value; }
            remove { Updater.UpdateEvent -= value; }
        }
#endif
    }
}