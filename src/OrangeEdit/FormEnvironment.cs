using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using miRobotEditor.Commands;

namespace miRobotEditor.OrangeEdit
{
    internal class FormEnvironment : ViewModelBase
    {
        #region Commands

        private RelayCommand _activateCommand;
        private RelayCommand _activateDefaultCommand;

        private RelayCommand _closeCommand;
        private RelayCommand _folderCommand;
        private RelayCommand _textChangedCommand;

        public ICommand ActivateCommand
        {
            get
            {
                return _activateCommand ?? (_activateCommand = new RelayCommand(param => Activate(), param => true));
            }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(param => Close(), param => true)); }
        }

        public ICommand ActivateDefaultCommand
        {
            get
            {
                return _activateDefaultCommand ??
                       (_activateDefaultCommand = new RelayCommand(param => ActivateDefault(), param => true));
            }
        }

        public ICommand FolderCommand
        {
            get { return _folderCommand ?? (_folderCommand = new RelayCommand(param => Folder(), param => true)); }
        }

        public ICommand TextChangedCommand
        {
            get
            {
                return _textChangedCommand ??
                       (_textChangedCommand = new RelayCommand(param => TextChanged(), param => true));
            }
        }

        #endregion

        #region Properties 

        private bool _activateEnabled;
        private string _dataPath = string.Empty;
        private string _directory = string.Empty;
        private string _name = string.Empty;
        private string _r1Path = string.Empty;
        private string _userTechPath = string.Empty;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string Directory
        {
            get { return _directory; }
            set
            {
                _directory = value;
                RaisePropertyChanged();
            }
        }


        public string UserTechPath
        {
            get { return _userTechPath; }
            set
            {
                _userTechPath = value;
                RaisePropertyChanged();
            }
        }

        public string DataPath
        {
            get { return _dataPath; }
            set
            {
                _dataPath = value;
                RaisePropertyChanged();
            }
        }

        public string R1Path
        {
            get { return _r1Path; }
            set
            {
                _r1Path = value;
                RaisePropertyChanged();
            }
        }

        public bool ActivateEnabled
        {
            get { return _activateEnabled; }
            set
            {
                _activateEnabled = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        private void TextChanged()
        {
            ActivateEnabled = true;
        }

        private void Activate()
        {
            ActivateEnabled = false;
        }

        private void ActivateDefault()
        {
            ActivateEnabled = false;
        }

        private void Folder()
        {
            var fbd = new FolderBrowserDialog {SelectedPath = Directory, ShowNewFolderButton = false};
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Directory = fbd.SelectedPath;
            }
        }

        private static void Close()
        {
        }
    }
}