using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using miRobotEditor.Classes;
using miRobotEditor.Core;

namespace miRobotEditor.Converters
{
    public class GetFileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch (Path.GetExtension(value.ToString().ToLower()))
                {
                    case ".src":
                        BitmapImage ico = Utilities.LoadBitmap(Global.ImgSrc);
                        return ico;
                    case ".dat":
                        return Utilities.LoadBitmap(Global.ImgDat);
                    case ".sub":
                        return Utilities.LoadBitmap(Global.ImgSps);
                }
            }
            catch (Exception ex)
            {
                MessageViewModel.AddError("Convert", ex);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}