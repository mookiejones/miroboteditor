using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class FileModel:ViewModelBase
    {
        public BitmapImage Icon { get; set; }
        public System.IO.FileInfo File { get; set; }

    }
}
