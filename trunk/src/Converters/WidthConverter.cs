using System;
using System.Globalization;
using System.Windows.Data;
using miRobotEditor.ViewModel;
using System.Windows.Controls;
namespace miRobotEditor.Converters
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridView)
            {
                var v = value as GridView;
                Console.WriteLine("Convertered");
                return v;
            }

            if (value is CartesianEnum)
            switch ((CartesianEnum)value)
            {
                case CartesianEnum.ABB_Quaternion:
                case CartesianEnum.Axis_Angle:
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