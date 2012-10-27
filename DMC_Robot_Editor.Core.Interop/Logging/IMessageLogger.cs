using System.Diagnostics;
namespace DMC_Robot_Editor.Globals.Logging
{
    using Exceptionhandling;
    public interface IMessageLogger
    {
      
        string Category
        {
            get;
        }
        string MessageResource
        {
            get;
        }
        void Add(TraceEventType severity, string messageKey, params object[] args);
        void AddError(string messageKey, params object[] args);
        void AddError(LocalizableException ex);
        void AddInfo(string messageKey, params object[] args);
        void AddWarning(string messageKey, params object[] args);
    }
}
