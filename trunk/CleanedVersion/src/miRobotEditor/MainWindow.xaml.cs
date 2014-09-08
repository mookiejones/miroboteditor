using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Core.Classes;
using miRobotEditor.Core.Classes.Messaging;
using miRobotEditor.Core.Classes.Messaging.Interfaces;
using miRobotEditor.EditorControl;
using miRobotEditor.Properties;
using miRobotEditor.Resources;
using miRobotEditor.Core;
using miRobotEditor.Core;
using miRobotEditor.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace miRobotEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public static MainWindow Instance { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ThemeManager.ChangeAppTheme(Application.Current, "Light");


//            KeyDown += (s, e) => StatusBarViewModel.Instance.ManageKeys(s, e);

        }

        private void Initialize()
        {
            AllowDrop = true;
            TitleForeground = Brushes.Black;
            WindowState = WindowState.Maximized;
            SnapsToDevicePixels = true;

            Closing += (s, e) => ViewModelLocator.Cleanup();
            DragEnter += (s, e) =>
            {

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effects = DragDropEffects.Copy;

            };
        }

        private void DropFiles(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var t in files)
            {

                var title = MessageResources.FileDropped;
                var description = String.Format(MessageResources.Opening, t);

                var msg = new OutputWindowMessage(title,description,MsgIcon.Info);
                Messenger.Default.Send<IMessage>(msg);

                var fm = new FileMessage(t);
                Messenger.Default.Send<FileMessage>(fm);
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.OpenDocuments = String.Empty;
            LayoutDocumentPane docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            if (docpane != null)
                foreach (
                    DocumentViewModel d in
                        docpane.Children.Select(doc => doc.Content as DocumentViewModel)
                            .Where(d => d != null && d.FilePath != null))
                {
                    Settings.Default.OpenDocuments += d.FilePath + ';';
                }

            Settings.Default.Save();

            SaveLayout();

            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.IsClosing = true;

//            App.Application.Shutdown();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //   LoadLayout();
            LoadItems();
//            Splasher.CloseSplash();
            LoadLayout();
        }
         private void LoadItems()
        {
            //Load Files that were closed with the window the last time the Program was executed
            LoadOpenFiles();
            //If No open files, Open one
            LayoutDocumentPane docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
             if (docpane != null && docpane.ChildrenCount == 0)
             {
                 var main = ServiceLocator.Current.GetInstance<MainViewModel>();
                 main.AddNewFile();
             }



             ProcessArgs();
        }


         private static void LoadOpenFiles()
         {
             return;
             /*
             //string[] s = Settings.Default.OpenDocuments.Split(';');
             for (int i = 0; i < s.Length - 1; i++)
             {
                 if (File.Exists(s[i]))
                     OpenFile(s[i]);
             }
              * */
         }
         /// <summary>
         ///     Open file from parameters sent to program
         /// </summary>
         private static void ProcessArgs()
         {
             string[] args = Environment.GetCommandLineArgs();

             for (int i = 1; i < args.Length; i++)
             {
                 OpenFile(args[i]);
             }
         }
         private void SaveLayout()
         {
             var serializer = new XmlLayoutSerializer(DockManager);
             using (var stream = new StreamWriter(Global.DockConfig))
                 serializer.Serialize(stream);
         }

       
         private void LoadLayout()
         {
             if (!File.Exists(Global.DockConfig))
                 return;

             var serializer = new XmlLayoutSerializer(DockManager);
             using (new StreamReader(Global.DockConfig)) serializer.Deserialize(Global.DockConfig);
         }
         private static void OpenFile(string filename)
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.Open(filename);

        }


    }
}