using System;

namespace DMC_Robot_Editor.Globals.Logging.Formatter
{
    public interface IEventLogEntryFormatter
    {
        string GetEntryText(string message, params string[] extraInformation);
        string GetEntryText(string message, Exception exception, params string[] extraInformation);
    }
}
