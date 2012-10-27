using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    public static class WorkbenchSingleton
	{
		const string uiIconStyle             = "IconMenuItem.IconMenuStyle";
		const string uiLanguageProperty      = "CoreProperties.UILanguage";
		const string workbenchMemento        = "WorkbenchMemento";
		const string activeContentState      = "Workbench.ActiveContent";
		
		static IWorkbench workbench;
		
		/// <summary>
		/// Gets the main form. Returns null in unit-testing mode.
		/// </summary>
		public static IWin32Window MainWin32Window {
			get {
				if (workbench != null) {
					return workbench.MainWin32Window;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the main window. Returns null in unit-testing mode.
		/// </summary>
		public static Window MainWindow {
			get {
				if (workbench != null) {
					return workbench.MainWindow;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the workbench. Returns null in unit-testing mode.
		/// </summary>
		public static IWorkbench Workbench {
			get {
				return workbench;
			}
		}
		
		public static IStatusBarService StatusBar {
			get {
				return workbench != null ? workbench.StatusBar : null;
			}
		}

        [Localizable(false)]
        public static void InitializeWorkbench(IWorkbench workbench, IWorkbenchLayout layout)
		{
			WorkbenchSingleton.workbench = workbench;
			
			LanguageService.ValidateLanguage();
			
			DisplayBindingService.InitializeService();
			LayoutConfiguration.LoadLayoutConfiguration();
			FileService.InitializeService();
			DomHostCallback.Register(); // must be called after StatusBarService.Initialize()
			ParserService.InitializeParserService();
			TaskService.Initialize();
			Bookmarks.BookmarkManager.Initialize();
			Project.CustomToolsService.Initialize();
			Project.BuildModifiedProjectsOnlyService.Initialize();
			
			var messageService = Core.Services.ServiceManager.Instance.MessageService as IDialogMessageService;
			if (messageService != null) {
				messageService.DialogOwner = workbench.MainWin32Window;
				Debug.Assert(messageService.DialogOwner != null);
				messageService.DialogSynchronizeInvoke = workbench.SynchronizingObject;
			}
			
			workbench.Initialize();
			workbench.SetMemento(PropertyService.Get(workbenchMemento, new Properties()));
			workbench.WorkbenchLayout = layout;
			
			ApplicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return WorkbenchSingleton.Workbench.ActiveContent; });
			
			OnWorkbenchCreated();
			
			// initialize workbench-dependent services:
			Project.ProjectService.InitializeService();
			NavigationService.InitializeService();
			
			workbench.ActiveContentChanged += delegate {
				Debug.WriteLine("ActiveContentChanged to " + workbench.ActiveContent);
				LoggingService.Debug("ActiveContentChanged to " + workbench.ActiveContent);
			};
			workbench.ActiveViewContentChanged += delegate {
				Debug.WriteLine("ActiveViewContentChanged to " + workbench.ActiveViewContent);
				LoggingService.Debug("ActiveViewContentChanged to " + workbench.ActiveViewContent);
			};
			workbench.ActiveWorkbenchWindowChanged += delegate {
				Debug.WriteLine("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
				LoggingService.Debug("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
			};
		}
		
		/// <summary>
		/// Runs workbench cleanup.
		/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
		/// </summary>
		public static void OnWorkbenchUnloaded()
		{
			if (!Project.ProjectService.IsClosingCanceled()) {
				Project.ProjectService.CloseSolution();
				NavigationService.Unload();
				
				ApplicationStateInfoService.UnregisterStateGetter(activeContentState);
				
				WorkbenchUnloaded(null, System.EventArgs.Empty);
				
				FileService.Unload();
			}
		}   
		
		#region Safe Thread Caller
        /// <summary>
        /// Invoke Required
        /// </summary>
		public static bool InvokeRequired {
			get {
				if (workbench == null)
					return false; // unit test mode, don't crash
				else
					return workbench.SynchronizingObject.InvokeRequired;
			}
		}
		
		/// <summary>
		/// Throws an exception if the current thread is not the main thread.
		/// For performance reasons, the thread check is only done in debug builds.
		/// </summary>
		[Conditional("DEBUG")]
		internal static void DebugAssertMainThread()
		{
			AssertMainThread();
		}
		
		/// <summary>
		/// Throws an exception if the current thread is not the main thread.
		/// </summary>
		public static void AssertMainThread()
		{
			if (InvokeRequired) {
				throw new InvalidOperationException("This operation can be called on the main thread only.");
			}
		}
		
		readonly static object[] emptyObjectArray = new object[0];
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<R>(Func<R> method)
		{
			// InvokeRequired test is necessary so that we don't run other actions in the message queue
			// when we're already running on the main thread (unexpected reentrancy)
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				return (R)workbench.SynchronizingObject.Invoke(method, emptyObjectArray);
			else
				return method();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static R SafeThreadFunction<A, R>(Func<A, R> method, A arg1)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				return (R)si.Invoke(method, new object[] { arg1 });
			else
				return method(arg1);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall(Action method)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, emptyObjectArray);
			else
				method();
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A>(Action<A> method, A arg1)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, new object[] { arg1 });
			else
				method(arg1);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, new object[] { arg1, arg2 });
			else
				method(arg1, arg2);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe. WARNING: This method waits for the result of the
		/// operation, which can result in a dead-lock when the main thread waits for a lock
		/// held by this thread!
		/// </summary>
		public static void SafeThreadCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			ISynchronizeInvoke si = workbench.SynchronizingObject;
			if (si.InvokeRequired)
				si.Invoke(method, new object[] { arg1, arg2, arg3 });
			else
				method(arg1, arg2, arg3);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall(Action method)
		{
			workbench.SynchronizingObject.BeginInvoke(method, emptyObjectArray);
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A>(Action<A> method, A arg1)
		{
			workbench.SynchronizingObject.BeginInvoke(method, new object[] { arg1 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B>(Action<A, B> method, A arg1, B arg2)
		{
			workbench.SynchronizingObject.BeginInvoke(method, new object[] { arg1, arg2 });
		}
		
		/// <summary>
		/// Makes a call GUI threadsafe without waiting for the returned value.
		/// </summary>
		public static void SafeThreadAsyncCall<A, B, C>(Action<A, B, C> method, A arg1, B arg2, C arg3)
		{
			workbench.SynchronizingObject.BeginInvoke(method, new object[] { arg1, arg2, arg3 });
		}
		
		/// <summary>
		/// Calls a method on the GUI thread, but delays the call a bit.
		/// </summary>
		public static void CallLater(TimeSpan delay, Action method)
		{
			var delayMilliseconds = (int)delay.TotalMilliseconds;
			if (delayMilliseconds < 0)
				throw new ArgumentOutOfRangeException("delay", delay, "Value must be positive");
			if (method == null)
				throw new ArgumentNullException("method");
			SafeThreadAsyncCall(
				delegate {
					var t = new Timer {Interval = Math.Max(1, delayMilliseconds)};
				             t.Tick += delegate {
						t.Stop();
						t.Dispose();
						method();
					};
					t.Start();
				});
		}
		#endregion
		
		static void OnWorkbenchCreated()
		{
			WorkbenchCreated(null, System.EventArgs.Empty);
		}
		
		/// <summary>
		/// Is called, when the workbench is created
		/// </summary>
		public static event EventHandler WorkbenchCreated = delegate {};
		
		/// <summary>
		/// Is called, when the workbench is unloaded
		/// </summary>
		public static event EventHandler WorkbenchUnloaded = delegate {};
	}

    /// <summary>
	/// This is the basic interface to the workspace.
	/// </summary>
	public interface IWorkbench : IMementoCapable
	{
		/// <summary>
		/// The main window as IWin32Window.
		/// </summary>
		IWin32Window MainWin32Window { get; }
		
		/// <summary>
		/// Object for executing methods on the main thread.
		/// </summary>
		ISynchronizeInvoke SynchronizingObject { get; }
		
		/// <summary>
		/// The main window.
		/// </summary>
		Window MainWindow { get; }
		
		/// <summary>
		/// Gets the status bar.
		/// </summary>
		IStatusBarService StatusBar { get; }
		
		/// <summary>
		/// Gets/Sets whether the window is displayed in full-screen mode.
		/// </summary>
		bool FullScreen { get; set; }
		
		/// <summary>
		/// The title shown in the title bar.
		/// </summary>
		string Title {
			get;
			set;
		}
		
		/// <summary>
		/// A collection in which all opened view contents (including all secondary view contents) are saved.
		/// </summary>
		ICollection<IViewContent> ViewContentCollection {
			get;
		}
		
		/// <summary>
		/// A collection in which all opened primary view contents are saved.
		/// </summary>
		ICollection<IViewContent> PrimaryViewContents {
			get;
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get;
		}
		
		
		/// <summary>
		/// The active workbench window.
		/// This is the window containing the active view content.
		/// </summary>
		IWorkbenchWindow ActiveWorkbenchWindow {
			get;
		}
		
		/// <summary>
		/// Is called, when the ActiveWorkbenchWindow property changes.
		/// </summary>
		event EventHandler ActiveWorkbenchWindowChanged;
		
		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		IViewContent ActiveViewContent {
			get;
		}
		
		/// <summary>
		/// Is called, when the active view content has changed.
		/// </summary>
		event EventHandler ActiveViewContentChanged;
		
		/// <summary>
		/// The active content, depending on where the focus currently is.
		/// If a document is currently active, this will be equal to ActiveViewContent,
		/// if a pad has the focus, this property will return the IPadContent instance.
		/// </summary>
		object ActiveContent {
			get;
		}
		
		/// <summary>
		/// Is called, when the active content has changed.
		/// </summary>
		event EventHandler ActiveContentChanged;
		/// <summary>
		/// WorkbenchLayout
		/// </summary>
		IWorkbenchLayout WorkbenchLayout {
			get;
			set;
		}
		
		/// <summary>
		/// Gets whether SharpDevelop is the active application in Windows.
		/// </summary>
		bool IsActiveWindow {
			get;
		}
		
		/// <summary>
		/// Initializes the workbench.
		/// </summary>
		void Initialize();
		
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace and switches to the new view.
		/// </summary>
		void ShowView(IViewContent content);
		
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace.
		/// </summary>
		void ShowView(IViewContent content, bool switchToOpenedView);
		
		/// <summary>
		/// Closes all views inside the workbench.
		/// </summary>
		void CloseAllViews();
		
		/// <summary>
		/// 	Closes all views related to current solution.
		/// </summary>
		/// <returns>
		/// 	True if all views were closed properly, false if closing was aborted.
		/// </returns>
		bool CloseAllSolutionViews();
		
		/// <summary>
		/// Is called, when a workbench view was opened
		/// </summary>
		/// <example>
		/// WorkbenchSingleton.WorkbenchCreated += delegate {
		/// 	WorkbenchSingleton.Workbench.ViewOpened += ...;
		/// };
		/// </example>
		event ViewContentEventHandler ViewOpened;
		
		/// <summary>
		/// Is called, when a workbench view was closed
		/// </summary>
		event ViewContentEventHandler ViewClosed;
	}
    /// <summary>
    /// The IWorkbenchLayout object is responsible for the layout of
    /// the workspace, it shows the contents, chooses the IWorkbenchWindow
    /// implementation etc. it could be attached/detached at the runtime
    /// to a workbench.
    /// </summary>
    public interface IWorkbenchLayout
    {
        /// <summary>
        /// The active workbench window.
        /// </summary>
        IWorkbenchWindow ActiveWorkbenchWindow
        {
            get;
        }

        /// <summary>
        /// Gets the open workbench windows.
        /// </summary>
        IList<IWorkbenchWindow> WorkbenchWindows
        {
            get;
        }

        /// <summary>
        /// The active content. This can be either a IViewContent or a IPadContent, depending on
        /// where the focus currently is.
        /// </summary>
        object ActiveContent
        {
            get;
        }

        /// <summary>
        /// Active WorkbenchWindowChanged
        /// </summary>
        event EventHandler ActiveWorkbenchWindowChanged;
        /// <summary>
        /// ActiveContentChanged
        /// </summary>
        event EventHandler ActiveContentChanged;

        /// <summary>
        /// Attaches this layout manager to a workbench object.
        /// </summary>
        void Attach(IWorkbench workbench);

        /// <summary>
        /// Detaches this layout manager from the current workspace.
        /// </summary>
        void Detach();


        /// <summary>
        /// Shows a new <see cref="IViewContent"/> and optionally switches to it.
        /// </summary>
        IWorkbenchWindow ShowView(IViewContent content, bool switchToOpenedView);

        /// <summary>
        /// Load Configuration
        /// </summary>
        void LoadConfiguration();
        /// <summary>
        /// Store Configuration
        /// </summary>
        void StoreConfiguration();
    }
    /// <summary>
    /// The IWorkbenchWindow is the basic interface to a window which
    /// shows a view (represented by the IViewContent object).
    /// </summary>
    public interface IWorkbenchWindow
    {
        /// <summary>
        /// The window title.
        /// </summary>
        string Title
        {
            get;
        }

        /// <summary>
        /// Gets if the workbench window has been disposed.
        /// </summary>
        [Obsolete("This property always returns false.")]
        bool IsDisposed
        {
            get;
        }

        /// <summary>
        /// Gets/Sets current view content which is shown inside this window.
        /// </summary>
        IViewContent ActiveViewContent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the icon of the view content.
        /// </summary>
        System.Windows.Media.ImageSource Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Is raised when the ActiveViewContent property has changed.
        /// </summary>
        event EventHandler ActiveViewContentChanged;

        /// <summary>
        /// Gets the list of view contents displayed in this window
        /// </summary>
        IList<IViewContent> ViewContents
        {
            get;
        }

        /// <summary>
        /// Activates the view with the specified index.
        /// </summary>
        void SwitchView(int viewNumber);

        /// <summary>
        /// Closes the window, if force == true it closes the window
        /// without asking, even the content is dirty.
        /// </summary>
        /// <returns>true, if window is closed</returns>
        bool CloseWindow(bool force);

        /// <summary>
        /// Brings this window to front and sets the user focus to this
        /// window.
        /// </summary>
        void SelectWindow();

        /// <summary>
        /// Is called when the title of this window has changed.
        /// </summary>
        event EventHandler TitleChanged;
    }
}

