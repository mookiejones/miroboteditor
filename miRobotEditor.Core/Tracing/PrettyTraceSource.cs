using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace miRobotEditor.Core.Tracing
{
    public class PrettyTraceSource : TraceSource
    {
        private TraceSourceCounters counters = new TraceSourceCounters();
        public TraceSourceCounters Counters
        {
            get
            {
                return this.counters;
            }
        }
        internal PrettyTraceSource(string name, SourceLevels defaultLevel)
            : base(name, defaultLevel)
        {
        }
        public void WriteLine(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            base.TraceInformation(message);
            this.IncCounters(TraceEventType.Information);
        }
        public void WriteLine(string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            base.TraceInformation(format, args);
            this.IncCounters(TraceEventType.Information);
        }
        public void WriteLine(TraceEventType eventType, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            base.TraceEvent(eventType, 0, message);
            this.IncCounters(eventType);
        }
        public void WriteLine(TraceEventType eventType, string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            base.TraceEvent(eventType, 0, format, args);
            this.IncCounters(eventType);
        }
        public void WriteException(Exception exception, bool includeStackTrace)
        {
            this.WriteException(exception, includeStackTrace, string.Empty, new object[0]);
        }
        public void WriteException(Exception exception, bool includeStackTrace, string format, params object[] args)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (format != string.Empty)
            {
                stringBuilder.AppendFormat(format, args);
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine(PrettyTraceSource.GetExceptionHeadline(exception));
            stringBuilder.Append(PrettyTraceSource.GetExceptionDetails(exception, includeStackTrace));
            this.WriteLine(TraceEventType.Error, stringBuilder.ToString());
        }
        public static string GetExceptionHeadline(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(exception.Message);
            stringBuilder.AppendFormat(" ({0})", exception.GetType().Name);
            var ex = exception as FileNotFoundException;
            if (ex != null)
            {
                string text = exception.Message.ToUpperInvariant();
                string text2 = (ex.FileName != null) ? ex.FileName.Trim() : null;
                if (!string.IsNullOrEmpty(text2) && !text.Contains(text2.ToUpperInvariant()))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("FileName=\"{0}\"", text2);
                }
                string text3 = (ex.FusionLog != null) ? ex.FusionLog.Trim() : null;
                if (!string.IsNullOrEmpty(text3) && !text.Contains(text3.ToUpperInvariant()))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("FusionLog=\"{0}\"", text3);
                }
            }
            if (!string.IsNullOrEmpty(exception.Source))
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("Source=\"{0}\"", exception.Source);
            }
            return stringBuilder.ToString();
        }
        public static string GetExceptionDetails(Exception exception, bool includeStackTrace)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            StringBuilder stringBuilder = new StringBuilder();
            Exception innerException = exception.InnerException;
            if (includeStackTrace)
            {
                PrettyTraceSource.AppendStackTrace(exception, stringBuilder);
                if (innerException != null)
                {
                    stringBuilder.AppendLine();
                }
            }
            while (innerException != null)
            {
                stringBuilder.AppendLine("Caused by:");
                stringBuilder.Append(PrettyTraceSource.GetExceptionHeadline(innerException));
                if (includeStackTrace)
                {
                    stringBuilder.AppendLine();
                    PrettyTraceSource.AppendStackTrace(innerException, stringBuilder);
                }
                innerException = innerException.InnerException;
                if (innerException != null)
                {
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }
        public PerformanceLogBracket OpenPerformanceLogBracket(string action)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentNullException("action");
            }
            this.TryPerformanceLog(action, "_Start");
            this.WriteLine(TraceEventType.Start, "Starting action \"{0}\" ...", new object[]
            {
                action
            });
            return new PerformanceLogBracket(this, action);
        }
        public void PerformanceLogMilestone(string milestone)
        {
            if (string.IsNullOrEmpty(milestone))
            {
                throw new ArgumentNullException("milestone");
            }
            this.TryPerformanceLog(milestone, string.Empty);
            this.WriteLine(TraceEventType.Verbose, "Milestone \"{0}\" reached", new object[]
            {
                milestone
            });
        }
        internal void OnCloseBracket(PerformanceLogBracket bracket)
        {
            this.TryPerformanceLog(bracket.Action, "_End");
            TimeSpan timeSpan = DateTime.Now - bracket.TimestampOpened;
            this.WriteLine(TraceEventType.Stop, "Action \"{0}\" completed, took {1}.", new object[]
            {
                bracket.Action,
                timeSpan
            });
        }
        private void TryPerformanceLog(string action, string postFix)
        {
            IPerformanceLogger performanceLogger = TraceSourceFactory.PerformanceLogger;
            if (performanceLogger != null)
            {
                string text = string.Format("{0}:{1}{2}", base.Name, action, postFix);
                try
                {
                    performanceLogger.LogTimestamp(text);
                }
                catch (Exception exception)
                {
                    this.WriteException(exception, false, "Writing timestamp for key \"{0}\" by \"{1}\" failed.", new object[]
                    {
                        text,
                        performanceLogger.GetType().FullName
                    });
                }
            }
        }
        private string MakeFullPerformanceLogKey(string action)
        {
            throw new NotImplementedException();
        }
        private void IncCounters(TraceEventType eventType)
        {
            if (base.Switch.ShouldTrace(eventType))
            {
                this.counters.IncrementCounter(eventType);
                TraceSourceFactory.TotalCounters.IncrementCounter(eventType);
            }
        }
        private static void AppendStackTrace(Exception exception, StringBuilder result)
        {
            result.Append(exception.StackTrace);
        }
    }
}