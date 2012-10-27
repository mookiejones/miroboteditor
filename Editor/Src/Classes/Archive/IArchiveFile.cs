using System.IO;
namespace DMC_Robot_Editor.Classes.Archive
{
    public interface IArchiveFile : IArchiveObject
    {
        Stream OpenWrite();
        Stream OpenRead();
        FileInfo Copy(DirectoryInfo dir, bool readOnly, bool overwrite);
        void Copy(FileInfo file, bool readOnly, bool overwrite);
        void Copy(IArchiveFile destination, bool overwrite);
    }
}
