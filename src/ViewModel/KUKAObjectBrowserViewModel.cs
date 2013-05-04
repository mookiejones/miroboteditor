/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/11/2013
 * Time: 6:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using System.Windows.Controls;
using miRobotEditor.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
namespace miRobotEditor.ViewModel
{
	/// <summary>
	/// Description of KUKAObjectBrowserViewModel.
	/// </summary>
	public class KUKAObjectBrowserViewModel:ViewModelBase
	{
		public class FunctionClass
		{
			string Name{get;set;}
			string Type {get;set;}
			string Path{get;set;}
			string IsGlobal{get;set;}
			string Info{get;set;}
		}
		public class VariableClass
			{
			string Name{get;set;}
			string Type {get;set;}
			string Path{get;set;}
			string IsGlobal{get;set;}
			string Info{get;set;}
		}
		public class EnumClass
			{
			string Name{get;set;}
			string Type {get;set;}
			string Path{get;set;}
			string IsGlobal{get;set;}
			string Info{get;set;}
		}
		public class StructureClass
			{
			string Name{get;set;}
			string Type {get;set;}
			string Path{get;set;}
			string IsGlobal{get;set;}
			string Info{get;set;}
		}
		
		
		private ObservableCollection<FunctionClass> _functionItems = new ObservableCollection<FunctionClass>();
		public ObservableCollection<FunctionClass> FunctionItems{get{return _functionItems;}set{_functionItems=value;RaisePropertyChanged("FunctionItems");}}
		private ObservableCollection<VariableClass> _variableItems = new ObservableCollection<VariableClass>();
		public ObservableCollection<VariableClass> VariableItems{get{return _variableItems;}set{_variableItems=value;RaisePropertyChanged("VariableItems");}}
		private ObservableCollection<EnumClass> _enumItems = new ObservableCollection<EnumClass>();
		public ObservableCollection<EnumClass> EnumItems{get{return _enumItems;}set{_enumItems=value;RaisePropertyChanged("EnumItems");}}
		private ObservableCollection<StructureClass> _structureItems = new ObservableCollection<StructureClass>();
		public ObservableCollection<StructureClass> StructureItems{get{return _structureItems;}set{_structureItems=value;RaisePropertyChanged("StructureItems");}}
		
		public static KUKAObjectBrowserViewModel Instance{get;set;}
		
		private  RelayCommand _clearFilterCommand;

        public  ICommand ClearFilterCommand
        {
        	get { 
        		return _clearFilterCommand ?? (_clearFilterCommand = new RelayCommand(param => FilterText = String.Empty, param => (!String.IsNullOrEmpty(FilterText)))); }
        }
        
		
		private ListViewItem _selectedItem = new ListViewItem();
		public ListViewItem SelectedItem {get{return _selectedItem;}set{_selectedItem=value;RaisePropertyChanged("SelectedItem");}}
		
		private string _filterText = String.Empty;
		public string FilterText {get{return _filterText;}set{_filterText=value;RaisePropertyChanged("FilterText");}}
		
		private string _functions = "2";
		public string Functions{get{return _functions;}set{_functions=value;RaisePropertyChanged("Functions");}}
		


		private string _variableitems = "0";
		public string VariablesItems{get{return _variableitems;}set{_variableitems=value;RaisePropertyChanged("VariableItems");}}
		
		public KUKAObjectBrowserViewModel()
		{
			Instance=this;
		}
	}
}
