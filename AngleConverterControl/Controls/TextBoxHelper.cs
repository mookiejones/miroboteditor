using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AngleConverterControl.Controls
{
    public static class TextBoxHelper
    {

        static TextBoxHelper()
        {
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                TextBox.PreviewTextInputEvent,
                new RoutedEventHandler(TextBoxPreviewEvent)
                );
        }
        private static readonly Regex _regex = new Regex("[\\d.]+"); //regex that matches disallowed text

        private static void TextBoxPreviewEvent(object sender, RoutedEventArgs e)
        {

            var args = e as TextCompositionEventArgs;

            var valid = _regex.IsMatch(args.Text);
            e.Handled = !valid; 
        }

        public static readonly DependencyProperty CheckPreview = DependencyProperty.RegisterAttached(
          nameof(CheckPreview), typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(default(bool)));

        public static void SetCheckPreview(DependencyObject element, bool value)
            => element.SetValue(CheckPreview, value);

        public static bool GetCheckPreview(DependencyObject element)
            => (bool)element.GetValue(CheckPreview);
    }
}
