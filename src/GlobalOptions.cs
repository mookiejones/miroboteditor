using miRobotEditor.ViewModel;

namespace miRobotEditor
{
    public class GlobalOptions
    {
        private static GlobalOptions _instance = new GlobalOptions();


        private GlobalOptionsViewModel _options = new GlobalOptionsViewModel();

        public static GlobalOptions Instance
        {
            get { return _instance ?? new GlobalOptions(); }
            set { _instance = value; }
        }

        public GlobalOptionsViewModel Options
        {
            get { return _options; }
            set { _options = value; }
        }
    }
}