using System.Collections.ObjectModel;
using System.IO;

namespace miRobotEditor.GUI.ExplorerWindow
{
    public class FileExplorerDemo : FileExplorerViewModel
    {
        public FileExplorerDemo()
        {
            SelectedItems = new ObservableCollection<string>();
            Drives = new ObservableCollectionEx<DriveViewModel>();
            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                Drives.Add(new DriveViewModel(driveInfo.Name, driveInfo.RootDirectory.Name));
            }
        }
    }
}