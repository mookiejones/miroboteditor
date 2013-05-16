using System;
using miRobotEditor.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using miRobotEditor.GUI;
using System.Windows.Input;
using miRobotEditor.Commands;
using System.IO;
using miRobotEditor.Classes;
using miRobotEditor.ViewModel;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using miRobotEditor.Properties;
using miRobotEditor.Languages;
using System.ComponentModel;
using System.Windows;
using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using miRobotEditor.Pads;
using AvalonDock;
namespace miRobotEditor
{
    public class Workspace:ViewModelBase
    {


        #region Constructor
        public Workspace()
        {
            Instance = this;
        }
        #endregion

        DockingManager Dock { get { return MainWindow.Instance.Dock; } set { MainWindow.Instance.Dock = value; } }

        private static Workspace _instance;
        public static Workspace Instance { get { return _instance ?? new Workspace(); } set { _instance = value; } }

        #region Properties
        private DummyDoc _activeEditor = new DummyDoc();
        public DummyDoc ActiveEditor { get { return _activeEditor; } set { _activeEditor = value; RaisePropertyChanged("ActiveEditor"); } }


        private bool _isClosing = false;
        public bool IsClosing { get { return _isClosing; } set { _isClosing = value; RaisePropertyChanged("IsClosing"); } }

        ObservableCollection<DummyDoc> _files = new ObservableCollection<DummyDoc>();
        ReadOnlyObservableCollection<DummyDoc> _readonyFiles = null;

        public ReadOnlyObservableCollection<DummyDoc> Files{get{return _readonyFiles ?? new ReadOnlyObservableCollection<DummyDoc>(_files);}}

        #endregion

        #region Commands

        private RelayCommand _newFileCommand;

        public ICommand NewFileCommand
        {
            get { return _newFileCommand ?? (_newFileCommand = new RelayCommand((p) => AddNewFile(), (p) => true)); }
        }

        private RelayCommand _closeWindowCommand;

        public ICommand CloseWindowCommand
        {
            get { return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand((p) => CloseWindow(p), (p) => true)); }
        }

        private RelayCommand _showOptionsCommand;

        public ICommand ShowOptionsCommand
        {
            get { return _showOptionsCommand ?? (_showOptionsCommand = new RelayCommand((p) => ShowOptions(), (p) => true)); }
        }


        private RelayCommand _showAboutCommand;

        public ICommand ShowAboutCommand
        {
            get { return _showAboutCommand ?? (_showAboutCommand = new RelayCommand((p) => ShowAbout(), (p) => true)); }
        }

        private RelayCommand _exitCommand;

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand((p) => Exit(),(p) => true)); }
        }

        private RelayCommand _importCommand;

        public ICommand ImportCommand
        {
            get { return _importCommand ?? (_importCommand = new RelayCommand((p) => ImportRobot(), (p) => (!(p is LanguageBase) | (p is Fanuc) | (p is Kawasaki) | p == null))); }
        }

        private RelayCommand _openFileCommand;

        public ICommand OpenFileCommand
        {
            get { return _openFileCommand ?? (_openFileCommand = new RelayCommand((p) => OnOpen(p), (p) => true)); }
        }

        private  RelayCommand _changeViewAsCommand;

        public  ICommand ChangeViewAsCommand
        {
            get { return _changeViewAsCommand ?? (_changeViewAsCommand = new RelayCommand(param => ChangeViewAs(param), param => true)); }
        }

        private  RelayCommand _addToolCommand;

        public  ICommand AddToolCommand
        {
            get { return _addToolCommand ?? (_addToolCommand = new RelayCommand(param => AddTool(param), param => true)); }
        }

        #endregion

        //TODO Do i Really need 8 different ways to open a program?

        #region OpenFile

        /// <summary>
        /// Open file from menu entry
        /// </summary>
        /// <param name="param"></param>
        public void OnOpen(object param)
        {
            DummyDoc doc = param != null ? param as DummyDoc : new DummyDoc();

            var fn = doc.Filename;

            var dir = doc.Filename != null ? Path.GetDirectoryName(fn) : null;
            var dlg = new OpenFileDialog
            {
                DefaultExt = File.Exists(fn) ? Path.GetExtension(fn) : "*.*",
                Filter = Properties.Resources.DefaultFilter,
                Multiselect = true,
                FilterIndex = Settings.Default.Filter,
                InitialDirectory = dir != null && Directory.Exists(dir) ? dir : String.Empty
            };

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                foreach (var f in dlg.FileNames)
                    doc = OpenFile(dlg.FileName);
            }
        }

    
        public void OpenFile(IVariable variable)
        {
            // Am i using dock or ActiveEditor?

            var doc = OpenFile(variable.Path);

            ActiveEditor.SelectText(variable);
//            ActiveEditor.TextBox.SelectText(variable);
        }

        [Localizable(false)]
        public DummyDoc OpenFile(string filename)
        {

            // Check to see if file exists before creating new document
            if (!String.IsNullOrEmpty(filename))
                if (!File.Exists(filename))
                    return null;

            var title = Path.GetFileNameWithoutExtension(filename);

            var docpane = Dock.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            var id = File.Exists(filename) ? Path.Combine(Path.GetDirectoryName(filename), title) : string.Empty;

            var dd = docpane.Children.FirstOrDefault(dm => dm.ContentId == id);
            if (dd != null)
            {
                dd.IsActive = true;
                return dd.Content as DummyDoc;
            }

            if (String.IsNullOrEmpty(id))
                id = String.Format("Document {0}", (docpane.Children.Count + 1));

            var document = new DummyDoc
            {
                Filename = id,
                FilePath = id,
                Visibility = Visibility.Visible
            };

            var doc = new LayoutDocument { Description = filename, Content = document, ContentId = id };
            docpane.Children.Add(doc);

            if (File.Exists(filename))
            {
                document.Load(filename);
                doc.Description = filename;
                doc.IconSource = document.FileLanguage.GetFile(filename).Icon;
                doc.ToolTip = filename;

                // Add file to Recent list
                RecentFileList.Instance.InsertFile(filename);
                System.Windows.Shell.JumpList.AddToRecentCategory(filename);
            }

            _files.Add(document);
            doc.Title = String.IsNullOrEmpty(document.Title)?id:document.Title;
            doc.IsActive = true;
            ActiveEditor = document;
            return document;
        }

        public void AddNewFile()
        {
            OpenFile(string.Empty);
        }


        public void LoadFile(IList<string> args)
        {
            for (var i = 1; i < args.Count; i++)
            {
                OpenFile(args[i]);
            }
        }



        #endregion

        //This can probably move to the language class section
        private void ChangeViewAs(object param)
        {

            var lang = param as AbstractLanguageClass;

            switch (param.ToString())
            {
                case "ABB":
                    lang = (ABB)lang;
                    //                        DummyDoc.Instance.FileLanguage = DummyDoc.Instance.FileLanguage as ABB;
                    break;
                case "KUKA":
                    lang = new KUKA();
                    //                       DummyDoc.Instance.FileLanguage = new KUKA();
                    break;
                case "Fanuc":
                    lang = (Fanuc)lang;
                    //                        DummyDoc.Instance.FileLanguage = DummyDoc.Instance.FileLanguage as Fanuc;
                    break;
                case "Kawasaki":
                    lang = (Kawasaki)lang;
                    //DummyDoc.Instance.FileLanguage = DummyDoc.Instance.FileLanguage as Kawasaki;
                    break;
            }

            //                ActiveEditor.TextBox.UpdateVisualText();

            //                DummyDocViewModel.Instance.TextBox.UpdateVisualText();
        }

        public void Exit() { MainWindow.Instance.Close(); }

        public void ShowOptions()
        {
            //TODO Need to Implement the ShowOptions
            new Options.OptionsWindow().ShowDialog();
              throw new NotImplementedException("ShowOptions");
        
        }

        private void OnDocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {
            MessageViewModel.Instance.Add("Not Implemented", "Need to add OnDocumentClosing", MSGIcon.ERROR);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
         //   SendMessage("Application Started", System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString());

            MessageViewModel.Instance.Add("Application Started", App.ProductName, MSGIcon.INFO, false);
        }


        [Localizable(false)]
        private void AddTool(object parameter)
        {
            var name = parameter as string;

            var tool = new LayoutAnchorable { Title = name, CanClose = false, CanFloat = true };

            switch (name)
            {
                case "Angle Converter":
                    tool.Content = new ViewModel.AngleConvertorViewModel();
                    tool.AutoHideMinWidth = 219;
                    break;
                case "Functions":
                    tool.Content = ActiveEditor.TextBox.Variables;
                    tool.AutoHideMinWidth = 300;
                    break;
                case "Explorer":
                    tool.Content = new GUI.ExplorerControl.FileExplorerWindow();
                    break;
                case "Object Browser":
                    tool.Content = new ObjectBrowserViewModel();
                    break;
                case "Output Window":
                    tool.Content = new MessageViewModel();
                    break;
                case "Notes":
                    tool.Content = new System.Windows.Controls.TextBox { TextWrapping = TextWrapping.Wrap };
                    break;
                case "ArchiveInfo":
                    tool.Content = new ArchiveInfoViewModel();
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
                    MessageViewModel.Instance.Add("Not Implemented", String.Format("Add Tool Parameter of {0} not Implemented", name), MSGIcon.ERROR);
                    break;
            }

            // Does Content Exist Allready?
            foreach (var dd in MainWindow.Instance.dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Where(dd => dd.Title == tool.Title))
            {
                dd.IsActive = true;
                return;
            }
            var group = new LayoutAnchorGroup();
            tool.IsActive = true;
            group.Children.Add(tool);
            MainWindow.Instance.dockManager.Layout.RightSide.Children.Add(group);

        }

        private void ImportRobot()
        {
           //TODO Reimplement this
            ObjectBrowserViewModel.Instance.ShowWizard("Backup Folder");
                       //AddTool("Object Browser");
                         BringToFront("Object Browser");
                      AddTool("ArchiveInfo");
        }

        public void CloseWindow(object param)
        {
            var ad = param as DummyDoc;
            var docpane = MainWindow.Instance.dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (docpane == null) return;

            foreach (var c in docpane.Children.Where(c => c.Content.Equals(ad)))
            {
                docpane.Children.Remove(c);
                return;
            }
        }


        public void BringToFront(string windowname)
        {
            // Does Content Exist Allready?
            foreach (var dd in MainWindow.Instance.dockManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                if (dd.Title == windowname)
                    dd.IsActive = true;
                Console.WriteLine(dd.Title);
            }

        }



        //TODO This could probably be moved to the KUKA FileLanguage SomeHow
        public void CleanDat(object sender, RoutedEventArgs e)
        {
            //	Language_Specific.DatCleanControl dcc = new miRobotEditor.Language_Specific.DatCleanControl();
            //	dcc.ShowDialog();
            //	Output.Add("Add","Need to Put Clean Dat to Command");
        }

        public void ShowAbout()
        {
            new AboutWindow().ShowDialog();
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
            // saveFileDialog.FileName = ActiveEditor.Filename;

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
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Allow the user to confirm whether they want to close a modified document.
        /// </summary>
        public bool QueryCloseModifiedDocument(DummyDoc document)
        {
            string msg = document.Filename + " has been modified but not saved.\r\nDo you really want to close it?";
            var result = MessageBox.Show( msg, "File modified but not saved", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Allow the user to confirm whether they want to close the application 
        /// when 1 or more documents are modified.
        /// </summary>
        public bool QueryCloseApplicationWhenDocumentsModified()
        {
            string msg = "1 or more open files have been modified but not saved.\n" +"Do you really want to exit?";
            var result = MessageBox.Show( msg, "File(s) modified but not saved", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /*    /// <summary>
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
    */



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
        /*
                /// <summary>
                /// Event raised when a document is being closed by clicking the 'X' button in AvalonDock.
                /// </summary>
                private void avalonDockHost_DocumentClosing(object sender, DocumentClosingEventArgs e)
                {
                    var document = (DummyDoc)e.Document;
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
        
        */
        #endregion
 

    }
}
