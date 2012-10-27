using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
namespace DMC_Robot_Editor.Globals.Logging
{
    using Implementation;
    public class Tracer : IDisposable
    {
        public const int priority = 5;
        public const int eventId = 1;
        public const string startTitle = "TracerEnter";
        public const string endTitle = "TracerExit";
        public const string ActivityIdPropertyKey = "TracerActivityId";
        private Stopwatch stopwatch;
        private long tracingStartTicks;
        private bool tracerDisposed;
        private bool tracingAvailable;
        private readonly LogWriter _writer;
        public Tracer(string operation)
        {
            if (CheckTracingAvailable())
            {
                if (GetActivityId().Equals(Guid.Empty))
                {
                    SetActivityId(Guid.NewGuid());
                }
                Initialize(operation);
            }
        }
        public Tracer(string operation, Guid activityId)
        {
            if (CheckTracingAvailable())
            {
                SetActivityId(activityId);
                Initialize(operation);
            }
        }
        public Tracer(string operation, LogWriter writer)
        {
            if (CheckTracingAvailable())
            {
                if (writer == null)
                {
                    throw new ArgumentNullException("writer", Resources.ExceptionWriterShouldNotBeNull);
                }
                _writer = writer;
                if (GetActivityId().Equals(Guid.Empty))
                {
                    SetActivityId(Guid.NewGuid());
                }
                Initialize(operation);
            }
        }
        public Tracer(string operation, Guid activityId, LogWriter writer)
        {
            if (CheckTracingAvailable())
            {
                if (writer == null)
                {
                    throw new ArgumentNullException("writer", Resources.ExceptionWriterShouldNotBeNull);
                }
                SetActivityId(activityId);
                _writer = writer;
                Initialize(operation);
            }
        }
        ~Tracer()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !tracerDisposed)
            {
                if (tracingAvailable)
                {
                    try
                    {
                        if (IsTracingEnabled)
                            WriteTraceEndMessage("TracerExit");
                    }
                    finally
                    {
                        try
                        {
                            StopLogicalOperation();
                        }
                        catch (SecurityException)
                        {
                        }
                    }
                }
                tracerDisposed = true;
            }
        }

        public bool IsTracingEnabled
        {
            get
            {
                var logWriter = GetWriter();
                return logWriter.IsTracingEnabled;
            }
        }
        internal static bool IsTracingAvailable()
        {
            bool result = false;
            try
            {
#pragma warning disable 612,618
                return result = SecurityManager.IsGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
#pragma warning restore 612,618
            }
            catch (SecurityException)
            {
            }
            return result;
        }
        private bool CheckTracingAvailable()
        {
            tracingAvailable = IsTracingAvailable();
            return tracingAvailable;
        }
        private void Initialize(string operation)
        {
            StartLogicalOperation(operation);
            if (IsTracingEnabled)
            {
                stopwatch = Stopwatch.StartNew();
                tracingStartTicks = Stopwatch.GetTimestamp();
                WriteTraceStartMessage("TracerEnter");
            }
        }
        private void WriteTraceStartMessage(string entryTitle)
        {
            string executingMethodName = GetExecutingMethodName();
            string message = string.Format(Resources.Culture, Resources.Tracer_StartMessageFormat, new object[]
			{
				GetActivityId(),
				executingMethodName,
				tracingStartTicks
			});
            WriteTraceMessage(message, entryTitle, TraceEventType.Start);
        }
        private void WriteTraceEndMessage(string entryTitle)
        {
            long timestamp = Stopwatch.GetTimestamp();
            decimal secondsElapsed = GetSecondsElapsed(stopwatch.ElapsedMilliseconds);
            string executingMethodName = GetExecutingMethodName();
            string message = string.Format(Resources.Culture, Resources.Tracer_EndMessageFormat, new object[]
			{
				GetActivityId(),
				executingMethodName,
				timestamp,
				secondsElapsed
			});
            WriteTraceMessage(message, entryTitle, TraceEventType.Stop);
        }
        private void WriteTraceMessage(string message, string entryTitle, TraceEventType eventType)
        {
            var properties = new Dictionary<string, object>();
            var log = new LogEntry(message, PeekLogicalOperationStack() as string, 5, 1, eventType, entryTitle, properties);
            LogWriter logWriter = GetWriter();
            logWriter.Write(log);
        }
        private string GetExecutingMethodName()
        {
            string result = "Unknown";
            var stackTrace = new StackTrace(false);
            for (var i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                if (method.DeclaringType != GetType())
                {
                    if (method.DeclaringType != null) result = method.DeclaringType.FullName + "." + method.Name;
                    break;
                }
            }
            return result;
        }
        private static decimal GetSecondsElapsed(long milliseconds)
        {
            decimal d = Convert.ToDecimal(milliseconds) / 1000m;
            return Math.Round(d, 6);
        }
        private LogWriter GetWriter()
        {
            return _writer ?? DebugLogger.Writer;
        }
        private static Guid GetActivityId()
        {
            return Trace.CorrelationManager.ActivityId;
        }
        private static Guid SetActivityId(Guid activityId)
        {
            Trace.CorrelationManager.ActivityId = activityId;
            return activityId;
        }
        private static void StartLogicalOperation(string operation)
        {
            Trace.CorrelationManager.StartLogicalOperation(operation);
        }
        private static void StopLogicalOperation()
        {
            Trace.CorrelationManager.StopLogicalOperation();
        }
        private static object PeekLogicalOperationStack()
        {
            return Trace.CorrelationManager.LogicalOperationStack.Peek();
        }
    }
}
