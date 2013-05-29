using System.Windows.Controls;
namespace miRobotEditor.Controls
{
    class OptionsPanel:UserControl
    {

        public OptionsPanel()
        {
            DataContext = this;
        }



// ReSharper disable UnusedMember.Local
        object Owner { get; set; }
//        object Control { get; }
        void LoadOptions() { }
        bool SaveOptions() { return true; }
        // ReSharper restore UnusedMember.Local

    }
}
