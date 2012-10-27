using System;

namespace miRobotEditor.Classes
{
    public class FunctionEventArgs : EventArgs
    {
        public string Text
        {
            get; private set;
        }
        public FunctionEventArgs(string text)
        {
            Text = text;
        }
    }
}
