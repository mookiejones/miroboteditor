using System.Windows.Media.Imaging;
using miRobotEditor.Classes;
using miRobotEditor.Enums;
using Mookie.WPF.Utilities;

namespace miRobotEditor.Messages
{
    public class MessageBase : IMessage
    {
        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public object Sender { get; protected set; }

        /// <summary>
        /// Gets or sets the message's intended target. This property can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.
        /// </summary>
        public object Target { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        public MessageBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        public MessageBase(object sender)
        {
            Sender = sender;
        }

        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        public MessageBase(object sender, object target)
            : this(sender)
        {
            Target = target;
        }

        protected MessageBase(string title, string description, MessageType icon, bool force = false)
        {
            Title = title;
            Description = description;
            Icon = GetIcon(icon);
            ForceActivation = force;
        }

        public BitmapImage Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool ForceActivation { get; set; }

        private BitmapImage GetIcon(MessageType icon)
        {
            switch (icon)
            {
                case MessageType.Error:
                    return ImageHelper.LoadBitmap(Global.ImgError);

                case MessageType.Information:
                    return ImageHelper.LoadBitmap(Global.ImgInfo);
            }
            return null;
        }
    }
}