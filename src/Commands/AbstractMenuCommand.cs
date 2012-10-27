using System;

namespace miRobotEditor.Commands
{
    public abstract class AbstractMenuCommand : AbstractCommand, IMenuCommand
    {
        bool isEnabled = true;

        public virtual bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }
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
    }
    public interface ICommand
    {

        /// <summary>
        /// Returns the owner of the command.
        /// </summary>
        object Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Invokes the command.
        /// </summary>
        void Run();

        /// <summary>
        /// Is called when the Owner property is changed.
        /// </summary>
        event EventHandler OwnerChanged;
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
