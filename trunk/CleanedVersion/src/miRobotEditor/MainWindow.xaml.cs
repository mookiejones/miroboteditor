using System;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using miRobotEditor.Core.Classes.Messaging;
using miRobotEditor.Core.Classes.Messaging.Interfaces;
using miRobotEditor.Resources;
using miRobotEditor.Core;
using miRobotEditor.Core;
using miRobotEditor.ViewModel;

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

    }
}