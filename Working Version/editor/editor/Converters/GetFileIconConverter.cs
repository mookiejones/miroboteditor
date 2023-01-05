using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using miRobotEditor.Classes;
using miRobotEditor.Enums;
using miRobotEditor.Messages;

namespace miRobotEditor.Converters
{
    public sealed class GetFileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result;
            try
            {
                string extension = Path.GetExtension(value.ToString().ToLower());
                if (!string.IsNullOrEmpty(extension))
                {
                    if (extension == ".src")
                    {
                        System.Windows.Media.Imaging.BitmapImage bitmapImage = ImageHelper.LoadBitmap(Global.ImgSrc);
                        result = bitmapImage;
                        return result;
                    }
                    if (extension == ".dat")
                    {
                        result = ImageHelper.LoadBitmap(Global.ImgDat);
                        return result;
                    }
                    if (extension == ".sub")
                    {
                        result = ImageHelper.LoadBitmap(Global.ImgSps);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new ErrorMessage("Convert", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}