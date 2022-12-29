

namespace miRobotEditor.ViewModel
{
    public sealed class ProgressBarViewModel : ViewModelBase
    {
        #region Minimum



        private int _minimum;

        /// <summary>
        ///     Sets and gets the Minimum property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Minimum
        {
            get => _minimum;

            set => SetProperty(ref _minimum, value);
        }

        #endregion

        #region Maximum

        private int _maximum = 100;

        /// <summary>
        ///     Sets and gets the Maximum property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Maximum
        {
            get => _maximum;

            set => SetProperty(ref _maximum, value);

        }

        #endregion

        #region Value

        private int _value;

        /// <summary>
        ///     Sets and gets the Value property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Value
        {
            get => _value;

            set => SetProperty(ref _value, value);

        }

        #endregion

        #region IsVisible

        private bool _isVisibile;

        /// <summary>
        ///     Sets and gets the IsVisible property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisibile;

            set => SetProperty(ref _isVisibile, value);
        }

        #endregion
    }
}