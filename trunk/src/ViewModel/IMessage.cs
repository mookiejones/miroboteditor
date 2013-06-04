using System.Windows.Media.Imaging;

namespace miRobotEditor.ViewModel
{
    public interface IMessage
    {
        BitmapImage Icon { get; set; }
        string Time { get; set; }
        string Title { get; set; }
        string Description { get; set; }
    }
}