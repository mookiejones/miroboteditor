using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;
using System.ComponentModel;

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
            get => _instance ?? (_instance = new GlobalOptions());
            set => _instance = value;
        }

        [DefaultValue(0.75)]
        public double FlyoutOpacity { get; set; }

        public GlobalOptionsViewModel Options
        {
            get => _options;
            set => _options = value;
        }

        [Localizable(false)]
        public string Title => "Global Options";
    }
}