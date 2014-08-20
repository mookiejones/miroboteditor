/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 3/28/2013
 * Time: 4:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;

namespace FanucTools
{
	/// <summary>
	/// Description of TaskHoldConditions.
	/// </summary>
	public class TaskHoldConditions:MookiesEditor.Core.ViewModelBase
	{
		public TaskHoldConditions()
		{
			Items = new ObservableCollection<TaskHoldConditions.TaskHoldItem>();
		}
		private const string TaskHoldConditionToolTip = "This is bit data. Each bit indicates why the execution of the task is\nheld. More than one bit can be set at same time. While the task is executing\r\nnormally, all bits are OFF."; 

		public ObservableCollection<TaskHoldItem> Items{get;set;}
		
		public class TaskHoldItem:ViewModelBase{
			private string _name = string.Empty;
			public string Name{get{return _name;}set{_name=value;RaisePropertyChanged("Name");}}
			private string _value = String.Empty;
			public string HexValue{get{return _value;}set{_value=value;RaisePropertyChanged("HexValue");}}
			private string _description = string.Empty;
			public string Description{get{return _description;}set{_description=value;}}			
		}
	}
}
