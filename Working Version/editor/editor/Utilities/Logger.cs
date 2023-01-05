using System;
using System.Diagnostics;
using System.Windows.Media;
using miRobotEditor.Classes;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Utilities
{
    internal static class Logger
    {
        /// <summary>
        /// Log message to console and trace
        /// </summary>
        /// <param name="value"></param>
        public static void Log(string value)
        {

            Trace.WriteLine(value);
        }

        public static void WriteLog(string message)
        {
            LogWriter.WriteLog(message);
        }

        public static void ErrorHandler(string message)
        {
            ErrorHandler(message, false);
        }

        private static void ErrorHandler(string message, bool showmessage)
        {
            Console.WriteLine(message);
            TraceWriter.Trace(message);
            LogWriter.WriteLog(message, showmessage ? Colors.Red : Colors.Gray);
            if (showmessage)
            {
                MessageViewModel.ShowMessage(message);
            }
        }
    }
}
