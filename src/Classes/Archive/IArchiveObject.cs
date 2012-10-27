using System.IO;
namespace DMC_Robot_Editor.Classes.Archive
{
    public interface IArchiveObject
    {
        IArchive Archive { get; }

        IArchiveFolder Parent { get; }

        string Name { get; set; }

        string FolderName { get; }

        string FullName { get; }

        FileAttributes Attributes { get; set; }

        bool Exists { get; }
        void Remove();
        void Create();
        ArchiveStatistic Statistic();
    }
}
