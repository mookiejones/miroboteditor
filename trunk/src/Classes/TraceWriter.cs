using System;
using System.ComponentModel;
using System.Globalization;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// TraceWriter
    /// </summary>
    public static class TraceWriter
    {
        /// <summary>
        /// Write Trace Message
        /// </summary>
        /// <param name="message"></param>
        [Localizable(false), System.Diagnostics.DebuggerStepThrough]
        public static void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("{0} : {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), message));
        }
    }
}