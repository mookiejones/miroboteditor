using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Annotations;
using miRobotEditor.Core;
using miRobotEditor.Forms;
using miRobotEditor.Properties;
using miRobotEditor.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Application = System.Windows.Application;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using Global = miRobotEditor.Classes.Global;

namespace miRobotEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Instance { get; set; }

        #region Constructor

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ThemeManager.ChangeAppTheme(Application.Current, "Light");


            KeyDown += (s, e) => StatusBarViewModel.Instance.ManageKeys(s, e);
        }

        #endregion

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

        private static void OpenFile(string filename)
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.Open(filename);
        }


        private static void LoadOpenFiles()
        {
            string[] s = Settings.Default.OpenDocuments.Split(';');
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (File.Exists(s[i]))
                    OpenFile(s[i]);
            }
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


        [Localizable(false)]
        private void DropFiles(object sender, DragEventArgs e)
        {
            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            foreach (string t in files)
            {
                MessageViewModel.Instance.Add("File Dropped", String.Format("Opening:={0}", t), MsgIcon.Info);

                main.Open(t);
            }
        }

        // ReSharper disable InconsistentNaming
        public void onDragEnter(object sender, DragEventArgs e)
            // ReSharper restore InconsistentNaming
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }


        /// <summary>
        ///     Makes a call GUI threadsafe without waiting for the returned value.
        /// </summary>
        public static void SafeThreadAsyncCall(Action method)
        {
            //      SynchronizingObject.BeginInvoke(method, emptyObjectArray);
        }

        public static void CallLater(TimeSpan delay, Action method)
        {
            var delayMilliseconds = (int) delay.TotalMilliseconds;
            if (delayMilliseconds < 0)
                throw new ArgumentOutOfRangeException("delay", delay, "Value must be positive");
            if (method == null)
                throw new ArgumentNullException("method");
            SafeThreadAsyncCall(
                delegate
                {
                    var t = new Timer {Interval = Math.Max(1, delayMilliseconds)};
                    t.Tick += delegate
                    {
                        t.Stop();
                        t.Dispose();
                        method();
                    };
                    t.Start();
                });
        }


        /// <summary>
        ///     Takes Place on Application Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            App.Application.Shutdown();
        }


        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //   LoadLayout();
            LoadItems();
            Splasher.CloseSplash();
            LoadLayout();
        }

        private void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(DockManager);
            using (var stream = new StreamWriter(Global.DockConfig))
                serializer.Serialize(stream);
        }

        [UsedImplicitly]
        private void LoadLayout()
        {
            if (!File.Exists(Global.DockConfig))
                return;

            var serializer = new XmlLayoutSerializer(DockManager);
            using (new StreamReader(Global.DockConfig)) serializer.Deserialize(Global.DockConfig);
        }

        public void CloseWindow(object param)
        {
            var ad = param as IDocument;
            LayoutDocumentPane docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane == null) return;

            foreach (LayoutContent c in docpane.Children.Where(c => c.Content.Equals(ad)))
            {
                docpane.Children.Remove(c);
                return;
            }
        }
    }
}