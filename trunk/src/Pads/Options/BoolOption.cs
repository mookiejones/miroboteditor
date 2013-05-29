/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/10/2013
 * Time: 09:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Options
{
	/// <summary>
	/// Description of OptionsBoolDescription.
	/// </summary>
	public class BoolOption:ViewModelBase
	{
		private string _description = String.Empty;
		public string Description {get{return _description;} set{_description=value;RaisePropertyChanged("Description");}}
		
		private bool _value;
		public bool Value{get{return _value;}set{_value=value;RaisePropertyChanged("Value");}}		
	}
}
