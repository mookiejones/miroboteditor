using System.Windows;

namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
// ReSharper disable RedundantExtendsListEntry
    public partial class DialogWindow : Window
// ReSharper restore RedundantExtendsListEntry
    {
        public DialogWindow()
        {
            InitializeComponent();
        }
    }

    public class DialogWindowViewModel : ViewModel.ViewModelBase
    {
        private string _title;
        public string Title { get { return _title; } set { _title = value;RaisePropertyChanged("Title"); } }
    }
}
