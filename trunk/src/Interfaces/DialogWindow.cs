using System.Windows.Input;

namespace miRobotEditor.Interfaces
{
    internal class DialogWindow
    {
// ReSharper disable UnusedMember.Local
        private string Title { get; set; }
        private string Description { get; set; }
        private ICommand OkCommand { get; set; }
        private ICommand CancelCommand { get; set; }
        // ReSharper restore UnusedMember.Local
    }
}