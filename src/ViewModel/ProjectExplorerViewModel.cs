/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/11/2013
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using System.Collections.ObjectModel;
using System.IO;
namespace miRobotEditor.ViewModel
{
	/// <summary>
	/// Description of ProjectExplorer.
	/// </summary>
	public class ProjectExplorerViewModel:ViewModelBase
	{
		
		public void GetNearByFiles(string filename)
		{
			Items = new ObservableCollection<ProjectItem>();
			var d = Path.GetDirectoryName(filename);
			if (Directory.Exists(d))
				foreach(var f in Directory.GetFiles(d))
					Items.Add(new ProjectItem{FileName=f,ShortName=Path.GetFileName(f)});
		}
		
		public static ProjectExplorerViewModel Instance {get;set;}
		private ObservableCollection<ProjectItem> _items = new ObservableCollection<ProjectItem>();
		public ObservableCollection<ProjectItem> Items{get{return _items;}set{_items = value;RaisePropertyChanged("Items");}}
		public ProjectExplorerViewModel()
		{
			Instance = this;
		}
	}
	public class ProjectItem
	{
		public string ShortName{get;set;}
		public string FileName{get;set;}
		public string Folder{get;set;}
	}
}
