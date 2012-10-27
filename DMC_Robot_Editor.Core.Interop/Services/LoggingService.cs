using System;

namespace miRobotEditor.Core.Services
{
    /// <summary>
    /// Class for easy logging.
    /// </summary>
    public static class LoggingService
    {
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(object message)
        {
            ServiceManager.Instance.LoggingService.Debug(message);
        }
        /// <summary>
        /// DebugFormatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void DebugFormatted(string format, params object[] args)
        {
            ServiceManager.Instance.LoggingService.DebugFormatted(format, args);
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="message"></param>
        public static void Info(object message)
        {
            ServiceManager.Instance.LoggingService.Info(message);
        }
        /// <summary>
        /// InfoFormatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void InfoFormatted(string format, params object[] args)
        {
            ServiceManager.Instance.LoggingService.InfoFormatted(format, args);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(object message)
        {
            ServiceManager.Instance.LoggingService.Warn(message);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(object message, Exception exception)
        {
            ServiceManager.Instance.LoggingService.Warn(message, exception);
        }
        /// <summary>
        /// Warning Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void WarnFormatted(string format, params object[] args)
        {
            ServiceManager.Instance.LoggingService.WarnFormatted(format, args);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        public static void Error(object message)
        {
            ServiceManager.Instance.LoggingService.Error(message);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(object message, Exception exception)
        {
            ServiceManager.Instance.LoggingService.Error(message, exception);
        }

        /// <summary>
        /// Error Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void ErrorFormatted(string format, params object[] args)
        {
            ServiceManager.Instance.LoggingService.ErrorFormatted(format, args);
        }

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(object message)
        {
            ServiceManager.Instance.LoggingService.Fatal(message);
        }

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(object message, Exception exception)
        {
            ServiceManager.Instance.LoggingService.Fatal(message, exception);
        }

        /// <summary>
        /// Fatal Formatted
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void FatalFormatted(string format, params object[] args)
        {
            ServiceManager.Instance.LoggingService.FatalFormatted(format, args);
        }

        /// <summary>
        /// Is Debug Enabled
        /// </summary>
        public static bool IsDebugEnabled
        {
            get
            {
                return ServiceManager.Instance.LoggingService.IsDebugEnabled;
            }
        }

        /// <summary>
        /// Is Info Enabled
        /// </summary>
        public static bool IsInfoEnabled
        {
            get
            {
                return ServiceManager.Instance.LoggingService.IsInfoEnabled;
            }
        }

        /// <summary>
        /// Is Warnings Enabled
        /// </summary>
        public static bool IsWarnEnabled
        {
            get
            {
                return ServiceManager.Instance.LoggingService.IsWarnEnabled;
            }
        }

        /// <summary>
        /// Is Error Enabled
        /// </summary>
        public static bool IsErrorEnabled
        {
            get
            {
                return ServiceManager.Instance.LoggingService.IsErrorEnabled;
            }
        }

        /// <summary>
        /// Is Fatal Enabled
        /// </summary>
        public static bool IsFatalEnabled
        {
            get
            {
                return ServiceManager.Instance.LoggingService.IsFatalEnabled;
            }
        }
    }
}
