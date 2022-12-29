

namespace miRobotEditor.ViewModel
{
    public sealed class SystemFunctionsViewModel : ViewModelBase
    {
        #region Structures

        private bool _structures = true;

        /// <summary>
        ///     Sets and gets the Structures property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Structures
        {
            get => _structures;

            set
            => SetProperty(ref _structures, value);
        }

        #endregion

        #region Programs

        private bool _programs = true;

        /// <summary>
        ///     Sets and gets the Programs property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Programs
        {
            get => _programs;

            set
            => SetProperty(ref _programs, value);
        }

        #endregion

        #region Functions

        private bool _functions = true;

        /// <summary>
        ///     Sets and gets the Functions property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Functions
        {
            get => _functions;

            set
            => SetProperty(ref _functions, value);
        }

        #endregion

        #region Variables

        private bool _variables = true;

        /// <summary>
        ///     Sets and gets the Variables property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Variables
        {
            get => _variables;

            set
            => SetProperty(ref _variables, value);
        }

        #endregion

        public void ShowDialog()
        {
        }
    }
}