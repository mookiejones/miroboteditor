using GalaSoft.MvvmLight;

namespace RobotEditor.ViewModel
{
    public sealed class SystemFunctionsViewModel : ViewModelBase
    {
        #region Structures

        /// <summary>
        ///     The <see cref="Structures" /> property's name.
        /// </summary>
        private const string StructuresPropertyName = "Structures";

        private bool _structures = true;

        /// <summary>
        ///     Sets and gets the Structures property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Structures
        {
            get { return _structures; }

            set
            {
                if (_structures == value)
                {
                    return;
                }

                
                _structures = value;
                RaisePropertyChanged(StructuresPropertyName);
            }
        }

        #endregion

        #region Programs

        /// <summary>
        ///     The <see cref="Programs" /> property's name.
        /// </summary>
        private const string ProgramsPropertyName = "Programs";

        private bool _programs = true;

        /// <summary>
        ///     Sets and gets the Programs property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Programs
        {
            get { return _programs; }

            set
            {
                if (_programs == value)
                {
                    return;
                }

                
                _programs = value;
                RaisePropertyChanged(ProgramsPropertyName);
            }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     The <see cref="Functions" /> property's name.
        /// </summary>
        private const string FunctionsPropertyName = "Functions";

        private bool _functions = true;

        /// <summary>
        ///     Sets and gets the Functions property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Functions
        {
            get { return _functions; }

            set
            {
                if (_functions == value)
                {
                    return;
                }

                
                _functions = value;
                RaisePropertyChanged(FunctionsPropertyName);
            }
        }

        #endregion

        #region Variables

        /// <summary>
        ///     The <see cref="Variables" /> property's name.
        /// </summary>
        private const string VariablesPropertyName = "Variables";

        private bool _variables = true;

        /// <summary>
        ///     Sets and gets the Variables property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool Variables
        {
            get { return _variables; }

            set
            {
                if (_variables == value)
                {
                    return;
                }

                
                _variables = value;
                RaisePropertyChanged(VariablesPropertyName);
            }
        }

        #endregion

        public void ShowDialog()
        {
        }
    }
}