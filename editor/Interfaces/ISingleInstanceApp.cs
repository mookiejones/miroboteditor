using System.Collections.Generic;

namespace RobotEditor.Interfaces
{
    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs(IList<string> args);
    }
}