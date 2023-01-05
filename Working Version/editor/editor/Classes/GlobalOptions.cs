using System.ComponentModel;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Classes
{
    public sealed class GlobalOptions : IOptions
    {
        private static GlobalOptions _instance;

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

        public GlobalOptionsViewModel Options { get; set; } = new GlobalOptionsViewModel();

        [Localizable(false)]
        public string Title => "Global Options";
    }
}