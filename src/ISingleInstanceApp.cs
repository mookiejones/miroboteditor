using System.Collections.Generic;

namespace miRobotEditor
{
    public interface ISingleInstanceApp 
    { 
        bool SignalExternalCommandLineArgs(IList<string> args); 
    }
}