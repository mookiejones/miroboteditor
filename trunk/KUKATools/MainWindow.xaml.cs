using System.Windows;
using KUKATools.Windows;

namespace KUKATools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetAutoLogonUsername_clicked(object sender, RoutedEventArgs e)
        {
            var t = new GetAutoLogonUsername();



        }

        private void ChangeLoginInfo_Clicked(object sender, RoutedEventArgs e)
        {
            var window = new LoginInfoWindow();
            window.ShowDialog();
        }
    }
}
