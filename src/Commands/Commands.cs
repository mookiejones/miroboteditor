using System.Windows.Input;

namespace miRobotEditor.Commands
{
    public static class Commands
    {
        private static RelayCommand _changeIndentCommand;

        public static ICommand ChangeIndentCommand
        {
            get
            {
                return _changeIndentCommand ?? (_changeIndentCommand = new RelayCommand(param => Workspace.Instance.ActiveEditor.TextBox.ChangeIndent(param), param => true));
            }
        }

        public static RoutedCommand AddNewFileCommand = new RoutedCommand();
    }
}