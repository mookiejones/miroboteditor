using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using miRobotEditor.Controls;
using miRobotEditor.Robot;
using Application = System.Windows.Application;
using MenuItem = System.Windows.Controls.MenuItem;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace miRobotEditor	
{
    using Classes;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged 
    {

        public static readonly RoutedCommand ImportCommand = new RoutedCommand();
     
        public MainWindow()
        {
            InitializeComponent();

            
            // Check for updates
           // var checker = new Forms.FrmUpdateChecker();

            //Get Initial Key Status
            ManageKeys(null,null);

            // Load Files That were open on Closing
            LoadOpenFiles();

      //      LoadLayout();
            ProcessArgs();

            RecentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);
            DataContext = this;
        }

        public void LoadFile(System.Collections.Generic.IList<string> args)
        {
        	for (var i = 1; i < args.Count;i++)
        	{ 
        		this.OpenFile(args[i]);
        	}   
        }

        private void ProcessArgs()
        {
        	string[] args = Environment.GetCommandLineArgs();
        	
        	for (int i = 1; i < args.Length;i++)
        	{
        		this.OpenFile(args[i]);
        	}        	
        }
        private void SwitchRobotMenu()
        {
            mnuRobot.Items.Clear();
            if (DummyDoc.ActiveEditor.Robot != null)
                switch (DummyDoc.ActiveEditor.Robot.RobotType)
            {
                case  Enums.TYPROBOT.KUKA:
                    mnuRobot = mnuKUKA;
                    mnuRobot.Items.Add(Robot.KUKA.MenuItems);
                    break;
                case  Enums.TYPROBOT.ABB:
                    mnuRobot.Visibility = System.Windows.Visibility.Visible;
                    mnuRobot.Visibility =Visibility.Visible;
                    mnuABB.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }
     
        private IEnumerable<string> GetFileName()
        {
            var ofd = new OpenFileDialog();
            // Set filter for file extension and default file extension
            ofd.DefaultExt = ".txt";
            ofd.Filter = "All _files (*.*)|*.*|ABB _files (.mod,.prg,.cfg)|*.mod;*.prg;*.cfg|Kawasaki _files(*.as)|*.as|KUKA _files (.sub,.src,.dat,.kfd)|*.sub;*.src;*.dat;*.kfd|Fanuc _files (*.ls)|*.ls";
            ofd.Multiselect = true;
            ofd.FilterIndex = Properties.Settings.Default.Filter;

            if (DummyDoc.ActiveEditor._file!=null)
            ofd.InitialDirectory = DummyDoc.ActiveEditor._file.DirectoryName;

            // Display OpenFileDialog by calling ShowDialog method
            var result = ofd.ShowDialog();

            Properties.Settings.Default.Filter = ofd.FilterIndex;
           
            // Get the selected file name and display in a TextBox
            return result==true ? ofd.FileNames : new string[]{String.Empty};
        }

        #region Function Window Section
        //TODO Add Function Items For ABB
        private void UpdateFunctions(object sender, Classes.FunctionEventArgs e)
        {
            
            if (sender is Editor)
            {
                var editor = sender as Editor; 
                
                if (DummyDoc.ActiveEditor != null)
                {

                    if ((DummyDoc.ActiveEditor.Robot == null) | DummyDoc.ActiveEditor.Robot is Robot.RobotBase)
                        Functions.Clear();
                    else
                    {
                        Functions.Clear();
                        Functions.UpdateFunctions(e.Text, DummyDoc.ActiveEditor.Robot.FunctionItems);
                    }

                }
            }
        }



        /// <summary>
        /// Function Window Clicked
        /// </summary>
        private void FunctionsFunctionClicked()
        {
            DummyDoc.ActiveEditor.Find();
        }
        #endregion


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            
            if (docpane != null)
                foreach (var doc in docpane.Children.Select(t => t.Content as DummyDoc).Where(doc => doc._file !=null))
                {
                    Properties.Settings.Default.OpenDocuments += doc._file.FullName + ';';
                }

            Properties.Settings.Default.Save();
            SaveLayout();
        }

        private void LoadOpenFiles()
        {
            var s = Properties.Settings.Default.OpenDocuments.Split(';');
            for (var i = 0; i<s.Length-1;i++) 
            {
                if (File.Exists(s[i]))
                    OpenFile(s[i]);
            }


        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void FunctionSelected(object sender, FunctionEventArgs e)
        {
            if (DummyDoc.ActiveEditor != null)
                DummyDoc.ActiveEditor.TextBox.SelectText(e.Text);
        }

        private void ChangeViewAs(object sender, RoutedEventArgs e)
        {
            var selected = sender as MenuItem;

            switch (selected.Header.ToString())
            {
                case "ABB":
                    DummyDoc.ActiveEditor.Robot = DummyDoc.ActiveEditor.Robot as  ABB;
                    break;
                case "KUKA":
                    DummyDoc.ActiveEditor.Robot = DummyDoc.ActiveEditor.Robot as KUKA;
                    break;
                case "Fanuc":
                    DummyDoc.ActiveEditor.Robot = DummyDoc.ActiveEditor.Robot as Fanuc;
                    break;
                case "Kawasaki":
                    DummyDoc.ActiveEditor.Robot = DummyDoc.ActiveEditor.Robot as Kawasaki;
                    break;
            }
            DummyDoc.ActiveEditor.TextBox.UpdateVisualText();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            SendMessage("Application Started",String.Empty);
        }

        #region Open _file 

        private void OpenMyFile(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (File.Exists(Explorer.tbSelectedPath.Text))
                OpenFile(Explorer.tbSelectedPath.Text);
        }

        private void OpenFile(string filename)
        {


            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            FileInfo file = null;
            if (!String.IsNullOrEmpty(filename))
             file = new FileInfo(filename);

            if (docpane == null) return;
            // If _file is open in another window, then select the file.
            foreach (LayoutContent t in from t in docpane.Children let edit = t.Content as DummyDoc where edit != null && edit._file != null where file.FullName.Substring(0, file.FullName.Length - file.Extension.Length) == t.ContentId select t)
            {
                t.IsActive = true;
                SendMessage("_file allready Opened", filename);
                return;
            }

            var document = new DummyDoc();


                
            var doc = new LayoutDocument { Description = filename,Content=document };
            docpane.Children.Add(doc);
                
            if (File.Exists(filename))
            {
                    
                doc.ContentId = file.FullName.Substring(0, file.FullName.Length - file.Extension.Length);
                document.Load(file);
                document.Name = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                doc.Title = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                doc.Description = file.Name;
                doc.IconSource = DummyDoc.ActiveEditor.Robot.GetFile(file).Icon;                    
                // Add file to Recent list
                RecentFileList.InsertFile(filename);
                System.Windows.Shell.JumpList.AddToRecentCategory(filename);
            }
            else
                doc.Title = "Document" + docpane.Children.Count;


            doc.IsActive = true;

            DummyDoc.ActiveEditor = document;
            //        doc.IsSelectedChanged += doc_IsSelectedChanged;
            DummyDoc.ActiveEditor.TextUpdated += UpdateFunctions;
            SendMessage("_file Opened",filename);
        }

        
        void doc_IsSelectedChanged(object sender, EventArgs e)
        {
        //   var doc= ((LayoutDocument) sender).Content as DummyDoc;
        //   if (doc == null) return;
        //   ActiveEditor = doc;
        //   SwitchRobotMenu();
        //
        //   if (doc.TextBox != null)
        //       UpdateFunctions(ActiveEditor.TextBox, new FunctionEventArgs(ActiveEditor.TextBox.Text));
        }

        private void AddNewFile(object sender, RoutedEventArgs e)
        {
            OpenFile(string.Empty);
        }


        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var fn = GetFileName();
            foreach (string f in fn)
            {
                if (File.Exists(f))
                    OpenFile(f);
            }
        }

        #endregion

        private void ShowOptions(object sender, RoutedEventArgs e)
        {

        }


        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            SendMessage("Not Implemented", "Need to add Document Closing");
        }

        private void SendMessage(string title, string description)
        {
            var img = (BitmapImage)Application.Current.Resources.MergedDictionaries[0]["error"];          
            Output.Add(img, title, description);
        }

        #region AvalonDock.DockingManager
        private void LoadLayout()
        {
          var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();
          
          var serializer = new XmlLayoutSerializer(dockManager);
          
          serializer.LayoutSerializationCallback += (s, args) =>
             {
                 var prevContent = currentContentsList.FirstOrDefault(c => c.ContentId == args.Model.ContentId);
                 if (prevContent != null)
                     args.Content = prevContent.Content;
             };

            if (File.Exists(Global.dockConfig))
                using (var stream = new StreamReader(Global.dockConfig))
              serializer.Deserialize(stream);
        } 

        private void SaveLayout()
        {           
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(Global.dockConfig))
                serializer.Serialize(stream);
        }
        #endregion

        private void AddTool(string name)
        {

            var tool = new LayoutAnchorable { Title = name, CanClose = false, CanFloat = true };

            switch (name)
            {
                case "Angle Converter":
                    tool.Content = new GUI.AngleConverter.frmAngleConverter() { VerticalContentAlignment = VerticalAlignment.Top, MaxWidth = 250 };
                    tool.AutoHideMinWidth = 219;
                    break;
                case "Functions":
                    tool.Content = new GUI.FunctionWindow.FunctionWindow();
                    tool.AutoHideMinWidth = 300;
                    break;
                case "Explorer":
                    tool.Content = new GUI.ExplorerWindow.ExplorerWindow();
                    break;
                case "Object Browser":                    
                    tool.Content = new GUI.ObjectBrowser.frmObjectBrowser();
                    break;
                case "Output Window":
                    tool.Content = new GUI.OutputWindow.OutputWindow();
                    break;
                case "Notes":
                    tool.Content = new miRobotEditor.Controls.frmNotes();
                    break;
                default:
                    OutputMessages.Messages.Add(new OutputWindowMessage().Add("Not Implemented", name, null));
                    break;
            }

            // Does Content Exist Allready?
            foreach (LayoutAnchorable dd in dockManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                if (dd.Title == tool.Title)
                {
                    dd.IsActive = true;
                    tool = null;
                    return;
                }
            }
            var group = new LayoutAnchorGroup();
            group.Children.Add(tool);
            dockManager.Layout.RightSide.Children.Add(group);
           
        }

        private void AddTool(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            AddTool(item.Header.ToString());

        }

        private void SplitWindow(object sender, RoutedEventArgs e)
        {

        }

        private void OnActiveContentChanged(object sender, EventArgs e)
        {
            
            var parent = ((DockingManager) sender).ActiveContent;
            
            if (parent is DummyDoc)
                DummyDoc.ActiveEditor = parent as DummyDoc;
            else                
                return;

            

            SwitchRobotMenu();

            if (DummyDoc.ActiveEditor.TextBox != null)
                UpdateFunctions(DummyDoc.ActiveEditor.TextBox, new FunctionEventArgs(DummyDoc.ActiveEditor.TextBox.Text));

            
        }
        private void CanImport(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (!(DummyDoc.ActiveEditor.Robot is RobotBase) | (DummyDoc.ActiveEditor.Robot is Fanuc) |
                            (DummyDoc.ActiveEditor.Robot is Kawasaki) | DummyDoc.ActiveEditor.Robot == null);
        }
        private void ImportRobot(object sender, ExecutedRoutedEventArgs e)
        {
            var backup = new BackupClass();
            backup.ShowWizard("Backup Folder");
            AddTool("Object Browser");
        }

        private void DropFiles(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
         
            foreach (string t in files)
            {
                Output.Add(null,"File Dropped", String.Format("Opening:={0}",t));
                OpenFile(t);
            }
           
        }

        private void onDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;

        }

        private void CanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DummyDoc.ActiveEditor != null;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
             var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();           
            if (docpane != null)
            {
                foreach (LayoutContent c in docpane.Children)
                {
                    if (c.Content.Equals(DummyDoc.ActiveEditor))
                    {
                        docpane.Children.Remove(c);
                        return;
                    }
                }

            }
        }



        #region Status Bar Items
        
        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int keyCode);

        #region Properties
        private bool _isScrollPressed;
        private bool _isNumPressed;
        private bool _isInsPressed;
        private bool _isCapsPressed;

        public bool IsScrollPressed
        {
            get { return _isScrollPressed; }
            set
            {
                _isScrollPressed = value;
                OnPropertyChanged("IsScrollPressed");
            }
        }
        public bool IsNumPressed
        {
            get { return _isNumPressed; }
            set
            {
                _isNumPressed = value;
                OnPropertyChanged("IsNumPressed");
            }
        }
        public bool IsInsPressed
        {
            get { return _isInsPressed; }
            set
            {
                _isInsPressed = value;
                OnPropertyChanged("IsInsPressed");
            }
        }
        public bool IsCapsPressed
        {
            get { return _isCapsPressed; }
            set
            {
                _isCapsPressed = value;
                OnPropertyChanged("IsCapsPressed");
            }
        }
        #endregion

        private void ManageKeys(object sender, KeyEventArgs e)
        {
            IsCapsPressed = GetKeyState((int) VKeyStates.CapsKey)!=0;
            IsInsPressed = GetKeyState((int)VKeyStates.InsKey) != 0;
            IsNumPressed = GetKeyState((int)VKeyStates.NumKey) != 0;
            IsScrollPressed = GetKeyState((int)VKeyStates.ScrollKey) != 0;
        }

        private enum VKeyStates
        {
            /// <summary>
            /// the caps lock key
            /// </summary>
            CapsKey = 0x14,
            /// <summary>
            /// the numlock key
            /// </summary>
            NumKey=0x90,
            /// <summary>
            /// the scroll key
            /// </summary>
            ScrollKey=0x91,
            /// <summary>
            /// the ins key
            /// </summary>
            InsKey=0x2d
        }

        #endregion

        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
