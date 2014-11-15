using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    public sealed class ProgressBarViewModel : ViewModelBase
    {
        #region Minimum

        /// <summary>
        ///     The <see cref="Minimum" /> property's name.
        /// </summary>
        private const string MinimumPropertyName = "Minimum";

        private int _minimum;

        /// <summary>
        ///     Sets and gets the Minimum property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Minimum
        {
            get { return _minimum; }

            set
            {
                if (_minimum == value)
                {
                    return;
                }

                
                _minimum = value;
                RaisePropertyChanged(MinimumPropertyName);
            }
        }

        #endregion

        #region Maximum

        /// <summary>
        ///     The <see cref="Maximum" /> property's name.
        /// </summary>
        private const string MaximumPropertyName = "Maximum";

        private int _maximum = 100;

        /// <summary>
        ///     Sets and gets the Maximum property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Maximum
        {
            get { return _maximum; }

            set
            {
                if (_maximum == value)
                {
                    return;
                }

                
                _maximum = value;
                RaisePropertyChanged(MaximumPropertyName);
            }
        }

        #endregion

        #region Value

        /// <summary>
        ///     The <see cref="Value" /> property's name.
        /// </summary>
        private const string ValuePropertyName = "Value";

        private int _value;

        /// <summary>
        ///     Sets and gets the Value property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Value
        {
            get { return _value; }

            set
            {
                if (_value == value)
                {
                    return;
                }

                
                _value = value;
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        #endregion

        #region IsVisible

        /// <summary>
        ///     The <see cref="IsVisible" /> property's name.
        /// </summary>
        private const string IsVisiblePropertyName = "IsVisible";

        private bool _isVisibile;

        /// <summary>
        ///     Sets and gets the IsVisible property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisibile; }

            set
            {
                if (_isVisibile == value)
                {
                    return;
                }

                
                _isVisibile = value;
                RaisePropertyChanged(IsVisiblePropertyName);
            }
        }

        #endregion
    }
}