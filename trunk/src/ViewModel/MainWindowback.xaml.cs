using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using miRobotEditor.Properties;
using miRobotEditor.ViewModel;

namespace miRobotEditor	
{
    using Classes;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class MainWindowback
    {
        public static MainWindowback Instance { get; set; }


        public DockingManager Dock { get { return DockManager; } set { DockManager = value; } }

        public void LoadItems()
        {            
            //Load Files that were closed with the window the last time the Program was executed
            LoadOpenFiles();

            //If No open files, Open one
            var docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane != null && docpane.ChildrenCount == 0)
                Workspace.Instance.AddNewFile();

            ProcessArgs();

        }

        static void OpenFile(string filename)
        {
            Workspace.Instance.Open(filename);
        }

        private static void LoadOpenFiles()
        {
            var s = Settings.Default.OpenDocuments.Split(';');
            for (var i = 0; i < s.Length - 1; i++)
            {
                if (File.Exists(s[i]))
                    OpenFile(s[i]);
            }
        }

        /// <summary>
        /// Open file from parameters sent to program
        /// </summary>
        private static void ProcessArgs()
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 1; i < args.Length; i++)
            {
                OpenFile(args[i]);
            }
        }


        [Localizable(false)]
        private void DropFiles(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var t in files)
            {
                MessageViewModel.Instance.Add("File Dropped", String.Format("Opening:={0}", t), MsgIcon.Info);
                Workspace.Instance.Open(t);
            }
        }

        // ReSharper disable InconsistentNaming
        public void onDragEnter(object sender, DragEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        public MainWindowback()
        {
            Instance = this;        
            InitializeComponent();
            KeyDown += (s, e) =>   StatusBarViewModel.Instance.ManageKeys(s, e);
        }


       
        /// <summary>
        /// Makes a call GUI threadsafe without waiting for the returned value.
        /// </summary>
        public static void SafeThreadAsyncCall(Action method)
        {
     //      SynchronizingObject.BeginInvoke(method, emptyObjectArray);
        }

        public static void CallLater(TimeSpan delay, Action method)
        {
            var delayMilliseconds = (int)delay.TotalMilliseconds;
            if (delayMilliseconds < 0)
                throw new ArgumentOutOfRangeException("delay", delay, "Value must be positive");
            if (method == null)
                throw new ArgumentNullException("method");
            SafeThreadAsyncCall(
                delegate
                {
                    var t = new System.Windows.Forms.Timer {Interval = Math.Max(1, delayMilliseconds)};
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
        /// Takes Place on Application Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            
            var docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            
            if (docpane != null)
                foreach (var d in docpane.Children.Select(doc => doc.Content as DocumentViewModel).Where(d => d != null && d.FilePath!=null))
                {
                    Settings.Default.OpenDocuments += d.FilePath + ';';
                }
            Settings.Default.Save();

            SaveLayout();

            Workspace.Instance.IsClosing = true;
            App._application.Shutdown();
        }


        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadItems();
        }

        private void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(DockManager);
            using (var stream = new StreamWriter(Global.DockConfig))
                serializer.Serialize(stream);
        }


        public void CloseWindow(object param)
        {
            var ad = param as IDocument;
            var docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane == null) return;
			
            foreach (var c in docpane.Children.Where(c => c.Content.Equals(ad)))
            {
                docpane.Children.Remove(c);
                return;
            } 
        }
     //  private void dockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
     //  {
     //          Workspace.Instance.Close(e.Document.Content as IDocument);
     //  }


   
       
    }
}
