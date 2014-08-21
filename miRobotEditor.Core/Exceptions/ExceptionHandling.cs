using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using miRobotEditor.Core.Tracing;

namespace miRobotEditor.Core.Exceptions
{
    public class ExceptionHandling : DispatcherObject
    {
        private delegate void HandleExceptionDelegate(Exception exception, bool canContinue);
        private PrettyTraceSource exceptionTrace = TraceSourceFactory.GetSource(PredefinedTraceSource.Exception);
        private bool shutdownInvoked;
        private bool defaultCanContinue;
        private int loggedExceptionsLimit = 2147483647;
        private bool skipLogExceptions;
        private bool showingDialog;

        public event EventHandler<ExceptionDialogShowingEventArgs> DialogShowing
        {
            add { }
            remove { }
        }
        public event EventHandler<ExceptionDialogShowingEventArgs> DialogShown
        {
            add { }
            remove { }
        }
        /*
        public event EventHandler<ExceptionDialogShowingEventArgs> DialogShowing
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                this.DialogShowing = (EventHandler<ExceptionDialogShowingEventArgs>)Delegate.Combine(this.DialogShowing, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                this.DialogShowing = (EventHandler<ExceptionDialogShowingEventArgs>)Delegate.Remove(this.DialogShowing, value);
            }
        }
        public event EventHandler<ExceptionDialogShownEventArgs> DialogShown
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                this.DialogShown = (EventHandler<ExceptionDialogShownEventArgs>)Delegate.Combine(this.DialogShown, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                this.DialogShown = (EventHandler<ExceptionDialogShownEventArgs>)Delegate.Remove(this.DialogShown, value);
            }
        }
         * */
        public int LoggedExceptionsLimit
        {
            get
            {
                return this.loggedExceptionsLimit;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("LoggedExceptionsLimit");
                }
                this.loggedExceptionsLimit = value;
                if (this.loggedExceptionsLimit < 2147483647)
                {
                    this.exceptionTrace.WriteLine("Logging exceptions is limited to {0}.", new object[]
					{
						this.loggedExceptionsLimit
					});
                }
            }
        }
        public bool SkipsLoggingExceptions
        {
            get
            {
                return this.skipLogExceptions;
            }
        }
        public bool DefaultCanContinue
        {
            get
            {
                return this.defaultCanContinue;
            }
            set
            {
                this.defaultCanContinue = value;
            }
        }
        public bool ShutdownInvoked
        {
            get
            {
                return this.shutdownInvoked;
            }
        }
        public ExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnAppDomainUnhandledException);
        }
        public void AnnounceApplicationInstance(Application application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }
            application.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(this.OnUnhandledApplicationException);
        }
//        public void AnnounceComponentFramework(AdeComponentFramework componentFramework)
 //       {
  //          componentFramework.UnexpectedException += new AdeComponentFramework.UnexpectedExceptionEventHandler(this.OnComponentFrameworkException);
   //     }
        public void HandleException(Exception exception, bool canContinue)
        {
            if (base.Dispatcher.CheckAccess())
            {
                this.SyncHandleException(exception, canContinue);
                return;
            }
            base.Dispatcher.Invoke(DispatcherPriority.Normal, new ExceptionHandling.HandleExceptionDelegate(this.SyncHandleException), exception, new object[]
			{
				canContinue
			});
        }
        public void InvokeShutdown()
        {
            if (!this.shutdownInvoked && Application.Current != null)
            {
                this.shutdownInvoked = true;
                this.exceptionTrace.WriteLine("Shutdown is invoked.");
                Application.Current.Shutdown(3);
            }
        }
        public static bool ShowExceptionDialog(Exception exception, bool canContinue)
        {
            string caption = "Virtual Remote Pendant Error";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(PrettyTraceSource.GetExceptionHeadline(exception));
            stringBuilder.Append(Environment.NewLine);
            string exceptionDetails = PrettyTraceSource.GetExceptionDetails(exception, false);
            if (!string.IsNullOrEmpty(exceptionDetails))
            {
                stringBuilder.AppendLine(exceptionDetails);
            }
            stringBuilder.Append(Environment.NewLine);
            Window window = null;
            try
            {
                Application current = Application.Current;
                if (current != null)
                {
                    window = current.MainWindow;
                }
            }
            catch (TypeInitializationException)
            {
            }
            bool flag = false;
            if (canContinue)
            {
                stringBuilder.Append("Shutdown?");
                MessageBoxResult messageBoxResult;
                if (window != null)
                {
                    messageBoxResult = MessageBox.Show(window, stringBuilder.ToString(), caption, MessageBoxButton.YesNo, MessageBoxImage.Hand);
                }
                else
                {
                    messageBoxResult = MessageBox.Show(stringBuilder.ToString(), caption, MessageBoxButton.YesNo, MessageBoxImage.Hand);
                }
                if (messageBoxResult == MessageBoxResult.No)
                {
                    flag = true;
                }
            }
            else
            {
                stringBuilder.Append("The application must shut down.");
                if (window != null)
                {
                    MessageBox.Show(window, stringBuilder.ToString(), caption, MessageBoxButton.OK, MessageBoxImage.Hand);
                }
                else
                {
                    MessageBox.Show(stringBuilder.ToString(), caption, MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
            return !flag;
        }
        private void OnUnhandledApplicationException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.HandleException(e.Exception, this.defaultCanContinue);
            e.Handled = true;
        }
        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.IsTerminating)
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    this.HandleException(ex, this.defaultCanContinue);
                }
            }
        }
        private void OnComponentFrameworkException(object sender, UnexpectedExceptionEventArgs e)
        {
            this.HandleException(e.Exception, this.defaultCanContinue);
        }
        private void SyncHandleException(Exception exception, bool canContinue)
        {
            if (!this.skipLogExceptions)
            {
                this.exceptionTrace.WriteException(exception, true);
                if (this.exceptionTrace.Counters.AllErrors >= this.loggedExceptionsLimit)
                {
                    this.exceptionTrace.WriteLine(TraceEventType.Warning, "Logging further exceptions is skipped because the limit of {0} errors is reached.", new object[]
					{
						this.loggedExceptionsLimit
					});
                    this.skipLogExceptions = true;
                }
            }
            bool flag = false;
            if (!this.shutdownInvoked && !this.showingDialog)
            {
                bool flag2 = true;
               /* if (this.DialogShowing != null)
                {
                    ExceptionDialogShowingEventArgs exceptionDialogShowingEventArgs = new ExceptionDialogShowingEventArgs(exception);
                    this.DialogShowing(this, exceptionDialogShowingEventArgs);
                    if (exceptionDialogShowingEventArgs.Cancel)
                    {
                        flag2 = false;
                    }
                }*/
                if (flag2)
                {
                    try
                    {
                        this.showingDialog = true;
                        flag = ExceptionHandling.ShowExceptionDialog(exception, canContinue);
                       // if (this.DialogShown != null)
                       // {
                       //     this.DialogShown(this, new ExceptionDialogShownEventArgs(exception));
                       // }
                    }
                    finally
                    {
                        this.showingDialog = false;
                    }
                }
            }
            if (!this.shutdownInvoked && Application.Current != null && (!canContinue || flag) && !this.showingDialog)
            {
                this.exceptionTrace.WriteLine("Exception handling invokes shutdown.");
                this.InvokeShutdown();
            }
        }
    }
    public class ExceptionDialogShowingEventArgs : CancelEventArgs
    {
        private Exception exception;
        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }
        internal ExceptionDialogShowingEventArgs(Exception exception)
        {
            this.exception = exception;
        }
    }
    public sealed class UnexpectedExceptionEventArgs : EventArgs
    {
        private string a;
        private Exception b;
        public Exception Exception
        {
            get
            {
                return this.b;
            }
        }
        public string Message
        {
            get
            {
                return this.a;
            }
        }
        internal UnexpectedExceptionEventArgs(Exception A_0, string A_1)
        {
            this.a = A_1;
            this.b = A_0;
        }
    }
    
}
