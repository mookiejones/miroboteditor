using System.Windows.Media.Imaging;

namespace miRobotEditor.ViewModel
{
    public class OutputWindowMessage:IMessage
    {
        public string Time { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public BitmapImage Icon { get; set; }
    }
}