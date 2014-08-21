using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Reflection;
using miRobotEditor.Core;
using miRobotEditor.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Diagnostics;

namespace miRobotEditor
{
	
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
// ReSharper disable once ClassCanBeSealed.Global
    public partial class App : ISingleInstanceApp
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
	    	 	var asm = Assembly.GetExecutingAssembly();
	    	 	return asm.GetName().Version.ToString();
    	 	}
    	 }

    	 public static string ProductName
    	 {
    	 	get 
    	 	{
                return Assembly.GetExecutingAssembly().GetName().ToString();
    	 	}
    	 }
    
    	  private const string Unique = "My_Unique_Application_String";
    	  public static App Application;
       
          [STAThread]
          public static void Main()
          {

             
#if !DEBUG
              Splasher.Splash = new SplashScreen();
              Splasher.ShowSplash();
#endif

              #if DEBUG
              Control.CheckForIllegalCrossThreadCalls = true;
              #endif
             if (!CheckEnvironment())
                 return;
             if (!SingleInstance<App>.InitializeAsFirstInstance(Unique)) return;
              Application = new App();

              Application.InitializeComponent();                 
              Application.Run();

            
            //  var _tools = Workspace.Instance.Tools;
            //  foreach (var tool in _tools)
            //  {
            //      if (tool is miRobotEditor.GUI.FindReplaceViewModel)
            //      {
            //          var obj = tool as miRobotEditor.GUI.FindReplaceViewModel;
            //          System.Xml.Serialization.XmlSerializer serial = new System.Xml.Serialization.XmlSerializer(typeof(miRobotEditor.GUI.Results));
            //          System.IO.TextWriter writer = new System.IO.StreamWriter("D:\\results.xml");
            //          serial.Serialize(writer,obj.FindReplaceResults);
            //      }
            //  }
              // Allow single instance code to perform cleanup operations
              SingleInstance<App>.Cleanup();
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
