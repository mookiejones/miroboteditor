using System.Collections.Generic;

namespace MookiesLibrary
{
    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs(IEnumerable<string> args);
    }
}
