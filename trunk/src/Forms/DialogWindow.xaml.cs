using System.Windows;
using GalaSoft.MvvmLight;

namespace miRobotEditor.Forms
{
    /// <summary>
    ///     Interaction logic for DialogWindow.xaml
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

    public class DialogWindowViewModel : ViewModelBase
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
    }
}