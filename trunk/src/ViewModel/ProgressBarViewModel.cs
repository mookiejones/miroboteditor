using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class ProgressBarViewModel:ViewModelBase
    {
        private int _maximum = 100;
        public int Maximum { get { return _maximum; } set { _maximum = value; RaisePropertyChanged(); } }

        private int _minimum;
        public int Minimum { get { return _minimum; } set { _minimum = value; RaisePropertyChanged(); } }

        private int _value;
        public int Value { get { return _value; } set { _value = value; RaisePropertyChanged(); } }

        private bool _isVisible;
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; RaisePropertyChanged(); } }


    }
}
