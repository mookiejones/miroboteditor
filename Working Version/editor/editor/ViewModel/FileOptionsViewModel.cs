

namespace miRobotEditor.ViewModel
{
    public sealed class FileOptionsViewModel : ViewModelBase
    {
        #region ShowFullName


        private bool _showFullName = true;

        /// <summary>
        ///     Sets and gets the ShowFullName property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowFullName
        {
            get => _showFullName;

            set => SetProperty(ref _showFullName, value);
        }

        #endregion
    }
}