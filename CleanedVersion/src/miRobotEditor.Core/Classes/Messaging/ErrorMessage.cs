using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;

namespace miRobotEditor.Core.Classes.Messaging
{
    public class ErrorMessage:MessageBase
    {
        public string Title { get; set; }
        public Exception Exception { get; set; }

        public ErrorMessage(string title, Exception exception)
        {
            Title = title;
            Exception = exception;

        }
    }
}
