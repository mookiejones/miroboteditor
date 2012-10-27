using System;
using System.Runtime.Serialization;

namespace miRobotEditor.Core.Exceptions
{
    /// <summary>
    /// Base class for exceptions thrown by the SharpDevelop core.
    /// </summary>
    [Serializable()]
    public class CoreException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CoreException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CoreException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
