using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    /// <summary>
    /// Cartesian Position to be used with the shift
    /// </summary>
    public class CartesianPosition:ViewModelBase
    {
        
        #region Header
        /// <summary>
        /// The <see cref="Header" /> property's name.
        /// </summary>
        public const string HeaderPropertyName = "Header";

        private string _header = string.Empty;

        /// <summary>
        /// Sets and gets the Header property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Header
        {
            get
            {
                return _header;
            }

            set
            {
                if (_header == value)
                {
                    return;
                }

                RaisePropertyChanging(HeaderPropertyName);
                _header = value;
                RaisePropertyChanged(HeaderPropertyName);
            }
        }
        #endregion

        
        #region X
        /// <summary>
        /// The <see cref="X" /> property's name.
        /// </summary>
        public const string XPropertyName = "X";

        private double _x = 0;

        /// <summary>
        /// Sets and gets the X property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                if (_x == value)
                {
                    return;
                }

                RaisePropertyChanging(XPropertyName);
                _x = value;
                RaisePropertyChanged(XPropertyName);
            }
        }
        #endregion

        
        #region Y
        /// <summary>
        /// The <see cref="Y" /> property's name.
        /// </summary>
        public const string YPropertyName = "Y";

        private double _y = 0;

        /// <summary>
        /// Sets and gets the Y property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                if (_y == value)
                {
                    return;
                }

                RaisePropertyChanging(YPropertyName);
                _y = value;
                RaisePropertyChanged(YPropertyName);
            }
        }
        #endregion

        
        #region Z
        /// <summary>
        /// The <see cref="Z" /> property's name.
        /// </summary>
        public const string ZPropertyName = "Z";

        private double _z = 0;

        /// <summary>
        /// Sets and gets the Z property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Z
        {
            get
            {
                return _z;
            }

            set
            {
                if (_z == value)
                {
                    return;
                }

                RaisePropertyChanging(ZPropertyName);
                _z = value;
                RaisePropertyChanged(ZPropertyName);
            }
        }
        #endregion
       
    }
}