using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class FileOptionsViewModel:ViewModelBase
    {
        private bool _showFullName=true;
        public bool ShowFullName{get{return _showFullName;}set{_showFullName=value;RaisePropertyChanged();}}
    }
}
