using System.Windows.Media.Imaging;

namespace miRobotEditor.Core
{
    public interface IMessage
    {
        BitmapImage Icon { get; }
        string Title { get; set; }
        string Description { get; set; }
    }
}