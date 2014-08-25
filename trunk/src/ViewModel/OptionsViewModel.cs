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
using miRobotEditor.Commands;

namespace miRobotEditor.ViewModel
{
    /// <summary>
    ///     Description of OptionsViewModel.
    /// </summary>
    public class OptionsViewModel : ViewModelBase
    {
        private RelayCommand _applyCommand;
        private RelayCommand _cancelCommand;

        private RelayCommand _okCommand;

        public OptionsViewModel()
        {
            Instance = this;
        }

        public OptionsViewModel Instance { get; set; }

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(param => Apply(), param => true)); }
        }

        public ICommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand(param => Ok(), param => true)); }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => Cancel(), param => true)); }
        }

        private static void Apply()
        {
        }

        private static void Ok()
        {
        }

        private static void Cancel()
        {
        }
    }
}