using System;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
namespace miRobotEditor.Commands
{
   public static class Commands
    {
   	
   		private static RelayCommand _changeIndentCommand;

        public static ICommand ChangeIndentCommand
        {
        	get { 
        		return _changeIndentCommand ?? (_changeIndentCommand = new RelayCommand(param => Workspace.Instance.ActiveEditor.TextBox.ChangeIndent(param), param => true)); }
        }
   	
        
   	private static object syncRoot = new object();
   	
   	public static RoutedCommand AddNewFileCommand = new RoutedCommand();
   	
   	
    }

}
