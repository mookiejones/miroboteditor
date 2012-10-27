using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
namespace DMC_Robot_Editor.Globals.Exceptionhandling
{
    using Logging;
    using Logging.Implementation;
    public class ExceptionLogger
    {
        private readonly string _defaultTitle;
        private readonly int _eventId;
        private readonly Type _formatterType;
        private readonly string _logCategory;
        private readonly LogWriter _logWriter;
        private readonly int _minimumPriority;
        private readonly TraceEventType _severity;
        public ExceptionLogger(string logCategory, int eventId, TraceEventType severity, string title, int priority, Type formatterType, LogWriter writer)
        {
            _logCategory = logCategory;
            _eventId = eventId;
            _severity = severity;
            _defaultTitle = title;
            _minimumPriority = priority;
            _formatterType = formatterType;
            _logWriter = writer;
        }
        public Exception HandleException(Exception exception)
        {
            WriteToLog(CreateMessage(exception), exception.Data);
            return exception;
        }
        protected virtual void WriteToLog(string logMessage, IDictionary exceptionData)
        {
            var logEntry = new LogEntry(logMessage, _logCategory, _minimumPriority, _eventId, _severity, _defaultTitle, null);
            foreach (DictionaryEntry dictionaryEntry in exceptionData)
            {
                if (dictionaryEntry.Key is string)
                {
                    logEntry.ExtendedProperties.Add(dictionaryEntry.Key as string, dictionaryEntry.Value);
                }
            }
            _logWriter.Write(logEntry);
        }
        protected virtual StringWriter CreateStringWriter()
        {
            return new StringWriter(CultureInfo.InvariantCulture);
        }
        protected virtual ExceptionFormatter CreateFormatter(StringWriter writer, Exception exception)
        {
            ConstructorInfo formatterConstructor = GetFormatterConstructor();
            return (ExceptionFormatter)formatterConstructor.Invoke(new object[]
			{
				writer,
				exception
			});
        }
        private ConstructorInfo GetFormatterConstructor()
        {
            var types = new Type[]
			{
				typeof(TextWriter),
				typeof(Exception)
			};
            ConstructorInfo constructor = _formatterType.GetConstructor(types);
            if (constructor == null)
            {
                throw new ExceptionHandlingException(string.Format(Properties.Culture, Properties.MissingConstructor, new object[]
				{
					_formatterType.AssemblyQualifiedName
				}));
            }
            return constructor;
        }
        private string CreateMessage(Exception exception)
        {
            StringWriter stringWriter = null;
            StringBuilder stringBuilder;
            try
            {
                stringWriter = CreateStringWriter();
                ExceptionFormatter exceptionFormatter = CreateFormatter(stringWriter, exception);
                exceptionFormatter.Format();
                stringBuilder = stringWriter.GetStringBuilder();
            }
            finally
            {
                if (stringWriter != null)
                {
                    stringWriter.Close();
                }
            }
            return stringBuilder.ToString();
        }
    }
}
