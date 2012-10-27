using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
namespace DMC_Robot_Editor.Globals.Logging
{
    using Implementation;
    public sealed class DebugLogger
    {
        private const string DefaultErrorCategory = "Errors";
        private const int DefaultEventId = 1;
        private const int DefaultPriority = -1;
        private const string DefaultTitle = "";
        private static readonly object sync = new object();
        private readonly TraceEventType _defaultSeverity;
        private readonly string _defaultTitle;
        private readonly int _eventId;
        private readonly string _logCategory;
        private readonly int _minimumPriority;
        private static volatile LogWriter _writer;
        public static IList<SourceSwitch> SourceSwitches
        {
            get
            {
                var list = new List<SourceSwitch>(_writer.TraceSources.Count);
                foreach (TraceSource current in _writer.TraceSources.Values)
                {
                    SourceSwitch @switch = current.Switch;
                    if (@switch != null)
                    {
                        list.Add(@switch);
                    }
                }
                return list.AsReadOnly();
            }
        }
        internal static LogWriter Writer
        {
            get
            {
                if (_writer == null)
                {
                    object obj;
                    Monitor.Enter(obj = sync);
                    try
                    {
                        if (_writer == null)
                        {
                            var traceSource = new TraceSource("General", SourceLevels.All);
                            var traceSource2 = new TraceSource("Errors", SourceLevels.Error);
                            _writer = new LogWriter(new TraceSource[]
							{
								traceSource,
								traceSource2
							}, traceSource, traceSource2, "General", false, false);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return _writer;
            }
        }
        public DebugLogger(string category, int eventId, TraceEventType severity, string title, int priority)
        {
            _logCategory = category;
            _eventId = eventId;
            _defaultSeverity = severity;
            _defaultTitle = title;
            _minimumPriority = priority;
            DebugLogger.RegisterCategory(_logCategory);
        }
        public static void RegisterCategory(string category)
        {
            if (!Writer.TraceSources.ContainsKey(category))
            {
                Writer.TraceSources.Add(category, new TraceSource(category));
            }
        }
        public static void RegisterCategory(TraceSource categorySource)
        {
            if (!Writer.TraceSources.ContainsKey(categorySource.Name))
            {
                Writer.TraceSources.Add(categorySource.Name, categorySource);
            }
        }
        public static void Write(string category, string message, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            Write(message, new string[]
			{
				category
			}, -1, eventId, severity, title, properties);
        }
        public static void Write(object message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            var log = new LogEntry
            {
                Message = message.ToString(),
                Categories = categories,
                Priority = priority,
                EventId = eventId,
                Severity = severity,
                Title = title,
                ExtendedProperties = properties
            };
            Write(log);
        }
        public static void Write(LogEntry log)
        {
            Writer.Write(log);
        }
        public static void WriteError(string message, int eventId)
        {
            Write("Errors", message, eventId, TraceEventType.Error, "", null);
        }
        public static void WriteError(string category, string message, int eventId)
        {
            Write(category, message, eventId, TraceEventType.Error, "", null);
        }
        public static void WriteError(Exception ex)
        {
            WriteError(ex, "");
        }
        public static void WriteError(string category, Exception ex)
        {
            WriteError(category, ex, "");
        }
        public static void WriteError(Exception ex, string titel)
        {
            var exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.GetMessage(ex);
            Write("Errors", message, 1, TraceEventType.Error, titel, null);
        }
        public static void WriteError(string category, Exception ex, string titel)
        {
            var exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.GetMessage(ex);
            Write(category, message, 1, TraceEventType.Error, titel, null);
        }
        public static void WriteInfo(string category, string message, int eventId)
        {
            Write(category, message, eventId, TraceEventType.Information, "", null);
        }
        public static void WriteWarning(string category, string message, int eventId)
        {
            Write(category, message, eventId, TraceEventType.Warning, "", null);
        }
        public void Write(string message)
        {
            Write(message, new string[] { _logCategory }, _minimumPriority, _eventId, _defaultSeverity, _defaultTitle, null);
        }
        public void WriteError(string message)
        {
            DebugLogger.Write(message, new string[]{_logCategory}, _minimumPriority, _eventId, TraceEventType.Error, _defaultTitle, null);
        }
        public void WriteException(Exception ex)
        {
            var exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.GetMessage(ex);
            WriteError(message);
        }
        public void WriteInfo(string message)
        {
            Write(message, new string[] { _logCategory }, _minimumPriority, _eventId, TraceEventType.Information, _defaultTitle, null);
        }
        public void WriteWarning(string message)
        {
            Write(message, new string[] { _logCategory }, _minimumPriority, _eventId, TraceEventType.Warning, _defaultTitle, null);
        }
    }
}
