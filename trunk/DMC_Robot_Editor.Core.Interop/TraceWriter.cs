using System;
using System.ComponentModel;

namespace Globals
{
    /// <summary>
    /// TraceWriter
    /// </summary>
    public static class TraceWriter
    {
        /// <summary>
        /// Write Trace Message
        /// </summary>
        /// <param name="Message"></param>
        [Localizable(false),System.Diagnostics.DebuggerStepThrough]
        public static void Trace(string Message)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("{0} : {1}", DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture), Message));
        }
    }

}
