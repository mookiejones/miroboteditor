using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miRobotEditor.ViewModel
{
    public class ProgressBarViewModel:ViewModelBase
    {
        private int _maximum = 100;
        public int Maximum { get { return _maximum; } set { _maximum = value; RaisePropertyChanged("Maximum"); } }

        private int _minimum = 0;
        public int Minimum { get { return _minimum; } set { _minimum = value; RaisePropertyChanged("Minimum"); } }

        private int _value = 0;
        public int Value { get { return _value; } set { _value = value; RaisePropertyChanged("Value"); } }

        private bool _isVisible = false;
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; RaisePropertyChanged("IsVisible"); } }


    }
}
