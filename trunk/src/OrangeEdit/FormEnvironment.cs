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


        

        #region Name
        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = string.Empty;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }
        #endregion
        
        #region Directory
        /// <summary>
        /// The <see cref="Directory" /> property's name.
        /// </summary>
        public const string DirectoryPropertyName = "Directory";

        private string _directory = string.Empty;

        /// <summary>
        /// Sets and gets the Directory property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Directory
        {
            get
            {
                return _directory;
            }

            set
            {
                if (_directory == value)
                {
                    return;
                }

                RaisePropertyChanging(DirectoryPropertyName);
                _directory = value;
                RaisePropertyChanged(DirectoryPropertyName);
            }
        }
        #endregion

        
        #region UserTechPath
        /// <summary>
        /// The <see cref="UserTechPath" /> property's name.
        /// </summary>
        public const string UserTechPathPropertyName = "UserTechPath";

        private string _userTechPack = string.Empty;

        /// <summary>
        /// Sets and gets the UserTechPath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UserTechPath
        {
            get
            {
                return _userTechPack;
            }

            set
            {
                if (_userTechPack == value)
                {
                    return;
                }

                RaisePropertyChanging(UserTechPathPropertyName);
                _userTechPack = value;
                RaisePropertyChanged(UserTechPathPropertyName);
            }
        }
        #endregion

        
        #region DataPath
        /// <summary>
        /// The <see cref="DataPath" /> property's name.
        /// </summary>
        public const string DataPathPropertyName = "DataPath";

        private string _dataPath = string.Empty;

        /// <summary>
        /// Sets and gets the DataPath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DataPath
        {
            get
            {
                return _dataPath;
            }

            set
            {
                if (_dataPath == value)
                {
                    return;
                }

                RaisePropertyChanging(DataPathPropertyName);
                _dataPath = value;
                RaisePropertyChanged(DataPathPropertyName);
            }
        }
        #endregion

        
        #region R1Path
        /// <summary>
        /// The <see cref="R1Path" /> property's name.
        /// </summary>
        public const string R1PathPropertyName = "R1Path";

        private string _r1Path = string.Empty;

        /// <summary>
        /// Sets and gets the R1Path property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string R1Path
        {
            get
            {
                return _r1Path;
            }

            set
            {
                if (_r1Path == value)
                {
                    return;
                }

                RaisePropertyChanging(R1PathPropertyName);
                _r1Path = value;
                RaisePropertyChanged(R1PathPropertyName);
            }
        }
        #endregion

        
        #region ActivateEnabled
        /// <summary>
        /// The <see cref="ActivateEnabled" /> property's name.
        /// </summary>
        public const string ActivateEnabledPropertyName = "ActivateEnabled";

        private bool _activateEnabled = false;

        /// <summary>
        /// Sets and gets the ActivateEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool ActivateEnabled
        {
            get
            {
                return _activateEnabled;
            }

            set
            {
                if (_activateEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(ActivateEnabledPropertyName);
                _activateEnabled = value;
                RaisePropertyChanged(ActivateEnabledPropertyName);
            }
        }
        #endregion

  

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