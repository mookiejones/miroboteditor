using System.ComponentModel;

namespace miRobotEditor.Classes
{
    internal sealed class IndentationSettings
    {
        [Localizable(false)] public string IndentString = "\t";
        public bool LeaveEmptyLines = true;
    }
}