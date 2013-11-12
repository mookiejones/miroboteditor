using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class GlobalOptionsViewModel:ViewModelBase
    {
    	private FileOptionsViewModel _fileOptions = new FileOptionsViewModel();

        public FileOptionsViewModel FileOptions { get { return _fileOptions; } set { _fileOptions = value; RaisePropertyChanged("FileOptions"); } }
    }
}
