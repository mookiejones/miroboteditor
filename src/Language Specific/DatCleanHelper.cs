/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/20/2012
 * Time: 10:21
 * 
 */
using System;
using System.Globalization;
using miRobotEditor.ViewModel;
using System.Windows.Input;
using miRobotEditor.Commands;
using System.Collections.ObjectModel;
using miRobotEditor.GUI;
using miRobotEditor.Classes;
using miRobotEditor.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace miRobotEditor.Language_Specific
{
	
	//TODO Need to Tie this into Code
	
	/// <summary>
	/// Description of DatCleanHelper.
	/// </summary>
	public class DatCleanHelper:ViewModelBase
	{		
		private readonly string _filename;
		private List<IVariable> _listItems;
		public List<IVariable> ListItems
		{
			get{
				if (_listItems==null)
					GetlistItems();
				return _listItems;}
		 private	set{ _listItems = value; RaisePropertyChanged("ListItems");}
		}
	
		private void GetlistItems()
		{			
			var result = new List<IVariable>();

			var langs = Languages.KUKA.GetDatFileName(_filename);
			var items = ObjectBrowserViewModel.Instance.GetVarForFile(langs).ToList();
			result.AddRange(items);

        	
			_listItems  = result;
		}
		
		private int _progress;
		public int Progress
		{
			get{return _progress;}
			set{_progress=value;RaisePropertyChanged("Progress");}
		}
		private static DatCleanHelper _instance;
		public static DatCleanHelper Instance
		{
			get { return _instance ?? (_instance = new DatCleanHelper()); }
		    set{ _instance=value;}
		}
		
		#region Commands
		public static RelayCommand Cleandat;
        public static ICommand CleanDatCMD => Cleandat ?? (Cleandat = new RelayCommand(param => Instance.CleanDat(), param => true));
        public void CleanDat(){throw new NotImplementedException();}
		
		private static RelayCommand _checked;
        public static ICommand CheckedCMD => _checked ?? (_checked = new RelayCommand(param => Instance.Checked(), param => true));
        public void Checked()
		{
			throw new NotImplementedException();
		}
		
		private static RelayCommand _deletevartype;
        public static ICommand DeleteVarTypeCMD => _deletevartype ?? (_deletevartype = new RelayCommand(param => Instance.DeleteVarType(), param => true));
        public void DeleteVarType(){throw new NotImplementedException();}
		private static RelayCommand _addvartype;
        public static ICommand AddVarTypeCMD => _addvartype ?? (_addvartype = new RelayCommand(param => Instance.AddVarType(), param => true));
        public void AddVarType(){throw new NotImplementedException();}
		private static RelayCommand _selectallcommand;
        public static ICommand SelectAllCommand => _selectallcommand ?? (_selectallcommand = new RelayCommand(param => Instance.SelectAll(), param => true));
        private static RelayCommand _invertselection;
        public static ICommand InvertSelectionCommand => _invertselection ??
                       (_invertselection = new RelayCommand(param => Instance.InvertSelection(), param => true));
        void SelectAll()
		{
			
			
		}
		void InvertSelection(){throw new NotImplementedException();}
		#endregion

	    private const int NItemsSelected = 0;
	    private const int NItemsTotal = 0;
	    private bool _ignoretypes;
		private bool _exclusivetypes;
		private bool _deletedeclaration;
		private bool _commentdeclaration;
		private readonly ObservableCollection<String> _usedvartypes = new ObservableCollection<string> {"actual selection","actual dat","all Dat's"};
		public bool IgnoreTypes
		{
			get{return _ignoretypes;}
			set{_ignoretypes=value; RaisePropertyChanged("Ignoretypes");}
		}
		public bool ExclusiveTypes
		{
			get{return _exclusivetypes;}
			set{_exclusivetypes = value;RaisePropertyChanged("ExclusiveTypes");}
		}
		public bool DeleteDeclaration
		{
			get{return _deletedeclaration;}
			set{_deletedeclaration=value; RaisePropertyChanged("DeleteDeclaration");}
		}
		public bool CommentDeclaration
		{
			get{return _commentdeclaration;}
			set{_commentdeclaration=value;RaisePropertyChanged("CommentDeclaration");}
		}
		
		private int _selectedVarIndex;
		public int SelectedVarIndex
		{
			get{ return _selectedVarIndex;}
			set{_selectedVarIndex=value; RaisePropertyChanged("SelectedVarIndex");}
		}
        public ObservableCollection<String> UsedVarTypes => _usedvartypes;

        public string NumberSelected => String.Format("Selected:({0}/{1})", NItemsSelected.ToString(CultureInfo.InvariantCulture), NItemsTotal.ToString(CultureInfo.InvariantCulture));

        public DatCleanHelper()
		{
			Instance=this;
			_filename=DummyDoc.Instance.Filename;
		}
		
		
		public void Clean()
		{
			
		}
	}
	
}
