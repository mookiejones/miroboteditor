using System.Windows.Controls;

namespace miRobotEditor.Controls
{
    internal class OptionsPanel : UserControl
    {
        public OptionsPanel()
        {
            DataContext = this;
        }


// ReSharper disable UnusedMember.Local
        private object Owner { get; set; }
//        object Control { get; }
        private void LoadOptions()
        {
        }

        private bool SaveOptions()
        {
            return true;
        }

        // ReSharper restore UnusedMember.Local
    }
}