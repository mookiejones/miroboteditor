

namespace miRobotEditor.ViewModel
{
    public sealed class CartesianPosition : ViewModelBase
    {
        #region Header

        private string _header = string.Empty;

        /// <summary>
        ///     Sets and gets the Header property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Header
        {
            get => _header;

            set
            => SetProperty(ref _header, value);
        }

        #endregion

        #region X

        private double _x;

        /// <summary>
        ///     Sets and gets the X property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double X
        {
            get => _x;

            set => SetProperty(ref _x, value);

        }

        #endregion

        #region Y

        private double _y;

        /// <summary>
        ///     Sets and gets the Y property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double Y
        {
            get => _y;

            set => SetProperty(ref _y, value);

        }

        #endregion

        #region Z

        private double _z;

        /// <summary>
        ///     Sets and gets the Z property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double Z
        {
            get => _z;

            set => SetProperty(ref _z, value);

        }

        #endregion
    }
}