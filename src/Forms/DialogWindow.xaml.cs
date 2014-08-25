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

        
        #region Title
        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _title = string.Empty;

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                RaisePropertyChanging(TitlePropertyName);
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }
        #endregion
       
    }
}