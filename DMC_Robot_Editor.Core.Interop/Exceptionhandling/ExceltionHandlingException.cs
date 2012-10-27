using System;
using System.Runtime.Serialization;
namespace DMC_Robot_Editor.Exceptionhandling
{
    [Serializable]
    public class ExceptionHandlingException : Exception
    {
        public ExceptionHandlingException()
        {
        }
        public ExceptionHandlingException(string message)
            : base(message)
        {
        }
        public ExceptionHandlingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected ExceptionHandlingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
