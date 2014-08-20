using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class SystemFunctionsViewModel : ViewModelBase
    {
        private bool _structures = true;
        public bool Structures { get { return _structures; } set { _structures = value; RaisePropertyChanged(); } }

        private bool _variables = true;
        public bool Variables { get { return _variables; } set { _variables = value; RaisePropertyChanged(); } }

        private bool _programs = true;
        public bool Programs { get { return _programs; } set { _programs = value; RaisePropertyChanged(); } }

        private bool _functions = true;
        public bool Functions { get { return _functions; } set { _functions = value; RaisePropertyChanged(); } }


        public void ShowDialog()
        {
        }
    }
}