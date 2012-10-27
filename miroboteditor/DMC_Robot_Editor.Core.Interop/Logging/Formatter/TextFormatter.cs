using System;
using System.Collections.Generic;
using System.Text;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class TextFormatter : LogFormatter
    {
        private const string timeStampToken = "{timestamp}";
        private const string messageToken = "{message}";
        private const string categoryToken = "{category}";
        private const string priorityToken = "{priority}";
        private const string eventIdToken = "{eventid}";
        private const string severityToken = "{severity}";
        private const string titleToken = "{title}";
        private const string errorMessagesToken = "{errorMessages}";
        private const string machineToken = "{machine}";
        private const string appDomainNameToken = "{appDomain}";
        private const string processIdToken = "{processId}";
        private const string processNameToken = "{processName}";
        private const string threadNameToken = "{threadName}";
        private const string win32ThreadIdToken = "{win32ThreadId}";
        private const string activityidToken = "{activity}";
        private const string newLineToken = "{newline}";
        private const string tabToken = "{tab}";
        private List<TokenFunction> tokenFunctions;
        public string Template { get; set; }

        public TextFormatter(string template)
        {
            Template = !string.IsNullOrEmpty(template) ? template : Resources.DefaultTextFormat;
            RegisterTokenFunctions();
        }

        public TextFormatter()  : this(Resources.DefaultTextFormat)
        {
        }
        public override string Format(LogEntry log)
        {
            return Format(CreateTemplateBuilder(), log);
        }
        protected virtual string Format(StringBuilder templateBuilder, LogEntry log)
        {
            templateBuilder.Replace(timeStampToken, log.TimeStampString);
            templateBuilder.Replace(titleToken, log.Title);
            templateBuilder.Replace(messageToken, log.Message);
            templateBuilder.Replace(eventIdToken, log.EventId.ToString(Resources.Culture));
            templateBuilder.Replace(priorityToken, log.Priority.ToString(Resources.Culture));
            templateBuilder.Replace(severityToken, log.Severity.ToString());
            templateBuilder.Replace(errorMessagesToken, log.ErrorMessages);
            templateBuilder.Replace(machineToken, log.MachineName);
            templateBuilder.Replace(appDomainNameToken, log.AppDomainName);
            templateBuilder.Replace(processIdToken, log.ProcessId);
            templateBuilder.Replace(processNameToken, log.ProcessName);
            templateBuilder.Replace(threadNameToken, log.ManagedThreadName);
            templateBuilder.Replace(win32ThreadIdToken, log.Win32ThreadId);
            templateBuilder.Replace(categoryToken, newValue: FormatCategoriesCollection(log.Categories));
            FormatTokenFunctions(templateBuilder, log);
            templateBuilder.Replace(newLineToken, Environment.NewLine);
            templateBuilder.Replace(tabToken, "\t");
            return templateBuilder.ToString();
        }
        public static string FormatCategoriesCollection(ICollection<string> categories)
        {
            var stringBuilder = new StringBuilder();
            int num = 0;
            foreach (string current in categories)
            {
                stringBuilder.Append(current);
                if (++num < categories.Count)
                {
                    stringBuilder.Append(", ");
                }
            }
            return stringBuilder.ToString();
        }
        protected StringBuilder CreateTemplateBuilder()
        {
            return new StringBuilder((Template == null || Template.Length > 0) ? Template : Resources.DefaultTextFormat);
        }
        private void FormatTokenFunctions(StringBuilder templateBuilder, LogEntry log)
        {
            foreach (TokenFunction current in tokenFunctions)
            {
                current.Format(templateBuilder, log);
            }
        }
        private void RegisterTokenFunctions()
        {
            tokenFunctions = new List<TokenFunction>
			{
				new DictionaryToken(),
				new KeyValueToken(),
				new TimeStampToken(),
				new ReflectedPropertyToken()
			};
        }
    }
}
