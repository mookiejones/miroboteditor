using System.Windows.Media.Imaging;

namespace miRobotEditor.ViewModel
{
    public class FileModel:ViewModelBase
    {
        public BitmapImage Icon { get; set; }
        public string FileName { get; set; }
    }
}
