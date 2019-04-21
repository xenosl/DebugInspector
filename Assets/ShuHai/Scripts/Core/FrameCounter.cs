using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai
{
    public class FrameCounter
    {
        public int FrameCount { get; private set; }

        public double DeltaTime { get; private set; }

        public double FrameRate { get; private set; }

        public double MaxFrameRate
        {
            get { return maxFrameRate; }
            set { maxFrameRate = value.Clamp(MinMaxFrameRate, double.MaxValue); }
        }

        public double MinFrameInterval { get { return 1 / MaxFrameRate; } }

        public bool Update()
        {
            var currentTime = Time;
            if (currentTime < nextUpdateTime)
                return false;

            DeltaTime = currentTime - lastUpdateTime;

            SmoothDeltaTimeUpdate();

            FrameCount++;
            FrameRate = 1 / DeltaTime;

            double interval = MinFrameInterval;
            nextUpdateTime = currentTime + interval - currentTime % interval;

            lastUpdateTime = currentTime;
            return true;
        }

        private double lastUpdateTime;
        private double nextUpdateTime;

        private double maxFrameRate = DefaultMaxFrameRate;

        private const double DefaultMaxFrameRate = 60;
        private const double MinMaxFrameRate = 24;

        #region Time Source

        public Func<double> TimeSource = DefaultTimeSource;

        public double Time { get { return TimeSource != null ? TimeSource() : DefaultTimeSource(); } }

        public static double DefaultTimeSource()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.timeSinceStartup;
#else
            return UnityEngine.Time.realtimeSinceStartup;
#endif
        }

        #endregion

        #region Smooth Delta Time

        public double SmoothDeltaTime { get; private set; }

        public const int DefaultDeltaTimeSmoothFrameCount = 60;
        public const int MaxDeltaTimeSmoothFrameCount = 6000;

        public int DeltaTimeSmoothFrameCount
        {
            get { return deltaTimeSmoothFrameCount; }
            set
            {
                value = Mathf.Clamp(value, 2, MaxDeltaTimeSmoothFrameCount);
                if (value == deltaTimeSmoothFrameCount)
                    return;

                deltaTimeSmoothFrameCount = value;
                deltaTimesWithinSmoothFrame = new Queue<double>(deltaTimeSmoothFrameCount);
                deltaTimeSumWithinSmoothFrame = 0;
            }
        }

        private int deltaTimeSmoothFrameCount = DefaultDeltaTimeSmoothFrameCount;

        private Queue<double> deltaTimesWithinSmoothFrame = new Queue<double>(DefaultDeltaTimeSmoothFrameCount);
        private double deltaTimeSumWithinSmoothFrame;

        private void SmoothDeltaTimeUpdate()
        {
            if (deltaTimesWithinSmoothFrame.Count == DeltaTimeSmoothFrameCount)
                deltaTimeSumWithinSmoothFrame -= deltaTimesWithinSmoothFrame.Dequeue();
            deltaTimesWithinSmoothFrame.Enqueue(DeltaTime);

            deltaTimeSumWithinSmoothFrame += DeltaTime;
            SmoothDeltaTime = deltaTimeSumWithinSmoothFrame / deltaTimesWithinSmoothFrame.Count;
        }

        #endregion Smooth Delta Time
    }

    //internal class EditorTimeWindow : EditorWindow
    //{
    //    public const string Title = "Editor Time";

    //    [MenuItem("Debug/" + Title)]
    //    public  void Open() { GetWindow<EditorTimeWindow>(Title).Show(); }

    //    private void Update() { Repaint(); }

    //    private void OnGUI()
    //    {
    //        using (new EditorGUI.DisabledScope(true))
    //        {
    //            EditorGUILayout.IntField("FrameCount", EditorTime.FrameCount);
    //            EditorGUILayout.DoubleField("FrameRate", EditorTime.FrameRate);
    //            EditorGUILayout.DoubleField("TimeSinceStartup", EditorTime.TimeSinceStartup);

    //            EditorGUILayout.DoubleField("DeltaTime", EditorTime.DeltaTime);
    //        }

    //        using (new EditorGUI.DisabledScope(true))
    //            EditorGUILayout.DoubleField("SmoothDeltaTime", EditorTime.SmoothDeltaTime);
    //        EditorTime.DeltaTimeSmoothFrameCount = EditorGUILayout
    //            .IntField("DeltaTimeSmoothFrameCount", EditorTime.DeltaTimeSmoothFrameCount);

    //        if (GUILayout.Button("Print DeltaTime&SmoothDeltaTime"))
    //            Debug.LogFormat("{0}, {1}", EditorTime.DeltaTime, EditorTime.SmoothDeltaTime);
    //    }
    //}
}