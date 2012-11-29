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

            switch ((AngleConvertorViewModel.CartesianEnum)value)
            {
                case AngleConvertorViewModel.CartesianEnum.AbbQuaternion:
                case AngleConvertorViewModel.CartesianEnum.AxisAngle:
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