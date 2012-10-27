using System.Collections.Generic;
using System.IO;
namespace DMC_Robot_Editor.Classes.Archive
{
    public interface IArchiveFolder : IArchiveObject
    {
        IList<IArchiveFile> Files
        {
            get;
        }
        IList<IArchiveFolder> Folders
        {
            get;
        }
        bool FileExists(string path);
        bool FolderExists(string path);
        IArchiveFile GetFile(string path);
        IArchiveFolder GetFolder(string path);
        IArchiveFolder[] GetFolders(string filter);
        IArchiveFile[] GetFiles(string filter);
        IArchiveFile CreateFile(string path);
        IArchiveFolder CreateFolder(string path);
        IArchiveFile Add(FileInfo file);
        IArchiveFolder Add(DirectoryInfo dir);
        void Copy(DirectoryInfo dir, bool readOnly, bool overwrite);
        void Copy(IArchiveFolder destination, bool overwrite);
    }
}
