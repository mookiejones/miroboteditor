using System.Windows;
using AngleConverter.ViewModels;

namespace AngleConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AngleConvertorViewModel();
        }
    }
}
