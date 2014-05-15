using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using miRobotEditor.ExplorerControl;
using miRobotEditor.Forms;
using miRobotEditor.GUI;
using miRobotEditor.GUI.Editor;
using miRobotEditor.Languages;
using miRobotEditor.Pads;
using miRobotEditor.Properties;
using miRobotEditor.Resources.StringResources;
using miRobotEditor.ViewModel;
using miRobotEditor.WindowMessages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace miRobotEditor
{
    [Localizable(false)]
    public class WorkspaceViewModel : ViewModelBase
    {
        #region Constructor

        public WorkspaceViewModel()
        {
            _instance = this;
            Tools = new ObservableCollection<ToolViewModel> { ObjectBrowser, MessageView, Notes, LocalVariables, Functions, AngleConverter };
            Files = new ObservableCollection<IDocument>();

            Messenger.Default.Register<OpenFileMessage>(this,msg =>{OpenFile(msg.FileName);});
        }

        #endregion Constructor

        private static WorkspaceViewModel _instance;

        public static WorkspaceViewModel Instance { get { return _instance ?? (_instance = new WorkspaceViewModel()); } }

        #region Tools

        //Object Browser
        private readonly ObjectBrowserViewModel _objectBrowser = null;

        public ObjectBrowserViewModel ObjectBrowser { get { return _objectBrowser ?? new ObjectBrowserViewModel(); } }

        private readonly NotesViewModel _notes = null;

        public NotesViewModel Notes { get { return _notes ?? new NotesViewModel(); } }

        private readonly MessageViewModel _messageView = null;

        public MessageViewModel MessageView
        {
            get { return _messageView ?? new MessageViewModel(); }
        }

        private readonly FunctionViewModel _functions = null;

        public FunctionViewModel Functions { get { return _functions ?? new FunctionViewModel(); } }

        private readonly LocalVariablesViewModel _localVariables = null;

        public LocalVariablesViewModel LocalVariables { get { return _localVariables ?? new LocalVariablesViewModel(); } }

        private readonly AngleConvertorViewModel _angleConverter = null;

        public AngleConvertorViewModel AngleConverter { get { return _angleConverter ?? new AngleConvertorViewModel(); } }

        #endregion Tools

        #region Properties

        #region ShowSettings

        private bool _showSettings;

        public bool ShowSettings { get { return _showSettings; } set { _showSettings = value; RaisePropertyChanged("ShowSettings"); } }

        #endregion ShowSettings

        #region AccentBrush

        [NonSerialized]
        private Accent _accentBrush = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public Accent AccentBrush { get { return _accentBrush; } set { _accentBrush = value; RaisePropertyChanged("AccentBrush"); } }

        #endregion AccentBrush

        #region Current theme

        private Theme _currentTheme = Theme.Dark;

        public Theme CurrentTheme { get { return _currentTheme; } set { _currentTheme = value; RaisePropertyChanged("CurrentTheme"); } }

        #endregion Current theme

        #region Show IO

        private bool _showIO;

        public bool ShowIO { get { return _showIO; } set { _showIO = value; RaisePropertyChanged("ShowIO"); } }

        #endregion Show IO

        #region Enable IO

        private bool _enableIO;

        public bool EnableIO { get { return _enableIO; } set { _enableIO = value; RaisePropertyChanged("EnableIO"); } }

        #endregion Enable IO

        #region Layout Strategy

        private ILayoutUpdateStrategy _layoutStrategy;

        public ILayoutUpdateStrategy LayoutStrategy { get { return _layoutStrategy ?? (_layoutStrategy = new LayoutInitializer()); } }

        #endregion Layout Strategy

        #region Tools

     
        public ObservableCollection<ToolViewModel> Tools { get; set; }

        #endregion Tools

        #region IsClosing

        private bool _isClosing;

        public bool IsClosing { get { return _isClosing; } set { _isClosing = value; RaisePropertyChanged("IsClosing"); } }

        #endregion IsClosing

        #region Files


        public ObservableCollection<IDocument> Files { get; set; }

        #endregion Files

        #endregion Properties

        #region ActiveEditor

        private IDocument _activeEditor;

        public IDocument ActiveEditor
        {
            get
            {
                return _activeEditor;
            }
            set
            {
                _activeEditor = value;

                lock (_activeEditor)
                {
                    //TODO 8/19/2013 Changed this and moved inside the lock;

                    RaisePropertyChanged("ActiveEditor");
                    RaisePropertyChanged("Title");
                    //            if (ActiveEditorChanged != null)
                    //                ActiveEditorChanged(this, EventArgs.Empty);
                }
            }
        }

#pragma warning disable 67

        public event EventHandler ActiveEditorChanged;

#pragma warning restore 67

        #endregion ActiveEditor

        private void ExecuteShowIO()
        {
            ShowIO = !ShowIO;
        }

        #region Commands

        #region ShowIOCommand

        private RelayCommand _showIOCommand;

        /// <summary>
        /// Gets the ShowIOCommand.
        /// </summary>
        public RelayCommand ShowIOCommand
        {
            get
            {
                return _showIOCommand
                    ?? (_showIOCommand = new RelayCommand(ExecuteShowIOCommand));
            }
        }

        private void ExecuteShowIOCommand()
        {
            ExecuteShowIO();
        }

        #endregion ShowIOCommand

        #region NewFile

        #region NewFileCommand

        private RelayCommand _newFileCommand;

        /// <summary>
        /// Gets the NewFileCommand.
        /// </summary>
        public RelayCommand NewFileCommand
        {
            get
            {
                return _newFileCommand
                    ?? (_newFileCommand = new RelayCommand(ExecuteNewFileCommand));
            }
        }

        private void ExecuteNewFileCommand()
        {
            AddNewFile();
        }

        #endregion NewFileCommand

        #endregion NewFile

        #region Change Theme

        #region MyCommand

        private RelayCommand<string> _changeThemeCommand;

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand<string> ChangeThemeCommand
        {
            get
            {
                return _changeThemeCommand
                    ?? (_changeThemeCommand = new RelayCommand<string>(ExecuteChangeThemeCommand));
            }
        }

        private void ExecuteChangeThemeCommand(string value)
        {
            CurrentTheme = value == "Light" ? Theme.Light : Theme.Dark;
            ThemeManager.ChangeTheme(MainWindow.Instance, AccentBrush, CurrentTheme);
        }

        #endregion MyCommand

        #endregion Change Theme

        #region Change Accent

        #region ChangeAccentCommand

        private RelayCommand<string> _changeAccentCommand;

        /// <summary>
        /// Gets the ChangeAccentCommand.
        /// </summary>
        public RelayCommand<string> ChangeAccentCommand
        {
            get
            {
                return _changeAccentCommand
                    ?? (_changeAccentCommand = new RelayCommand<string>(ExecuteChangeAccentCommand));
            }
        }

        private void ExecuteChangeAccentCommand(string value)
        {
            AccentBrush = ThemeManager.DefaultAccents.First(x => x.Name == value);
            ThemeManager.ChangeTheme(MainWindow.Instance, AccentBrush, CurrentTheme);
        }

        #endregion ChangeAccentCommand

        #endregion Change Accent

        #region ShowSettingsCommand

        private RelayCommand _showSettingsCommand;

        /// <summary>
        /// Gets the ShowSettingsCommand.
        /// </summary>
        public RelayCommand ShowSettingsCommand
        {
            get
            {
                return _showSettingsCommand
                    ?? (_showSettingsCommand = new RelayCommand(ExecuteShowSettingsCommand));
            }
        }

        private void ExecuteShowSettingsCommand()
        {
            ExecuteShowSettings();
        }

        #endregion ShowSettingsCommand

        #region ShowFindReplaceCommand

        private RelayCommand _showFindReplaceCommand;

        /// <summary>
        /// Gets the ShowFindReplaceCommand.
        /// </summary>
        public RelayCommand ShowFindReplaceCommand
        {
            get
            {
                return _showFindReplaceCommand
                    ?? (_showFindReplaceCommand = new RelayCommand(ExecuteShowFindReplaceCommand));
            }
        }

        private void ExecuteShowFindReplaceCommand()
        {
            var fnr = new FindandReplaceControl(MainWindow.Instance);
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            fnr.ShowDialog().GetValueOrDefault();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        #endregion ShowFindReplaceCommand

        #region ShowAboutCommand

        private RelayCommand _showAboutCommand;

        /// <summary>
        /// Gets the ShowAboutCommand.
        /// </summary>
        public RelayCommand ShowAboutCommand
        {
            get
            {
                return _showAboutCommand
                    ?? (_showAboutCommand = new RelayCommand(ExecuteShowAboutCommand));
            }
        }

        private void ExecuteShowAboutCommand()
        {
            ShowAbout();
        }

        #endregion ShowAboutCommand

        #region ExitCommand

        private RelayCommand _exitCommand;

        /// <summary>
        /// Gets the ExitCommand.
        /// </summary>
        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand
                    ?? (_exitCommand = new RelayCommand(ExecuteExitCommand));
            }
        }

        private void ExecuteExitCommand()
        {
            Exit();
        }

        #endregion ExitCommand

        #region ImportCommand

        private RelayCommand _importCommand;

        /// <summary>
        /// Gets the ImportCommand.
        /// </summary>
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand
                    ?? (_importCommand = new RelayCommand(ExecuteImportCommand));
            }
        }

        private void ExecuteImportCommand()
        {
            ImportRobot();
        }

        #endregion ImportCommand

        #region OpenFileCommand

        private RelayCommand _openFileCommand;

        /// <summary>
        /// Gets the OpenFileCommand.
        /// </summary>
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand
                    ?? (_openFileCommand = new RelayCommand(ExecuteOpenFileCommand));
            }
        }

        private void ExecuteOpenFileCommand()
        {
            OnOpen(null);
        }

        #endregion OpenFileCommand

        #region ChangeViewAsCommand

        private RelayCommand<object> _changeViewAsCommand;

        /// <summary>
        /// Gets the ChangeViewAsCommand.
        /// </summary>
        public RelayCommand<object> ChangeViewAsCommand
        {
            get
            {
                return _changeViewAsCommand
                    ?? (_changeViewAsCommand = new RelayCommand<object>(ExecuteChangeViewAsCommand));
            }
        }

        private void ExecuteChangeViewAsCommand(object param)
        {
            ChangeViewAs(param);
        }

        #endregion ChangeViewAsCommand

        #region AddToolCommand

        private RelayCommand<object> _addToolCommand;

        /// <summary>
        /// Gets the AddToolCommand.
        /// </summary>
        public RelayCommand<object> AddToolCommand
        {
            get
            {
                return _addToolCommand
                    ?? (_addToolCommand = new RelayCommand<object>(ExecuteAddToolCommand));
            }
        }

        private void ExecuteAddToolCommand(object param)
        {
            AddTool(param);
        }

        #endregion AddToolCommand

        #endregion Commands

        #region Show Settings

        private void ExecuteShowSettings()
        {
            ShowSettings = !ShowSettings;
        }

        #endregion Show Settings

        #region OpenFile

        /// <summary>
        /// Open file from menu entry
        /// </summary>
        /// <param name="param"></param>
        private void OnOpen(object param)
        {
            var path = Path.GetDirectoryName(ActiveEditor.FilePath);
            var dir = Directory.Exists(path) ? path : "C:\\";
            var dlg = new OpenFileDialog
            {
                // Find a way to check for network directory
                //                InitialDirectory="C:\\",
                Filter = Settings.Default.Filter,
                Multiselect = true,
                FilterIndex = 1,
            };

            if (!dlg.ShowDialog().GetValueOrDefault()) return;
            foreach (var file in dlg.FileNames)
                Open(file);
        }

        public IDocument Open(string filepath)
        {
            var fileViewModel = OpenFile(filepath);

            ActiveEditor = fileViewModel;
            ActiveEditor.TextBox.FilenameChanged += TextBox_FilenameChanged;

            ActiveEditor.IsActive = true;
            return fileViewModel;
        }

        private void TextBox_FilenameChanged(object sender, EventArgs e)
        {
            var editor = sender as EditorClass;
            Debug.Assert(editor != null, "editor != null");
            var fileViewModel = Files.FirstOrDefault(fi => fi.ContentId == editor.Tag.ToString());

            Debug.Assert(fileViewModel != null, "fileViewModel != null");

            fileViewModel.ContentId = editor.Filename;
            RaisePropertyChanged("Title");

            Console.WriteLine();
        }

        private void fileViewModel_FilenameChanged(object sender, EventArgs e)
        {
        }

        private void OpenFile(OpenFileMessage msg)
        {
            OpenFile(msg.FileName);
        }

        /// <summary>
        /// Gets document type of file to open
        /// </summary>
        /// <param name="filepath">Destination of file</param>
        /// <returns>Document Type</returns>
        private IDocument OpenFile(string filepath)
        {
            var fileViewModel = Files.FirstOrDefault(fm => fm.ContentId == filepath);

            if (fileViewModel != null)
            {
                fileViewModel.IsSelected = true;
                fileViewModel.IsActive = true;
                ActiveEditor = fileViewModel;
                return fileViewModel;
            }

            fileViewModel = AbstractLanguageClass.GetViewModel(filepath);

            if (File.Exists(filepath))
            {
                fileViewModel.Load(filepath);
                fileViewModel.TextBox.Tag = filepath;
                // Add file to Recent list
                RecentFileList.Instance.InsertFile(filepath);
                System.Windows.Shell.JumpList.AddToRecentCategory(filepath);
            }

            // 7/23/2013 Changed order
            Files.Add(fileViewModel);
            fileViewModel.IsActive = true;
            fileViewModel.IsSelected = true;

            ActiveEditor = fileViewModel;
            return fileViewModel;
        }

        public void OpenFile(IVariable variable)
        {
            // Am i using dock or ActiveEditor?

            var fileViewModel = Open(variable.Path);

            fileViewModel.SelectText(variable);
            //            ActiveEditor.TextBox.SelectText(variable);
        }

        public void AddNewFile()
        {
            Files.Add(new DocumentViewModel(null));
            ActiveEditor = Files.Last();
        }

        public void LoadFile(IList<string> args)
        {
            // Argument 0 is The Path of the main application so i start with argument 1

            for (int i = 1; i < args.Count; i++)
                Open(args[i]);
        }

        #endregion OpenFile

        //This can probably move to the language class section
        private void ChangeViewAs(object param)
        {
            var lang = param as AbstractLanguageClass;

            if (Equals(ActiveEditor.FileLanguage, lang)) return;

            switch (param.ToString())
            {
                case "ABB":
                    // ReSharper disable RedundantCast
                    ActiveEditor.FileLanguage = (ABB)lang;
                    break;

                case "KUKA":
                    ActiveEditor.FileLanguage = new KUKA();
                    break;

                case "Fanuc":
                    ActiveEditor.FileLanguage = (Fanuc)lang;
                    break;

                case "Kawasaki":
                    ActiveEditor.FileLanguage = (Kawasaki)lang;
                    break;
                // ReSharper restore RedundantCast
            }

            //                ActiveEditor.TextBox.UpdateVisualText();

            //                DummyDocViewModel.Instance.TextBox.UpdateVisualText();
        }

        public void Exit()
        {
            MainWindow.Instance.Close();
        }

        internal void Close(IDocument fileToClose)
        {
            Files.Remove(fileToClose);
            RaisePropertyChanged("ActiveEditor");
        }

        public void AddTool(ToolViewModel toolModel)
        {
            var tool = new LayoutAnchorable();
            if (toolModel == null) return;
            tool.Title = toolModel.Title;
            tool.Content = toolModel;

            // Does Content Exist Allready?
            foreach (var t in Tools.Where(t => t.Title == toolModel.Title))
            {
                t.IsActive = true;
                return;
            }

            toolModel.IsActive = true;
            Tools.Add(toolModel);
            toolModel.IsActive = true;
            RaisePropertyChanged("Tools");
        }

        [Localizable(false)]
        private void AddTool(object parameter)
        {
            var name = parameter as string;
            ToolViewModel toolModel = null;
            var tool = new LayoutAnchorable();
            switch (name)
            {
                case "Angle Converter":
                    toolModel = new AngleConvertorViewModel();
                    tool.AutoHideMinWidth = 219;
                    break;

                case "Functions":
                    toolModel = new FunctionViewModel();
                    tool.AutoHideMinWidth = 300;
                    break;

                case "Explorer":
                    tool.Content = new FileExplorerWindow();
                    break;

                case "Object Browser":
                    toolModel = new ObjectBrowserViewModel();
                    break;

                case "Output Window":
                    toolModel = new MessageViewModel();
                    break;

                case "Notes":
                    toolModel = new NotesViewModel();
                    break;

                case "ArchiveInfo":
                    toolModel = new ArchiveInfoViewModel();
                    break;

                case "Rename Positions":
                    //TODO Change this
                    tool.Content = new Language_Specific.RenamePositionWindow();
                    break;

                case "Shift":
                    //TODO Change this
                    tool.Content = new ShiftWindow();
                    break;

                case "CleanDat":
                    toolModel = new DatCleanHelper();
                    tool.AutoHideMinWidth = DatCleanHelper.Instance.Width;
                    break;

                default:
                    MessageViewModel.Instance.Add(ErrorResources.NotImplemented, String.Format(ErrorResources.NotImplementedError, name), MsgIcon.Error);
                    break;
            }

            if (toolModel != null)
            {
                tool.Title = toolModel.Title;
                tool.Content = toolModel;

                // Does Content Exist Allready?
                foreach (var t in Tools.Where(t => t.Title == toolModel.Title))
                {
                    t.IsActive = true;
                    return;
                }

                toolModel.IsActive = true;
                Tools.Add(toolModel);
            }
            RaisePropertyChanged("Tools");
        }

        private void ImportRobot()
        {
            BringToFront("Object Browser");
            AddTool("ArchiveInfo");
        }

        public void BringToFront(string windowname)
        {
            // Does Content Exist Allready?
            foreach (var dd in MainWindow.Instance.DockManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                if (dd.Title == windowname)
                    dd.IsActive = true;
                Console.WriteLine(dd.Title);
            }
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
            if (result.Value)
            {
                filePath = openFileDialog.FileName;
                return true;
            }
            filePath = null;
            return false;
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
            if (result.Value)
            {
                newFilePath = saveFileDialog.FileName;
                return true;
            }
            newFilePath = string.Empty;
            return false;
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
        /// Event raised when AvalonDock has loaded.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        private void avalonDockHost_AvalonDockLoaded(object sender, EventArgs e)
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
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

        #endregion IDialogProvider Interface
    }
}