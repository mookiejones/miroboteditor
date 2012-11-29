using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace miRobotEditor.GUI.AngleConverter
{
    public sealed class EnumtoInt32 : IValueConverter
    {
        public object Convert(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            return (Int32)(AngleConvertor.CartesianEnum)value;           
            // Do the conversion from bool to visibility
        }

        public object ConvertBack(object value, Type targetType,  object parameter, CultureInfo culture)
        {
           return (AngleConvertor.CartesianEnum)Enum.Parse(typeof(AngleConvertor.CartesianEnum), ((Int32)value).ToString());            
        }
    }
}
