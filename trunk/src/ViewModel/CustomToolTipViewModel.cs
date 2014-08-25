using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    public class CustomToolTipViewModel : ViewModelBase
    {
        private string _additional;
        private string _message;
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        public string Additional
        {
            get { return _additional; }
            set
            {
                _additional = value;
                RaisePropertyChanged("Additional");
            }
        }
    }
}