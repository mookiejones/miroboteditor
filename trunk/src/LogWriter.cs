using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Media;

namespace miRobotEditor
{
    /// <summary>
    /// Class for Writing To LogFile
    /// </summary>
    public sealed class LogWriter
    {
        //TODO Use this or get rid of it
        //
        /// <summary>
        /// Constructor
        /// </summary>
        [Localizable(false)]
        public LogWriter()
        {
            // Delete Current log
            File.Delete(String.Format("{0}{1}", App.StartupPath, @"\KRC Editor.log"));
            WriteLog(String.Format("{0} {1} Created", App.ProductName, App.Version));
        }
        /// <summary>
        /// Write to Log
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="ex">Exception</param>
        public static void WriteLog(string message, Exception ex)
        {
            WriteLog(message, Colors.Red);
            WriteLog(ex.Message, Colors.Red);
        }
        /// <summary>
        /// Write to Log
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="color">Color Text</param>
        [Localizable(false)]
        public static void WriteLog(string message, Color color)
        {
            var fn = string.Format("{0}{1}", App.StartupPath, @"\KRC Editor.log");
            File.AppendAllText(fn,message + "/r/n",System.Text.Encoding.Default);

        }

        /// <summary>
        /// Write To Log
        /// </summary>
        /// <param name="message">Message</param>
        public static void WriteLog(string message)
        {
            WriteLog(message, Colors.Gray);
        }



    }
}
