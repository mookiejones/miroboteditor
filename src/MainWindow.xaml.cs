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
using miRobotEditor.Controls;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Languages;
using Application = System.Windows.Application;
using MenuItem = System.Windows.Controls.MenuItem;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace miRobotEditor	
{
    using Classes;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class MainWindow : Window,INotifyPropertyChanged 
    {
        public static readonly RoutedCommand ImportCommand = new RoutedCommand("ImportCommand",typeof(MainWindow));
        public static MainWindow Instance { get; set; }
	
        public MainWindow()
        {
            InitializeComponent();
            // Check for updates
           // var checker = new Forms.FrmUpdateChecker();

            //Get Initial Key Status
            ManageKeys(null,null);

            // Load Files That were open on Closing
            LoadOpenFiles();

       //     LoadLayout();
            ProcessArgs();

            RecentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);
            DataContext = this;
            Instance = this;
        }
        
        
       
        
        /// <summary>
        /// Makes a call GUI threadsafe without waiting for the returned value.
        /// </summary>
        public static void SafeThreadAsyncCall(Action method)
        {
     //       SynchronizingObject.BeginInvoke(method, emptyObjectArray);
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
        public void LoadFile(IList<string> args)
        {
        	for (var i = 1; i < args.Count;i++)
        	{ 
        		OpenFile(args[i]);
        	}   
        }

        private void ProcessArgs()
        {
        	string[] args = Environment.GetCommandLineArgs();
        	
        	for (int i = 1; i < args.Length;i++)
        	{
        		OpenFile(args[i]);
        	}        	
        }

        

        [Localizable(false)]
        private IEnumerable<string> GetFileName()
        {
            var ofd = new OpenFileDialog
                          {
                              DefaultExt = ".txt",
                              Filter =
                                  "All _files (*.*)|*.*|ABB _files (.mod,.prg,.cfg)|*.mod;*.prg;*.cfg|Kawasaki _files(*.as)|*.as|KUKA _files (.sub,.src,.dat,.kfd)|*.sub;*.src;*.dat;*.kfd|Fanuc _files (*.ls)|*.ls",
                              Multiselect = true,
                              FilterIndex = Properties.Settings.Default.Filter
                          };
            // Set filter for file extension and default file extension

            if (DummyDoc.Instance.File!=null)

                if (DummyDoc.Instance.File.Directory != null && DummyDoc.Instance.File.Directory.Exists)
            ofd.InitialDirectory = DummyDoc.Instance.File.DirectoryName;

            // Display OpenFileDialog by calling ShowDialog method
            var result = ofd.ShowDialog();

            Properties.Settings.Default.Filter = ofd.FilterIndex;
           
            // Get the selected file name and display in a TextBox
            return result==true ? ofd.FileNames : new string[]{String.Empty};
        }

        #region Function Window Section
        //TODO Add Function Items For ABB
        private void UpdateFunctions(object sender, FunctionEventArgs e)
        {
            
            if (sender is Editor)
            {
                var editor = sender as Editor; 
                
                if (DummyDoc.Instance != null)
                {
                    if ((editor.Parent.FileLanguage == null) | editor.Parent.FileLanguage is LanguageBase)
                        Functions.Clear();
                    else
                    {
                        Functions.Clear();
                        Functions.UpdateFunctions(e.Text, editor.Parent.FileLanguage.FunctionItems);
                    }

                }
            }
        }



        /// <summary>
        /// Function Window Clicked
        /// </summary>
        #endregion


        private void WindowClosing(object sender, CancelEventArgs e)
        {
            
            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            
            if (docpane != null)
                foreach (var doc in docpane.Children.Select(t => t.Content as DummyDoc).Where(doc => doc.File !=null))
                {
                    Properties.Settings.Default.OpenDocuments += doc.File.FullName + ';';
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

        private void ChangeViewAs(object sender, RoutedEventArgs e)
        {
            var selected = sender as MenuItem;

            if (selected != null)
                switch (selected.Header.ToString())
                {
                    case "ABB":
                        DummyDoc.Instance.FileLanguage = DummyDoc.Instance.FileLanguage as  ABB;
                        break;
                    case "KUKA":
                        DummyDoc.Instance.FileLanguage = new KUKA();
                        break;
                    case "Fanuc":
                        DummyDoc.Instance.FileLanguage = DummyDoc.Instance.FileLanguage as Fanuc;
                        break;
                    case "Kawasaki":
                        DummyDoc.Instance.FileLanguage = DummyDoc.Instance.FileLanguage as Kawasaki;
                        break;
                }
            DummyDoc.Instance.TextBox.UpdateVisualText();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            SendMessage("Application Started",String.Empty);
        }

        #region Open _file 

        public void OpenFile(IVariable variable)
        {
            var doc = OpenFile(variable.Path);
            
            doc.TextBox.SelectText(variable.Offset, variable.Name);
        }

        [Localizable(false)]
        public DummyDoc OpenFile(string filename)
        {

            // Check to see if file exists before creating new document
            if (!String.IsNullOrEmpty(filename))
                if (!File.Exists(filename))
                    return null;

            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            FileInfo file = null;
            if (!String.IsNullOrEmpty(filename))
             file = new FileInfo(filename);

            if (docpane == null) return null;
            // If _file is open in another window, then select the file.
            if (file!=null)            
            foreach (LayoutContent t in from t in docpane.Children let edit = t.Content as DummyDoc where edit != null && edit.File != null where file.FullName.Substring(0, file.FullName.Length - file.Extension.Length) == t.ContentId select t)
            {
                t.IsActive = true;
                SendMessage("File allready Opened", filename);
                return t.Content as DummyDoc;
            }

            var document = new DummyDoc();


                
            var doc = new LayoutDocument { Description = filename,Content=document };
            docpane.Children.Add(doc);
                
            if (File.Exists(filename))
            {
                if (file != null)
                {
                    doc.ContentId = file.FullName.Substring(0, file.FullName.Length - file.Extension.Length);
                    document.Load(file);

                    // Can Throw an Error for $Config.Dat because it doesnt like the character
                    try
                    {
                        document.Name = file.Name.Substring(0, file.Name.Length - file.Extension.Length);
                    }
                    catch
                    {
                    }
                }
                document.Host = doc;
                if (file != null)
                {
                    doc.Title = file.Name;
                    doc.Description = file.Name;
                    doc.IconSource = DummyDoc.Instance.FileLanguage.GetFile(file).Icon;
                }
                // Add file to Recent list
                RecentFileList.InsertFile(filename);
                if (filename != null)
                {
                    System.Windows.Shell.JumpList.AddToRecentCategory(filename);
                    SetTitle(filename);
                }
            }
            else
                doc.Title = "Document" + docpane.Children.Count;

            doc.IsActive = true;

            DummyDoc.Instance = document;
            //        doc.IsSelectedChanged += doc_IsSelectedChanged;
            DummyDoc.Instance.TextUpdated += UpdateFunctions;
            SendMessage("File Opened",filename);
            return DummyDoc.Instance;
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

        [Localizable(false)]
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

        [Localizable(false)]
        private void AddTool(string name)
        {

            var tool = new LayoutAnchorable { Title = name, CanClose = false, CanFloat = true };

            switch (name)
            {
                case "Angle Converter":
                    tool.Content = new GUI.AngleConverter.AngleConverterWPF { VerticalContentAlignment = VerticalAlignment.Top, MaxWidth = 250 };
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
                    tool.Content = new GUI.ObjectBrowser.ObjectBrowserTool();
                    break;
                case "Output Window":
                    tool.Content = new GUI.OutputWindow.OutputWindow();
                    break;
                case "Notes":
                    tool.Content = new frmNotes();
                    break;
                case "ArchiveInfo":
                    tool.Content= new Pads.ArchiveInfo();
                    break;
                default:
                    OutputMessages.Add("Not Implemented",name,null);
                    break;
            }

            // Does Content Exist Allready?
            foreach (LayoutAnchorable dd in dockManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                if (dd.Title == tool.Title)
                {
                    dd.IsActive = true;
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
            if (item != null) AddTool(item.Header.ToString());
        }

        private void SplitWindow(object sender, RoutedEventArgs e)
        {

        }

        private void OnActiveContentChanged(object sender, EventArgs e)
        {
            
            var parent = ((DockingManager) sender).ActiveContent;

            if (!(parent is DummyDoc))
                return;
                DummyDoc.Instance = parent as DummyDoc;

            if ((DummyDoc.Instance.TextBox!=null)&&(DummyDoc.Instance.TextBox.File!=null))
                SetTitle(DummyDoc.Instance.TextBox.File.FullName);

          
            if (DummyDoc.Instance.TextBox != null)
                UpdateFunctions(DummyDoc.Instance.TextBox, new FunctionEventArgs(DummyDoc.Instance.TextBox.Text));

            
        }


        [Localizable(false)]
        void SetTitle(string name)
        {
            try
            {
                var s = Application.ResourceAssembly.GetName().Name;

                Title = String.Format("{0} := {1}", s, name );
            }
            catch (Exception)
            {
                Title = Application.ResourceAssembly.GetName().Name;
            }
        }
        
        
        private void CanImport(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (!(DummyDoc.Instance.FileLanguage is LanguageBase) | (DummyDoc.Instance.FileLanguage is Fanuc) |
                            (DummyDoc.Instance.FileLanguage is Kawasaki) | DummyDoc.Instance.FileLanguage == null);
        }

        private void ImportRobot(object sender, ExecutedRoutedEventArgs e)
        {
            objectBrowser.ShowWizard("Backup Folder");           
            AddTool("Object Browser");
            AddTool("ArchiveInfo");
        }

        [Localizable(false)]
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
            e.CanExecute = DummyDoc.Instance != null;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
             var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();           
            if (docpane != null)
            {
                foreach (LayoutContent c in docpane.Children)
                {
                    if (c.Content.Equals(DummyDoc.Instance))
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

    
        protected virtual void OnPropertyChanged(string propertyName)
        {
        	if (PropertyChanged!=null)
        		PropertyChanged(this,new PropertyChangedEventArgs(propertyName));

        }
        
		void CleanDat(object sender, RoutedEventArgs e)
		{
			Language_Specific.DatCleanControl dcc = new miRobotEditor.Language_Specific.DatCleanControl();
			dcc.ShowDialog();
			Output.Add("Add","Need to Put Clean Dat to Command");
		}
    	
		public event PropertyChangedEventHandler PropertyChanged;
    }
}
