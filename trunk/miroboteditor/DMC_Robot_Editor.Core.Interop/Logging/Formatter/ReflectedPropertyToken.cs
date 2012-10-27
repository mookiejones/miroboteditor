using System;
using System.Reflection;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{

    public class ReflectedPropertyToken : TokenFunction
    {
        private const string StartDelimiter = "{property(";
        public ReflectedPropertyToken()
            : base("{property(")
        {
        }
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            Type type = log.GetType();
            PropertyInfo property = type.GetProperty(tokenTemplate);
            if (property == null)
            {
                return string.Format(Resources.Culture, Resources.ReflectedPropertyTokenNotFound, new object[]
				{
					tokenTemplate
				});
            }
            object value = property.GetValue(log, null);
            if (value == null)
            {
                return string.Empty;
            }
            return value.ToString();
        }
    }
}
