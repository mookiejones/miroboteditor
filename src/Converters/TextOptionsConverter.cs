using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using miRobotEditor.GUI;

namespace miRobotEditor.Converters
{
    [Localizable(false)]
    public class TextOptionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditorOptions)
                return value as EditorOptions;


            Debug.WriteLine("TextOptions Converter Failed {0}", value);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("TextOptions Converter Failed ConvertBack {0}", value);

            return Binding.DoNothing;
        }
    }
}