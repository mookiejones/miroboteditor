using System.Collections.ObjectModel;
using System.IO;
using miRobotEditor.Controls.Infrastructure;

namespace miRobotEditor.GUI.ExplorerWindow
{
	public class FileExplorerViewModel : ViewModelBase
    {
        #region Property:Filter

	    private string _filter;
	    public string Filter
	    {
	        get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged("Filter");
            }
	    }

        #endregion

        #region Property: SelectedItem
        private string selectedItem;
		public string SelectedItem
		{
			get
			{
				return selectedItem;
			}
			set
			{
				selectedItem = value;
				RaisePropertyChanged("SelectedItem");
			}
		}
		#endregion
		
		#region Property: SelectedItems
		private ObservableCollection<string> selectedItems;
		public ObservableCollection<string> SelectedItems
		{
			get
			{
				return selectedItems;
			}
			set
			{
				selectedItems = value;
				RaisePropertyChanged("SelectedItems");               
			}
		}
		#endregion

		private static FileExplorerViewModel instance;
		public static FileExplorerViewModel Instance
		{
			get { return instance ?? (instance = new FileExplorerViewModel()); }
		}

	    public ObservableCollectionEx<DriveViewModel> Drives { get; set; }

	    public void Initialize()
		{
			SelectedItems = new ObservableCollection<string>();
			Drives = new ObservableCollectionEx<DriveViewModel>();
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				Drives.Add(new DriveViewModel(driveInfo.Name, driveInfo.RootDirectory.Name));
			}
		    UpdateView();
		}

		private void UpdateView()
		{
			if (Drives == null) return;

			foreach (var driveViewModel in Drives)
			{
				driveViewModel.UpdateView();
			}
		}
	}

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
