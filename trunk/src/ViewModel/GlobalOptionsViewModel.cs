using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class GlobalOptionsViewModel:ViewModelBase
    {
    	private FileOptionsViewModel _fileOptions = new FileOptionsViewModel();

        public FileOptionsViewModel FileOptions { get { return _fileOptions; } set { _fileOptions = value; RaisePropertyChanged(); } }
    }
}
