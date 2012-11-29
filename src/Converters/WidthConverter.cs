using System;
using System.Globalization;
using System.Windows.Data;
using miRobotEditor.GUI.AngleConverter;

namespace miRobotEditor.Converters
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((AngleConvertor.CartesianEnum)value)
            {
                case AngleConvertor.CartesianEnum.AbbQuaternion:
                case AngleConvertor.CartesianEnum.AxisAngle:
                    return "25*";
                default:
                    return "33*";
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}