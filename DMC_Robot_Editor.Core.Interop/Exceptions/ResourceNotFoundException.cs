using System;
using System.Runtime.Serialization;

namespace miRobotEditor.Core.Exceptions
{
    /// <summary>
    /// Is thrown when the GlobalResource manager can't find a requested
    /// resource.
    /// </summary>
    [Serializable()]
    public class ResourceNotFoundException : CoreException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resource"></param>
        public ResourceNotFoundException(string resource)
            : base("Resource not found : " + resource)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceNotFoundException()
            : base()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
