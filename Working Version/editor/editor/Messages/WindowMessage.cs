using miRobotEditor.Enums;

namespace miRobotEditor.Messages
{
    public sealed class WindowMessage : MessageBase, IMessage
    {
        public WindowMessage(string title, string description, MessageType icon, bool force = false)
            : base(title, description, icon, force)
        {
        }
    }
}