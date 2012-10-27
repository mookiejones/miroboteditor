using System;
using System.Collections.Generic;
using System.IO;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    /// <summary>
    /// Represents an opened file.
    /// </summary>
    public abstract class OpenedFile : ICanBeDirty
    {
        protected IViewContent currentView;
        bool inLoadOperation;
        bool inSaveOperation;

        /// <summary>
        /// holds unsaved file content in memory when view containing the file was closed but no other view
        /// activated
        /// </summary>
        byte[] fileData;

        #region IsDirty
        bool isDirty;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler IsDirtyChanged;

        /// <summary>
        /// Gets/sets if the file is has unsaved changes.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;

                    if (IsDirtyChanged != null)
                    {
                        IsDirtyChanged(this, System.EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Marks the file as dirty if it currently is not in a load operation.
        /// </summary>
        public virtual void MakeDirty()
        {
            if (!inLoadOperation)
            {
                this.IsDirty = true;
            }
        }
        #endregion

        bool isUntitled;

        /// <summary>
        /// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
        /// </summary>
        public bool IsUntitled
        {
            get { return isUntitled; }
            protected set { isUntitled = value; }
        }

        FileName fileName;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public FileName FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    ChangeFileName(value);
                }
            }
        }

        protected virtual void ChangeFileName(FileName newValue)
        {
            WorkbenchSingleton.AssertMainThread();

            fileName = newValue;

            if (FileNameChanged != null)
            {
                FileNameChanged(this, System.EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the file name has changed.
        /// </summary>
        public event EventHandler FileNameChanged;

        public abstract event EventHandler FileClosed;

        /// <summary>
        /// Use this method to save the file to disk using a new name.
        /// </summary>
        public void SaveToDisk(string newFileName)
        {
            this.FileName = new FileName(newFileName);
            this.IsUntitled = false;
            SaveToDisk();
        }

        public abstract void RegisterView(IViewContent view);
        public abstract void UnregisterView(IViewContent view);

        public virtual void CloseIfAllViewsClosed()
        {
        }

        /// <summary>
        /// Forces initialization of the specified view.
        /// </summary>
        public virtual void ForceInitializeView(IViewContent view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            try
            {
                if (currentView != view)
                {
                    if (currentView == null)
                    {
                        SwitchedToView(view);
                    }
                    else
                    {
                        try
                        {
                            inLoadOperation = true;
                            using (Stream sourceStream = OpenRead())
                            {
                                view.Load(this, sourceStream);
                            }
                        }
                        finally
                        {
                            inLoadOperation = false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                view.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets the list of view contents registered with this opened file.
        /// </summary>
        public abstract IList<IViewContent> RegisteredViewContents
        {
            get;
        }

        /// <summary>
        /// Gets the view content that currently edits this file.
        /// If there are multiple view contents registered, this returns the view content that was last
        /// active. The property might return null even if view contents are registered if the last active
        /// content was closed. In that case, the file is stored in-memory and loaded when one of the
        /// registered view contents becomes active.
        /// </summary>
        public IViewContent CurrentView
        {
            get { return currentView; }
        }

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        public virtual Stream OpenRead()
        {
            if (fileData != null)
            {
                return new MemoryStream(fileData, false);
            }
            else
            {
                return new FileStream(FileName, FileMode.Open, FileAccess.Read);
            }
        }

        /// <summary>
        /// Sets the internally stored data to the specified byte array.
        /// This method should only be used when there is no current view or by the
        /// current view.
        /// </summary>
        /// <remarks>
        /// Use this method to specify the initial file content if you use a OpenedFile instance
        /// for a file that doesn't exist on disk but should be automatically created when a view
        /// with the file is saved, e.g. for .resx files created by the forms designer.
        /// </remarks>
        public virtual void SetData(byte[] fileData)
        {
            if (fileData == null)
                throw new ArgumentNullException("fileData");
            if (inLoadOperation)
                throw new InvalidOperationException("SetData cannot be used while loading");
            if (inSaveOperation)
                throw new InvalidOperationException("SetData cannot be used while saving");

            this.fileData = fileData;
        }

        /// <summary>
        /// Save the file to disk using the current name.
        /// </summary>
        public virtual void SaveToDisk()
        {
            if (IsUntitled)
                throw new InvalidOperationException("Cannot save an untitled file to disk!");

            LoggingService.Debug("Save " + FileName);
            var safeSaving = FileService.SaveUsingTemporaryFile && File.Exists(FileName);
            string saveAs = safeSaving ? FileName + ".bak" : FileName;
            using (var fs = new FileStream(saveAs, FileMode.Create, FileAccess.Write))
            {
                if (currentView != null)
                {
                    SaveCurrentViewToStream(fs);
                }
                else
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
            }
            if (safeSaving)
            {
                DateTime creationTime = File.GetCreationTimeUtc(FileName);
                File.Delete(FileName);
                try
                {
                    File.Move(saveAs, FileName);
                }
                catch (UnauthorizedAccessException)
                {
                    // sometime File.Move raise exception (TortoiseSVN, Anti-vir ?)
                    // try again after short delay
                    System.Threading.Thread.Sleep(250);
                    File.Move(saveAs, FileName);
                }
                File.SetCreationTimeUtc(FileName, creationTime);
            }
            IsDirty = false;
        }

        //		/// <summary>
        //		/// Called before saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
        //		/// </summary>
        //		public event EventHandler SavingCurrentView;
        //
        //		/// <summary>
        //		/// Called after saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
        //		/// </summary>
        //		public event EventHandler SavedCurrentView;


        void SaveCurrentViewToStream(Stream stream)
        {
            //			if (SavingCurrentView != null)
            //				SavingCurrentView(this, EventArgs.Empty);
            inSaveOperation = true;
            try
            {
                currentView.Save(this, stream);
            }
            finally
            {
                inSaveOperation = false;
            }
            //			if (SavedCurrentView != null)
            //				SavedCurrentView(this, EventArgs.Empty);
        }

        protected void SaveCurrentView()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                SaveCurrentViewToStream(memoryStream);
                fileData = memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newView"></param>
        public void SwitchedToView(IViewContent newView)
        {
            if (newView == null)
                throw new ArgumentNullException("newView");
            if (currentView == newView)
                return;
            if (currentView != null)
            {
                if (newView.SupportsSwitchToThisWithoutSaveLoad(this, currentView)
                    || currentView.SupportsSwitchFromThisWithoutSaveLoad(this, newView))
                {
                    // switch without Save/Load
                    currentView.SwitchFromThisWithoutSaveLoad(this, newView);
                    newView.SwitchToThisWithoutSaveLoad(this, currentView);

                    currentView = newView;
                    return;
                }
                SaveCurrentView();
            }
            try
            {
                inLoadOperation = true;
                Services.Properties memento = GetMemento(newView);
                using (Stream sourceStream = OpenRead())
                {
                    IViewContent oldView = currentView;
                    bool success = false;
                    try
                    {
                        currentView = newView;
                        // don't reset fileData if the file is untitled, because OpenRead() wouldn't be able to read it otherwise
                        if (this.IsUntitled == false)
                            fileData = null;
                        newView.Load(this, sourceStream);
                        success = true;
                    }
                    finally
                    {
                        // Use finally instead of catch+rethrow so that the debugger
                        // breaks at the original crash location.
                        if (!success)
                        {
                            // stay with old view in case of exceptions
                            currentView = oldView;
                        }
                    }
                }
                RestoreMemento(newView, memento);
            }
            finally
            {
                inLoadOperation = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void ReloadFromDisk()
        {
            var r = FileUtility.ObservedLoad(ReloadFromDiskInternal, FileName);
            if (r == FileOperationResult.Failed)
            {
                if (currentView != null && currentView.WorkbenchWindow != null)
                {
                    currentView.WorkbenchWindow.CloseWindow(true);
                }
            }
        }

        void ReloadFromDiskInternal()
        {
            fileData = null;
            if (currentView != null)
            {
                try
                {
                    inLoadOperation = true;
                    Services.Properties memento = GetMemento(currentView);
                    using (Stream sourceStream = OpenRead())
                    {
                        currentView.Load(this, sourceStream);
                    }
                    IsDirty = false;
                    RestoreMemento(currentView, memento);
                }
                finally
                {
                    inLoadOperation = false;
                }
            }
        }

        static Services.Properties GetMemento(IViewContent viewContent)
        {
            var mementoCapable = viewContent as IMementoCapable;
            if (mementoCapable == null)
            {
                return null;
            }
            else
            {
                return mementoCapable.CreateMemento();
            }
        }

        static void RestoreMemento(IViewContent viewContent, Services.Properties memento)
        {
            if (memento != null)
            {
                ((IMementoCapable)viewContent).SetMemento(memento);
            }
        }
    }
    /// <summary>
    /// Interface for classes that implement the IsDirty property and the DirtyChanged event.
    /// </summary>
    public interface ICanBeDirty
    {
        /// <summary>
        /// If this property returns true the content has changed since
        /// the last load/save operation.
        /// </summary>
        bool IsDirty
        {
            get;
        }

        /// <summary>
        /// Is called when the content is changed after a save/load operation
        /// and this signals that changes could be saved.
        /// </summary>
        event EventHandler IsDirtyChanged;
    }
    /// <summary>
    /// IViewContent is the base interface for "windows" in the document area of SharpDevelop.
    /// A view content is a view onto multiple files, or other content that opens like a document
    /// (e.g. the start page).
    /// </summary>
    public interface IViewContent : IDisposable, ICanBeDirty, IServiceProvider
    {
        /// <summary>
        /// This is the UI element for the view.
        /// You can use both Windows.Forms and WPF controls.
        /// </summary>
        object Control
        {
            get;
        }

        /// <summary>
        /// Gets the control which has focus initially.
        /// </summary>
        object InitiallyFocusedControl
        {
            get;
        }

        /// <summary>
        /// The workbench window in which this view is displayed.
        /// </summary>
        IWorkbenchWindow WorkbenchWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Is raised when the value of the TabPageText property changes.
        /// </summary>
        event EventHandler TabPageTextChanged;

        /// <summary>
        /// The text on the tab page when more than one view content
        /// is attached to a single window.
        /// </summary>
        string TabPageText
        {
            get;
        }

        /// <summary>
        /// The title of the view content. This normally is the title of the primary file being edited.
        /// </summary>
        string TitleName
        {
            get;
        }

        /// <summary>
        /// Is called each time the name for the content has changed.
        /// </summary>
        event EventHandler TitleNameChanged;

        /// <summary>
        /// The tooltip that will be shown when you hover the mouse over the title
        /// </summary>
        string InfoTip
        {
            get;
        }

        /// <summary>
        /// Is called each time the info tip for the content has changed.
        /// </summary>
        event EventHandler InfoTipChanged;

        /// <summary>
        /// Saves the content to the location <code>fileName</code>
        /// </summary>
        /// <remarks>
        /// When the user switches between multiple views editing the same file, a view
        /// change will trigger one view content to save that file into a memory stream
        /// and the other view content will load the file from that memory stream.
        /// </remarks>
        void Save(OpenedFile file, Stream stream);

        /// <summary>
        /// Load or reload the content of the specified file from the stream.
        /// </summary>
        /// <remarks>
        /// When the user switches between multiple views editing the same file, a view
        /// change will trigger one view content to save that file into a memory stream
        /// and the other view content will load the file from that memory stream.
        /// </remarks>
        void Load(OpenedFile file, Stream stream);

        /// <summary>
        /// Gets the list of files that are being edited using this view content.
        /// The returned collection usually is read-only.
        /// </summary>
        IList<OpenedFile> Files { get; }

        /// <summary>
        /// Gets the primary file being edited. Might return null if no file is edited.
        /// </summary>
        OpenedFile PrimaryFile { get; }

        /// <summary>
        /// Gets the name of the primary file being edited. Might return null if no file is edited.
        /// </summary>
        FileName PrimaryFileName { get; }

        /// <summary>
        /// Builds an <see cref="INavigationPoint"/> for the current position.
        /// </summary>
        INavigationPoint BuildNavPoint();

        bool IsDisposed { get; }

        event EventHandler Disposed;

        /// <summary>
        /// Gets if the view content is read-only (can be saved only when choosing another file name).
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets if the view content is view-only (cannot be saved at all).
        /// </summary>
        bool IsViewOnly { get; }

        /// <summary>
        /// Gets whether this view content should be closed when the solution is closed.
        /// </summary>
        bool CloseWithSolution { get; }

        #region Secondary view content support
        /// <summary>
        /// Gets the collection that stores the secondary view contents.
        /// </summary>
        ICollection<IViewContent> SecondaryViewContents { get; }


        /// <summary>
        /// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
        /// when switching from this view content to <paramref name="newView"/>.
        /// </summary>
        bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView);

        /// <summary>
        /// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
        /// when switching from <paramref name="oldView"/> to this view content.
        /// </summary>
        bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView);

        /// <summary>
        /// Executes an action before switching from this view content to the new view content.
        /// </summary>
        void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView);

        /// <summary>
        /// Executes an action before switching from the old view content to this view content.
        /// </summary>
        void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView);
        #endregion
    }
    /// <summary>
    /// Represents a directory path or filename.
    /// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
    /// </summary>
    public sealed class FileName : IEquatable<FileName>
    {
        readonly string normalizedFileName;

        public FileName(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (fileName.Length == 0)
                throw new ArgumentException("The empty string is not a valid FileName");
            this.normalizedFileName = FileUtility.NormalizePath(fileName);
        }

        /// <summary>
        /// Creates a FileName instance from the string.
        /// It is valid to pass null or an empty string to this method (in that case, a null reference will be returned).
        /// </summary>
        public static FileName Create(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            else
                return new FileName(fileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static implicit operator string(FileName fileName)
        {
            if (fileName != null)
                return fileName.normalizedFileName;
            else
                return null;
        }

        public override string ToString()
        {
            return normalizedFileName;
        }

        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            return Equals(obj as FileName);
        }

        public bool Equals(FileName other)
        {
            if (other != null)
                return string.Equals(normalizedFileName, other.normalizedFileName, StringComparison.OrdinalIgnoreCase);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(normalizedFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(FileName left, FileName right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(FileName left, FileName right)
        {
            return !(left == right);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator ==(FileName left, string right)
        {
            return (string)left == right;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator !=(FileName left, string right)
        {
            return (string)left != right;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator ==(string left, FileName right)
        {
            return left == (string)right;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
        public static bool operator !=(string left, FileName right)
        {
            return left != (string)right;
        }
        #endregion
    }
}
