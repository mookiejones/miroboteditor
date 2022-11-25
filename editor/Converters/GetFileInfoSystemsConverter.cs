using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    public sealed class GetFileSystemInfosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result;
            try
            {
                var directoryInfo = value as DirectoryInfo;
                if (directoryInfo != null)
                {
                    result = directoryInfo.GetFileSystemInfos();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}