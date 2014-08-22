using System.Windows.Input;

namespace miRobotEditor.Commands
{
    public interface IMenuCommand : ICommand
    {
        bool IsEnabled { get; set; }
    }
}