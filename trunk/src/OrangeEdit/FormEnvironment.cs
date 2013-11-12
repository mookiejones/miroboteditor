using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace miRobotEditor.OrangeEdit
{
    [Localizable(false)]
    class FormEnvironment:ViewModelBase
    {
        #region Commands

        #region ActivateCommand

        private RelayCommand _activateCommand;
        /// <summary>
        /// Gets the ActivateCommand.
        /// </summary>
        public RelayCommand ActivateCommand
        {
            get
            {
                return _activateCommand
                    ?? (_activateCommand = new RelayCommand(ExecuteActivateCommand));
            }
        }

        private void ExecuteActivateCommand()
        {
            Activate();
        }
        #endregion

        #region CloseCommand

        private RelayCommand _closeCommand;
        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(ExecuteCloseCommand));
            }
        }

        private void ExecuteCloseCommand()
        {
            Close();
        }
        #endregion


        #region ActivateDefaultCommand

        private RelayCommand _activateDefaultCommand;
        /// <summary>
        /// Gets the ActivateDefaultCommand.
        /// </summary>
        public RelayCommand ActivateDefaultCommand
        {
            get
            {
                return _activateDefaultCommand
                    ?? (_activateDefaultCommand = new RelayCommand(ExecuteActivateDefaultCommand));
            }
        }

        private void ExecuteActivateDefaultCommand()
        {
            ActivateDefault();
        }

        #region FolderCommand

        private RelayCommand _folderCommand;
        /// <summary>
        /// Gets the FolderCommand.
        /// </summary>
        public RelayCommand FolderCommand
        {
            get
            {
                return _folderCommand
                    ?? (_folderCommand = new RelayCommand(ExecuteFolderCommand));
            }
        }

        private void ExecuteFolderCommand()
        {
            Folder();
        }
        #endregion
        #endregion

        #region TextChangedCommand

        private RelayCommand _textChangedCommand;
        /// <summary>
        /// Gets the TextChangedCommand.
        /// </summary>
        public RelayCommand TextChangedCommand
        {
            get
            {
                return _textChangedCommand
                    ?? (_textChangedCommand = new RelayCommand(ExecuteTextChangedCommand));
            }
        }

        private void ExecuteTextChangedCommand()
        {
            TextChanged();
        }
        #endregion

     

        #endregion

        #region Properties 
        string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }

        string _directory = string.Empty;
        public string Directory { get { return _directory; } set { _directory = value; RaisePropertyChanged("Directory"); } }


        string _userTechPath = string.Empty;
        public string UserTechPath { get { return _userTechPath; } set { _userTechPath = value; RaisePropertyChanged("UserTechPath"); } }

        string _dataPath = string.Empty;
        public string DataPath { get { return _dataPath; } set { _dataPath = value; RaisePropertyChanged("DataPath"); } }

        string _r1Path = string.Empty;
        public string R1Path { get { return _r1Path; } set { _r1Path = value; RaisePropertyChanged("R1Path"); } }

        private bool _activateEnabled;
        public bool ActivateEnabled { get { return _activateEnabled; } set { _activateEnabled = value; RaisePropertyChanged("ActivateEnabled"); } }


        #endregion

        void TextChanged()
        {
            ActivateEnabled = true;
        }

        void Activate()
        {
            ActivateEnabled = false;
        }
        void ActivateDefault()
        {
            ActivateEnabled = false;
        }
        void Folder()
        {
            var fbd = new FolderBrowserDialog {SelectedPath = Directory, ShowNewFolderButton = false};
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directory = fbd.SelectedPath;
            }
        }

        static void Close() { }
    }
}
