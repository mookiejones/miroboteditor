using System;
using System.Windows;

namespace miRobotEditor.Windows
{
    /// <summary>
    ///     Interaction logic for AboutWindow.xaml
    /// </summary>
    public sealed partial class AboutWindow : Window
    {
        public AboutWindow() => InitializeComponent();

        private void CloseClick(object sender, RoutedEventArgs e) => throw new NotImplementedException();
    }
}