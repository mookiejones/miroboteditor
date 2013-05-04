using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using miRobotEditor.Commands;
using miRobotEditor.Forms;
using miRobotEditor.GUI;
using miRobotEditor.Languages;
using miRobotEditor.Pads;
using miRobotEditor.Pads.Shift;
using miRobotEditor.Properties;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

using miRobotEditor.ViewModel;
using miRobotEditor.Interfaces;
using System.Reflection;
using System.Collections.ObjectModel;
namespace miRobotEditor	
{
    using Classes;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class MainWindow
    {
        public static MainWindow Instance { get; set; }


        public DockingManager Dock { get { return dockManager; } set { dockManager = value; } }

        public void LoadItems()
        {
            //Load Files that were closed with the window the last time the Program was executed
            LoadOpenFiles();

            //If No open files, Open one
            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane.ChildrenCount == 0)
                OpenFile(String.Empty);

            ProcessArgs();

        }

        void OpenFile(string filename)
        {
            Workspace.Instance.OpenFile(filename);
        }

        private void LoadOpenFiles()
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
        private void ProcessArgs()
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 1; i < args.Length; i++)
            {
                OpenFile(args[i]);
            }
        }


        private void OpenFunction(object sender, MouseButtonEventArgs e)
        {

            var i = (IVariable)((System.Windows.Controls.ListViewItem)sender).Content;
            DummyDoc.Instance.TextBox.SelectText(i);
        }


        [Localizable(false)]
        private void DropFiles(object sender, System.Windows.DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

            foreach (var t in files)
            {
                MessageViewModel.Instance.Add("File Dropped", String.Format("Opening:={0}", t), MSGIcon.INFO);
                Workspace.Instance.OpenFile(t);
            }
        }

        // ReSharper disable InconsistentNaming
        public void onDragEnter(object sender, System.Windows.DragEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)) e.Effects = System.Windows.DragDropEffects.Copy;
        }

        public MainWindow()
        {
            Instance = this;        
            InitializeComponent();
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
            
            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            
            if (docpane != null)
                foreach(var doc in docpane.Children)
                {
                    var d = doc.Content as DummyDoc;
                    if (d.Filename!=null)
                    Settings.Default.OpenDocuments += d.Filename + ';';                   
                }
            Settings.Default.Save();

            SaveLayout();

            Workspace.Instance.IsClosing = true;
            miRobotEditor.App._application.Shutdown();
        }


        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadItems();
        }

        private void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(Global.DockConfig))
                serializer.Serialize(stream);

            serializer = null;
        }


        public void CloseWindow(object param)
        {
            var ad = param as DummyDoc;
            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane == null) return;
			
            foreach (var c in docpane.Children.Where(c => c.Content.Equals(ad)))
            {
                docpane.Children.Remove(c);
                return;
            } 
        }
        private void dockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {
//            DummyDoc active = (sender as DockingManager).ActiveContent;
  //          if (active.Source.IsModified)

            
        }

       
    }
}
