using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using miRobotEditor.Core.Enums;

namespace miRobotEditor.Converters
{
    [Localizable(false)]
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridView)
            {
                var v = value as GridView;
                return v;
            }

            if (value is CartesianType)
                switch ((CartesianType)value)
            {
                case CartesianType.ABB_Quaternion:
                case CartesianType.Axis_Angle:
                    return "25*";
                default:
                    return "33*";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}