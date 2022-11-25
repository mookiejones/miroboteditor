using GalaSoft.MvvmLight;

namespace RobotEditor.ViewModel
{
    public sealed class GlobalOptionsViewModel : ViewModelBase
    {
        #region FileOptions

        /// <summary>
        ///     The <see cref="FileOptions" /> property's name.
        /// </summary>
        private const string FileOptionsPropertyName = "FileOptions";

        private FileOptionsViewModel _fileOptions = new FileOptionsViewModel();

        /// <summary>
        ///     Sets and gets the FileOptions property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FileOptionsViewModel FileOptions
        {
            get { return _fileOptions; }

            set
            {
                if (_fileOptions == value)
                {
                    return;
                }

                
                _fileOptions = value;
                RaisePropertyChanged(FileOptionsPropertyName);
            }
        }

        #endregion
    }
}