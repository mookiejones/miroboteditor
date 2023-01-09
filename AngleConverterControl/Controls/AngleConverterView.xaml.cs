using System.Windows.Controls;
using AngleConverterControl.ViewModels;

namespace AngleConverterControl.Controls
{
    /// <summary>
    /// Interaction logic for AngleConverterControlView.xaml
    /// </summary>
    public partial class AngleConverterView : UserControl
    {
        public AngleConverterView()
        {
            InitializeComponent();
            DataContext = new AngleConvertorViewModel();
        }
    }
}
