using miRobotEditor.ViewModel;

namespace miRobotEditor
{
    public class PaneViewModel : ViewModelBase,IPaneViewModel
    {
        #region Title

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        #endregion

        public System.Windows.Media.ImageSource IconSource
        {
            get;
            set;
        }

        #region ContentId

        private string _contentId;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId == value) return;
                _contentId = value;
                RaisePropertyChanged("ContentId");
            }
        }

        #endregion

        #region IsSelected

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        #endregion

        #region IsActive

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        #endregion


    }
}