using System;

namespace RobotEditor.Classes
{
    public sealed class DocumentClosingEventArgs : EventArgs
    {
        internal DocumentClosingEventArgs(object document)
        {
            Document = document;
            Cancel = false;
        }

        private object Document { get; set; }
        private bool Cancel { get; set; }
    }
}