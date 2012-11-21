/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/20/2012
 * Time: 10:21
 * 
 */
using System;
using miRobotEditor.Classes;
using System.Windows.Input;
using miRobotEditor.Commands;
using System.Collections.ObjectModel;
namespace miRobotEditor.Language_Specific
{
	
	//TODO Need to Tie this into Code
	
	/// <summary>
	/// Description of DatCleanHelper.
	/// </summary>
	public class DatCleanHelper:Classes.ViewModelBase
	{		
		private string filename;
		private System.Collections.Generic.List<IVariable> _listItems;
		public System.Collections.Generic.List<IVariable> ListItems
		{
			get{
				if (_listItems==null)
					getlistItems();
				return _listItems;}
			set{ _listItems = value; OnPropertyChanged("ListItems");}
		}
	
		private void getlistItems()
		{			
						
			
			var result = new System.Collections.Generic.List<IVariable>();
			result.AddRange(Classes.ObjectBrowserModel.Instance.GetVarForFile(Languages.KUKA.GetDatFileName(filename)));
			
			_listItems  = result;
		}
		
		private int _progress = 0;
		public int Progress
		{
			get{return _progress;}
			set{_progress=value;OnPropertyChanged("Progress");}
		}
		private static DatCleanHelper _instance;
		public static DatCleanHelper Instance
		{
			get
			{
				if (_instance==null)
					_instance= new DatCleanHelper();
				return _instance;
			}
			set{ _instance=value;}
		}
		
		#region Commands
		public static RelayCommand _cleandat;
		public static ICommand CleanDatCMD
		{
			get
			{
				if (_cleandat==null)
					_cleandat=new Commands.RelayCommand(param=>Instance.CleanDat(),param=>true);
				
				return _cleandat as ICommand;
			}
		}
		public void CleanDat(){throw new NotImplementedException();}
		public static RelayCommand _deletevartype;
		public static ICommand DeleteVarTypeCMD
		{
			get
			{
				if (_deletevartype==null)
					_deletevartype=new Commands.RelayCommand(param=>Instance.DeleteVarType(),param=>true);
				
				return _deletevartype as ICommand;
			}
		}
		public void DeleteVarType(){throw new NotImplementedException();}
		public static RelayCommand _addvartype;
		public static ICommand AddVarTypeCMD
		{
			get
			{
				if (_addvartype==null)
					_addvartype=new Commands.RelayCommand(param=>Instance.AddVarType(),param=>true);
				
				return _addvartype as ICommand;
			}
		}
		public void AddVarType(){throw new NotImplementedException();}
		public static RelayCommand _selectallcommand;
		public static ICommand SelectAllCommand
		{
			get
			{
				if (_selectallcommand==null)
					_selectallcommand=new Commands.RelayCommand(param=>Instance.SelectAll(),param=>true);
				
				return _selectallcommand as ICommand;
			}
		}
		public static RelayCommand _invertselection;
		public static ICommand InvertSelectionCommand
		{
			get
			{
				if (_invertselection==null)
					_invertselection=new Commands.RelayCommand(param=>Instance.InvertSelection(),param=>true);
				
				return _invertselection as ICommand;
			}
		}
		void SelectAll(){}
		void InvertSelection(){throw new NotImplementedException();}
		#endregion
		private int nItemsSelected = 0;
		private int nItemsTotal=0;
		private bool _ignoretypes = false;
		private bool _exclusivetypes=false;
		private bool _deletedeclaration=false;
		private bool _commentdeclaration=false;
		private ObservableCollection<String> _usedvartypes = new ObservableCollection<string>(){"actual selection","actual dat","all Dat's"};
		public bool IgnoreTypes
		{
			get{return _ignoretypes;}
			set{_ignoretypes=value; OnPropertyChanged("Ignoretypes");}
		}
		public bool ExclusiveTypes
		{
			get{return _exclusivetypes;}
			set{_exclusivetypes = value;OnPropertyChanged("ExclusiveTypes");}
		}
		public bool DeleteDeclaration
		{
			get{return _deletedeclaration;}
			set{_deletedeclaration=value; OnPropertyChanged("DeleteDeclaration");}
		}
		public bool CommentDeclaration
		{
			get{return _commentdeclaration;}
			set{_commentdeclaration=value;OnPropertyChanged("CommentDeclaration");}
		}
		
		private int _selectedVarIndex = 0;
		public int SelectedVarIndex
		{
			get{ return _selectedVarIndex;}
			set{_selectedVarIndex=value; OnPropertyChanged("SelectedVarIndex");}
		}
		public ObservableCollection<String> UsedVarTypes
		{
			get{return _usedvartypes;}
		}
		
		public string NumberSelected
		{
			get
			{
				return String.Format("Selected:({0}/{1})",nItemsSelected.ToString(),nItemsTotal.ToString());
			}
		}
		
		public DatCleanHelper()
		{
			Instance=this;
			filename=Controls.DummyDoc.Instance.File.FullName;
		}
		
		
		public void Clean()
		{
			
		}
	}
	
}
