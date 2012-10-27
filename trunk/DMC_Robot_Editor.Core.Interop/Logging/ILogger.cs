namespace DMC_Robot_Editor.Globals.Logging
{
    public interface ILogger
    {
        void WriteError(string message);
        void WriteInfo(string message);
        void WriteWarning(string message);
    }
}
