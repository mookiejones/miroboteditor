using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Commands
{
    public static class Commands
    {
        private static RelayCommand _changeIndentCommand;


        public static RoutedCommand AddNewFileCommand = new RoutedCommand();

        public static ICommand ChangeIndentCommand
        {
            get
            {
                var main = ServiceLocator.Current.GetInstance<MainViewModel>();
                return _changeIndentCommand ??
                       (_changeIndentCommand =
                           new RelayCommand(param => main.ActiveEditor.TextBox.ChangeIndent(param), param => true));
            }
        }
    }
}