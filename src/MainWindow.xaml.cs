using MahApps.Metro;
using miRobotEditor.Annotations;
using miRobotEditor.Core;
using miRobotEditor.Forms;
using miRobotEditor.Properties;
using miRobotEditor.Resources;
using miRobotEditor.Resources.StringResources;
using miRobotEditor.ViewModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace miRobotEditor
{
    using Classes;
    using GalaSoft.MvvmLight.Messaging;
    using miRobotEditor.Classes.Messages;
    using miRobotEditor.WindowMessages;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class MainWindow
    {
        #region · Members ·

        public static MainWindow Instance { get; set; }

        [NonSerialized]
        private Accent _currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public Accent CurrentAccent { get { return _currentAccent; } set { _currentAccent = value; } }

        private readonly Theme _currentTheme = ThemeManager.ThemeIsDark ? Theme.Dark : Theme.Light;

        public Theme Theme { get; set; }

        #endregion · Members ·

        #region Constructor

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ThemeManager.ChangeTheme(this, _currentAccent, _currentTheme);
            KeyDown += (s, e) => StatusBarViewModel.Instance.ManageKeys(s, e);
            Messenger.Default.Register<GetFileMessage>(this,GetFileNames);
        }



        #endregion Constructor

        #region · Public Methods ·

        public void LoadItems()
        {
            //If No open files, Open one
            var docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            LoadLayout();

            // Load files that were opened when i last closed the window
            LoadOpenFiles();

            ProcessArgs();


            //Load Files that were closed with the window the last time the Program was executed
            if ((docpane != null && docpane.ChildrenCount == 0) & (WorkspaceViewModel.Instance.Files.Count < 1))
                WorkspaceViewModel.Instance.AddNewFile();
        }

        #endregion · Public Methods ·

    //   private static void OpenFile(string filename)
    //   {
    //       WorkspaceViewModel.Instance.Open(filename);
    //   }

        private static void LoadOpenFiles()
        {
            var s = Settings.Default.OpenDocuments.Split(';');
          
            for (var i = 0; i < s.Length - 1; i++)
            {
                if (File.Exists(s[i]))
                {
                    Messenger.Default.Send<OpenFileMessage>(new OpenFileMessage(s[i]));
//                    OpenFile(s[i]);
                }

            }
        }

        /// <summary>
        /// Open file from parameters sent to program
        /// </summary>
        private void ProcessArgs()
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 1; i < args.Length; i++)
            {
                Messenger.Default.Send<OpenFileMessage>(new OpenFileMessage(args[i]));

//                OpenFile(args[i]);
            }
        }

        [Localizable(false)]
        private void DropFiles(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var t in files)
            {
                //TODO This Magically Works
                MessageViewModel.Instance.Add(Findahome.FileDropped, String.Format(Findahome.OpeningFile, t), MsgIcon.Info);
                WorkspaceViewModel.Instance.Open(t);
            }
        }

        // ReSharper disable InconsistentNaming
        public void onDragEnter(object sender, DragEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
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
                    var t = new System.Windows.Forms.Timer { Interval = Math.Max(1, delayMilliseconds) };
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
            Settings.Default.OpenDocuments = String.Empty;

            var docpane = DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            if (docpane != null)
                foreach (var d in docpane.Children.Select(doc => doc.Content as DocumentViewModel).Where(d => d != null && d.FilePath != null))
                {
                    Settings.Default.OpenDocuments += d.FilePath + ';';
                }
            Settings.Default.Save();

            SaveLayout();

            WorkspaceViewModel.Instance.IsClosing = true;          
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //   LoadLayout();
            LoadItems();
            Splasher.CloseSplash();
            Console.WriteLine("Loaded Items");
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
            try
            {
                var serializer = new XmlLayoutSerializer(DockManager);
                if (File.Exists(Global.DockConfig))
                    using (new StreamReader(Global.DockConfig)) serializer.Deserialize(Global.DockConfig);

            }
            catch(Exception ex)
            {

            }
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

        private void GetFileNames(GetFileMessage msg)
        {
            msg.GetFile(this);
            msg.Callback(msg);
        }
    }
}