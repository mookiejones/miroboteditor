using GalaSoft.MvvmLight;

namespace RobotEditor.ViewModel
{
    public sealed class ToolTipViewModel : ViewModelBase
    {
        private static ToolTipViewModel _instance;
        private string _message = string.Empty;
        private string _title = string.Empty;

        public static ToolTipViewModel Instance
        {
            get { return _instance ?? new ToolTipViewModel(); }
            set { _instance = value; }
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

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }
    }
}