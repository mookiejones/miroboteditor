namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public class KeyValueToken : TokenFunction
    {
        public KeyValueToken()
            : base("{keyvalue(")
        {
        }
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            string result = "";
            object obj;
            if (log.ExtendedProperties.TryGetValue(tokenTemplate, out obj))
            {
                result = obj.ToString();
            }
            return result;
        }
    }
}
