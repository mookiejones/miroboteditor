using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using MainApplication.GUI.Workbench;
using miRobotEditor.Core.Controls;
using miRobotEditor.Core;
using miRobotEditor.Core.EventArgs;
using miRobotEditor.Core.Services;
using IWin32Window = System.Windows.Forms.IWin32Window;
using ToolBar = System.Windows.Forms.ToolBar;

namespace MainApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    sealed partial class WpfWorkbench : FullScreenEnabledWindow ,IWorkbench ,IWin32Window
    {

        #region Constant Members

        const string mainMenuPath = "/SharpDevelop/Workbench/MainMenu";
        const string viewContentPath = "/SharpDevelop/Workbench/Pads";
#endregion
        #region Events
        public event EventHandler ActiveWorkbenchWindowChanged;
        public event EventHandler ActiveViewContentChanged;
        public event EventHandler ActiveContentChanged;
        public event ViewContentEventHandler ViewOpened;
        public event ViewContentEventHandler ViewClosed;
#endregion

        #region Private Members
                miStatusBar statusBar = new miStatusBar();
		        ToolBar[] toolBars;
        #endregion

          public WpfWorkbench()
        {
           this.SynchronizingObject = new WpfSynchronizeInvoke(this.Dispatcher);
			this.StatusBar = new StatusBarService(statusBar);
			InitializeComponent();
			InitFocusTrackingEvents();
        }
        protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource.FromHwnd(this.MainWin32Window.Handle).AddHook(SingleInstanceHelper.WndProc);
			// validate after PresentationSource is initialized
			Rect bounds = new Rect(Left, Top, Width, Height);
			bounds = FormLocationHelper.Validate(bounds.TransformToDevice(this).ToSystemDrawing()).ToWpf().TransformFromDevice(this);
			SetBounds(bounds);
			// Set WindowState after PresentationSource is initialized, because now bounds and location are properly set.
			this.WindowState = lastNonMinimizedWindowState;
		}
         void SetBounds(Rect bounds)
		{
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
		}
		
		public void Initialize()
		{
			UpdateFlowDirection();
			
			foreach (PadDescriptor content in AddInTree.BuildItems<PadDescriptor>(viewContentPath, this, false)) {
				if (content != null) {
					ShowPad(content);
				}
			}
			
			mainMenu.ItemsSource = MenuService.CreateMenuItems(this, this, mainMenuPath, activationMethod: "MainMenu", immediatelyExpandMenuBuildersForShortcuts: true);
			
			toolBars = ToolBarService.CreateToolBars(this, this, "/SharpDevelop/Workbench/ToolBar");
			foreach (ToolBar tb in toolBars) {
				DockPanel.SetDock(tb, Dock.Top);
				dockPanel.Children.Insert(1, tb);
			}
			DockPanel.SetDock(statusBar, Dock.Bottom);
			dockPanel.Children.Insert(dockPanel.Children.Count - 2, statusBar);
			
			UpdateMenu();
			
			AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
			Project.ProjectService.CurrentProjectChanged += SetProjectTitle;
			
			FileService.FileRemoved += CheckRemovedOrReplacedFile;
			FileService.FileReplaced += CheckRemovedOrReplacedFile;
			FileService.FileRenamed += CheckRenamedFile;
			
			FileService.FileRemoved += FileService.RecentOpen.FileRemoved;
			FileService.FileRenamed += FileService.RecentOpen.FileRenamed;
			
			requerySuggestedEventHandler = new EventHandler(CommandManager_RequerySuggested);
			CommandManager.RequerySuggested += requerySuggestedEventHandler;
			ResourceService.LanguageChanged += OnLanguageChanged;
			
			this.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
		// keep a reference to the event handler to prevent it from being garbage collected
		// (CommandManager.RequerySuggested only keeps weak references to the event handlers)
		EventHandler requerySuggestedEventHandler;

		void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			UpdateMenu();
		}
		
		void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			e.Handled = true;
			if (e.Uri.Scheme == "mailto") {
				try {
					Process.Start(e.Uri.ToString());
				} catch {
					// catch exceptions - e.g. incorrectly installed mail client
				}
			} else {
				FileService.OpenFile(e.Uri.ToString());
			}
		}
		
		void SetProjectTitle(object sender, Project.ProjectEventArgs e)
		{
			if (e.Project != null) {
				Title = e.Project.Name + " - " + ResourceService.GetString("MainWindow.DialogName");
			} else {
				Title = ResourceService.GetString("MainWindow.DialogName");
			}
		}
		
		void CheckRemovedOrReplacedFile(object sender, FileEventArgs e)
		{
			foreach (OpenedFile file in FileService.OpenedFiles) {
				if (FileUtility.IsBaseDirectory(e.FileName, file.FileName)) {
					foreach (IViewContent content in file.RegisteredViewContents.ToArray()) {
						// content.WorkbenchWindow can be null if multiple view contents
						// were in the same WorkbenchWindow and both should be closed
						// (e.g. Windows Forms Designer, Subversion History View)
						if (content.WorkbenchWindow != null) {
							content.WorkbenchWindow.CloseWindow(true);
						}
					}
				}
			}
			Editor.PermanentAnchorService.FileDeleted(e);
		}
		
		void CheckRenamedFile(object sender, FileRenameEventArgs e)
		{
			if (e.IsDirectory) {
				foreach (OpenedFile file in FileService.OpenedFiles) {
					if (file.FileName != null && FileUtility.IsBaseDirectory(e.SourceFile, file.FileName)) {
						file.FileName = new FileName(FileUtility.RenameBaseDirectory(file.FileName, e.SourceFile, e.TargetFile));
					}
				}
			} else {
				OpenedFile file = FileService.GetOpenedFile(e.SourceFile);
				if (file != null) {
					file.FileName = new FileName(e.TargetFile);
				}
			}
			Editor.PermanentAnchorService.FileRenamed(e);
		}
		
		void UpdateMenu()
		{
			MenuService.UpdateStatus(mainMenu.ItemsSource);
			foreach (ToolBar tb in toolBars) {
				ToolBarService.UpdateStatus(tb.ItemsSource);
			}
		}
		
		void OnLanguageChanged(object sender, EventArgs e)
		{
			MenuService.UpdateText(mainMenu.ItemsSource);
			UpdateFlowDirection();
		}
        internal void OnViewOpened(ViewContentEventArgs e)
        {
            if (ViewOpened != null)
            {
                ViewOpened(this, e);
            }
        }

        internal void OnViewClosed(ViewContentEventArgs e)
        {
            if (ViewClosed != null)
                ViewClosed(this, e);
        }






        public miRobotEditor.Core.Services.Properties CreateMemento()
        {
            throw new NotImplementedException();
        }

        public void SetMemento(miRobotEditor.Core.Services.Properties memento)
        {
            throw new NotImplementedException();
        }

        	void InitFocusTrackingEvents()
		{
			#if DEBUG
			this.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(WpfWorkbench_PreviewLostKeyboardFocus);
			this.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(WpfWorkbench_PreviewGotKeyboardFocus);
			#endif
		}
        IntPtr IWin32Window.Handle
        {
            get
            {
                var wnd = PresentationSource.FromVisual(this) as System.Windows.Interop.IWin32Window;
                if (wnd != null)
                    return wnd.Handle;
                else
                    return IntPtr.Zero;
            }
        }

        public IWin32Window MainWin32Window { get; private set; }
        public ISynchronizeInvoke SynchronizingObject { get; private set; }
        public Window MainWindow { get; private set; }
        public IStatusBarService StatusBar { get; private set; }
        public ICollection<IViewContent> ViewContentCollection { get; private set; }
        public ICollection<IViewContent> PrimaryViewContents { get; private set; }
        public IList<IWorkbenchWindow> WorkbenchWindowCollection { get; private set; }
        public IWorkbenchWindow ActiveWorkbenchWindow { get; private set; }
        public IViewContent ActiveViewContent { get; private set; }
        public object ActiveContent { get; private set; }
        public IWorkbenchLayout WorkbenchLayout { get; set; }
        public bool IsActiveWindow { get; private set; }
      

        public void ShowView(IViewContent content)
        {
            throw new NotImplementedException();
        }

        public void ShowView(IViewContent content, bool switchToOpenedView)
        {
            throw new NotImplementedException();
        }

        public void CloseAllViews()
        {
            throw new NotImplementedException();
        }

        public bool CloseAllSolutionViews()
        {
            throw new NotImplementedException();
        }

       
        public IntPtr Handle { get; private set; }
        public miRobotEditor.Core.Services.Properties CreateMemento()
        {
            throw new NotImplementedException();
        }

        public void SetMemento(miRobotEditor.Core.Services.Properties memento)
        {
            throw new NotImplementedException();
        }


        void WpfWorkbench_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			FocusDebug("GotKeyboardFocus: oldFocus={0}, newFocus={1}", e.OldFocus, e.NewFocus);
		}

		#if DEBUG
		static bool enableFocusDebugOutput;
		void WpfWorkbench_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			FocusDebug("LostKeyboardFocus: oldFocus={0}, newFocus={1}", e.OldFocus, e.NewFocus);
		}
        protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (!e.Handled && e.Key == Key.D && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				enableFocusDebugOutput = !enableFocusDebugOutput;
				
				var output = new StringWriter();
				output.WriteLine("Keyboard.FocusedElement = " + GetElementName(Keyboard.FocusedElement));
				output.WriteLine("ActiveContent = " + GetElementName(this.ActiveContent));
				output.WriteLine("ActiveViewContent = " + GetElementName(this.ActiveViewContent));
				output.WriteLine("ActiveWorkbenchWindow = " + GetElementName(this.ActiveWorkbenchWindow));
				((AvalonDockLayout)workbenchLayout).WriteState(output);
				LoggingService.Debug(output.ToString());
				e.Handled = true;
			}
			if (!e.Handled && e.Key == Key.F && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				if (TextOptions.GetTextFormattingMode(this) == TextFormattingMode.Display)
					TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
				else
					TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
				this.StatusBar.SetMessage("TextFormattingMode=" + TextOptions.GetTextFormattingMode(this));
			}
			if (!e.Handled && e.Key == Key.R && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				switch (TextOptions.GetTextRenderingMode(this)) {
					case TextRenderingMode.Auto:
					case TextRenderingMode.ClearType:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.Grayscale);
						break;
					case TextRenderingMode.Grayscale:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.Aliased);
						break;
					default:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
						break;
				}
				this.StatusBar.SetMessage("TextRenderingMode=" + TextOptions.GetTextRenderingMode(this));
			}
			if (!e.Handled && e.Key == Key.G && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) {
				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
				this.StatusBar.SetMessage("Total memory = " + (GC.GetTotalMemory(true) / 1024 / 1024f).ToString("f1") + " MB");
			}
		}
		#endif

        	internal static string GetElementName(object element)
        	{
        	    if (element == null)
				return "<null>";
        	    return element.GetType().FullName + ": " + element;
        	}
    }
}
