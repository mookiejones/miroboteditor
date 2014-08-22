using GalaSoft.MvvmLight;

namespace miRobotEditor
{
    public sealed class SystemFunctionsViewModel : ViewModelBase
    {
        private bool _functions = true;
        private bool _programs = true;
        private bool _structures = true;

        private bool _variables = true;

        public bool Structures
        {
            get { return _structures; }
            set
            {
                _structures = value;
                RaisePropertyChanged("Structures");
            }
        }

        public bool Variables
        {
            get { return _variables; }
            set
            {
                _variables = value;
                RaisePropertyChanged("Variables");
            }
        }

        public bool Programs
        {
            get { return _programs; }
            set
            {
                _programs = value;
                RaisePropertyChanged("Programs");
            }
        }

        public bool Functions
        {
            get { return _functions; }
            set
            {
                _functions = value;
                RaisePropertyChanged("Functions");
            }
        }


        public void ShowDialog()
        {
        }
    }
}