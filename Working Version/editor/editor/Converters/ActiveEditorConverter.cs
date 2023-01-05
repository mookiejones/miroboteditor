using System;
using System.Globalization;
using System.Windows.Data;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Converters
{
    internal sealed class ActiveEditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEditorDocument editor = value as IEditorDocument;
            return editor ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEditorDocument editor = value as IEditorDocument;
            return editor ?? Binding.DoNothing;
        }
    }
}