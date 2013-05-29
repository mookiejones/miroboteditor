using System.Windows;

namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
// ReSharper disable RedundantExtendsListEntry
    public partial class AboutWindow : Window
// ReSharper restore RedundantExtendsListEntry
    {
        public AboutWindow()
        {
            InitializeComponent();
        }
        
		void CloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
    }
}
