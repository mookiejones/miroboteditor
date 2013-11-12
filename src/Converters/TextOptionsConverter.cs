using System;
using System.ComponentModel;
using System.Windows.Data;
using miRobotEditor.GUI.Editor;

namespace miRobotEditor.Converters
{
    [Localizable(false)]
    public class TextOptionsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is EditorOptions)
                return value as EditorOptions;


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
