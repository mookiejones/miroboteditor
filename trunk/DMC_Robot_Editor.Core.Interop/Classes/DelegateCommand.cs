using System;
using System.Windows.Input;
namespace DMC_Robot_Editor.Classes
{
   public class DelegateCommand:ICommand
    {
       readonly Action<object> _execute;
       readonly Predicate<object> _canExecute;

       public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
       {
           _execute = execute;
           _canExecute = canExecute;
       }

       public DelegateCommand(Action<object> execute)
           : this(execute, null)
       {
       }

       public event EventHandler CanExecuteChanged
       {
           add { CommandManager.RequerySuggested += value; }
           remove { CommandManager.RequerySuggested -= value; }
       }

       public void Execute(object parameter)
       {
           _execute(parameter);
       }

       public bool CanExecute(object parameter)
       {
           return _canExecute == null || _canExecute(parameter);
       }

       #region ICommand Members

       public object Owner
       {
           get
           {
               throw new NotImplementedException();
           }
           set
           {
               throw new NotImplementedException();
           }
       }

       public void Run()
       {
           throw new NotImplementedException();
       }

       
       private void FireOwnerChanged()
       {
       	if (OwnerChanged!=null)
       		OwnerChanged(this, new EventArgs());
       }
       public event EventHandler OwnerChanged;

       #endregion
    }
}
