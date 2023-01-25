using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using AvalonDock.Layout;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using miRobotEditor.Classes;
using miRobotEditor.Controls;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Messages;
using miRobotEditor.Properties;
using miRobotEditor.ViewModel;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace miRobotEditor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        public static MainWindow Instance { get; set; }

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            KeyDown += (s, e) => StatusBarViewModel.Instance.ManageKeys(s, e);
        }

        #endregion Constructor

        private void LoadItems()
        {
            AppCommands.LoadOpenFiles();
            LayoutDocumentPane layoutDocumentPane =
                DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();

            if (layoutDocumentPane != null && layoutDocumentPane.ChildrenCount == 0)
            {
                MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
                instance.AddNewFile();
            }
            AppCommands.ProcessArgs();
        }







        [Localizable(false)]
        private void DropFiles(object sender, DragEventArgs e)
        {
            string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (WindowMessage msg in array.Select(text => new WindowMessage("File Dropped", text, MessageType.Information)))
            {
                _ = WeakReferenceMessenger.Default.Send(msg);
            }
        }

        private void onDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        public static void SafeThreadAsyncCall(Action method)
        {
        }

        public static void CallLater(TimeSpan delay, Action method)
        {
            int delayMilliseconds = (int)delay.TotalMilliseconds;
            if (delayMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("delay", delay, Properties.Resources.ValueMustBePositive);
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            SafeThreadAsyncCall(delegate
            {
                Timer t = new()
                {
                    Interval = Math.Max(1, delayMilliseconds)
                };
                t.Tick += delegate
                {
                    t.Stop();
                    t.Dispose();
                    method();
                };
                t.Start();
            });
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.OpenDocuments = string.Empty;
            LayoutDocumentPane layoutDocumentPane =
                DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
            if (layoutDocumentPane != null)
            {
                foreach (DocumentViewModel current in
                    from doc in layoutDocumentPane.Children
                    select doc.Content as DocumentViewModel
                    into d
                    where d != null && d.FilePath != null
                    select d)
                {
                    Settings settings = Settings.Default;

                    settings.OpenDocuments = settings.OpenDocuments + current.FilePath + ';';
                }
            }
            Settings.Default.Save();

            DockManager.SaveLayout();

            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.IsClosing = true;
            App.Application?.Shutdown();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadItems();
            Splasher.CloseSplash();
            DockManager.LoadLayout();

            WindowMessage msg = new("Application Loaded", "Application Loaded", MessageType.Information);
            _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
        }



        public void CloseWindow(object param)
        {
            IEditorDocument ad = param as IEditorDocument;
            LayoutDocumentPane layoutDocumentPane =
                DockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault<LayoutDocumentPane>();
            if (layoutDocumentPane != null)
            {
                using (System.Collections.Generic.IEnumerator<LayoutContent> enumerator = (
                    from c in layoutDocumentPane.Children
                    where c.Content.Equals(ad)
                    select c).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        LayoutContent current = enumerator.Current;
                        _ = layoutDocumentPane.Children.Remove(current);
                    }
                }
            }
        }
    }
}