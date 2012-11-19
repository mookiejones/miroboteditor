using System;
using System.Windows.Input;
namespace miRobotEditor.Commands
{
    public abstract class AbstractMenuCommand : AbstractCommand, IMenuCommand
    {
      
		private bool _isenabled = true;
		public virtual bool IsEnabled{get{return _isenabled;}set{_isenabled=value;}}
	
    }
    public abstract class AbstractCommand : ICommand
    {
        object owner;

        /// <summary>
        /// Returns the owner of the command.
        /// </summary>
        public virtual object Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
                OnOwnerChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Invokes the command.
        /// </summary>
        public abstract void Run();


        protected virtual void OnOwnerChanged(EventArgs e)
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, e);
            }
        }

        public event EventHandler OwnerChanged;
    	
		public event EventHandler CanExecuteChanged;
    	
		public void Execute(object parameter)
		{
			throw new NotImplementedException();
		}
    	
		public bool CanExecute(object parameter)
		{
			throw new NotImplementedException();
		}
    }

    public interface IMenuCommand : ICommand
    {
        bool IsEnabled
        {
            get;
            set;
        }
    }
}
