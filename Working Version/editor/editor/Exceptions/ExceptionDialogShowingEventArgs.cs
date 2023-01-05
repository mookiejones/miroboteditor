using System;
using System.ComponentModel;

namespace miRobotEditor.Exceptions
{
    public class ExceptionDialogShowingEventArgs : CancelEventArgs
    {
        internal ExceptionDialogShowingEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}