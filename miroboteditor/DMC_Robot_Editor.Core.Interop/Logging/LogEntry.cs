using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management.Instrumentation;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
namespace DMC_Robot_Editor.Globals.Logging
{
    using Formatter;
    [InstrumentationClass(InstrumentationType.Event), ManagedName("LogEntryV20"), XmlRoot("logEntry")]
    [Serializable]
    public class LogEntry : ICloneable
    {
        private static readonly TextFormatter toStringFormatter = new TextFormatter(Resources.SingleLineTextFormat);
        private StringBuilder errorMessages;
        private IDictionary<string, object> extendedProperties;
        public string Message { get; set; }

        [IgnoreMember]
        public ICollection<string> Categories { get; set; }

        public int Priority { get; set; }

        public int EventId { get; set; }

        [IgnoreMember]
        public TraceEventType Severity { get; set; }

        public string LoggedSeverity
        {
            get { return Severity.ToString(); }
        }

        public string Title { get; set; }

        public DateTime TimeStamp { get; set; }

        public string MachineName { get; set; }

        public string AppDomainName { get; set; }

        public string ProcessId { get; set; }

        public string ProcessName { get; set; }

        public string ManagedThreadName { get; set; }

        public string Win32ThreadId { get; set; }

        [IgnoreMember]
        public IDictionary<string, object> ExtendedProperties
        {
            get { return extendedProperties ?? (extendedProperties = new Dictionary<string, object>()); }
            set { extendedProperties = value; }
        }

        public string TimeStampString
        {
            get { return TimeStamp.ToString(CultureInfo.CurrentCulture); }
        }

        public string ErrorMessages
        {
            get
            {
                if (errorMessages == null)
                {
                    return null;
                }
                return errorMessages.ToString();
            }
        }

        public string[] CategoriesStrings
        {
            get
            {
                var array = new string[Categories.Count];
                Categories.CopyTo(array, 0);
                return array;
            }
        }
        public LogEntry()
        {
            MachineName = string.Empty;
            TimeStamp = DateTime.MaxValue;
            Title = string.Empty;
            Message = string.Empty;
            Categories = new List<string>(0);
            Priority = -1;
            Severity = TraceEventType.Information;
            CollectIntrinsicProperties();
        }
        public LogEntry(object message, string category, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
            : this(message, BuildCategoriesCollection(category), priority, eventId, severity, title, properties)
        {
        }
        public LogEntry(object message, ICollection<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (categories == null)
            {
                throw new ArgumentNullException("categories");
            }
            MachineName = string.Empty;
            TimeStamp = DateTime.MaxValue;
            Title = string.Empty;
            Message = string.Empty;
            Categories = new List<string>(0);
            Priority = -1;
            Severity = TraceEventType.Information;
            Message = message.ToString();
            Priority = priority;
            Categories = categories;
            EventId = eventId;
            Severity = severity;
            Title = title;
            ExtendedProperties = properties;
            CollectIntrinsicProperties();
        }
        public object Clone()
        {
            var logEntry = new LogEntry
            {
                Message = Message,
                EventId = EventId,
                Title = Title,
                Severity = Severity,
                Priority = Priority,
                TimeStamp = TimeStamp,
                MachineName = MachineName,
                AppDomainName = AppDomainName,
                ProcessId = ProcessId,
                ProcessName = ProcessName,
                ManagedThreadName = ManagedThreadName,
                Categories = new List<string>(Categories)
            };
            if (extendedProperties != null)
            {
                logEntry.ExtendedProperties = new Dictionary<string, object>(extendedProperties);
            }
            if (errorMessages != null)
            {
                logEntry.errorMessages = new StringBuilder(errorMessages.ToString());
            }
            return logEntry;
        }
        public virtual void AddErrorMessage(string message)
        {
            if (errorMessages == null)
            {
                errorMessages = new StringBuilder();
            }
            errorMessages.Insert(0, Environment.NewLine);
            errorMessages.Insert(0, Environment.NewLine);
            errorMessages.Insert(0, message);
        }
        public override string ToString()
        {
            return toStringFormatter.Format(this);
        }
        private void CollectIntrinsicProperties()
        {
            TimeStamp = DateTime.UtcNow;
            try
            {
                MachineName = Environment.MachineName;
            }
            catch (Exception ex)
            {
                MachineName = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
				{
					ex.Message
				});
            }
            try
            {
                AppDomainName = AppDomain.CurrentDomain.FriendlyName;
            }
            catch (Exception ex2)
            {
                AppDomainName = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
				{
					ex2.Message
				});
            }
            bool flag = false;
            var securityPermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
#pragma warning disable 612,618
            if (SecurityManager.IsGranted(securityPermission))
#pragma warning restore 612,618
            {
                try
                {
                    securityPermission.Demand();
                    flag = true;
                }
                catch (SecurityException)
                {
                }
            }
            if (flag)
            {
                try
                {
                    ProcessId = GetCurrentProcessId();
                }
                catch (Exception ex3)
                {
                    ProcessId = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
					{
						ex3.Message
					});
                }
                try
                {
                    ProcessName = GetProcessName();
                }
                catch (Exception ex4)
                {
                    ProcessName = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
					{
						ex4.Message
					});
                }
                try
                {
                    Win32ThreadId = GetCurrentThreadId();
                    goto IL_1D2;
                }
                catch (Exception ex5)
                {
                    Win32ThreadId = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
					{
						ex5.Message
					});
                    goto IL_1D2;
                }
            }
            ProcessId = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
			{
				Resources.LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError
			});
            ProcessName = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
			{
				Resources.LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError
			});
            Win32ThreadId = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
			{
				Resources.LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError
			});
        IL_1D2:
           
            try
            {
                ManagedThreadName = Thread.CurrentThread.Name;
            }
            catch (Exception ex6)
            {
                ManagedThreadName = string.Format(Resources.Culture, Resources.IntrinsicPropertyError, new object[]
				{
					ex6.Message
				});
            }
        }
        public static string GetProcessName()
        {
            var stringBuilder = new StringBuilder(1024);
            NativeMethods.GetModuleFileName(NativeMethods.GetModuleHandle(null), stringBuilder, stringBuilder.Capacity);
            return stringBuilder.ToString();
        }
        private static ICollection<string> BuildCategoriesCollection(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentException("IsNullOrEmpty", "category");
            }
            return new string[]
			{
				category
			};
        }
        private static string GetCurrentProcessId()
        {
            return NativeMethods.GetCurrentProcessId().ToString(NumberFormatInfo.InvariantInfo);
        }
        private static string GetCurrentThreadId()
        {
            return NativeMethods.GetCurrentThreadId().ToString(NumberFormatInfo.InvariantInfo);
        }
    }
}
