using System;
using MahApps.Metro;
using miRobotEditor.Core;
using miRobotEditor.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.IO;
using miRobotEditor.Classes;
using miRobotEditor.GUI;
using miRobotEditor.ViewModel;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using miRobotEditor.Properties;
using miRobotEditor.Languages;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using miRobotEditor.Pads;
using RelayCommand = miRobotEditor.Commands.RelayCommand;

namespace miRobotEditor
{
    public class Workspace:ViewModelBase
    {


        #region Constructor
        public Workspace()
        {
            Instance = this;
            AddNewFile();
            _tools = new ObservableCollection<ToolViewModel> { ObjectBrowser,MessageView, Notes, LocalVariables, Functions, AngleConverter };

        }

        #endregion

        public string Title{
            get
            {
                var fn = ActiveEditor.FilePath ?? string.Empty;
                return ShortenPathname(fn, 100);
        }
        }


        /// <summary>
        /// Shortens a pathname for display purposes.
        /// </summary>
        /// <param labelName="pathname">The pathname to shorten.</param>
        /// <param labelName="maxLength">The maximum number of characters to be displayed.</param>
        /// <param name="pathname"> </param>
        /// <param name="maxLength"> </param>
        /// <remarks>Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three elipses (...)
        /// <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
        /// <para>If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.</para>
        /// <para>This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)</para>
        /// </remarks>
        /// <returns></returns>
        public static string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname.Length <= maxLength) return pathname;

            var root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
                root += Path.DirectorySeparatorChar;

            if (true)
            {
                var elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                var filenameIndex = elements.GetLength(0) - 1;

                if (elements.GetLength(0) == 1) // pathname is just a root and filename
                {
                    if (elements[0].Length > 5) // long enough to shorten
                    {
                        // if path is a UNC path, root may be rather long
                        if (root.Length + 6 >= maxLength)
                        {
                            return root + elements[0].Substring(0, 3) + "...";
                        }
                        return pathname.Substring(0, maxLength - 3) + "...";
                    }
                }
                else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
                {
                    root += "...\\";

                    var len = elements[filenameIndex].Length;
                    if (len < 6)
                        return root + elements[filenameIndex];

                    if ((root.Length + 6) >= maxLength)
                    {
                        len = 3;
                    }
                    else
                    {
                        len = maxLength - root.Length - 3;
                    }
                    return root + elements[filenameIndex].Substring(0, len) + "...";
                }
                else if (elements.GetLength(0) == 2)
                {
                    return root + "...\\" + elements[1];
                }
                else
                {
                    var len = 0;
                    var begin = 0;

                    for (var i = 0; i < filenameIndex; i++)
                    {
                        if (elements[i].Length <= len) continue;
                        begin = i;
                        len = elements[i].Length;
                    }

                    var totalLength = pathname.Length - len + 3;
                    var end = begin + 1;

                    while (totalLength > maxLength)
                    {
                        if (begin > 0)
                            totalLength -= elements[--begin].Length - 1;

                        if (totalLength <= maxLength)
                            break;

                        if (end < filenameIndex)
                            totalLength -= elements[++end].Length - 1;

                        if (begin == 0 && end == filenameIndex)
                            break;
                    }

                    // assemble final string

                    for (var i = 0; i < begin; i++)
                    {
                        root += elements[i] + '\\';
                    }

                    root += "...\\";

                    for (var i = end; i < filenameIndex; i++)
                    {
                        root += elements[i] + '\\';
                    }

                    return root + elements[filenameIndex];
                }
            }
            return pathname;
        }



        private static Workspace _instance;
        public static Workspace Instance { get { return _instance ?? (_instance = new Workspace()); } set { _instance = value; } }

        #region Tools

        //Object Browser
        readonly ObjectBrowserViewModel _objectBrowser = null;
        public ObjectBrowserViewModel ObjectBrowser { get { return _objectBrowser ?? new ObjectBrowserViewModel(); } }

        readonly NotesViewModel _notes = null;
        public NotesViewModel Notes { get { return _notes ?? new NotesViewModel(); } }

        readonly MessageViewModel _messageView = null;
        public MessageViewModel MessageView
        {
            get { return _messageView ?? new MessageViewModel(); }
        }

        readonly FunctionViewModel _functions = null;
        public FunctionViewModel Functions { get { return _functions ?? new FunctionViewModel(); } }


        readonly LocalVariablesViewModel _localVariables = null;
        public LocalVariablesViewModel LocalVariables { get { return _localVariables ?? new LocalVariablesViewModel(); } }


        readonly AngleConvertorViewModel _angleConverter = null;
        public AngleConvertorViewModel AngleConverter { get { return _angleConverter ?? new AngleConvertorViewModel(); } }
        #endregion

        #region Properties
        private bool _showSettings;
        public bool ShowSettings { get { return _showSettings; } set { _showSettings = value; RaisePropertyChanged(); } }
        [NonSerialized]
        private Accent _accentBrush = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public Accent AccentBrush { get { return _accentBrush; } set { _accentBrush = value;RaisePropertyChanged(); } }

        private Theme _currentTheme = Theme.Dark;
        public Theme CurrentTheme { get { return _currentTheme; } set { _currentTheme = value;RaisePropertyChanged(); } }
        private bool _showIO;
        public bool ShowIO { get { return _showIO; } set { _showIO = value;RaisePropertyChanged(); } }

        private bool _enableIO;
        public bool EnableIO { get { return _enableIO; } set { _enableIO = value;RaisePropertyChanged(); } }

        private  ILayoutUpdateStrategy _layoutInitializer;
        public  ILayoutUpdateStrategy LayoutStrategy { get { return _layoutInitializer ?? (_layoutInitializer = new LayoutInitializer()); } }

        readonly ObservableCollection<ToolViewModel> _tools = new ObservableCollection<ToolViewModel>();
        readonly IEnumerable<ToolViewModel> _readonlyTools = null;
        public IEnumerable<ToolViewModel> Tools { get { return _readonlyTools ?? new ObservableCollection<ToolViewModel>(_tools); } }


        private bool _isClosing;
        public bool IsClosing { get { return _isClosing; } set { _isClosing = value; RaisePropertyChanged(); } }

        readonly ObservableCollection<IDocument> _files = new ObservableCollection<IDocument>();
        readonly ReadOnlyObservableCollection<IDocument> _readonyFiles = null;
        public ReadOnlyObservableCollection<IDocument> Files { get { return _readonyFiles ?? new ReadOnlyObservableCollection<IDocument>(_files); } }

        #endregion

        #region ActiveEditor
        private  IDocument _activeEditor;
        public IDocument ActiveEditor 
        {
            get 
            {
                return _activeEditor; 
            } 
            set 
            {

                Console.WriteLine(value.FileName);
               // if (_activeEditor.ContentId == value.ContentId) return;
                _activeEditor = value;
                
                RaisePropertyChanged("ActiveEditor");
                _activeEditor.TextBox.Focus();
                _activeEditor.TextBox.InvalidateVisual();
                RaisePropertyChanged("Title");
    //            if (ActiveEditorChanged != null)
    //                ActiveEditorChanged(this, EventArgs.Empty);
            } 
        }



#pragma warning disable 67
        public event EventHandler ActiveEditorChanged;
#pragma warning restore 67
        #endregion


        void ExecuteShowIO()
        {
            ShowIO = !ShowIO;
        }
        #region Commands

        private RelayCommand _showIOCommand;
        public ICommand ShowIOCommand
        {
            get { return _showIOCommand ?? (_showIOCommand = new RelayCommand(p => ExecuteShowIO(), p => true)); }
        }

        #region NewFile
        private RelayCommand _newFileCommand;
         public ICommand NewFileCommand
        {
            get { return _newFileCommand ?? (_newFileCommand = new RelayCommand(p => AddNewFile(), p => true)); }
        }
        #endregion

         #region Change Theme

        private RelayCommand _changeThemeCommand;
        public ICommand ChangeThemeCommand
        {
            get
            {
                return _changeThemeCommand ??
                       (_changeThemeCommand = new RelayCommand(p => ChangeTheme(p), p => true));
            }
        }
         #endregion

         #region Change Accent
         private RelayCommand _changeAccentCommand;
         public ICommand ChangeAccentCommand
         {
             get { return _changeAccentCommand ?? (_changeAccentCommand = new RelayCommand(p => ChangeAccent(p), p => true)); }
         }
         #endregion

         #region ShowSettings
         private RelayCommand _showSettingsCommand;
         public ICommand ShowSettingsCommand
      {
          get { return _showSettingsCommand ?? (_showSettingsCommand = new RelayCommand(p => ExecuteShowSettings(), p => true)); }
      }
        #endregion


         private RelayCommand _showFindReplace;

         public ICommand ShowFindReplaceCommand
         {
             get { return _showFindReplace ?? (_showFindReplace = new RelayCommand(p => ShowFindReplace(), p => true)); }
         }

       void ShowFindReplace()
       {
           var fnr = new FindandReplaceControl(MainWindow.Instance) ;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
           fnr.ShowDialog().GetValueOrDefault();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
       }


        private RelayCommand _showAboutCommand;

      public ICommand ShowAboutCommand
        {
            get { return _showAboutCommand ?? (_showAboutCommand = new RelayCommand(p => ShowAbout(), p => true)); }
        }

        private RelayCommand _exitCommand;

      public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(p => Exit(),p => true)); }
        }

        private RelayCommand _importCommand;

        public ICommand ImportCommand
        {
            get { return _importCommand ?? (_importCommand = new RelayCommand(p => ImportRobot(), p => (!(p is LanguageBase) | (p is Fanuc) | (p is Kawasaki) | p == null))); }
        }

        private RelayCommand _openFileCommand;

      public ICommand OpenFileCommand
        {
            get { return _openFileCommand ?? (_openFileCommand = new RelayCommand(p => OnOpen(p), p => true)); }
        }

        private  RelayCommand _changeViewAsCommand;

      public ICommand ChangeViewAsCommand
        {
            get { return _changeViewAsCommand ?? (_changeViewAsCommand = new RelayCommand(param => ChangeViewAs(param), param => true)); }
        }

        private  RelayCommand _addToolCommand;

        public ICommand AddToolCommand
        {
            get { return _addToolCommand ?? (_addToolCommand = new RelayCommand(param => AddTool(param), param => true)); }
        }

        #endregion

        #region Show Settings

      void ExecuteShowSettings()
      {
          ShowSettings = !ShowSettings;
      }
      #endregion


        void ChangeAccent(object param)
        {
            AccentBrush = ThemeManager.DefaultAccents.First(x => x.Name == param.ToString());
            ThemeManager.ChangeTheme(MainWindow.Instance,AccentBrush,CurrentTheme);
        }

        void ChangeTheme(object param)
        {
          
           CurrentTheme = param.ToString() == "Light" ? Theme.Light : Theme.Dark;
           ThemeManager.ChangeTheme(MainWindow.Instance, AccentBrush, CurrentTheme);
           
        }

       
  
      #region OpenFile

      /// <summary>
        /// Open file from menu entry
        /// </summary>
        /// <param name="param"></param>
        void OnOpen(object param)
      {
            var path = Path.GetDirectoryName(ActiveEditor.FilePath);
            var dir = Directory.Exists(path)?path:"C:\\";
            var dlg = new OpenFileDialog
            {
                // Find a way to check for network directory
//                InitialDirectory="C:\\",
                Filter = Resources.DefaultFilter,
                Multiselect = true,
                FilterIndex = Settings.Default.Filter,
            };
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                Open(dlg.FileName);
            }
        }


        public IDocument Open(string filepath)
        {
            var fileViewModel = OpenFile(filepath);
            ActiveEditor = fileViewModel;
            ActiveEditor.IsActive = true;
            return fileViewModel;
        }

        private IDocument OpenFile(string filepath)
        {
            var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath == filepath);
            if (fileViewModel != null)
                return fileViewModel;


            fileViewModel = AbstractLanguageClass.GetViewModel(filepath);
          
            if (File.Exists(filepath))
            {
                fileViewModel.Load(filepath);
                // Add file to Recent list
                RecentFileList.Instance.InsertFile(filepath);
                System.Windows.Shell.JumpList.AddToRecentCategory(filepath);            
            }
            fileViewModel.IsActive = true;
            _files.Add(fileViewModel);
            ActiveEditor = fileViewModel;
            return fileViewModel;
        }

    
        public void OpenFile(IVariable variable)
        {
            // Am i using dock or ActiveEditor?

          var fileViewModel =   Open(variable.Path);

          fileViewModel.SelectText(variable);
//            ActiveEditor.TextBox.SelectText(variable);
        }

     
        public void AddNewFile()
        {
            _files.Add(new DocumentViewModel(null));
            ActiveEditor = _files.Last();
        }

        public void LoadFile(IList<string> args)
        {

            // Argument 0 is The Path of the main application so i start with argument 1
            for (var i = 1; i < args.Count; i++)
            {
                Open(args[i]);
            }
        }

        #endregion

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

        public void Exit() { MainWindow.Instance.Close(); }

        internal void Close(IDocument fileToClose)
        {          
                _files.Remove(fileToClose);
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
            _tools.Add(toolModel);
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
                    tool.Content = new GUI.ExplorerControl.FileExplorerWindow();
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
                    MessageViewModel.Instance.Add("Not Implemented", String.Format("Add Tool Parameter of {0} not Implemented", name), MsgIcon.Error);
                    break;
            }

            if (toolModel != null)
            {
                tool.Title = toolModel.Title;
                tool.Content = toolModel;

                // Does Content Exist Allready?
                foreach (var t in Tools.Where(t => t.Title==toolModel.Title))
                {
                    t.IsActive = true;
                    return;
                }

                toolModel.IsActive = true;
                _tools.Add(toolModel);
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
 
        #endregion
 

    }

}
