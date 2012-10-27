using System;
using System.Text;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class EventLogEntryFormatter : IEventLogEntryFormatter
    {
        private readonly string _applicationName;
        public EventLogEntryFormatter()
            : this(GetApplicationName())
        {
        }
        public EventLogEntryFormatter(string applicationName)
        {
            _applicationName = applicationName;
        }
        public string GetEntryText(string message, params string[] extraInformation)
        {
            return BuildEntryText(message, null, extraInformation);
        }
        public string GetEntryText(string message, Exception exception, params string[] extraInformation)
        {
            return BuildEntryText(message, exception, extraInformation);
        }
        private string BuildEntryText(string message, Exception exception, string[] extraInformation)
        {
            var stringBuilder = new StringBuilder(string.Format(Resources.Culture, Resources.EventLogEntryHeaderTemplate, new object[] { _applicationName , "" }));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(message);
            for (var i = 0; i < extraInformation.Length; i++)
            {
                stringBuilder.AppendLine(extraInformation[i]);
            }
            if (exception != null)
            {
                stringBuilder.AppendLine(string.Format(Resources.Culture, Resources.EventLogEntryExceptionTemplate, new object[]
				{
					exception
				}));
            }
            return stringBuilder.ToString();
        }
        private static string GetApplicationName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }
    }
}
