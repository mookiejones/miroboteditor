using System;
using System.Globalization;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class TimeStampToken : TokenFunction
    {
        private const string LocalStartDelimiter = "local";
        private const string LocalStartDelimiterWithFormat = "local:";
        public TimeStampToken()
            : base("{timestamp(")
        {
        }
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            string result;
            if (tokenTemplate.Equals("local", StringComparison.OrdinalIgnoreCase))
            {
                result = log.TimeStamp.ToLocalTime().ToString();
            }
            else
            {
                if (tokenTemplate.StartsWith("local:", StringComparison.OrdinalIgnoreCase))
                {
                    string format = tokenTemplate.Substring("local:".Length);
                    result = log.TimeStamp.ToLocalTime().ToString(format, CultureInfo.CurrentCulture);
                }
                else
                {
                    result = log.TimeStamp.ToString(tokenTemplate, CultureInfo.CurrentCulture);
                }
            }
            return result;
        }
    }
}
