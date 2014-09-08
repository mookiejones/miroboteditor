/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/16/2012
 * Time: 10:24
 * 
 */

using System;
using System.Diagnostics;
using System.Windows.Input;
using miRobotEditor.Core.Helpers;

namespace miRobotEditor.Core.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly WeakAction<T> _execute;
        private readonly WeakFunc<T, bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this._canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (this._canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = new WeakAction<T>(execute);
            if (canExecute != null)
            {
                this._canExecute = new WeakFunc<T, bool>(canExecute);
            }
        }
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }
            if (!this._canExecute.IsStatic && !this._canExecute.IsAlive)
            {
                return false;
            }
            if (parameter == null && typeof(T).IsValueType)
            {
                return this._canExecute.Execute(default(T));
            }
            return this._canExecute.Execute((T)((object)parameter));
        }
        public virtual void Execute(object parameter)
        {
            object obj = parameter;
            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
            {
                obj = Convert.ChangeType(parameter, typeof(T), null);
            }
            if (this.CanExecute(obj) && this._execute != null && (this._execute.IsStatic || this._execute.IsAlive))
            {
                if (obj == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        this._execute.Execute(default(T));
                        return;
                    }
                    this._execute.Execute((T)((object)obj));
                    return;
                }
                else
                {
                    this._execute.Execute((T)((object)obj));
                }
            }
        }
    }

    [DebuggerStepThrough]
    public sealed class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors



        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
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

        #endregion // ICommand Members

        public event EventHandler OwnerChanged;

        private void OnOwnerChanged(EventArgs e)
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, e);
            }
        }
    }


}
