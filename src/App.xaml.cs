using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Reflection;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.Diagnostics;
using miRobotEditor.ViewModel;
namespace miRobotEditor
{
	
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public static string StartupPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }


    	
    	 public static string Version
    	 {
    	 	get
    	 	{
	    	 	var asm = System.Reflection.Assembly.GetExecutingAssembly();
	    	 	return asm.GetName().Version.ToString();
    	 	}
    	 }

    	 public static string ProductName
    	 {
    	 	get 
    	 	{
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();
    	 	}
    	 }
    
    	  private const string Unique = "My_Unique_Application_String";
    	  public static App _application;
          [STAThread]
          public static void Main()
          {

              #if DEBUG
              //Control.CheckForIllegalCrossThreadCalls = true;
              #endif
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
          
          
		public bool SignalExternalCommandLineArgs(IList<string> args)
		{			
			MainWindow.Activate();
	        Workspace.Instance.LoadFile(args);
			return true;
		}

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageViewModel.AddError("App",e.Exception);
            Console.Write(e);
            e.Handled = true;
        }
        
        #region Unused Overrides
        /*
          protected override void OnExit(ExitEventArgs e)
          {
              base.OnExit(e);
          }
            [Localizable(false)]
          protected override void OnStartup(StartupEventArgs e)
          {
             if (e.Args.Length > 0)
              {
              	
              	foreach(var v in e.Args)
              	{
              		
              	}
              	System.Diagnostics.Debugger.Break();
              	MessageBox.Show(e.ToString());
                  MessageBox.Show("You have the latest version.");
                  Shutdown();
              }

            var task = new JumpTask { Title = "Check for Updates", Arguments = "/update", Description = "Checks for Software Updates", CustomCategory = "Actions", IconResourcePath = Assembly.GetEntryAssembly().CodeBase, ApplicationPath = Assembly.GetEntryAssembly().CodeBase};

          var asm = Assembly.GetExecutingAssembly();

          var version = new JumpTask
                                 {
                                     CustomCategory = "Version",
                                     Title = asm.GetName().Version.ToString(),
                                     IconResourcePath = asm.Location,
                                     IconResourceIndex = 0
                                 };

         var jumpList = new JumpList();
          jumpList.JumpItems.Add(version);
          jumpList.ShowFrequentCategory = true;
          jumpList.ShowRecentCategory = true;
          JumpList.SetJumpList(Current, jumpList);
          jumpList.Apply();

          base.OnStartup(e);
          }
*/
        #endregion
    }
    

}
