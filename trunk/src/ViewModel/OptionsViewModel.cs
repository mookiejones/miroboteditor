/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/8/2013
 * Time: 8:41 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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


        #region ApplyCommand

        private RelayCommand _applyCommand;
        /// <summary>
        /// Gets the ApplyCommand.
        /// </summary>
        public RelayCommand ApplyCommand
        {
            get
            {
                return _applyCommand
                    ?? (_applyCommand = new RelayCommand(ExecuteApplyCommand));
            }
        }

        private void ExecuteApplyCommand()
        {
            Apply();
        }
        #endregion


        #region OKCommand

        private RelayCommand _okCommand;
        /// <summary>
        /// Gets the OKCommand.
        /// </summary>
        public RelayCommand OKCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand(ExecuteOKCommand));
            }
        }

        private void ExecuteOKCommand()
        {
            Ok();
        }
        #endregion



        #region CancelCommand

        private RelayCommand _cancelCommand;
        /// <summary>
        /// Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand(ExecuteCancelCommand));
            }
        }

        private void ExecuteCancelCommand()
        {
            Cancel();
        }
        #endregion

	    static void Apply(){}
	    static void Ok(){}
	    static void Cancel(){}

        
	}
}
