using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public interface IOutputMessages
    {
        OutputWindowMessage Add(string title, string description, BitmapImage icon);
        BitmapImage Icon { get; set; }
        string Time { get; }
        string Title { get; set; }
        string Description { get; set; }
    }
}