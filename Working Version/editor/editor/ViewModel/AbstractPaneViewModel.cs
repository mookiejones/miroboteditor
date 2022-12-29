

namespace miRobotEditor.ViewModel
{
    public abstract class AbstractPaneViewModel : ViewModelBase
    {
        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value); 
        }
    }
}