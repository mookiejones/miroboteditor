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
using miRobotEditor.GUI.Notes;
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
using MahApps.Metro.Controls;
namespace miRobotEditor	
{
    using Classes;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Localizable(false)]
    public partial class MainWindow2 : MetroWindow,  INotifyPropertyChanged ,IDialogProvider
    {
        public static readonly RoutedCommand ImportCommand = new RoutedCommand("ImportCommand",typeof(MainWindow));
        
        public static readonly DependencyProperty ActiveDocumentProperty=DependencyProperty.Register("ActiveDocument",typeof(DummyDoc),typeof(MainWindow2),new PropertyMetadata(new DummyDoc()));
        public DummyDoc ActiveDocument {get{return (DummyDoc)GetValue(ActiveDocumentProperty);}set{SetValue(ActiveDocumentProperty,value);}}
        
        public static MainWindow2 Instance { get; set; }

        public static readonly DependencyProperty TextOptionProperty = DependencyProperty.Register("TextOptions",
                                                                                                   typeof ( TextEditorOptions
                                                                                                       ),
                                                                                                   typeof (MainWindow));
        #region DependencyProperties
  
        public TextEditorOptions TextOptions
        {
            get { return TextEditorOptions.Instance; }
            set { TextEditorOptions.Instance=value;
            }
        }

      //  private DummyDoc _activeeditor = new DummyDoc();
     //   public DummyDoc _activeEditor { get { return _activeeditor; } set { _activeeditor = value;OnPropertyChanged("_activeEditor"); } }
        
        #endregion


        public bool IsClosing { get; set; }

        #region Commands


        
        


     
        private static RelayCommand _openFileCommand;

        public static ICommand OpenFileCommand
        {
            get { return _openFileCommand ?? (_openFileCommand = new RelayCommand(param => Instance.OpenFile(), param => true)); }
        }
        private static RelayCommand _exitCommand;

        public static ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(param => Instance.Close(), param => true)); }
        }

        private static RelayCommand _changeViewAsCommand;

        public static ICommand ChangeViewAsCommand
        {
            get { return _changeViewAsCommand ?? (_changeViewAsCommand = new RelayCommand(param => Instance.ChangeViewAs(param), param => true)); }
        }
       
        private static RelayCommand _addToolCommand;

        public static ICommand AddToolCommand
        {
            get { return _addToolCommand ?? (_addToolCommand = new RelayCommand(param => Instance.AddTool(param), param => true)); }
        }

        #endregion
        void ButtonMinimiseOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void ButtonMaxRestoreOnClick(object sender, RoutedEventArgs e)
        {
            ToggleMaximized();
        }
        void ToggleMaximized()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        public MainWindow2()
        {
        	Instance=this;
            
            InitializeComponent();


           // if (Settings.Default.CheckForUpdates)
            //    new FrmUpdateChecker();
            
            OpenFile(String.Empty);
          
            // Load Files That were open on Closing
            LoadOpenFiles();

       //     LoadLayout();
            ProcessArgs();

            //RecentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);
         //   DataContext = new MainViewModel(this);
              DataContext = this;
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
        	var args = Environment.GetCommandLineArgs();
        	
        	for (var i = 1; i < args.Length;i++)
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
                              FilterIndex = Settings.Default.Filter
                          };
            // Set filter for file extension and default file extension

            var dir = Path.GetDirectoryName(ActiveDocument.Filename);
            

               
            ofd.InitialDirectory = dir != null && Directory.Exists(dir)?dir:String.Empty;

            // Display OpenFileDialog by calling ShowDialog method
            var result = ofd.ShowDialog();

            Settings.Default.Filter = ofd.FilterIndex;
           
            // Get the selected file name and display in a TextBox
            return result==true ? ofd.FileNames : new[]{String.Empty};
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

            IsClosing = true;
            miRobotEditor.App._application.Shutdown();
        }


        private void LoadOpenFiles()
        {
            var s = Settings.Default.OpenDocuments.Split(';');
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
       
        private void ChangeViewAs(object param)
        {
         
        	var language = ActiveDocument.FileLanguage;
        	
                switch (param.ToString())
                {
                    case "ABB":
                        language = language as ABB;
                        break;
                    case "KUKA":
                        language = new KUKA();
                        break;
                    case "Fanuc":
                        language = language as Fanuc;
                        break;
                    case "Kawasaki":
                        language = language as Kawasaki;
                        break;
                }
                DummyDocViewModel.Instance.TextBox.UpdateVisualText();
        }
      

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            SendMessage("Application Started",System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString());
        }

        #region Open _file 

        public void OpenFile(IVariable variable)
        {
        	//TODO Verify this. This opens from the Local Variable Window
            DummyDoc doc = OpenFile(variable.Path);
            
         //   var vm = doc.DataContext as DummyDocViewModel;
      //      vm.SelectText(variable);
            
            //TODO Need to make sure that when a program is loaded, the correct TextEditor is invoked to insure that we dont get errors

            //TODO Check to see if this works correctly
            
           DummyDoc.Instance.SelectText(variable);
           DummyDoc.Instance.TextBox.SelectText(variable);
            
            
          //  doc.TextBox.SelectText(variable.Offset, variable.Name);
        }

        [Localizable(false)]
        public DummyDoc OpenFile(string filename)
        {

            // Check to see if file exists before creating new document
            if (!String.IsNullOrEmpty(filename))
                if (!File.Exists(filename))
                    return null;


            var f = Path.GetFileNameWithoutExtension(filename);

            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
        
           
            if (docpane == null) return null;
            // If _file is open in another window, then select the file.
            if (File.Exists(filename))
            foreach (var t in from t in docpane.Children let edit = t.Content as DummyDoc where edit != null && edit.Filename != null where f  == t.ContentId select t)
            {
                t.IsActive = true;
                SendMessage("File allready Opened", filename,false);
                return t.Content as DummyDoc;
            }

            var document = new DummyDoc();
                
            var doc = new LayoutDocument { Description = filename,Content=document };
            document.Host = doc;
            docpane.Children.Add(doc);
                

            if (File.Exists(filename))
            {

                    doc.ContentId = f;
                    document.Load(filename);

                    // Can Throw an Error for $Config.Dat because it doesnt like the character
                    try
                    {
                        document.Name = doc.ContentId;
                    }
                    catch (Exception ex)
                    {
                        MessageViewModel.Instance.AddError(ex);
                    }
                    
//                    document.Host = doc;
                
                    doc.Title = f;
                    doc.Description = filename;
                    doc.IconSource = document.FileLanguage.GetFile(filename).Icon;
                    doc.ToolTip = filename;
                
                // Add file to Recent list
               RecentFileList.Instance.InsertFile(filename);

                if (filename != null)
                {
                    System.Windows.Shell.JumpList.AddToRecentCategory(filename);
                    //SetTitle(filename);
                }
            }
            else
                doc.Title = "Document" + docpane.Children.Count;

            doc.IsActive = true;

            DummyDoc.Instance = document;
            ActiveDocument = document;
            //        doc.IsSelectedChanged += doc_IsSelectedChanged;
//            DummyDoc.Instance.TextUpdated += UpdateFunctions;
            SendMessage("File Opened",filename);
            return DummyDoc.Instance;
        }

        public void AddNewFile()
        {
            OpenFile(string.Empty);
        }

        void OpenFile()
        {
            var fn = GetFileName();
            foreach (var f in fn)
            {
                if (File.Exists(f))
                    OpenFile(f);
            }
        }

        #endregion

        public void ShowOptions() 
        {
        	new Options.OptionsWindow().Show();
        	
        }

        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            SendMessage("Not Implemented", "Need to add Document Closing");
        }

        [Localizable(false)]
        private void SendMessage(string title, string description, bool forceactivate = true)
        {
            var img = (BitmapImage)Application.Current.Resources.MergedDictionaries[0]["error"];          
            MessageWindow.Add(title,description,img,forceactivate);
        }

        #region AvalonDock.DockingManager
// ReSharper disable UnusedMember.Local
        private void LoadLayout()
// ReSharper restore UnusedMember.Local
        {
          var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();
          
          var serializer = new XmlLayoutSerializer(dockManager);
          
          serializer.LayoutSerializationCallback += (s, args) =>
             {
                 var prevContent = currentContentsList.FirstOrDefault(c => c.ContentId == args.Model.ContentId);
                 if (prevContent != null)
                     args.Content = prevContent.Content;
             };

            if (File.Exists(Global.DockConfig))
                using (var stream = new StreamReader(Global.DockConfig))
              serializer.Deserialize(stream);
        } 

        private void SaveLayout()
        {           
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(Global.DockConfig))
                serializer.Serialize(stream);
            
            serializer = null;
        }
        #endregion

       
        [Localizable(false)]
        private void AddTool(object parameter)
        {
            var name = parameter as string;

            var tool = new LayoutAnchorable { Title = name, CanClose = false, CanFloat = true };

            switch (name)
            {
                case "Angle Converter":
                    tool.Content = new GUI.AngleConverter.AngleConverterWpf();
                    tool.AutoHideMinWidth = 219;
                    break;
                case "Functions":
                    tool.Content = new FunctionWindow();
                    tool.AutoHideMinWidth = 300;
                    break;
                case "Explorer":
                    tool.Content = new GUI.ExplorerControl.FileExplorerWindow();
                    break;
                case "Object Browser":                    
                    tool.Content = new ObjectBrowserWindow();
                    break;
                case "Output Window":
                    tool.Content = new MessageWindow();
                    break;
                case "Notes":
                    tool.Content = new NotesWindowTemplate();
                    break;
                case "ArchiveInfo":
                    tool.Content= new ArchiveInfo();
                    break;
                case "Rename Positions":
                    tool.Content = new Language_Specific.RenamePositionWindow();
                    break;
                case "Shift":
                    tool.Content = new ShiftWindow();
                    break;
                case "CleanDat":
                    tool.Content = new Language_Specific.DatCleanControl();                    
                    tool.AutoHideMinWidth = Language_Specific.DatCleanControl.Instance.Width;                    
                    break;
                default:
                    MessageViewModel.Instance.Add("Not Implemented", name, null);
                    break;
            }

            // Does Content Exist Allready?
            foreach (var dd in dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Where(dd => dd.Title == tool.Title))
            {
                dd.IsActive = true;
                return;
            }
            var group = new LayoutAnchorGroup();
            group.Children.Add(tool);
            dockManager.Layout.RightSide.Children.Add(group);
           
        }

        private void OnActiveContentChanged(object sender, EventArgs e)
        {
            
            var parent = ((DockingManager) sender).ActiveContent;

            if (!(parent is DummyDoc))
                return;
                DummyDoc.Instance = parent as DummyDoc;
                ActiveDocument = parent as DummyDoc;
                DummyDocViewModel.Instance = DummyDoc.Instance.DataContext as DummyDocViewModel;
                ProjectExplorerViewModel.Instance.GetNearByFiles(DummyDoc.Instance.Filename);
                
            if ((DummyDoc.Instance.TextBox!=null)&&(DummyDoc.Instance.TextBox.Filename!=null))
            {
               // SetTitle(DummyDoc.Instance.TextBox.Filename);
                //TODO UpdateFunctions
                //UpdateFunctions(DummyDoc.Instance.TextBox, new FunctionEventArgs(DummyDoc.Instance.TextBox.Text));
            }            
        }
        


        [Localizable(false)]
        void SetTitle(string name)
        {
        	var AppTitle = Application.ResourceAssembly.GetName().Name;        	
        	Title = String.IsNullOrEmpty(name)? AppTitle:String.Format("{0} := {1}", AppTitle, name );
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
               MainWindow.Instance.BringToFront("Object Browser");
            AddTool("ArchiveInfo");
        }

        [Localizable(false)]
        private void DropFiles(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
         
            foreach (var t in files)
            {
                MessageViewModel.Instance.Add("File Dropped", String.Format("Opening:={0}", t), null);
                OpenFile(t);
            }           
        }

// ReSharper disable InconsistentNaming
        public void onDragEnter(object sender, DragEventArgs e)
// ReSharper restore InconsistentNaming
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;

        }

        public void CloseWindow()
        {
            var docpane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane == null) return;
			
            foreach (var c in docpane.Children.Where(c => c.Content.Equals(DummyDoc.Instance)))
            {
                docpane.Children.Remove(c);
                return;
            } 
        }
     
        public void BringToFront(string windowname)
        {
        	   // Does Content Exist Allready?
        	   foreach (var dd in dockManager.Layout.Descendents().OfType<LayoutAnchorable>())
        	   {
        	   	if (dd.Title == windowname)
        	   		dd.IsActive=true;
        	   	Console.WriteLine(dd.Title);
        	   }

        }

     
    
        protected virtual void OnPropertyChanged(string propertyName)
        {
        	if (PropertyChanged!=null)
        		PropertyChanged(this,new PropertyChangedEventArgs(propertyName));

        }
        
		public void CleanDat(object sender, RoutedEventArgs e)
		{
		//	Language_Specific.DatCleanControl dcc = new miRobotEditor.Language_Specific.DatCleanControl();
		//	dcc.ShowDialog();
		//	Output.Add("Add","Need to Put Clean Dat to Command");
		}
    	
		public event PropertyChangedEventHandler PropertyChanged;
		public void ShowAbout()
        {
            new AboutWindow().ShowDialog();
        }

        private void TryClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
		void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		
    	
		#region IDialogProvider Interface
		  /// <summary>
        /// This method allows the user to select a file to open 
        /// (so the view-model can implement 'Open File' functionality).
        /// </summary>
        public bool UserSelectsFileToOpen(out string filePath)
        {
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                filePath = openFileDialog.FileName;
                return true;
            }
            else
            {
                filePath = null;
                return false;
            }
        }

        /// <summary>
        /// This method allows the user to select a new filename for an existing file 
        /// (so the view-model can implement 'Save As' functionality).
        /// </summary>
        public bool UserSelectsNewFilePath(string oldFilePath, out string newFilePath)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = this.ViewModel.ActiveDocument.Filename;

            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                newFilePath = saveFileDialog.FileName;
                return true;
            }
            else
            {
                newFilePath = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Display an error message dialog box.
        /// This allows the view-model to display error messages.
        /// </summary>
        public void ErrorMessage(string msg)
        {
            MessageBox.Show(this, msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Allow the user to confirm whether they want to close a modified document.
        /// </summary>
        public bool QueryCloseModifiedDocument(DummyDocViewModel document)
        {
            string msg = document.Filename + " has been modified but not saved.\n" +
                         "Do you really want to close it?";
            var result = MessageBox.Show(this, msg, "File modified but not saved", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Allow the user to confirm whether they want to close the application 
        /// when 1 or more documents are modified.
        /// </summary>
        public bool QueryCloseApplicationWhenDocumentsModified()
        {
            string msg = "1 or more open files have been modified but not saved.\n" +
                         "Do you really want to exit?";
            var result = MessageBox.Show(this, msg, "File(s) modified but not saved", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        private MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)this.DataContext;
            }
        }

        /// <summary>
        /// Event raised when the 'NewFile' command is executed.
        /// </summary>
        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.NewFile();
        }

        /// <summary>
        /// Event raised when the 'OpenFile' command is executed.
        /// </summary>
        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.OpenFile();
        }

        /// <summary>
        /// Event raised when the 'SaveFile' command is executed.
        /// </summary>
        private void SaveFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveFile();
        }

        /// <summary>
        /// Event raised when the 'SaveFileAs' command is executed.
        /// </summary>
        private void SaveFileAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveFileAs();
        }

        /// <summary>
        /// Event raised when the 'SaveAllFiles' command is executed.
        /// </summary>
        private void SaveAllFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveAllFiles();
        }

        /// <summary>
        /// Event raised when the 'CloseFile' command is executed.
        /// </summary>
        private void CloseFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.CloseFile();
        }

        /// <summary>
        /// Event raised when the 'CloseAllFiles' command is executed.
        /// </summary>
        private void CloseAllFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.CloseAllFiles();
        }

        /// <summary>
        /// Event raised when the 'ShowAllPanes' command is executed.
        /// </summary>
        private void ShowAllPanes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.ShowAllPanes();
        }

        /// <summary>
        /// Event raised when the 'HideAllPanes' command is executed.
        /// </summary>
        private void HideAllPanes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.HideAllPanes();
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event raised when AvalonDock has loaded.
        /// </summary>
        private void avalonDockHost_AvalonDockLoaded(object sender, EventArgs e)
        {
        	
        	throw new NotImplementedException();
        // if (System.IO.File.Exists(LayoutFileName))
        // {
        //     //
        //     // If there is already a saved layout file, restore AvalonDock layout from it.
        //     //
        //     avalonDockHost.DockingManager.RestoreLayout(LayoutFileName);
        // }
        // else
        // {
        //     //
        //     // This line of code can be uncommented to get a list of resources.
        //     //
        //     //string[] names = this.GetType().Assembly.GetManifestResourceNames();
		//
        //     //
        //     // Load the default AvalonDock layout from an embedded resource.
        //     //
        //     var assembly = Assembly.GetExecutingAssembly();
        //     using (var stream = assembly.GetManifestResourceStream(DefaultLayoutResourceName))
        //     {
        //         avalonDockHost.DockingManager.RestoreLayout(stream);
        //     }
        // }
        }

        /// <summary>
        /// Event raised when a document is being closed by clicking the 'X' button in AvalonDock.
        /// </summary>
        private void avalonDockHost_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
        	var document = (miRobotEditor.ViewModel.DummyDocViewModel)e.Document;
            if (!this.ViewModel.QueryCanCloseFile(document))
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Event raised when the window is about to close.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //
            // Notify the view-model that the application is closing,
            // allows the view-model the chance to cancel application exit.
            //
            if (!this.ViewModel.OnApplicationClosing())
            {
                //
                // The view-model has cancelled application exit.
                // This will happen when the 1 or more documents have been modified but not saved
                // and the user has selected 'No' when asked to confirm application exit.
                //
                e.Cancel = true;
                return;
            }

            //
            // When the window is closing, save AvalonDock layout to a file.
            //
            
            throw new NotImplementedException();
         //   avalonDockHost.DockingManager.SaveLayout(LayoutFileName);
        }
        
		
		#endregion
    }
}
