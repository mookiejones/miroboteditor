using System;
using System.Runtime.Serialization;

namespace miRobotEditor.Core.Exceptions
{
    /// <summary>
    /// Is thrown when the ServiceManager cannot find a required service.
    /// </summary>
    [Serializable()]
    public class ServiceNotFoundException : CoreException
    {
        /// <summary>
        /// 
        /// </summary>
        public ServiceNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        public ServiceNotFoundException(Type serviceType)
            : base("Required service not found: " + serviceType.FullName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ServiceNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ServiceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ServiceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
