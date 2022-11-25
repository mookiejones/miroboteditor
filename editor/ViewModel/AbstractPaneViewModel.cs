using GalaSoft.MvvmLight;

namespace RobotEditor.ViewModel
{
    public abstract class AbstractPaneViewModel : ViewModelBase
    {
        private bool _isVisible = true;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }
    }
}