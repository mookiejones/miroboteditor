using System;

namespace Globals
{
    public static class TraceWriter
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static void Trace(string Message)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("{0} : {1}", DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture), Message));
        }
    }

}
