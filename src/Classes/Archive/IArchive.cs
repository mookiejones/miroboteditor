using System;
using System.ComponentModel;
using System.IO;
namespace DMC_Robot_Editor.Classes.Archive
{
    public interface IArchive : IArchiveFolder, IDisposable
    {
        event EventHandler Reload;
        event CancelEventHandler Recover;
        event EventHandler StateChanged;
        bool IsSubArchive { get; }

        bool IsOpen { get; }

        bool Changed { get; }

        bool ReadOnly { get; }

        string DataSource { get; }

        bool SupportsCompression { get; }

        int Count { get; }
        void BeginUpdate();
        void EndUpdate();
        void AbortUpdate();
        void Create(string dataSource);
        void Open(string dataSource, FileAccess access);
        void Reopen(string dataSource);
        void Close();
        void Compact(string dataSource);
        void Copy(IArchive destination);
        void Rename(string dataSource, string newName);
        void Delete();
        void Delete(string dataSource);
        void Clear();
        void Export(string fileName);
        void Import(string fileName);
        IArchive CreateSubArchive(IArchiveFolder root);
    }
}
