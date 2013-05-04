/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/8/2013
 * Time: 8:41 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
using miRobotEditor.Commands;
using System.Windows.Input;
namespace miRobotEditor.ViewModel
{
	/// <summary>
	/// Description of OptionsViewModel.
	/// </summary>
	public class OptionsViewModel:ViewModelBase
	{
		
		public OptionsViewModel Instance{get;set;}
		public OptionsViewModel()
		{
			Instance=this;
		}
		
		
		
		 private  RelayCommand _applyCommand;
        public  ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(param => Apply(), param => true)); }
        }
        
        private  RelayCommand _okCommand;
        public  ICommand OKCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand(param => OK(), param => true)); }
        }
        
        private  RelayCommand _cancelCommand;
        public  ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => Cancel(), param => true)); }
        }
        
        void Apply(){}
        void OK(){}
        void Cancel(){}

        
	}
}
