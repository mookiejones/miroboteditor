using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
namespace DMC_Robot_Editor.Globals.Logging
{
    using Implementation;
    using Formatter;
    public class UserLogWriter
    {
        private const int defaultTimeout = 2500;
        public const int LogWriterFailureEventID = 6352;
        private static int readerLockAcquireTimeout = 2500;
        private static int writerLockAcquireTimeout = 2500;
        private readonly ReaderWriterLock structureHolderLock = new ReaderWriterLock();
        private readonly TraceSource userMessageSource = new TraceSource("UserMessage", SourceLevels.All);
        private readonly TraceSource _errorsTraceSource;
        internal UserLogWriter()
            : this(new TraceSource("Errors", SourceLevels.Error), false)
        {
        }
        public UserLogWriter(TraceSource errorsTraceSource)
            : this(errorsTraceSource, false)
        {
        }
        public UserLogWriter(TraceSource errorsTraceSource, bool tracingEnabled)
        {
            if (errorsTraceSource == null)
            {
                throw new ArgumentNullException("errorsTraceSource");
            }
            _errorsTraceSource = errorsTraceSource;
            IsTracingEnabled = tracingEnabled;
        }
        internal static void SetLockTimeouts(int readerTimeout, int writerTimeout)
        {
            readerLockAcquireTimeout = readerTimeout;
            writerLockAcquireTimeout = writerTimeout;
        }

        public void Write(LogEntry log)
        {
            structureHolderLock.AcquireReaderLock(readerLockAcquireTimeout);
            try
            {
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

        public bool IsTracingEnabled { get; private set; }
        private void ProcessLog(LogEntry log)
        {
            try
            {
                userMessageSource.TraceData(log.Severity, log.EventId, log);
            }
            catch (Exception exception)
            {
                ReportExceptionDuringTracing(exception, log, userMessageSource);
            }
        }
        private void ReportExceptionDuringTracing(Exception exception, LogEntry log, TraceSource traceSource)
        {
            try
            {
                var exceptionFormatter = new ExceptionFormatter(new NameValueCollection
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
							log.ToString()
						})
					}
				});
                var logEntry = new LogEntry
                                   {
                                       Severity = TraceEventType.Error,
                                       Message = exceptionFormatter.GetMessage(exception),
                                       EventId = 6352
                                   };
                _errorsTraceSource.TraceData(logEntry.Severity, logEntry.EventId, logEntry);
            }
            catch
            {
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
							log.ToString()
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
            catch
            {
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
						logEntry.ToString()
					}),
                    EventId = 6352
                };
                _errorsTraceSource.TraceData(logEntry2.Severity, logEntry2.EventId, logEntry2);
            }
            catch
            {
            }
        }
    }
}
