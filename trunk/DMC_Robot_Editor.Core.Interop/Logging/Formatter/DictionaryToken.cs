using System.Collections.Generic;
using System.Text;
namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class DictionaryToken : TokenFunction
    {
        private const string DictionaryKeyToken = "{key}";
        private const string DictionaryValueToken = "{value}";
        public DictionaryToken()
            : base("{dictionary(")
        {
        }
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> current in log.ExtendedProperties)
            {
                StringBuilder stringBuilder2 = new StringBuilder(tokenTemplate);
                string newValue = "";
                if (current.Key != null)
                {
                    newValue = current.Key.ToString();
                }
                stringBuilder2.Replace("{key}", newValue);
                string newValue2 = "";
                if (current.Value != null)
                {
                    newValue2 = current.Value.ToString();
                }
                stringBuilder2.Replace("{value}", newValue2);
                stringBuilder.Append(stringBuilder2.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}
