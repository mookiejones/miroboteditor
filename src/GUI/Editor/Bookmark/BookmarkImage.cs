using System.Drawing;
using miRobotEditor.Interfaces;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace miRobotEditor.GUI.Editor.Bookmark
{
    /// <summary>
    ///     Description of BookmarkImage.
    /// </summary>
    public class BookmarkImage : IImage
    {
        private readonly IImage _baseimage = null;

        private readonly BitmapImage _bitmap;

        public BookmarkImage(BitmapImage bitmap)
        {
            _bitmap = bitmap;
        }

        public ImageSource ImageSource => _baseimage.ImageSource;

        public BitmapImage Bitmap => _bitmap;

        public Icon Icon => _baseimage.Icon;
    }
}
