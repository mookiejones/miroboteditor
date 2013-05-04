using System;
using System.Windows.Data;
using miRobotEditor.GUI;
namespace miRobotEditor.Converters
{
    public class TextOptionsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TextEditorOptions)
                return value as TextEditorOptions;


            System.Diagnostics.Debug.WriteLine("TextOptions Converter Failed {0}", value);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            System.Diagnostics.Debug.WriteLine("TextOptions Converter Failed ConvertBack {0}", value);

            return Binding.DoNothing;
        }
    }
}
