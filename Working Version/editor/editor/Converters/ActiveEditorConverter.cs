using miRobotEditor.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

namespace miRobotEditor.Converters
{
    internal sealed class ActiveEditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var editor = value as IEditorDocument;
            return editor ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var editor = value as IEditorDocument;
            return editor ?? Binding.DoNothing;
        }
    }
}