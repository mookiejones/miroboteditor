using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Shell;
using Microsoft.Win32;

namespace miRobotEditor.Classes
{
    public static class FileService
    {
        static bool serviceInitialized;

        #region RecentOpen
        static RecentOpen recentOpen = null;

        public static RecentOpen RecentOpen
        {
            get
            {
                if (recentOpen == null)
                {
                    recentOpen = RecentOpen.FromXmlElement(PropertyService.Get("RecentOpen", new Properties()));
                }
                return recentOpen;
            }
        }

        static void ProjectServiceSolutionLoaded(object sender, SolutionEventArgs e)
        {
            RecentOpen.AddLastProject(e.Solution.FileName);
        }
        #endregion

        public static void Unload()
        {
            if (recentOpen != null)
            {
                PropertyService.Set("RecentOpen", recentOpen.ToProperties());
            }
            ProjectService.SolutionLoaded -= ProjectServiceSolutionLoaded;
            ParserService.LoadSolutionProjectsThreadEnded -= ParserServiceLoadSolutionProjectsThreadEnded;
            serviceInitialized = false;
        }

        internal static void InitializeService()
        {
            if (!serviceInitialized)
            {
//                ProjectService.SolutionLoaded += ProjectServiceSolutionLoaded;
  //              ParserService.LoadSolutionProjectsThreadEnded += ParserServiceLoadSolutionProjectsThreadEnded;
                serviceInitialized = true;
            }
        }

        #region OpenedFile
        static Dictionary<FileName, OpenedFile> openedFileDict = new Dictionary<FileName, OpenedFile>();

        /// <summary>
        /// Gets a collection containing all currently opened files.
        /// The returned collection is a read-only copy of the currently opened files -
        /// it will not reflect future changes of the list of opened files.
        /// </summary>
        public static ICollection<OpenedFile> OpenedFiles
        {
            get
            {
                return openedFileDict.Values.ToArray();
            }
        }

        /// <summary>
        /// Gets an opened file, or returns null if the file is not opened.
        /// </summary>
        public static OpenedFile GetOpenedFile(string fileName)
        {
            return GetOpenedFile(FileName.Create(fileName));
        }

        /// <summary>
        /// Gets an opened file, or returns null if the file is not opened.
        /// </summary>
        public static OpenedFile GetOpenedFile(FileName fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            WorkbenchSingleton.AssertMainThread();

            OpenedFile file;
            openedFileDict.TryGetValue(fileName, out file);
            return file;
        }

        /// <summary>
        /// Gets or creates an opened file.
        /// Warning: the opened file will be a file without any views attached.
        /// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
        /// unload the OpenedFile instance if no views were attached to it.
        /// </summary>
        public static OpenedFile GetOrCreateOpenedFile(string fileName)
        {
            return GetOrCreateOpenedFile(FileName.Create(fileName));
        }

        /// <summary>
        /// Gets or creates an opened file.
        /// Warning: the opened file will be a file without any views attached.
        /// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
        /// unload the OpenedFile instance if no views were attached to it.
        /// </summary>
        public static OpenedFile GetOrCreateOpenedFile(FileName fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            OpenedFile file;
            if (!openedFileDict.TryGetValue(fileName, out file))
            {
                openedFileDict[fileName] = file = new FileServiceOpenedFile(fileName);
            }
            return file;
        }

        /// <summary>
        /// Creates a new untitled OpenedFile.
        /// </summary>
        public static OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content)
        {
            if (defaultName == null)
                throw new ArgumentNullException("defaultName");

            OpenedFile file = new FileServiceOpenedFile(content);
            file.FileName = new FileName(file.GetHashCode() + "/" + defaultName);
            openedFileDict[file.FileName] = file;
            return file;
        }

        /// <summary>Called by OpenedFile.set_FileName to update the dictionary.</summary>
        internal static void OpenedFileFileNameChange(OpenedFile file, FileName oldName, FileName newName)
        {
            if (oldName == null) return; // File just created with NewFile where name is being initialized.

            LoggingService.Debug("OpenedFileFileNameChange: " + oldName + " => " + newName);

            if (openedFileDict[oldName] != file)
                throw new ArgumentException("file must be registered as oldName");
            if (openedFileDict.ContainsKey(newName))
            {
                OpenedFile oldFile = openedFileDict[newName];
                if (oldFile.CurrentView != null)
                {
                    oldFile.CurrentView.WorkbenchWindow.CloseWindow(true);
                }
                else
                {
                    throw new ArgumentException("there already is a file with the newName");
                }
            }
            openedFileDict.Remove(oldName);
            openedFileDict[newName] = file;
        }

        /// <summary>Called by OpenedFile.UnregisterView to update the dictionary.</summary>
        internal static void OpenedFileClosed(OpenedFile file)
        {
            OpenedFile existing;
            if (openedFileDict.TryGetValue(file.FileName, out existing) && existing != file)
                throw new ArgumentException("file must be registered");

            openedFileDict.Remove(file.FileName);
            LoggingService.Debug("OpenedFileClosed: " + file.FileName);
        }
        #endregion

        /// <summary>
        /// Checks if the path is valid <b>and shows a MessageBox if it is not valid</b>.
        /// Do not use in non-UI methods.
        /// </summary>
        public static bool CheckFileName(string path)
        {
            if (FileUtility.IsValidPath(path))
                return true;
            MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", path)));
            return false;
        }

        /// <summary>
        /// Checks that a single directory entry (file or subdirectory) name is valid.
        /// </summary>
        /// <param name="name">A single file name not the full path</param>
        public static bool CheckDirectoryEntryName(string name)
        {
            if (FileUtility.IsValidDirectoryEntryName(name))
                return true;
            MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", name)));
            return false;
        }

        /// <summary>
        /// Checks that a single directory name is valid.
        /// </summary>
        /// <param name="name">A single directory name not the full path</param>
        [Obsolete("Use CheckDirectoryEntryName instead")]
        public static bool CheckDirectoryName(string name)
        {
            return CheckDirectoryEntryName(name);
        }

        internal sealed class LoadFileWrapper
        {
            readonly IDisplayBinding binding;
            readonly bool switchToOpenedView;

            public LoadFileWrapper(IDisplayBinding binding, bool switchToOpenedView)
            {
                this.binding = binding;
                this.switchToOpenedView = switchToOpenedView;
            }

            public void Invoke(string fileName)
            {
                OpenedFile file = FileService.GetOrCreateOpenedFile(FileName.Create(fileName));
                try
                {
                    IViewContent newContent = binding.CreateContentForFile(file);
                    if (newContent != null)
                    {
                        DisplayBindingService.AttachSubWindows(newContent, false);
                        WorkbenchSingleton.Workbench.ShowView(newContent, switchToOpenedView);
                    }
                }
                finally
                {
                    file.CloseIfAllViewsClosed();
                }
            }
        }

        static void ParserServiceLoadSolutionProjectsThreadEnded(object sender, EventArgs e)
        {
            foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection.ToArray())
            {
                DisplayBindingService.AttachSubWindows(content, true);
            }
        }

        public static bool IsOpen(string fileName)
        {
            return GetOpenFile(fileName) != null;
        }

        /// <summary>
        /// Opens a view content for the specified file and switches to the opened view
        /// or switches to and returns the existing view content for the file if it is already open.
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
        public static IViewContent OpenFile(string fileName)
        {
            return OpenFile(fileName, true);
        }

        /// <summary>
        /// Opens a view content for the specified file
        /// or returns the existing view content for the file if it is already open.
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified file.</param>
        /// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
        public static IViewContent OpenFile(string fileName, bool switchToOpenedView)
        {
            fileName = FileUtility.NormalizePath(fileName);
            LoggingService.Info("Open file " + fileName);

            IViewContent viewContent = GetOpenFile(fileName);
            if (viewContent != null)
            {
                if (switchToOpenedView)
                {
                    viewContent.WorkbenchWindow.SelectWindow();
                }
                return viewContent;
            }

            IDisplayBinding binding = DisplayBindingService.GetBindingPerFileName(fileName);

            if (binding == null)
            {
                binding = new ErrorFallbackBinding("Could not find any display binding for " + Path.GetFileName(fileName));
            }
            if (FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, switchToOpenedView).Invoke), fileName) == FileOperationResult.OK)
            {
                FileService.RecentOpen.AddLastFile(fileName);
            }
            return GetOpenFile(fileName);
        }

        /// <summary>
        /// Opens a new unsaved file.
        /// </summary>
        /// <param name="defaultName">The (unsaved) name of the to open</param>
        /// <param name="content">Content of the file to create</param>
        public static IViewContent NewFile(string defaultName, string content)
        {
            return NewFile(defaultName, ParserService.DefaultFileEncoding.GetBytesWithPreamble(content));
        }

        /// <summary>
        /// Opens a new unsaved file.
        /// </summary>
        /// <param name="defaultName">The (unsaved) name of the to open</param>
        /// <param name="content">Content of the file to create</param>
        public static IViewContent NewFile(string defaultName, byte[] content)
        {
            if (defaultName == null)
                throw new ArgumentNullException("defaultName");
            if (content == null)
                throw new ArgumentNullException("content");

            IDisplayBinding binding = DisplayBindingService.GetBindingPerFileName(defaultName);

            if (binding == null)
            {
                binding = new ErrorFallbackBinding("Can't create display binding for file " + defaultName);
            }
            OpenedFile file = CreateUntitledOpenedFile(defaultName, content);

            IViewContent newContent = binding.CreateContentForFile(file);
            if (newContent == null)
            {
                LoggingService.Warn("Created view content was null - DefaultName:" + defaultName);
                file.CloseIfAllViewsClosed();
                return null;
            }

            DisplayBindingService.AttachSubWindows(newContent, false);

            WorkbenchSingleton.Workbench.ShowView(newContent);
            return newContent;
        }

        /// <summary>
        /// Gets a list of the names of the files that are open as primary files
        /// in view contents.
        /// </summary>
        public static IList<FileName> GetOpenFiles()
        {
            List<FileName> fileNames = new List<FileName>();
            foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection)
            {
                FileName contentName = content.PrimaryFileName;
                if (contentName != null && !fileNames.Contains(contentName))
                    fileNames.Add(contentName);
            }
            return fileNames;
        }

        /// <summary>
        /// Gets the IViewContent for a fileName. Returns null if the file is not opened currently.
        /// </summary>
        public static IViewContent GetOpenFile(string fileName)
        {
            if (fileName != null && fileName.Length > 0)
            {
                foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection)
                {
                    string contentName = content.PrimaryFileName;
                    if (contentName != null)
                    {
                        if (FileUtility.IsEqualFileName(fileName, contentName))
                            return content;
                    }
                }
            }
            return null;
        }

        public static bool DeleteToRecycleBin
        {
            get
            {
                return PropertyService.Get("SharpDevelop.DeleteToRecycleBin", true);
            }
            set
            {
                PropertyService.Set("SharpDevelop.DeleteToRecycleBin", value);
            }
        }

        public static bool SaveUsingTemporaryFile
        {
            get
            {
                return PropertyService.Get("SharpDevelop.SaveUsingTemporaryFile", true);
            }
            set
            {
                PropertyService.Set("SharpDevelop.SaveUsingTemporaryFile", value);
            }
        }

        public static int DefaultFileEncodingCodePage
        {
            get { return PropertyService.Get("SharpDevelop.DefaultFileEncoding", 65001); }
            set { PropertyService.Set("SharpDevelop.DefaultFileEncoding", value); }
        }

        static readonly ReadOnlyCollection<EncodingInfo> allEncodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToArray().AsReadOnly();

        public static ReadOnlyCollection<EncodingInfo> AllEncodings
        {
            get { return allEncodings; }
        }

        public static EncodingInfo DefaultFileEncoding
        {
            get
            {
                int cp = FileService.DefaultFileEncodingCodePage;
                return allEncodings.Single(e => e.CodePage == cp);
            }
            set
            {
                FileService.DefaultFileEncodingCodePage = value.CodePage;
            }
        }

        /// <summary>
        /// Removes a file, raising the appropriate events. This method may show message boxes.
        /// </summary>
        public static void RemoveFile(string fileName, bool isDirectory)
        {
            FileCancelEventArgs eargs = new FileCancelEventArgs(fileName, isDirectory);
            OnFileRemoving(eargs);
            if (eargs.Cancel)
                return;
            if (!eargs.OperationAlreadyDone)
            {
                if (isDirectory)
                {
                    try
                    {
                        if (Directory.Exists(fileName))
                        {
                            if (DeleteToRecycleBin)
                                NativeMethods.DeleteToRecycleBin(fileName);
                            else
                                Directory.Delete(fileName, true);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageService.ShowHandledException(e, "Can't remove directory " + fileName);
                    }
                }
                else
                {
                    try
                    {
                        if (File.Exists(fileName))
                        {
                            if (DeleteToRecycleBin)
                                NativeMethods.DeleteToRecycleBin(fileName);
                            else
                                File.Delete(fileName);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageService.ShowHandledException(e, "Can't remove file " + fileName);
                    }
                }
            }
            OnFileRemoved(new FileEventArgs(fileName, isDirectory));
        }

        /// <summary>
        /// Renames or moves a file, raising the appropriate events. This method may show message boxes.
        /// </summary>
        public static bool RenameFile(string oldName, string newName, bool isDirectory)
        {
            if (FileUtility.IsEqualFileName(oldName, newName))
                return false;
            FileChangeWatcher.DisableAllChangeWatchers();
            try
            {
                FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
                OnFileRenaming(eargs);
                if (eargs.Cancel)
                    return false;
                if (!eargs.OperationAlreadyDone)
                {
                    try
                    {
                        if (isDirectory && Directory.Exists(oldName))
                        {

                            if (Directory.Exists(newName))
                            {
                                MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                                return false;
                            }
                            Directory.Move(oldName, newName);

                        }
                        else if (File.Exists(oldName))
                        {
                            if (File.Exists(newName))
                            {
                                MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                                return false;
                            }
                            File.Move(oldName, newName);
                        }
                    }
                    catch (Exception e)
                    {
                        if (isDirectory)
                        {
                            MessageService.ShowHandledException(e, "Can't rename directory " + oldName);
                        }
                        else
                        {
                            MessageService.ShowHandledException(e, "Can't rename file " + oldName);
                        }
                        return false;
                    }
                }
                OnFileRenamed(new FileRenameEventArgs(oldName, newName, isDirectory));
                return true;
            }
            finally
            {
                FileChangeWatcher.EnableAllChangeWatchers();
            }
        }

        /// <summary>
        /// Copies a file, raising the appropriate events. This method may show message boxes.
        /// </summary>
        public static bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite)
        {
            if (FileUtility.IsEqualFileName(oldName, newName))
                return false;
            FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
            OnFileCopying(eargs);
            if (eargs.Cancel)
                return false;
            if (!eargs.OperationAlreadyDone)
            {
                try
                {
                    if (isDirectory && Directory.Exists(oldName))
                    {

                        if (!overwrite && Directory.Exists(newName))
                        {
                            MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                            return false;
                        }
                        FileUtility.DeepCopy(oldName, newName, overwrite);

                    }
                    else if (File.Exists(oldName))
                    {
                        if (!overwrite && File.Exists(newName))
                        {
                            MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                            return false;
                        }
                        File.Copy(oldName, newName, overwrite);
                    }
                }
                catch (Exception e)
                {
                    if (isDirectory)
                    {
                        MessageService.ShowHandledException(e, "Can't copy directory " + oldName);
                    }
                    else
                    {
                        MessageService.ShowHandledException(e, "Can't copy file " + oldName);
                    }
                    return false;
                }
            }
            OnFileCopied(new FileRenameEventArgs(oldName, newName, isDirectory));
            return true;
        }

        /// <summary>
        /// Opens the specified file and jumps to the specified file position.
        /// Line and column start counting at 1.
        /// </summary>
        public static IViewContent JumpToFilePosition(string fileName, int line, int column)
        {
            LoggingService.InfoFormatted("FileService\n\tJumping to File Position:  [{0} : {1}x{2}]", fileName, line, column);

            if (fileName == null || fileName.Length == 0)
            {
                return null;
            }

            NavigationService.SuspendLogging();
            bool loggingResumed = false;

            try
            {
                IViewContent content = OpenFile(fileName);
                if (content is IPositionable)
                {
                    // TODO: enable jumping to a particular view
                    content.WorkbenchWindow.ActiveViewContent = content;
                    NavigationService.ResumeLogging();
                    loggingResumed = true;
                    ((IPositionable)content).JumpTo(Math.Max(1, line), Math.Max(1, column));
                }
                else
                {
                    NavigationService.ResumeLogging();
                    loggingResumed = true;
                    NavigationService.Log(content);
                }

                return content;

            }
            finally
            {
                LoggingService.InfoFormatted("FileService\n\tJumped to File Position:  [{0} : {1}x{2}]", fileName, line, column);

                if (!loggingResumed)
                {
                    NavigationService.ResumeLogging();
                }
            }
        }

        /// <summary>
        /// Creates a FolderBrowserDialog that will initially select the
        /// specified folder. If the folder does not exist then the default
        /// behaviour of the FolderBrowserDialog is used where it selects the
        /// desktop folder.
        /// </summary>
        public static FolderBrowserDialog CreateFolderBrowserDialog(string description, string selectedPath)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = StringParser.Parse(description);
            if (selectedPath != null && selectedPath.Length > 0 && Directory.Exists(selectedPath))
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.SelectedPath = selectedPath;
            }
            return dialog;
        }

        /// <summary>
        /// Creates a FolderBrowserDialog that will initially select the
        /// desktop folder.
        /// </summary>
        public static FolderBrowserDialog CreateFolderBrowserDialog(string description)
        {
            return CreateFolderBrowserDialog(description, null);
        }

        #region Event Handlers

        static void OnFileRemoved(FileEventArgs e)
        {
            if (FileRemoved != null)
            {
                FileRemoved(null, e);
            }
        }

        static void OnFileRemoving(FileCancelEventArgs e)
        {
            if (FileRemoving != null)
            {
                FileRemoving(null, e);
            }
        }

        static void OnFileRenamed(FileRenameEventArgs e)
        {
            if (FileRenamed != null)
            {
                FileRenamed(null, e);
            }
        }

        static void OnFileRenaming(FileRenamingEventArgs e)
        {
            if (FileRenaming != null)
            {
                FileRenaming(null, e);
            }
        }

        static void OnFileCopied(FileRenameEventArgs e)
        {
            if (FileCopied != null)
            {
                FileCopied(null, e);
            }
        }

        static void OnFileCopying(FileRenamingEventArgs e)
        {
            if (FileCopying != null)
            {
                FileCopying(null, e);
            }
        }

        #endregion Event Handlers

        #region Static event firing methods

        /// <summary>
        /// Fires the event handlers for a file being created.
        /// </summary>
        /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
        /// <param name="isDirectory">Set to true if this is a directory</param>
        public static bool FireFileReplacing(string fileName, bool isDirectory)
        {
            FileCancelEventArgs e = new FileCancelEventArgs(fileName, isDirectory);
            if (FileReplacing != null)
            {
                FileReplacing(null, e);
            }
            return !e.Cancel;
        }

        /// <summary>
        /// Fires the event handlers for a file being replaced.
        /// </summary>
        /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
        /// <param name="isDirectory">Set to true if this is a directory</param>
        public static void FireFileReplaced(string fileName, bool isDirectory)
        {
            if (FileReplaced != null)
            {
                FileReplaced(null, new FileEventArgs(fileName, isDirectory));
            }
        }

        /// <summary>
        /// Fires the event handlers for a file being created.
        /// </summary>
        /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
        /// <param name="isDirectory">Set to true if this is a directory</param>
        public static void FireFileCreated(string fileName, bool isDirectory)
        {
            if (FileCreated != null)
            {
                FileCreated(null, new FileEventArgs(fileName, isDirectory));
            }
        }

        #endregion Static event firing methods

        #region Events

        public static event EventHandler<FileEventArgs> FileCreated;

        public static event EventHandler<FileRenamingEventArgs> FileRenaming;
        public static event EventHandler<FileRenameEventArgs> FileRenamed;

        public static event EventHandler<FileRenamingEventArgs> FileCopying;
        public static event EventHandler<FileRenameEventArgs> FileCopied;

        public static event EventHandler<FileCancelEventArgs> FileRemoving;
        public static event EventHandler<FileEventArgs> FileRemoved;

        public static event EventHandler<FileCancelEventArgs> FileReplacing;
        public static event EventHandler<FileEventArgs> FileReplaced;

        #endregion Events

        sealed class ErrorFallbackBinding : IDisplayBinding
        {
            string errorMessage;

            public ErrorFallbackBinding(string errorMessage)
            {
                this.errorMessage = errorMessage;
            }

            public bool CanCreateContentForFile(string fileName)
            {
                return true;
            }

            public IViewContent CreateContentForFile(OpenedFile file)
            {
                return new SimpleViewContent(errorMessage) { TitleName = Path.GetFileName(file.FileName) };
            }

            public bool IsPreferredBindingForFile(string fileName)
            {
                return false;
            }

            public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
            {
                return double.NegativeInfinity;
            }
        }
    }
    /// <summary>
    /// This class handles the recent open files and the recent open project files of SharpDevelop
    /// it checks, if the files exists at every creation, and if not it doesn't list them in the
    /// recent files, and they'll not be saved during the next option save.
    /// </summary>
    public sealed class RecentOpen
    {
        /// <summary>
        /// This variable is the maximal length of lastfile/lastopen entries
        /// must be > 0
        /// </summary>
        int MAX_LENGTH = 10;

        readonly ObservableCollection<string> lastfile = new ObservableCollection<string>();
        readonly ObservableCollection<string> lastproject = new ObservableCollection<string>();

        public IList<string> RecentFile
        {
            get
            {
                return lastfile;
            }
        }

        public IList<string> RecentProject
        {
            get
            {
                return lastproject;
            }
        }

        public RecentOpen()
        {
        }

        public RecentOpen(Globals.Services.Properties p)
        {
            // don't check whether files exist because that might be slow (e.g. if file is on network
            // drive that's unavailable)

            // if one of these entries is a string, then it's from a previous SharpDevelop version - don't try loading it
            if (p.Contains("Files") && !(p.Get("Files") is string))
            {
                lastfile.AddRange(p.Get("Files", new string[0]));
            }

            if (p.Contains("Projects") && !(p.Get("Files") is string))
            {
                lastproject.AddRange(p.Get("Projects", new string[0]));
            }
        }

        public void AddLastFile(string name)
        {
            for (int i = 0; i < lastfile.Count; ++i)
            {
                if (lastfile[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lastfile.RemoveAt(i);
                }
            }

            while (lastfile.Count >= MAX_LENGTH)
            {
                lastfile.RemoveAt(lastfile.Count - 1);
            }

            lastfile.Insert(0, name);
        }

        public void ClearRecentFiles()
        {
            lastfile.Clear();
        }

        public void ClearRecentProjects()
        {
            lastproject.Clear();
        }

        public void AddLastProject(string name)
        {
            for (int i = 0; i < lastproject.Count; ++i)
            {
                if (lastproject[i].ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lastproject.RemoveAt(i);
                }
            }

            while (lastproject.Count >= MAX_LENGTH)
            {
                lastproject.RemoveAt(lastproject.Count - 1);
            }

            lastproject.Insert(0, name);
            JumpList.AddToRecentCategory(name);
        }

        public static RecentOpen FromXmlElement(Properties properties)
        {
            return new RecentOpen(properties);
        }

        public Globals.Services.Properties ToProperties()
        {
            Globals.Services.Properties p = new Globals.Services.Properties();
            p.Set("Files", lastfile.ToArray());
            p.Set("Projects", lastproject.ToArray());
            return p;
        }

        internal void FileRemoved(object sender, FileEventArgs e)
        {
            for (int i = 0; i < lastfile.Count; ++i)
            {
                string file = lastfile[i].ToString();
                if (e.FileName == file)
                {
                    lastfile.RemoveAt(i);
                    break;
                }
            }
        }

        internal void FileRenamed(object sender, FileRenameEventArgs e)
        {
            for (int i = 0; i < lastfile.Count; ++i)
            {
                string file = lastfile[i].ToString();
                if (e.SourceFile == file)
                {
                    lastfile.RemoveAt(i);
                    lastfile.Insert(i, e.TargetFile);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// A utility class related to file utilities.
    /// </summary>
    public static partial class FileUtility
    {
        readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
        static string applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;
        const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";

        public static string ApplicationRootPath
        {
            get
            {
                return applicationRootPath;
            }
            set
            {
                applicationRootPath = value;
            }
        }

        static string GetPathFromRegistry(string key, string valueName)
        {
            using (var installRootKey = Registry.LocalMachine.OpenSubKey(key))
            {
                if (installRootKey != null)
                {
                    object o = installRootKey.GetValue(valueName);
                    if (o != null)
                    {
                        string r = o.ToString();
                        if (!string.IsNullOrEmpty(r))
                            return r;
                    }
                }
            }
            return null;
        }

        static string GetPathFromRegistryX86(string key, string valueName)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                using (var installRootKey = baseKey.OpenSubKey(key))
                {
                    if (installRootKey != null)
                    {
                        object o = installRootKey.GetValue(valueName);
                        if (o != null)
                        {
                            string r = o.ToString();
                            if (!string.IsNullOrEmpty(r))
                                return r;
                        }
                    }
                }
            }
            return null;
        }
        #region InstallRoot Properties

        static string netFrameworkInstallRoot = null;
        /// <summary>
        /// Gets the installation root of the .NET Framework (@"C:\Windows\Microsoft.NET\Framework\")
        /// </summary>
        public static string NetFrameworkInstallRoot
        {
            get {
                return netFrameworkInstallRoot ??
                       (netFrameworkInstallRoot =
                        GetPathFromRegistry(@"SOFTWARE\Microsoft\.NETFramework", "InstallRoot") ?? string.Empty);
            }
        }

        static string netSdk20InstallRoot = null;
        /// <summary>
        /// Location of the .NET 2.0 SDK install root.
        /// </summary>
        public static string NetSdk20InstallRoot
        {
            get {
                return netSdk20InstallRoot ??
                       (netSdk20InstallRoot =
                        GetPathFromRegistry(@"SOFTWARE\Microsoft\.NETFramework", "sdkInstallRootv2.0") ?? string.Empty);
            }
        }

        static string windowsSdk60InstallRoot = null;
        /// <summary>
        /// Location of the .NET 3.0 SDK (Windows SDK 6.0) install root.
        /// </summary>
        public static string WindowsSdk60InstallRoot
        {
            get
            {
                if (windowsSdk60InstallRoot == null)
                {
                    windowsSdk60InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk60InstallRoot;
            }
        }

        static string windowsSdk60aInstallRoot = null;
        /// <summary>
        /// Location of the Windows SDK Components in Visual Studio 2008 (.NET 3.5; Windows SDK 6.0a).
        /// </summary>
        public static string WindowsSdk60aInstallRoot
        {
            get
            {
                if (windowsSdk60aInstallRoot == null)
                {
                    windowsSdk60aInstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0a", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk60aInstallRoot;
            }
        }

        static string windowsSdk61InstallRoot = null;
        /// <summary>
        /// Location of the .NET 3.5 SDK (Windows SDK 6.1) install root.
        /// </summary>
        public static string WindowsSdk61InstallRoot
        {
            get
            {
                if (windowsSdk61InstallRoot == null)
                {
                    windowsSdk61InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.1", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk61InstallRoot;
            }
        }

        static string windowsSdk70InstallRoot = null;
        /// <summary>
        /// Location of the .NET 3.5 SP1 SDK (Windows SDK 7.0) install root.
        /// </summary>
        public static string WindowsSdk70InstallRoot
        {
            get
            {
                if (windowsSdk70InstallRoot == null)
                {
                    windowsSdk70InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.0", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk70InstallRoot;
            }
        }

        static string windowsSdk71InstallRoot = null;
        /// <summary>
        /// Location of the .NET 4.0 SDK (Windows SDK 7.1) install root.
        /// </summary>
        public static string WindowsSdk71InstallRoot
        {
            get
            {
                if (windowsSdk71InstallRoot == null)
                {
                    windowsSdk71InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk71InstallRoot;
            }
        }

        static string windowsSdk80InstallRoot = null;
        /// <summary>
        /// Location of the .NET 4.5 SDK (Windows SDK 8.0) install root.
        /// </summary>
        public static string WindowsSdk80NetFxTools
        {
            get
            {
                if (windowsSdk80InstallRoot == null)
                {
                    windowsSdk80InstallRoot = GetPathFromRegistryX86(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk80InstallRoot;
            }
        }
        #endregion

        [Obsolete("Use System.IO.Path.Combine instead")]
        public static string Combine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return String.Empty;
            }

            string result = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                result = Path.Combine(result, paths[i]);
            }
            return result;
        }

        public static bool IsUrl(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            return path.IndexOf("://", StringComparison.Ordinal) > 0;
        }

        public static bool IsEqualFileName(FileName fileName1, FileName fileName2)
        {
            return fileName1 == fileName2;
        }

        public static string GetCommonBaseDirectory(string dir1, string dir2)
        {
            if (dir1 == null || dir2 == null) return null;
            if (IsUrl(dir1) || IsUrl(dir2)) return null;

            dir1 = NormalizePath(dir1);
            dir2 = NormalizePath(dir2);

            string[] aPath = dir1.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] bPath = dir2.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var result = new StringBuilder();
            int indx = 0;
            for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
            {
                if (bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
                {
                    if (result.Length > 0) result.Append(Path.DirectorySeparatorChar);
                    result.Append(aPath[indx]);
                }
                else
                {
                    break;
                }
            }
            if (indx == 0)
                return null;
            else
                return result.ToString();
        }

        /// <summary>
        /// Searches all the .net sdk bin folders and return the path of the
        /// exe from the latest sdk.
        /// </summary>
        /// <param name="exeName">The EXE to search for.</param>
        /// <returns>The path of the executable, or null if the exe is not found.</returns>
        public static string GetSdkPath(string exeName)
        {
            string execPath;
            if (!string.IsNullOrEmpty(WindowsSdk80NetFxTools))
            {
                execPath = Path.Combine(WindowsSdk80NetFxTools, exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk71InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk71InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk70InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk70InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk61InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk61InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk60aInstallRoot))
            {
                execPath = Path.Combine(WindowsSdk60aInstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk60InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk60InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(NetSdk20InstallRoot))
            {
                execPath = Path.Combine(NetSdk20InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            return null;
        }

        /// <summary>
        /// Converts a given absolute path and a given base path to a path that leads
        /// from the base path to the absoulte path. (as a relative path)
        /// </summary>
        public static string GetRelativePath(string baseDirectoryPath, string absPath)
        {
            if (string.IsNullOrEmpty(baseDirectoryPath))
            {
                return absPath;
            }
            if (IsUrl(absPath) || IsUrl(baseDirectoryPath))
            {
                return absPath;
            }

            baseDirectoryPath = NormalizePath(baseDirectoryPath);
            absPath = NormalizePath(absPath);

            string[] bPath = baseDirectoryPath.Split(separators);
            string[] aPath = absPath.Split(separators);
            int indx = 0;
            for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
            {
                if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
                    break;
            }

            if (indx == 0)
            {
                return absPath;
            }

            StringBuilder erg = new StringBuilder();

            if (indx == bPath.Length)
            {
                //				erg.Append('.');
                //				erg.Append(Path.DirectorySeparatorChar);
            }
            else
            {
                for (int i = indx; i < bPath.Length; ++i)
                {
                    erg.Append("..");
                    erg.Append(Path.DirectorySeparatorChar);
                }
            }
            erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
            return erg.ToString();
        }

        /// <summary>
        /// Gets the normalized version of fileName.
        /// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
        /// </summary>
        public static string NormalizePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            int i;

            bool isWeb = false;
            for (i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '/' || fileName[i] == '\\')
                    break;
                if (fileName[i] == ':')
                {
                    if (i > 1)
                        isWeb = true;
                    break;
                }
            }

            char outputSeparator = isWeb ? '/' : System.IO.Path.DirectorySeparatorChar;

            StringBuilder result = new StringBuilder();
            if (isWeb == false && fileName.StartsWith(@"\\") || fileName.StartsWith("//"))
            {
                i = 2;
                result.Append(outputSeparator);
            }
            else
            {
                i = 0;
            }
            int segmentStartPos = i;
            for (; i <= fileName.Length; i++)
            {
                if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
                {
                    int segmentLength = i - segmentStartPos;
                    switch (segmentLength)
                    {
                        case 0:
                            // ignore empty segment (if not in web mode)
                            // On unix, don't ignore empty segment if i==0
                            if (isWeb || (i == 0 && Environment.OSVersion.Platform == PlatformID.Unix))
                            {
                                result.Append(outputSeparator);
                            }
                            break;
                        case 1:
                            // ignore /./ segment, but append other one-letter segments
                            if (fileName[segmentStartPos] != '.')
                            {
                                if (result.Length > 0) result.Append(outputSeparator);
                                result.Append(fileName[segmentStartPos]);
                            }
                            break;
                        case 2:
                            if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
                            {
                                // remove previous segment
                                int j;
                                for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--) ;
                                if (j > 0)
                                {
                                    result.Length = j;
                                }
                                break;
                            }
                            else
                            {
                                // append normal segment
                                goto default;
                            }
                        default:
                            if (result.Length > 0) result.Append(outputSeparator);
                            result.Append(fileName, segmentStartPos, segmentLength);
                            break;
                    }
                    segmentStartPos = i + 1; // remember start position for next segment
                }
            }
            if (isWeb == false)
            {
                if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
                {
                    result.Length -= 1;
                }
                if (result.Length == 2 && result[1] == ':')
                {
                    result.Append(outputSeparator);
                }
            }
            return result.ToString();
        }

        public static bool IsEqualFileName(string fileName1, string fileName2)
        {
            return string.Equals(NormalizePath(fileName1),
                                 NormalizePath(fileName2),
                                 StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
        {
            if (baseDirectory == null || testDirectory == null)
                return false;
            baseDirectory = NormalizePath(baseDirectory) + Path.DirectorySeparatorChar;
            testDirectory = NormalizePath(testDirectory) + Path.DirectorySeparatorChar;

            return testDirectory.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Combines baseDirectoryPath with relPath and normalizes the resulting path.
        /// </summary>
        public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
        {
            return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
        }

        public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
        {
            fileName = NormalizePath(fileName);
            oldDirectory = NormalizePath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            newDirectory = NormalizePath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (IsBaseDirectory(oldDirectory, fileName))
            {
                if (fileName.Length == oldDirectory.Length)
                {
                    return newDirectory;
                }
                return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
            }
            return fileName;
        }

        public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            foreach (string fileName in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
            }
            foreach (string directoryName in Directory.GetDirectories(sourceDirectory))
            {
                DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
            }
        }

        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
        {
            return SearchDirectoryInternal(directory, filemask, searchSubdirectories, ignoreHidden).Select(file => file.ToString()).ToList();
        }

        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
        {
            return SearchDirectory(directory, filemask, searchSubdirectories, true);
        }

        public static List<string> SearchDirectory(string directory, string filemask)
        {
            return SearchDirectory(directory, filemask, true, true);
        }

        public static IEnumerable<FileName> LazySearchDirectory(string directory, string filemask, bool searchSubdirectories = true, bool ignoreHidden = true)
        {
            return SearchDirectoryInternal(directory, filemask, searchSubdirectories, ignoreHidden);
        }

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path
        /// <paramref name="directory"/> and all subdirectories
        /// (if <paramref name="searchSubdirectories"/> is true).
        /// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
        /// </summary>
        static IEnumerable<FileName> SearchDirectoryInternal(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
        {
            // If Directory.GetFiles() searches the 8.3 name as well as the full name so if the filemask is
            // "*.xpt" it will return "Template.xpt~"
            bool isExtMatch = Regex.IsMatch(filemask, @"^\*\..{3}$");
            string ext = null;
            if (isExtMatch) ext = filemask.Remove(0, 1);
            var empty = new string[0];
            IEnumerable<string> dir = new[] { directory };

            if (searchSubdirectories)
                dir = dir.Flatten(
                    d =>
                    {
                        try
                        {
                            if (ignoreHidden)
                                return Directory.EnumerateDirectories(d).Where(IsNotHidden);
                            else
                                return Directory.EnumerateDirectories(d);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            return empty;
                        }
                    });
            foreach (var d in dir)
            {
                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(d, filemask);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                foreach (string f in files)
                {
                    if (!ignoreHidden || IsNotHidden(f))
                        yield return new FileName(f);
                }
            }
        }

        static bool IsNotHidden(string dir)
        {
            try
            {
                return (File.GetAttributes(dir) & FileAttributes.Hidden) != FileAttributes.Hidden;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // This is an arbitrary limitation built into the .NET Framework.
        // Windows supports paths up to 32k length.
        public static readonly int MaxPathLength = 260;

        /// <summary>
        /// This method checks if a path (full or relative) is valid.
        /// </summary>
        public static bool IsValidPath(string fileName)
        {
            // Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
            //        I can't find a .NET property or method for determining this variable.

            if (string.IsNullOrEmpty(fileName) || fileName.Length >= MaxPathLength)
            {
                return false;
            }

            // platform independend : check for invalid path chars

            if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }
            if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0)
            {
                return false;
            }

            if (!Regex.IsMatch(fileName, fileNameRegEx))
            {
                return false;
            }

            if (fileName[fileName.Length - 1] == ' ')
            {
                return false;
            }

            if (fileName[fileName.Length - 1] == '.')
            {
                return false;
            }

            // platform dependend : Check for invalid file names (DOS)
            // this routine checks for follwing bad file names :
            // CON, PRN, AUX, NUL, COM1-9 and LPT1-9

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (nameWithoutExtension != null)
            {
                nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
            }

            if (nameWithoutExtension == "CON" ||
                nameWithoutExtension == "PRN" ||
                nameWithoutExtension == "AUX" ||
                nameWithoutExtension == "NUL")
            {
                return false;
            }

            char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';

            return !((nameWithoutExtension.StartsWith("COM") ||
                      nameWithoutExtension.StartsWith("LPT")) &&
                     Char.IsDigit(ch));
        }

        /// <summary>
        /// Checks that a single directory name (not the full path) is valid.
        /// </summary>
        [ObsoleteAttribute("Use IsValidDirectoryEntryName instead")]
        public static bool IsValidDirectoryName(string name)
        {
            return IsValidDirectoryEntryName(name);
        }

        /// <summary>
        /// Checks that a single directory name (not the full path) is valid.
        /// </summary>
        public static bool IsValidDirectoryEntryName(string name)
        {
            if (!IsValidPath(name))
            {
                return false;
            }
            if (name.IndexOfAny(new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar, Path.VolumeSeparatorChar }) >= 0)
            {
                return false;
            }
            if (name.Trim(' ').Length == 0)
            {
                return false;
            }
            return true;
        }

        public static bool TestFileExists(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageService.ShowWarning(String.Format("Cannot find File:{0}",filename));
                return false;
            }
            return true;
        }

        public static bool IsDirectory(string filename)
        {
            if (!Directory.Exists(filename))
            {
                return false;
            }
            FileAttributes attr = File.GetAttributes(filename);
            return (attr & FileAttributes.Directory) != 0;
        }

        //TODO This code is Windows specific
        static bool MatchN(string src, int srcidx, string pattern, int patidx)
        {
            int patlen = pattern.Length;
            int srclen = src.Length;
            char next_char;

            for (; ; )
            {
                if (patidx == patlen)
                    return (srcidx == srclen);
                next_char = pattern[patidx++];
                if (next_char == '?')
                {
                    if (srcidx == src.Length)
                        return false;
                    srcidx++;
                }
                else if (next_char != '*')
                {
                    if ((srcidx == src.Length) || (src[srcidx] != next_char))
                        return false;
                    srcidx++;
                }
                else
                {
                    if (patidx == pattern.Length)
                        return true;
                    while (srcidx < srclen)
                    {
                        if (MatchN(src, srcidx, pattern, patidx))
                            return true;
                        srcidx++;
                    }
                    return false;
                }
            }
        }

        static bool Match(string src, string pattern)
        {
            if (pattern[0] == '*')
            {
                // common case optimization
                int i = pattern.Length;
                int j = src.Length;
                while (--i > 0)
                {
                    if (pattern[i] == '*')
                        return MatchN(src, 0, pattern, 0);
                    if (j-- == 0)
                        return false;
                    if ((pattern[i] != src[j]) && (pattern[i] != '?'))
                        return false;
                }
                return true;
            }
            return MatchN(src, 0, pattern, 0);
        }

        public static bool MatchesPattern(string filename, string pattern)
        {
            filename = filename.ToUpper();
            pattern = pattern.ToUpper();
            string[] patterns = pattern.Split(';');
            foreach (string p in patterns)
            {
                if (Match(filename, p))
                {
                    return true;
                }
            }
            return false;
        }

        // Observe SAVE functions
        public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            System.Diagnostics.Debug.Assert(IsValidPath(fileName));
            try
            {
                saveFile();
                RaiseFileSaved(new FileNameEventArgs(fileName));
                return FileOperationResult.OK;
            }
            catch (IOException e)
            {
                return ObservedSaveHandleException(e, saveFile, fileName, message, policy);
            }
            catch (UnauthorizedAccessException e)
            {
                return ObservedSaveHandleException(e, saveFile, fileName, message, policy);
            }
        }

        static FileOperationResult ObservedSaveHandleException(Exception e, FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
        {
            switch (policy)
            {
                case FileErrorPolicy.Inform:
                    ServiceManager.Instance.MessageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e);
                    break;
                case FileErrorPolicy.ProvideAlternative:
                    ChooseSaveErrorResult r = ServiceManager.Instance.MessageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, false);
                    if (r.IsRetry)
                    {
                        return ObservedSave(saveFile, fileName, message, policy);
                    }
                    else if (r.IsIgnore)
                    {
                        return FileOperationResult.Failed;
                    }
                    break;
            }
            return FileOperationResult.Failed;
        }

        public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedSave(saveFile,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
                                policy);
        }

        public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            System.Diagnostics.Debug.Assert(IsValidPath(fileName));
            try
            {
                string directory = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                saveFileAs(fileName);
                RaiseFileSaved(new FileNameEventArgs(fileName));
                return FileOperationResult.OK;
            }
            catch (IOException e)
            {
                return ObservedSaveHandleError(e, saveFileAs, fileName, message, policy);
            }
            catch (UnauthorizedAccessException e)
            {
                return ObservedSaveHandleError(e, saveFileAs, fileName, message, policy);
            }
        }

        static FileOperationResult ObservedSaveHandleError(Exception e, NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
        {
            switch (policy)
            {
                case FileErrorPolicy.Inform:
                    ServiceManager.Instance.MessageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e);
                    break;
                case FileErrorPolicy.ProvideAlternative:
                    ChooseSaveErrorResult r = ServiceManager.Instance.MessageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, true);
                    if (r.IsRetry)
                    {
                        return ObservedSave(saveFileAs, fileName, message, policy);
                    }
                    else if (r.IsIgnore)
                    {
                        return FileOperationResult.Failed;
                    }
                    else if (r.IsSaveAlternative)
                    {
                        return ObservedSave(saveFileAs, r.AlternativeFileName, message, policy);
                    }
                    break;
            }
            return FileOperationResult.Failed;
        }

        public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedSave(saveFileAs,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
                                policy);
        }

        // Observe LOAD functions
        public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, string message, FileErrorPolicy policy)
        {
            try
            {
                loadFile();
                OnFileLoaded(new FileNameEventArgs(fileName));
                return FileOperationResult.OK;
            }
            catch (IOException e)
            {
                return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
            }
            catch (UnauthorizedAccessException e)
            {
                return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
            }
        }

        static FileOperationResult ObservedLoadHandleException(Exception e, FileOperationDelegate loadFile, string fileName, string message, FileErrorPolicy policy)
        {
            switch (policy)
            {
                case FileErrorPolicy.Inform:
                    ServiceManager.Instance.MessageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e);
                    break;
                case FileErrorPolicy.ProvideAlternative:
                    ChooseSaveErrorResult r = ServiceManager.Instance.MessageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e, false);
                    if (r.IsRetry)
                        return ObservedLoad(loadFile, fileName, message, policy);
                    else if (r.IsIgnore)
                        return FileOperationResult.Failed;
                    break;
            }
            return FileOperationResult.Failed;
        }

        public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedLoad(loadFile,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
                                policy);
        }

        public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedLoad(new FileOperationDelegate(delegate { saveFileAs(fileName); }), fileName, message, policy);
        }

        public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedLoad(saveFileAs,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
                                policy);
        }

        static void OnFileLoaded(FileNameEventArgs e)
        {
            if (FileLoaded != null)
            {
                FileLoaded(null, e);
            }
        }

        public static void RaiseFileSaved(FileNameEventArgs e)
        {
            if (FileSaved != null)
            {
                FileSaved(null, e);
            }
        }

        public static event EventHandler<FileNameEventArgs> FileLoaded;
        public static event EventHandler<FileNameEventArgs> FileSaved;
    }
    public enum FileErrorPolicy
    {
        Inform,
        ProvideAlternative
    }

    public enum FileOperationResult
    {
        OK,
        Failed,
        SavedAlternatively
    }

    public delegate void FileOperationDelegate();

    public delegate void NamedFileOperationDelegate(string fileName);
    /// <summary>
    /// EventArgs with a file name.
    /// </summary>
    public class FileNameEventArgs : System.EventArgs
    {
        FileName fileName;

        public FileName FileName
        {
            get
            {
                return fileName;
            }
        }

        public FileNameEventArgs(FileName fileName)
        {
            this.fileName = fileName;
        }

        public FileNameEventArgs(string fileName)
        {
            this.fileName = FileName.Create(fileName);
        }
    }
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Converts a recursive data structure into a flat list.
        /// </summary>
        /// <param name="input">The root elements of the recursive data structure.</param>
        /// <param name="recursion">The function that gets the children of an element.</param>
        /// <returns>Iterator that enumerates the tree structure in preorder.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
        {
            Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
            try
            {
                stack.Push(input.GetEnumerator());
                while (stack.Count > 0)
                {
                    while (stack.Peek().MoveNext())
                    {
                        T element = stack.Peek().Current;
                        yield return element;
                        IEnumerable<T> children = recursion(element);
                        if (children != null)
                        {
                            stack.Push(children.GetEnumerator());
                        }
                    }
                    stack.Pop().Dispose();
                }
            }
            finally
            {
                while (stack.Count > 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }
    }
}
