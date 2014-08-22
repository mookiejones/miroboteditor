using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace miRobotEditor.GUI.Dialogs
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Icon)
            {
                BitmapDecoder bitmapDecoder;
                using (Stream stream = new MemoryStream(1000))
                {
                    (value as Icon).Save(stream);
                    bitmapDecoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad);
                }
                return bitmapDecoder.Frames[0];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}