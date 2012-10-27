using System;
using System.Diagnostics;
using System.Reflection;
namespace DMC_Robot_Editor.Globals.Logging
{
    public class MonitoredActivity : IDisposable
    {
        private static string category = String.Format("{0} PerformanceLog", Assembly.GetEntryAssembly().GetName().ToString());
        private static bool categoryRegistered;
        public string Text
        {
            get;
            private set;
        }
        public DateTime StartTime
        {
            get;
            private set;
        }
        public MonitoredActivity(string text)
        {
            StartTime = DateTime.Now;
            Text = text;
            if (!MonitoredActivity.categoryRegistered)
            {
                DebugLogger.RegisterCategory(category);
                MonitoredActivity.categoryRegistered = true;
            }
            DebugLogger.Write("KukaRoboter.PerformanceLog", Text, 0, TraceEventType.Start, "Start", null);
        }
        public void Dispose()
        {
            DebugLogger.Write(category, string.Format("{0}. Duration: {1}", Text, DateTime.Now - StartTime), 0, TraceEventType.Stop, "Stop", null);
        }
    }
}
