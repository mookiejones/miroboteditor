using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace miRobotEditor.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? new SolidColorBrush((Color)value) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? ((SolidColorBrush)value).Color : value;
        }
    }
}