using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Shell;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace miRobotEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
    	  private const string Unique = "My_Unique_Application_String";
    	  static App _application;
    [STAThread]
    public static void Main()
    {

//#if DEBUG
        Control.CheckForIllegalCrossThreadCalls = true;
//#endif
        if (!CheckEnvironment())
            return;
        if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
        {
            _application = new App();

            _application.InitializeComponent();
            _application.Run();
            // Allow single instance code to perform cleanup operations
            SingleInstance<App>.Cleanup();
        }
    }

        [Localizable(false)]
        static bool CheckEnvironment()
    {
        // Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case SharpDevelop is
        // used on another machine than it was installed on (e.g. "SharpDevelop on USB stick")
        if (Environment.Version < new Version(4, 0, 30319))
        {            
            MessageBox.Show(String.Format(miRobotEditor.Properties.Resources.CheckEnvironment, Assembly.GetExecutingAssembly().GetName().Name, Environment.Version));
            return false;
        }
        // Work around a WPF issue when %WINDIR% is set to an incorrect path
        var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows, Environment.SpecialFolderOption.DoNotVerify);
        if (Environment.GetEnvironmentVariable("WINDIR") != windir)
        {
            Environment.SetEnvironmentVariable("WINDIR", windir);
        }
        return true;
    }

        [Localizable(false)]
        protected override void OnStartup(StartupEventArgs e)
	{
	//if (e.Args.Length > 0)
	//{
    //    
	//	MessageBox.Show("You have the latest version.");
	//	Shutdown();
	//}
	//	JumpTask task = new JumpTask
    // {
    //     Title = "Check for Updates",
    //     Arguments = "/update",
    //     Description = "Checks for Software Updates",
    //     CustomCategory = "Actions",
    //     IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
    //     ApplicationPath = Assembly.GetEntryAssembly().CodeBase 
    // };

        var asm = Assembly.GetExecutingAssembly();

	    var version = new JumpTask
	                           {
                                   CustomCategory="Version",
                                   Title = asm.GetName().Version.ToString(),
                                   IconResourcePath= asm.Location,
                                   IconResourceIndex=0 };

        var jumpList = new JumpList();
        jumpList.JumpItems.Add(version);
      //  jumpList.JumpItems.Add(task);
        jumpList.ShowFrequentCategory = true;
        jumpList.ShowRecentCategory = true; 
        JumpList.SetJumpList(Current, jumpList);
        jumpList.Apply();
		
		base.OnStartup(e);
	}
    	
		public bool SignalExternalCommandLineArgs(IList<string> args)
		{			
			MainWindow.Activate();
			((MainWindow)_application.MainWindow).LoadFile(args);
			return true;
		}

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Classes.MessageViewModel.Instance.AddError(e.Exception);
         //   MessageBox.Show(e.Exception.Message);
            e.Handled = true;
         //   throw e.Exception;

        }
    }
}
