using System;
using System.Globalization;
using System.Windows.Data;

namespace miRobotEditor.Converters
{
    public class FunctionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}