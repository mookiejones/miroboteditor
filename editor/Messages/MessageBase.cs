using System.Windows.Media.Imaging;
using RobotEditor.Classes;
using RobotEditor.Enums;
using RobotEditor.Interfaces;

namespace RobotEditor.Messages
{
    public class MessageBase : GalaSoft.MvvmLight.Messaging.MessageBase, IMessage
    {
        protected MessageBase(string title, string description, MessageType icon, bool force = false)
        {
            Title = title;
            Description = description;
            Icon = GetIcon(icon);
            ForceActivation = force;
        }

        public BitmapImage Icon { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool ForceActivation { get; set; }

        private BitmapImage GetIcon(MessageType icon)
        {
            switch (icon)
            {
                case MessageType.Error:
                    return Utilities.LoadBitmap(Global.ImgError);
                case MessageType.Information:
                    return Utilities.LoadBitmap(Global.ImgInfo);
            }
            return null;
        }
    }
}