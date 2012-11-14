using miRobotEditor.Controls.Infrastructure;

namespace miRobotEditor.GUI.ExplorerWindow
{
	public class ItemViewModel : ViewModelBase
	{
        public string Filter { get; set; }
		public string FullPath { get; set; }
		public string Name { get; set; }
		public object ImageName { get; set; }

		#region Property: IsSelected
		private bool isSelected;
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				isSelected = value;
				RaisePropertyChanged("IsSelected");

				FileExplorerViewModel.Instance.SelectedItem = FullPath;
              
			}
		}
		#endregion

	}

}
