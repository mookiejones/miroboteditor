using CommunityToolkit.Mvvm.ComponentModel;

namespace miRobotEditor.ViewModel
{
    public sealed class GlobalOptionsViewModel :  ObservableRecipient
    {
        #region FileOptions

      
        private FileOptionsViewModel _fileOptions = new FileOptionsViewModel();

        /// <summary>
        ///     Sets and gets the FileOptions property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FileOptionsViewModel FileOptions
        {
            get => _fileOptions;
            set=>SetProperty(ref _fileOptions, value);
             
        }

        #endregion
    }
}