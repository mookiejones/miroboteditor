namespace miRobotEditor.ViewModel
{
    public class ToolTipViewModel:ViewModelBase
    {

        private static ToolTipViewModel _instance;
        public static ToolTipViewModel Instance{ get 
            {
                return _instance ?? new ToolTipViewModel();
            } set
            {
                _instance = value;
            }}

        private string _message = string.Empty;
        public string Message {get { return _message; }set { _message = value;RaisePropertyChanged("Message"); }}

        private string _title = string.Empty;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged("Title"); } }
    }
}
