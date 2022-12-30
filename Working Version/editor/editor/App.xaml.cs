using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using miRobotEditor.Classes;
using miRobotEditor.Design;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Messages;
using miRobotEditor.Model;
using miRobotEditor.ViewModel;
using miRobotEditor.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shell;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace miRobotEditor
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "miRobotEditor";
        public static App Application;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            // Services
            if (ViewModelBase.IsInDesignModeStatic)
            {
                services.AddSingleton<IDataService, DesignDataService>();
            }
            else
            {
                services.AddSingleton<IDataService, DataService>();
            }
            services.AddSingleton<IDataService, DataService>();
            //services.AddSingleton<ISettingsService, SettingsService>();
            //services.AddSingleton<IClipboardService, ClipboardService>();
            //services.AddSingleton<IShareService, ShareService>();
            //services.AddSingleton<IEmailService, EmailService>();
            // Viewmodels

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<StatusBarViewModel>();
            services.AddSingleton<ObjectBrowserViewModel>();
            services.AddSingleton<MessageViewModel>();

            return services.BuildServiceProvider();
        }

        static App()
        {
            // DispatcherHelper.Initialize();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow.Activate();
            var main = Ioc.Default.GetRequiredService<MainViewModel>();
            main.LoadFile(args);
            return true;
        }

        [Localizable(false)]
        private static bool CheckEnvironment()
        {
            // Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case SharpDevelop is
            // used on another machine than it was installed on (e.g. "SharpDevelop on USB stick")
            if (Environment.Version < new Version(4, 0, 30319))
            {
                MessageBox.Show(String.Format(miRobotEditor.Properties.Resources.CheckEnvironment,
                    Assembly.GetExecutingAssembly().GetName().Name, Environment.Version));
                return false;
            }
            // Work around a WPF issue when %WINDIR% is set to an incorrect path
            var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows,
                Environment.SpecialFolderOption.DoNotVerify);
            if (Environment.GetEnvironmentVariable("WINDIR") != windir)
            {
                Environment.SetEnvironmentVariable("WINDIR", windir);
            }
            return true;
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var msg = new ErrorMessage("App", e.Exception, MessageType.Error);
            WeakReferenceMessenger.Default.Send(msg);

            Console.Write(e);
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e) => base.OnExit(e);

        [Localizable(false)]
        protected override void OnStartup(StartupEventArgs e)
        {
            Splasher.Splash = new SplashScreenWindow();
            Splasher.ShowSplash();

            Trace.WriteLine("Try Color Picker from MaterialDesignThemes");
#if DEBUG
            Control.CheckForIllegalCrossThreadCalls = true;
#endif
            if (!CheckEnvironment())
                return;
            if (!SingleInstance<App>.InitializeAsFirstInstance(Unique)) return;
            //    Application = new App();

            //    Application.InitializeComponent();
            //   Application.Run();

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

            if (e.Args.Length > 0)
            {
                foreach (var v in e.Args)
                {
                }
                Debugger.Break();
                MessageBox.Show(e.ToString());
                MessageBox.Show("You have the latest version.");
                Shutdown();
            }

            var task = new JumpTask
            {
                Title = "Check for Updates",
                Arguments = "/update",
                Description = "Checks for Software Updates",
                CustomCategory = "Actions",
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };

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
    }
}