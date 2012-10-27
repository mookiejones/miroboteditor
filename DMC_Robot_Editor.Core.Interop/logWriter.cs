using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;

namespace miRobotEditor
{
    /// <summary>
    /// Class for Writing To LogFile
    /// </summary>
    public sealed class logWriter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        [Localizable(false)]
        public logWriter()
        {
            // Delete Current log
            File.Delete(String.Format("{0}{1}", Application.StartupPath, @"\KRC Editor.log"));
            WriteLog(String.Format("{0} {1} Created", Application.ProductName, Application.ProductVersion));
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
            using (var stream = new FileStream(string.Format("{0}{1}", Application.StartupPath, @"\KRC Editor.log"), FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine("{0}:={1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), message);

                }
            }
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