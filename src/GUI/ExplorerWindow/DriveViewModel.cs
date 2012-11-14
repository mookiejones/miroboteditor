using System.IO;
using miRobotEditor.Controls.Infrastructure;

namespace miRobotEditor.GUI.ExplorerWindow
{
	public class DriveViewModel : ItemViewModel
	{
	   
		public bool IsReady { get; set; }
		public bool ShowDirectories { get; set; }
		public bool ShowFiles { get; set; }

		private bool isExpanded;
		public bool IsExpanded
		{
			get { return isExpanded; }
			set
			{
				if (value != isExpanded)
				{
					isExpanded = value;
					RaisePropertyChanged("IsExpanded");

					foreach (var child in Childs)
						{
							if (child is DirectoryViewModel)
							{
								((DirectoryViewModel)child).LoadChilds(true);
							}
						}				
				}
			}
		}

		public ObservableCollectionEx<ViewModelBase> Childs {get;set;}


		public DriveViewModel( string name, string fullPath)
		{
			Name = name;
			FullPath = fullPath;
			ImageName = "../../Resources/diskdrive.png";
			Childs = new ObservableCollectionEx<ViewModelBase>();

			LoadChilds();
		}

		public void UpdateView()
		{
			foreach (ViewModelBase child in Childs)
			{
				child.RaisePropertyChanged("HasCheckbox");
	
				if (child is DirectoryViewModel)
					((DirectoryViewModel)child).UpdateView();
			}

		}

		public void LoadChilds()
		{
            try
            {
                foreach (string directory in Directory.GetDirectories(Name))
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    Childs.Add(new DirectoryViewModel(directory, directoryInfo.Name, true));
                }

                foreach (string file in Directory.GetFiles(Name))
                {
                    var fileInfo = new FileInfo(file);
                    Childs.Add(new FileViewModel(file, fileInfo.Name));
                }

            }
            catch (IOException)
            {


            }
			
		}
	}

}
