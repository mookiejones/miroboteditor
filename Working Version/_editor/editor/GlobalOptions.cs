using System.ComponentModel;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Classes
{
    public sealed class GlobalOptions : IOptions
    {
        private static GlobalOptions _instance;
        private GlobalOptionsViewModel _options = new GlobalOptionsViewModel();

        private GlobalOptions()
        {
            FlyoutOpacity = 0.85;
        }

        public static GlobalOptions Instance
        {
            get { return _instance ?? (_instance = new GlobalOptions()); }
            set { _instance = value; }
        }

        [DefaultValue(0.75)]
        public double FlyoutOpacity { get; set; }

        public GlobalOptionsViewModel Options
        {
            get { return _options; }
            set { _options = value; }
        }

        [Localizable(false)]
        public string Title
        {
            get { return "Global Options"; }
        }
    }
}