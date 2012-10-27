namespace miRobotEditor.Core.EventArgs
{
    /// <summary>
    /// File Event Args
    /// </summary>
    public class FileEventArgs : System.EventArgs
    {
        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// IsDirectory
        /// </summary>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isDirectory"></param>
        public FileEventArgs(string fileName, bool isDirectory)
        {
            this.FileName = fileName;
            this.IsDirectory = isDirectory;
        }
    }
    /// <summary>
    /// FileCancelEventArgs
    /// </summary>
    public class FileCancelEventArgs : FileEventArgs
    {
        /// <summary>
        /// Cancel
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// OperationAlreadyDone
        /// </summary>
        public bool OperationAlreadyDone { get; set; }

        /// <summary>
        /// FileCancelEventArgs
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isDirectory"></param>
        public FileCancelEventArgs(string fileName, bool isDirectory)
            : base(fileName, isDirectory)
        {
        }
    }
    /// <summary>
    /// FileRenameEventArgs
    /// </summary>
    public class FileRenameEventArgs : System.EventArgs
    {


        /// <summary>
        /// Source File
        /// </summary>
        public string SourceFile { get; private set; }

        /// <summary>
        /// Target File
        /// </summary>
        public string TargetFile { get; private set; }


        /// <summary>
        /// Is Directory
        /// </summary>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <param name="isDirectory"></param>
        public FileRenameEventArgs(string sourceFile, string targetFile, bool isDirectory)
        {
            this.SourceFile = sourceFile;
            this.TargetFile = targetFile;
            this.IsDirectory = isDirectory;
        }
    }

    /// <summary>
    /// File Renaming Event Args
    /// </summary>
    public class FileRenamingEventArgs : FileRenameEventArgs
    {
        /// <summary>
        /// Cancel
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool OperationAlreadyDone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFile"></param>
        /// <param name="isDirectory"></param>
        public FileRenamingEventArgs(string sourceFile, string targetFile, bool isDirectory)
            : base(sourceFile, targetFile, isDirectory)
        {
        }
    }

    /// <summary>
    /// EventArgs with a file name.
    /// </summary>
    public class FileNameEventArgs : System.EventArgs
    {
        FileName fileName;
        /// <summary>
        /// Filename
        /// </summary>
        public FileName FileName
        {
            get
            {
                return fileName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public FileNameEventArgs(FileName fileName)
        {
            this.fileName = fileName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public FileNameEventArgs(string fileName)
        {
            this.fileName = FileName.Create(fileName);
        }
    }
   
}
