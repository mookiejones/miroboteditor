using System.Windows.Media.Imaging; 
using miRobotEditor.Interfaces;
using miRobotEditor.Messages;

namespace miRobotEditor.Classes
{
    public sealed class OutputWindowMessage : MessageBase, IMessage
    {
        public string Time { get; set; }
       
    }
}