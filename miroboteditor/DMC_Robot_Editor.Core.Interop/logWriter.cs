using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;

namespace miRobotEditor
{
    public sealed class logWriter
    {
        public logWriter()
        {
            // Delete Current log
            File.Delete(String.Format("{0}{1}", Application.StartupPath, @"\KRC Editor.log"));
            WriteLog(String.Format("{0} {1} Created", Application.ProductName, Application.ProductVersion));
        }

        public static void WriteLog(string message, Exception ex)
        {
            WriteLog(message, Colors.Red);
            WriteLog(ex.Message, Colors.Red);
        }
        public static void WriteLog(string message, Color color)
        {
            using (FileStream stream = new FileStream(string.Format("{0}{1}", Application.StartupPath, @"\KRC Editor.log"), FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.Default))
                {
                    writer.WriteLine(String.Format("{0}:={1}", DateTime.Now.ToString(), message));

                }
            }
        }

        public static void WriteLog(string message)
        {
            WriteLog(message, Colors.Gray);
        }

    

    }
}