using System;

namespace miRobotEditor.Core.Services
{
    /// <summary>
    /// Logging Service Interface
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        void Debug(object message);
        /// <summary>
        /// Debug Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void DebugFormatted(string format, params object[] args);
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="message"></param>
        void Info(object message);
        /// <summary>
        /// Info Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void InfoFormatted(string format, params object[] args);
        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message"></param>
        void Warn(object message);
        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Warn(object message, Exception exception);
        /// <summary>
        /// Warn Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void WarnFormatted(string format, params object[] args);
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        void Error(object message);
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(object message, Exception exception);
        /// <summary>
        /// Error Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void ErrorFormatted(string format, params object[] args);
        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="message"></param>
        void Fatal(object message);
        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(object message, Exception exception);
        /// <summary>
        /// Fatal Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void FatalFormatted(string format, params object[] args);
        /// <summary>
        /// Is Debug Enabled
        /// </summary>
        bool IsDebugEnabled { get; }
        /// <summary>
        /// Is Info Enabled
        /// </summary>
        bool IsInfoEnabled { get; }
        /// <summary>
        /// Is Warn Enabled
        /// </summary>
        bool IsWarnEnabled { get; }
        /// <summary>
        /// Is Error Enabled
        /// </summary>
        bool IsErrorEnabled { get; }
        /// <summary>
        /// Is Fatal Enabled
        /// </summary>
        bool IsFatalEnabled { get; }
    }
}
