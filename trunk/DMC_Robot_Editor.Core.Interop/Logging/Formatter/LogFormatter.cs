namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public abstract class LogFormatter : ILogFormatter
    {
        public abstract string Format(LogEntry log);
    }
}
