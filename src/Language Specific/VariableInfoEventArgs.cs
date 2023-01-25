using System;

namespace miRobotEditor.Language_Specific
{
    public class VariableInfoEventArgs : EventArgs
    {
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public VariableInfoEventArgs(string name)
        {
            _name = name;
        }

    }
}

