using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.GUI.Editor;
using miRobotEditor.ViewModel;

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
            WorkspaceViewModel.Instance.ActiveEditor.TextBox.ChangeIndent(param);
        }
        #endregion


        #region ToggleIntellisenseCommand

        private static RelayCommand<EditorOptions> _toggleIntellisenseCommand;
        /// <summary>
        /// Gets the ToggleIntellisenseCommand.
        /// </summary>
        public static RelayCommand<EditorOptions> ToggleIntellisenseCommand
        {
            get
            {
                return _toggleIntellisenseCommand
                    ?? (_toggleIntellisenseCommand = new RelayCommand<EditorOptions>(ExecuteToggleIntellisense));
            }
        }

        private static void ExecuteToggleIntellisense(EditorOptions options)
        {
            options.AllowIntellisense=!options.AllowIntellisense;
        }
        #endregion



        public static RoutedCommand AddNewFileCommand = new RoutedCommand();
    }
}