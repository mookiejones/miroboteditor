
using miRobotEditor.Core.Classes.Messaging;

namespace miRobotEditor.Core
{
    public class CommandMessage:MessageBase
    {
        public string Command { get; set; }
    }
}
