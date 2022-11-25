using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace RobotEditor.ViewModel
{
    public class PaneViewModel : ViewModelBase
    {
        private string _contentId;
        private bool _isActive;
        private bool _isSelected;
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (!(_title == value))
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public ImageSource IconSource { get; set; }

        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (!(_contentId == value))
                {
                    _contentId = value;
                    RaisePropertyChanged("ContentId");
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }
    }
}