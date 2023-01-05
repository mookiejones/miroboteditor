using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FTPBrowser.Model;
using FTPBrowser.ViewModels;

namespace FTPBrowser.Views
{
    /// <summary>
    /// Interaction logic for ConnectionForm.xaml
    /// </summary>
    public partial class ConnectionForm : Window
    {
        #region Members

        public static ConnectionForm Instance { get; set; }

        //Save A Copy Of AccountCollection on new instantiation
        private ObservableCollection<FTPAccount> _current;
        #endregion
        #region Commands
        static RelayCommand _cancelCommand;



        public static ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(() => Instance.Cancel(), () => CanCancel);
                }
                return _cancelCommand;
            }
        }
        #endregion

        /// <summary>
        /// Returns True if you can revert back to the initial value
        /// </summary>
        static bool CanCancel
        {
            get { return FTPAccountViewModel.Instance.Accounts != Instance._current; }
        }
        public ConnectionForm()
        {
            Instance = this;
            InitializeComponent();
            _current = FTPAccountViewModel.Instance.Accounts;
            DataContext = this;
        }

        void Cancel()
        {
            FTPAccountViewModel.Instance.Accounts = _current;
        }

        void btnOK_Press(object sender, RoutedEventArgs e)
        {
            if (btnOK.Content.ToString() != "OK")
            {
                PromptForSave();
                return;
            }
            Close();
        }

        void Help_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            FTPAccountViewModel.Instance.SerializeAccounts();
        }


        void ItemChanged(object sender, KeyEventArgs e)
        {
            FTPAccountViewModel.Instance.IsSelectedEqual = false;
        }

        void PromptForSave()
        {
            //Prompt the user to save the Setup
        }

    }
}
