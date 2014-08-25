using System.Windows.Media.Imaging;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public sealed class OutputWindowMessage : IMessage
    {
        public string Time { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public BitmapImage Icon { get; set; }
    }
}