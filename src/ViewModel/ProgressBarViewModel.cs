using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    public class ProgressBarViewModel : ViewModelBase
    {
        private bool _isVisible;
        private int _maximum = 100;

        private int _minimum;

        private int _value;

        public int Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                RaisePropertyChanged("Maximum");
            }
        }

        public int Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                RaisePropertyChanged("Minimum");
            }
        }

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }
    }
}