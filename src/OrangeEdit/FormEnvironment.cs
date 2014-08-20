using System.Windows.Input;
using miRobotEditor.Core;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using RelayCommand = miRobotEditor.Commands.RelayCommand;

namespace miRobotEditor.OrangeEdit
{
    class FormEnvironment:ViewModelBase
    {
        #region Commands
        private RelayCommand _activateCommand;
        public ICommand ActivateCommand
        {
            get { return _activateCommand ?? (_activateCommand = new RelayCommand(param => Activate(), param => true)); }
        }

        private RelayCommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(param => Close(), param => true)); }
        }
        private RelayCommand _activateDefaultCommand;
        public ICommand ActivateDefaultCommand
        {
            get { return _activateDefaultCommand ?? (_activateDefaultCommand = new RelayCommand(param => ActivateDefault(), param => true)); }
        }
        private RelayCommand _folderCommand;
        public ICommand FolderCommand
        {
            get { return _folderCommand ?? (_folderCommand = new RelayCommand(param => Folder(), param => true)); }
        }

        private RelayCommand _textChangedCommand;
        public ICommand TextChangedCommand
        {
            get { return _textChangedCommand ?? (_textChangedCommand = new RelayCommand(param => TextChanged(), param => true)); }
        }

        #endregion

        #region Properties 
        string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged(); } }

        string _directory = string.Empty;
        public string Directory { get { return _directory; } set { _directory = value; RaisePropertyChanged(); } }


        string _userTechPath = string.Empty;
        public string UserTechPath { get { return _userTechPath; } set { _userTechPath = value; RaisePropertyChanged(); } }

        string _dataPath = string.Empty;
        public string DataPath { get { return _dataPath; } set { _dataPath = value; RaisePropertyChanged(); } }

        string _r1Path = string.Empty;
        public string R1Path { get { return _r1Path; } set { _r1Path = value; RaisePropertyChanged(); } }

        private bool _activateEnabled;
        public bool ActivateEnabled { get { return _activateEnabled; } set { _activateEnabled = value; RaisePropertyChanged(); } }


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
