using System.Collections.Generic;

namespace miRobotEditor.Interfaces
{
    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs(IList<string> args);
    }
}