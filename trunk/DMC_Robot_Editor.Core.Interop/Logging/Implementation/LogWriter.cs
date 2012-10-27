using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
namespace DMC_Robot_Editor.Globals.Logging.Implementation
{
    using Formatter;
    public class LogWriter
    {
        private const int defaultTimeout = 2500;
        private const int readerLockAcquireTimeout = 2500;
        public const int LogWriterFailureEventID = 6352;
        private readonly ReaderWriterLock structureHolderLock = new ReaderWriterLock();

        private readonly TraceSource _notProcessedTraceSource;
        private readonly TraceSource _errorsTraceSource;
        private readonly string _defaultCategory;
        private readonly bool _logWarningsWhenNoCategoriesMatch;       
        internal IDictionary<string, TraceSource> TraceSources { get; private set; }

        internal LogWriter()
            : this(new Collection<TraceSource>(), new TraceSource("General", SourceLevels.All), new TraceSource("Errors", SourceLevels.Error), "General", false, false)
        {
        }
        public LogWriter(IDictionary<string, TraceSource> traceSources, TraceSource errorsTraceSource, string defaultCategory)
            : this(traceSources, null, errorsTraceSource, defaultCategory, false, false)
        {
        }
        public LogWriter(IDictionary<string, TraceSource> traceSources, TraceSource notProcessedTraceSource, TraceSource errorsTraceSource, string defaultCategory, bool tracingEnabled, bool logWarningsWhenNoCategoriesMatch)
        {
            if (traceSources == null)
            {
                throw new ArgumentNullException("traceSources");
            }
            if (errorsTraceSource == null)
            {
                throw new ArgumentNullException("errorsTraceSource");
            }
            TraceSources = traceSources;
            _notProcessedTraceSource = notProcessedTraceSource;
            _errorsTraceSource = errorsTraceSource;
            _defaultCategory = defaultCategory;
            IsTracingEnabled = tracingEnabled;
            _logWarningsWhenNoCategoriesMatch = logWarningsWhenNoCategoriesMatch;
        }
        public LogWriter(ICollection<TraceSource> traceSources, TraceSource errorsTraceSource, string defaultCategory)
            : this(CreateTraceSourcesDictionary(traceSources), errorsTraceSource, defaultCategory)
        {
        }
        public LogWriter(ICollection<TraceSource> traceSources, TraceSource notProcessedTraceSource, TraceSource errorsTraceSource, string defaultCategory, bool tracingEnabled, bool logWarningsWhenNoCategoriesMatch)
            : this(CreateTraceSourcesDictionary(traceSources), notProcessedTraceSource, errorsTraceSource, defaultCategory, tracingEnabled, logWarningsWhenNoCategoriesMatch)
        {
        }
        public void Write(LogEntry log)
        {
            structureHolderLock.AcquireReaderLock(2500);
            try
            {
                if (log.Categories.Count == 0)
                {
                    log.Categories = new List<string>(1) {_defaultCategory};
                }
                ProcessLog(log);
            }
            catch (Exception exception)
            {
                ReportUnknownException(exception, log);
            }
            finally
            {
                structureHolderLock.ReleaseReaderLock();
            }
        }
        private void ProcessLog(LogEntry log)
        {
            IEnumerable<TraceSource> matchingTraceSources = GetMatchingTraceSources(log);
            foreach (TraceSource current in matchingTraceSources)
            {
                try
                {
                    current.TraceData(log.Severity, log.EventId, log);
                }
                catch (Exception exception)
                {
                    ReportExceptionDuringTracing(exception, log, current);
                }
            }
        }
        internal IEnumerable<TraceSource> GetMatchingTraceSources(LogEntry logEntry)
        {
            structureHolderLock.AcquireReaderLock(2500);
            IEnumerable<TraceSource> result;
            try
            {
                result = DoGetMatchingTraceSources(logEntry);
            }
            finally
            {
                structureHolderLock.ReleaseReaderLock();
            }
            return result;
        }
        private IEnumerable<TraceSource> DoGetMatchingTraceSources(LogEntry logEntry)
        {
            var list = new List<TraceSource>(logEntry.Categories.Count);
            var list2 = new List<string>();
            foreach (string current in logEntry.Categories)
            {
                TraceSource traceSource;
                TraceSources.TryGetValue(current, out traceSource);
                if (traceSource != null)
                {
                    list.Add(traceSource);
                }
                else
                {
                    list2.Add(current);
                }
            }
            if (list2.Count > 0)
            {
                if (IsValidTraceSource(_notProcessedTraceSource))
                {
                    list.Add(_notProcessedTraceSource);
                }
                else
                {
                    if (_logWarningsWhenNoCategoriesMatch)
                    {
                        ReportMissingCategories(list2, logEntry);
                    }
                }
            }
            return list;
        }
        private static bool IsValidTraceSource(TraceSource traceSource)
        {
            return traceSource != null && traceSource.Listeners.Count > 0;
        }
        private void ReportExceptionDuringTracing(Exception exception, LogEntry log, TraceSource traceSource)
        {
            try
            {
                var additionalInfo = new NameValueCollection
				{

					{
						ExceptionFormatter.Header,
						string.Format(Resources.Culture, Resources.TraceSourceFailed, new object[]
						{
							traceSource.Name
						})
					},

					{
						Resources.TraceSourceFailed2,
						string.Format(Resources.Culture, Resources.TraceSourceFailed3, new object[]
						{
							log
						})
					}
				};
                var exceptionFormatter = new ExceptionFormatter(additionalInfo);
                var logEntry = new LogEntry
                {
                    Severity = TraceEventType.Error,
                    Message = exceptionFormatter.GetMessage(exception),
                    EventId = 6352
                };
                _errorsTraceSource.TraceData(logEntry.Severity, logEntry.EventId, logEntry);
            }
            catch (Exception exception2)
            {
                IEventLogEntryFormatter eventLogEntryFormatter = new EventLogEntryFormatter();
                string entryText = eventLogEntryFormatter.GetEntryText(Resources.FailureWhileTracing, exception2, new string[0]);
                EventLog.WriteEntry(AppDomain.CurrentDomain.FriendlyName, entryText, EventLogEntryType.Error);
            }
        }
        private void ReportUnknownException(Exception exception, LogEntry log)
        {
            try
            {
                var additionalInfo = new NameValueCollection
				{

					{ 
						ExceptionFormatter.Header,
						Resources.ProcessMessageFailed
					},

					{
						Resources.ProcessMessageFailed2,
						string.Format(Resources.Culture, Resources.ProcessMessageFailed3, new object[]
						{
							log
						})
					}
				};
                var exceptionFormatter = new ExceptionFormatter(additionalInfo);
                var logEntry = new LogEntry
                {
                    Severity = TraceEventType.Error,
                    Message = exceptionFormatter.GetMessage(exception),
                    EventId = 6352
                };
                _errorsTraceSource.TraceData(logEntry.Severity, logEntry.EventId, logEntry);
            }
            catch (Exception exception2)
            {
                IEventLogEntryFormatter eventLogEntryFormatter = new EventLogEntryFormatter();
                string entryText = eventLogEntryFormatter.GetEntryText(Resources.UnknownFailure, exception2, new string[0]);
                EventLog.WriteEntry(AppDomain.CurrentDomain.FriendlyName, entryText, EventLogEntryType.Error);
            }
        }
        private void ReportMissingCategories(ICollection<string> missingCategories, LogEntry logEntry)
        {
            try
            {
                var logEntry2 = new LogEntry
                {
                    Severity = TraceEventType.Error,
                    Message = string.Format(Resources.Culture, Resources.MissingCategories, new object[]
					{
						TextFormatter.FormatCategoriesCollection(missingCategories),
						logEntry
					}),
                    EventId = 6352
                };
                _errorsTraceSource.TraceData(logEntry2.Severity, logEntry2.EventId, logEntry2);
            }
            catch (Exception exception)
            {
                IEventLogEntryFormatter eventLogEntryFormatter = new EventLogEntryFormatter();
                string entryText = eventLogEntryFormatter.GetEntryText(Resources.FailureWhileReportingMissingCategories, exception, new string[0]);
                EventLog.WriteEntry(AppDomain.CurrentDomain.FriendlyName, entryText, EventLogEntryType.Error);
            }
        }
        private static IDictionary<string, TraceSource> CreateTraceSourcesDictionary(ICollection<TraceSource> traceSources)
        {
            IDictionary<string, TraceSource> dictionary = new Dictionary<string, TraceSource>(traceSources.Count);
            foreach (TraceSource current in traceSources)
            {
                dictionary.Add(current.Name, current);
            }
            return dictionary;
        }

        public bool IsTracingEnabled { get; private set; }

    }
}
