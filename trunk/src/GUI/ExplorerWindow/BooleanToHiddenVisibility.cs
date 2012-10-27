using System;
using System.Windows;
using System.Windows.Data;
using miRobotEditor.Classes;

namespace miRobotEditor.GUI.ExplorerWindow
{
    public class BooleanToHiddenVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rv = Visibility.Visible;
            try
            {
                rv = bool.Parse(value.ToString()) ? Visibility.Hidden : Visibility.Visible;
            }
            catch (Exception ex)
            {
                OutputMessages.AddError(ex);
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }
}