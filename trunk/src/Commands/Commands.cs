using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace miRobotEditor.Commands
{
    public static class Commands
    {


        #region ChangeIndentCommand

        private static RelayCommand<object> _changeIndentCommand;
        /// <summary>
        /// Gets the ChangeIndentCommand.
        /// </summary>
        public static RelayCommand<object> ChangeIndentCommand
        {
            get
            {
                return _changeIndentCommand
                    ?? (_changeIndentCommand = new RelayCommand<object>(ExecuteChangeIndentCommand));
            }
        }

        private static void ExecuteChangeIndentCommand(object param)
        {
            Workspace.Instance.ActiveEditor.TextBox.ChangeIndent(param);
        }
        #endregion
     
   

        public static RoutedCommand AddNewFileCommand = new RoutedCommand();
    }
}