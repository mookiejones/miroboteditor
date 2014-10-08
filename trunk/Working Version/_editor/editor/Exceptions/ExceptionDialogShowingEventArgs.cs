using System;
using System.ComponentModel;

namespace miRobotEditor.Exceptions
{
    public class ExceptionDialogShowingEventArgs : CancelEventArgs
    {
        private readonly Exception exception;

        internal ExceptionDialogShowingEventArgs(Exception exception)
        {
            this.exception = exception;
        }

        public Exception Exception
        {
            get { return exception; }
        }
    }
}