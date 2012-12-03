using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public interface IOutputMessages
    {
        BitmapImage Icon { get; set; }
        string Time { get; }
        string Title { get; set; }
        string Description { get; set; }
    }
}