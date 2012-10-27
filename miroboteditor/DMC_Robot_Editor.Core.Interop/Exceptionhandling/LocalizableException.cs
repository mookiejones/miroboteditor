using System;
using System.Runtime.Serialization;
namespace DMC_Robot_Editor.Globals.Exceptionhandling
{
    public class LocalizableException : Exception
    {
        public enum LogLevel
        {
            None,
            Debug,
            Event
        }
        public enum SeverityLevel
        {
            Critical,
            Error,
            Warning,
            Information
        }
        public MessageInfo Error
        {
            get;
            private set;
        }
        public LocalizableException.LogLevel LogLevelOfException
        {
            get;
            set;
        }
        public LocalizableException.SeverityLevel SeverityLevelOfException
        {
            get;
            set;
        }
        public override string Message
        {
            get
            {
                if (Error == null || Error.Message == null)
                {
                    return base.Message;
                }
                return Error.Message;
            }
        }
        public LocalizableException()
        {
        }
        public LocalizableException(MessageInfo error)
            : this(error, LocalizableException.SeverityLevel.Critical, LocalizableException.LogLevel.None)
        {
        }
        public LocalizableException(MessageInfo error, LocalizableException.SeverityLevel severityLevel, LocalizableException.LogLevel logLevel)
            : base(error.Message)
        {
            Error = error;
            SeverityLevelOfException = severityLevel;
            LogLevelOfException = logLevel;
        }
        public LocalizableException(MessageInfo error, Exception inner)
            : base(error.Message, inner)
        {
            Error = error;
        }
        protected LocalizableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}