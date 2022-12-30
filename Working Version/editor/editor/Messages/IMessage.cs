using System.Windows.Media.Imaging;

namespace miRobotEditor.Messages
{
    public interface IMessage
    {
        BitmapImage Icon { get; }
        string Title { get; set; }
        string Description { get; set; }

        bool ForceActivation { get; set; }
    }
}