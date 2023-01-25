﻿

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using AvalonDock.Layout;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlzEx.Theming;
using Microsoft.Win32; 
using miRobotEditor.Classes;
using miRobotEditor.Controls;
using miRobotEditor.Controls.TextEditor.Language;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Languages;
using miRobotEditor.Messages;
using miRobotEditor.Windows;
using Mookie.WPF.Behaviors;

namespace miRobotEditor.ViewModel
{

    public sealed class MainViewModel : ViewModelBase,IFilesDropped
    {
        private static IEditorDocument _activeEditor;
        private ILayoutUpdateStrategy _layoutInitializer;

        public MainViewModel()
        {
            AddNewFile();
            _tools = new ObservableCollection<ToolViewModel>
            {
                ObjectBrowser,
                MessageView,
                Notes,
                LocalVariables,
                Functions,
                AngleConverter
            };

            WeakReferenceMessenger.Default.Register<WindowMessage>(this, GetMessage);

        }

        private void GetMessage(object recipient, WindowMessage message)
        {
            _ = Open(message.Description);
        }

        public string Title
        {
            get
            {
                string pathname = ActiveEditor.FilePath ?? string.Empty;
                return ShortenPathname(pathname, 100);
            }
        }

        #region Properties

        #region Tools

        private readonly IEnumerable<ToolViewModel> _readonlyTools = null;
        private readonly ObservableCollection<ToolViewModel> _tools = new ObservableCollection<ToolViewModel>();

        public ObjectBrowserViewModel ObjectBrowser { get; } = new ObjectBrowserViewModel();

        public NotesViewModel Notes { get; } = new NotesViewModel();

        public MessageViewModel MessageView { get; } = new MessageViewModel();

        public FunctionViewModel Functions { get; } = new FunctionViewModel();

        public LocalVariablesViewModel LocalVariables { get; } = new LocalVariablesViewModel();

        public AngleConvertorViewModel AngleConverter { get; } = new AngleConvertorViewModel();

        #endregion Tools

        #region Files

        private readonly ObservableCollection<IEditorDocument> _files = new ObservableCollection<IEditorDocument>();
        private readonly ReadOnlyObservableCollection<IEditorDocument> _readonlyFiles = null;

        public IEnumerable<IEditorDocument> Files => _readonlyFiles ?? new ReadOnlyObservableCollection<IEditorDocument>(_files);

        #endregion Files

        #region ShowSettings

        private bool _showSettings;

        /// <summary>
        ///     Sets and gets the ShowSettings property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowSettings
        {
            get => _showSettings;

            set => SetProperty(ref _showSettings, value);

        }

        #endregion ShowSettings



        #region CurrentTheme

        private Theme _currentTheme;

        /// <summary>
        ///     Sets and gets the CurrentTheme property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Theme CurrentTheme
        {
            get => _currentTheme;

            set => SetProperty(ref _currentTheme, value);

        }

        #endregion CurrentTheme

        #region ShowIO

        private bool _showIO;

        /// <summary>
        ///     Sets and gets the ShowIO property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowIO
        {
            get => _showIO;

            set => SetProperty(ref _showIO, value);
        }

        #endregion ShowIO

        #region EnableIO

        private bool _enableIO;

        /// <summary>
        ///     Sets and gets the EnableIO property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool EnableIO
        {
            get => _enableIO;

            set => SetProperty(ref _enableIO, value);
        }

        #endregion EnableIO

        public ILayoutUpdateStrategy LayoutStrategy => _layoutInitializer ?? (_layoutInitializer = new LayoutInitializer());

        public IEnumerable<ToolViewModel> Tools => _readonlyTools ?? new ObservableCollection<ToolViewModel>(_tools);

        public IEditorDocument ActiveEditor
        {
            get => _activeEditor;
            set
            {
                if (_activeEditor != value)
                {
                    _activeEditor = value;
                    _ = (_activeEditor?.TextBox.Focus());
                    // ReSharper disable once RedundantArgumentDefaultValue
                    OnPropertyChanged(nameof(ActiveEditor));
                    // ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        #region IsClosing

        private bool _isClosing;

        /// <summary>
        ///     Sets and gets the IsClosing property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsClosing
        {
            get => _isClosing;

            set
            => SetProperty(ref _isClosing, value);
        }

        #endregion IsClosing

        #endregion Properties

        #region Commands

        #region CloseCommand

        private RelayCommand<object> _closeCommand;

        /// <summary>
        ///     Gets the CloseCommand.
        /// </summary>
        public RelayCommand<object> CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand<object>(ExecuteCloseCommand, CanExecuteCloseCommand));

        private void ExecuteCloseCommand(object obj)
        {
            _ = _files.Remove(ActiveEditor);

            ActiveEditor.Close();
            ActiveEditor = _files.FirstOrDefault();
            // Close(ActiveEditor);
            OnPropertyChanged(nameof(ActiveEditor));
        }

        private bool CanExecuteCloseCommand(object arg)
        {
            return true;
        }

        #endregion CloseCommand

        #region ShowIOCommand

        private RelayCommand _showIOCommand;

        /// <summary>
        ///     Gets the ShowIOCommand.
        /// </summary>
        public RelayCommand ShowIOCommand => _showIOCommand
                       ?? (_showIOCommand = new RelayCommand(ExecuteShowIO));

        #endregion ShowIOCommand


        #region NewFileCommand

        private RelayCommand _newFileCommand;

        /// <summary>
        ///     Gets the NewFileCommand.
        /// </summary>
        public RelayCommand NewFileCommand => _newFileCommand
                       ?? (_newFileCommand = new RelayCommand(
                           AddNewFile));

        #endregion NewFileCommand

        #region ShowFindReplaceCommand

        private RelayCommand _showFileReplaceCommand;

        /// <summary>
        ///     Gets the ShowFindReplaceCommand.
        /// </summary>
        public RelayCommand ShowFindReplaceCommand => _showFileReplaceCommand
                       ?? (_showFileReplaceCommand = new RelayCommand(ShowFindReplace));

        #endregion ShowFindReplaceCommand

        #region ShowSettingsCommand

        private RelayCommand _showSettingsCommand;

        /// <summary>
        ///     Gets the ShowSettingsCommand.
        /// </summary>
        public RelayCommand ShowSettingsCommand => _showSettingsCommand
                       ?? (_showSettingsCommand = new RelayCommand(ExecuteShowSettings));

        #endregion ShowSettingsCommand

        #region ChangeViewAsCommand

        private RelayCommand<string> _changeViewAsCommand;

        /// <summary>
        ///     Gets the ChangeViewAsCommand.
        /// </summary>
        public RelayCommand<string> ChangeViewAsCommand => _changeViewAsCommand ?? (_changeViewAsCommand = new RelayCommand<string>(ChangeViewAs));

        #endregion ChangeViewAsCommand

        #region ExitCommand

        private RelayCommand _exitCommand;

        /// <summary>
        ///     Gets the ExitCommand.
        /// </summary>
        public RelayCommand ExitCommand => _exitCommand
                       ?? (_exitCommand = new RelayCommand(Exit));

        #endregion ExitCommand

        #region ImportCommand

        private RelayCommand<object> _importCommand;

        /// <summary>
        ///     Gets the ImportCommand.
        /// </summary>
        public RelayCommand<object> ImportCommand => _importCommand ?? (_importCommand = new RelayCommand<object>(p => ImportRobot(), CanImport));

        public bool CanImport(object p)
        {
            return !((p is LanguageBase) | p is Fanuc | p is Kawasaki | p == null);
        }

        #endregion ImportCommand

        #region AddToolCommand

        private RelayCommand<object> _addToolCommand;

        /// <summary>
        ///     Gets the AddToolCommand.
        /// </summary>
        public RelayCommand<object> AddToolCommand => _addToolCommand ?? (_addToolCommand = new RelayCommand<object>(AddTool));

        #endregion AddToolCommand

        #region ShowAboutCommand

        private RelayCommand _showAboutCommand;

        /// <summary>
        ///     Gets the ShowAboutCommand.
        /// </summary>
        public RelayCommand ShowAboutCommand => _showAboutCommand ?? (_showAboutCommand = new RelayCommand(ShowAbout));

        #endregion ShowAboutCommand

        #region OpenFileCommand

        private RelayCommand<object> _openFileCommand;

        /// <summary>
        ///     Gets the OpenFileCommand.
        /// </summary>
        public RelayCommand<object> OpenFileCommand => _openFileCommand
                       ?? (_openFileCommand = new RelayCommand<object>(OnOpen));

        #endregion OpenFileCommand

        #endregion Commands

        private static string ShortenPathname(string pathname, int maxLength)
        {
            string result;
            if (pathname.Length <= maxLength)
            {
                result = pathname;
            }
            else
            {
                string text = Path.GetPathRoot(pathname);
                if (text.Length > 3)
                {
                    text += Path.DirectorySeparatorChar;
                }
                string[] array = pathname.Substring(text.Length).Split(new[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                });
                int num = array.GetLength(0) - 1;
                if (array.GetLength(0) == 1)
                {
                    result = array[0].Length > 5
                        ? text.Length + 6 >= maxLength ? text + array[0].Substring(0, 3) + "..." : pathname.Substring(0, maxLength - 3) + "..."
                        : pathname;
                }
                else
                {
                    if (text.Length + 4 + array[num].Length > maxLength)
                    {
                        text += "...\\";
                        int num2 = array[num].Length;
                        if (num2 < 6)
                        {
                            result = text + array[num];
                        }
                        else
                        {
                            num2 = text.Length + 6 >= maxLength ? 3 : maxLength - text.Length - 3;
                            result = text + array[num].Substring(0, num2) + "...";
                        }
                    }
                    else
                    {
                        if (array.GetLength(0) == 2)
                        {
                            result = text + "...\\" + array[1];
                        }
                        else
                        {
                            int num2 = 0;
                            int num3 = 0;
                            for (int i = 0; i < num; i++)
                            {
                                if (array[i].Length > num2)
                                {
                                    num3 = i;
                                    num2 = array[i].Length;
                                }
                            }
                            int j = pathname.Length - num2 + 3;
                            int num4 = num3 + 1;
                            while (j > maxLength)
                            {
                                if (num3 > 0)
                                {
                                    j -= array[--num3].Length - 1;
                                }
                                if (j <= maxLength)
                                {
                                    break;
                                }
                                if (num4 < num)
                                {
                                    j -= array[++num4].Length - 1;
                                }
                                if (num3 == 0 && num4 == num)
                                {
                                    break;
                                }
                            }
                            for (int i = 0; i < num3; i++)
                            {
                                text = text + array[i] + '\\';
                            }
                            text += "...\\";
                            for (int i = num4; i < num; i++)
                            {
                                text = text + array[i] + '\\';
                            }
                            result = text + array[num];
                        }
                    }
                }
            }
            return result;
        }

        private void ExecuteShowIO()
        {
            ShowIO = !ShowIO;
        }

        private void ShowFindReplace()
        {
            // FindandReplaceControl findandReplaceControl = new FindandReplaceControl(MainWindow.Instance);
            //findandReplaceControl.ShowDialog().GetValueOrDefault();
        }

        private void ExecuteShowSettings()
        {
            ShowSettings = !ShowSettings;
        }

        private void OnOpen(object param)
        {
            string directoryName = Path.GetDirectoryName(ActiveEditor.FilePath);
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Allfiles (*.*)|*.*",
                Multiselect = true,
                FilterIndex = 1,
                InitialDirectory = directoryName
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                _ = Open(openFileDialog.FileName);
            }
        }

        public IEditorDocument Open(string filepath)
        {
            IEditorDocument document = OpenFile(filepath);
            ActiveEditor = document;
            ActiveEditor.IsActive = true;
            return document;
        }

        private IEditorDocument OpenFile(string filepath)
        {
            IEditorDocument document = _files.FirstOrDefault(fm => fm.FileName == filepath);
            IEditorDocument result;
            if (document != null)
            {
                result = document;
            }
            else
            {
                document = AbstractLanguageClass.GetViewModel(filepath);
                if (File.Exists(filepath))
                {
                    //document
                    document.Load(filepath);
                    RecentFileList.Instance.InsertFile(filepath);
                    JumpList.AddToRecentCategory(filepath);
                }
                document.IsActive = true;
                _files.Add(document);
                ActiveEditor = document;
                result = document;
            }
            return result;
        }

        public void OpenFile(IVariable variable)
        {
            IEditorDocument document = Open(variable.Path);
            document.SelectText(variable);
        }

        public void AddNewFile()
        {
            _files.Add(new DocumentViewModel(null));
            ActiveEditor = _files.Last();
        }

        public void LoadFile(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                _ = Open(arg);
            }
        }

        private void ChangeViewAs(object param)
        {
            _ = ActiveEditor.FileLanguage;

            string text = param.ToString();

            switch (text)
            {
                case "ABB":
                    ABB abb = new ABB(ActiveEditor.FilePath);
                    ActiveEditor.FileLanguage = abb;
                    break;

                case "Fanuc":
                    Fanuc fanuc = new Fanuc(ActiveEditor.FilePath);
                    ActiveEditor.FileLanguage = fanuc;
                    break;

                case "KUKA":
                    KUKA kuka = new KUKA(ActiveEditor.FilePath);
                    ActiveEditor.FileLanguage = kuka;
                    break;

                case "Kawasaki":
                    Kawasaki kawasaki = new Kawasaki(ActiveEditor.FilePath);
                    ActiveEditor.FileLanguage = kawasaki;
                    break;
            }
        }

        private void Exit()
        {
            MainWindow.Instance.Close();
        }

        internal void Close(IEditorDocument fileToClose)
        {
            _ = _files.Remove(fileToClose);
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(ActiveEditor));
        }

        public void AddTool(ToolViewModel toolModel)
        {
            LayoutAnchorable layoutAnchorable = new LayoutAnchorable();
            if (toolModel != null)
            {
                layoutAnchorable.Title = toolModel.Title;
                layoutAnchorable.Content = toolModel;
                using (IEnumerator<ToolViewModel> enumerator = (
                    from t in Tools
                    where t.Title == toolModel.Title
                    select t).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        ToolViewModel current = enumerator.Current;
                        current.IsActive = true;
                        return;
                    }
                }
                toolModel.IsActive = true;
                _tools.Add(toolModel);
                toolModel.IsActive = true;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(Tools));
            }
        }

        [Localizable(false)]
        private void AddTool(object parameter)
        {
            string text = parameter as string;
            ToolViewModel toolModel = null;
            LayoutAnchorable layoutAnchorable = new LayoutAnchorable();
            string text2 = text;
            switch (text2)
            {
                case "Angle Converter":
                    toolModel = new AngleConvertorViewModel();
                    layoutAnchorable.AutoHideMinWidth = 219.0;
                    goto IL_1EE;
                case "Functions":
                    toolModel = new FunctionViewModel();
                    layoutAnchorable.AutoHideMinWidth = 300.0;
                    goto IL_1EE;
                case "Explorer":
                    layoutAnchorable.Content = new FileExplorerWindow();
                    goto IL_1EE;
                case "Object Browser":
                    toolModel = new ObjectBrowserViewModel();
                    goto IL_1EE;
                case "Output Window":
                    toolModel = new MessageViewModel();
                    goto IL_1EE;
                case "Notes":
                    toolModel = new NotesViewModel();
                    goto IL_1EE;
                case "ArchiveInfo":
                    toolModel = new ArchiveInfoViewModel();
                    goto IL_1EE;
                case "Rename Positions":
                    layoutAnchorable.Content = new RenamePositionWindow();
                    goto IL_1EE;
                case "Shift":
                    layoutAnchorable.Content = new ShiftWindow();
                    goto IL_1EE;
                case "CleanDat":
                    toolModel = new DatCleanHelper();
                    layoutAnchorable.AutoHideMinWidth = DatCleanHelper.Instance.Width;
                    goto IL_1EE;
            }

            ErrorMessage msg = new ErrorMessage("Not Implemented",
                string.Format("Add Tool Parameter of {0} not Implemented", text), MessageType.Error);
            _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);

        IL_1EE:
            if (toolModel != null)
            {
                layoutAnchorable.Title = toolModel.Title;
                layoutAnchorable.Content = toolModel;
                using (IEnumerator<ToolViewModel> enumerator = (
                    from t in Tools
                    where t.Title == toolModel.Title
                    select t).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        ToolViewModel current = enumerator.Current;
                        current.IsActive = true;
                        return;
                    }
                }
                toolModel.IsActive = true;
                _tools.Add(toolModel);
            }
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(Tools));
        }

        private void ImportRobot()
        {
            BringToFront("Object Browser");
            AddTool("ArchiveInfo");
        }

        public void BringToFront(string windowname)
        {
            foreach (
                LayoutAnchorable current in
                    MainWindow.Instance.DockManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                if (current.Title == windowname)
                {
                    current.IsActive = true;
                }
            }
        }

        public void ShowAbout()
        {
            _ = new AboutWindow().ShowDialog();
        }

        public bool UserSelectsFileToOpen(out string filePath)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool result;
            // ReSharper disable once PossibleInvalidOperationException
            if (openFileDialog.ShowDialog().Value)
            {
                filePath = openFileDialog.FileName;
                result = true;
            }
            else
            {
                filePath = null;
                result = false;
            }
            return result;
        }

        // ReSharper disable once UnusedMember.Global
        public bool UserSelectsNewFilePath(string oldFilePath, out string newFilePath)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool result;
            // ReSharper disable once PossibleInvalidOperationException
            if (saveFileDialog.ShowDialog().Value)
            {
                newFilePath = saveFileDialog.FileName;
                result = true;
            }
            else
            {
                newFilePath = string.Empty;
                result = false;
            }
            return result;
        }

        // ReSharper disable UnusedMember.Global
        public void ErrorMessage(string msg)
        {
            _ = MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        // ReSharper disable  UnusedMember.Local
        // ReSharper disable  UnusedParameter.Local

        private void avalonDockHost_AvalonDockLoaded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnFilesDropped(string[] files)
        {
            throw new NotImplementedException();
        }
    }

    // ReSharper enable UnusedMember.Local
    // ReSharper enable UnusedParameter.Local
}