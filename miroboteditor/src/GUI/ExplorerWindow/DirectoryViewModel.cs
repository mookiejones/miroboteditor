using System;
using System.IO;
using miRobotEditor.Controls.Infrastructure;
using miRobotEditor.GUI.ExplorerWindow;

namespace miRobotEditor.Controls
{
	public class DirectoryViewModel : ItemViewModel
	{

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


		public ObservableCollectionEx<ViewModelBase> Childs { get; set; }

		public DirectoryViewModel(string fullPath, string name,bool showFiles)
		{
			FullPath = fullPath;
			Name = name;
			ImageName = "../../Resources/folder.png";
			ShowFiles = showFiles;

			Childs = new ObservableCollectionEx<ViewModelBase>();
		}

		public void UpdateView()
		{
			foreach (ViewModelBase child in Childs)
			{
				child.RaisePropertyChanged("HasCheckbox");
				child.RaisePropertyChanged("ShowIcons");

				if (child is DirectoryViewModel)
					((DirectoryViewModel)child).UpdateView();
			}

		}

		public void LoadChilds(bool showFiles)
		{
			try
			{
				foreach (string directory in Directory.GetDirectories(FullPath))
				{
					var directoryInfo = new DirectoryInfo(directory);
					Childs.Add(new DirectoryViewModel(directory, directoryInfo.Name,showFiles));
				}

				if (showFiles)
				{
					foreach (string file in Directory.GetFiles(FullPath))
					{
						var fileInfo = new FileInfo(file);
						Childs.Add(new FileViewModel(file, fileInfo.Name));
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				
			}
			
		}
	}

}
