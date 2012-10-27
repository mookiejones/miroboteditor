using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class BinaryLogFormatter : LogFormatter
    {
        public override string Format(LogEntry log)
        {
            string result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryLogFormatter.GetFormatter().Serialize(memoryStream, log);
                result = Convert.ToBase64String(memoryStream.ToArray());
            }
            return result;
        }
        public static LogEntry Deserialize(string serializedLogEntry)
        {
            LogEntry result;
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(serializedLogEntry)))
            {
                result = (LogEntry)BinaryLogFormatter.GetFormatter().Deserialize(memoryStream);
            }
            return result;
        }
        private static BinaryFormatter GetFormatter()
        {
            return new BinaryFormatter();
        }
    }
}
