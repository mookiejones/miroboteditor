using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
namespace DMC_Robot_Editor.Globals.Logging
{
    using Implementation;
    using Exceptionhandling;
    public class MessageLogger : IMessageLogger
    {
        private const int DefaultEventId = 1;
        private const int DefaultPriority = -1;
        private const string DefaultTitle = "";
        private static readonly object sync = new object();
        private static LogWriter writer;
        public string Category
        {
            get;
            private set;
        }
        public string MessageResource
        {
            get;
            private set;
        }
        internal static LogWriter Writer
        {
            get
            {
                if (MessageLogger.writer == null)
                {
                    object obj;
                    Monitor.Enter(obj = MessageLogger.sync);
                    try
                    {
                        if (MessageLogger.writer == null)
                        {
                            TraceSource traceSource = new TraceSource("UserMessage", SourceLevels.All);
                            TraceSource traceSource2 = new TraceSource("Errors", SourceLevels.Error);
                            MessageLogger.writer = new LogWriter(new TraceSource[]
							{
								traceSource2,
								traceSource
							}, traceSource, traceSource2, "UserMessage", false, false);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return MessageLogger.writer;
            }
        }
        public MessageLogger(string category, string messageResource)
        {
            Category = category;
            MessageResource = messageResource;
        }
        public static void Write(string category, LocalizableException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            object[] array = new object[0];
            if (exception.Error.MessageArguments != null && exception.Error.MessageArguments.Count > 0)
            {
                array = new object[exception.Error.MessageArguments.Count];
                exception.Error.MessageArguments.ToArray().CopyTo(array, 0);
            }
            Dictionary<string, object> properties = new Dictionary<string, object>
			{
				
				{
					"MessageCategory",
					category
				},
				
				{
					"MessageKey",
					exception.Error.MessageKey
				},
				
				{
					"MessageResource",
					exception.Error.MessageResource
				},
				
				{
					"MessageArgs",
					array
				}
			};
            string str = string.IsNullOrEmpty(exception.Error.MessageResource) ? exception.Error.MessageKey : (exception.Error.MessageKey + "@" + exception.Error.MessageResource);
            MessageLogger.Write("Message: " + str, -1, 1, MessageLogger.GetSeverity(exception.SeverityLevelOfException), "", properties);
        }
        public static void Write(string category, TraceEventType severity, string messageKey, string messageResource, params object[] args)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>
			{
				
				{
					"MessageCategory",
					category
				},
				
				{
					"MessageKey",
					messageKey
				},
				
				{
					"MessageResource",
					messageResource
				},
				
				{
					"MessageArgs",
					args
				}
			};
            string str = string.IsNullOrEmpty(messageResource) ? messageKey : (messageKey + "@" + messageResource);
            MessageLogger.Write("Message: " + str, -1, 1, severity, "", properties);
        }
        public static void WriteError(string category, string messageKey, string messageResource, params object[] args)
        {
            MessageLogger.Write(category, TraceEventType.Error, messageKey, messageResource, args);
        }
        public static void WriteInfo(string category, string messageKey, string messageResource, params object[] args)
        {
            MessageLogger.Write(category, TraceEventType.Information, messageKey, messageResource, args);
        }
        public static void WriteWarning(string category, string messageKey, string messageResource, params object[] args)
        {
            MessageLogger.Write(category, TraceEventType.Warning, messageKey, messageResource, args);
        }
        public void Add(TraceEventType severity, string messageKey, params object[] args)
        {
            MessageLogger.Write(Category, severity, messageKey, MessageResource, args);
        }
        public void AddError(string messageKey, params object[] args)
        {
            MessageLogger.WriteError(Category, messageKey, MessageResource, args);
        }
        public void AddError(LocalizableException ex)
        {
            MessageLogger.Write(Category, ex);
        }
        public void AddInfo(string messageKey, params object[] args)
        {
            MessageLogger.WriteInfo(Category, messageKey, MessageResource, args);
        }
        public void AddWarning(string messageKey, params object[] args)
        {
            MessageLogger.WriteWarning(Category, messageKey, MessageResource, args);
        }
        private static TraceEventType GetSeverity(LocalizableException.SeverityLevel exception)
        {
            switch (exception)
            {
                case LocalizableException.SeverityLevel.Critical:
                    return TraceEventType.Critical;
                case LocalizableException.SeverityLevel.Error:
                    return TraceEventType.Error;
                case LocalizableException.SeverityLevel.Warning:
                    return TraceEventType.Warning;
                case LocalizableException.SeverityLevel.Information:
                    return TraceEventType.Information;
                default:
                    return TraceEventType.Error;
            }
        }
        private static void Write(string message, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            LogEntry log = new LogEntry
            {
                Message = message,
                Categories = new string[]
				{
					"UserMessage"
				},
                Priority = priority,
                EventId = eventId,
                Severity = severity,
                Title = title,
                ExtendedProperties = properties
            };
            MessageLogger.Writer.Write(log);
        }
    }
}
