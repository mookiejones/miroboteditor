namespace miRobotEditor.ViewModel
{
    public class ShiftViewModel:ViewModelBase
    {

        private CartesianPosition _oldvalues = new CartesianPosition {Header="Old Values"};
        public CartesianPosition OldValues { get { return _oldvalues; } set { _oldvalues = value; RaisePropertyChanged("OldValues"); } }
        private CartesianPosition _newvalues = new CartesianPosition{Header="New Values"};
        public CartesianPosition NewValues { get { return _newvalues; } set { _newvalues = value; RaisePropertyChanged("NewValues"); } }
        private CartesianPosition _diffvalues = new CartesianPosition{Header="Difference"};
        public CartesianPosition DiffValues { get { return _diffvalues; } set { _diffvalues = value; RaisePropertyChanged("DiffValues"); } }

       
        private static ShiftViewModel _instance;
        public static ShiftViewModel Instance { get { return _instance ?? new ShiftViewModel(); }
            set
            {
                _instance = value;
             
            }
        }
    }

    public class CartesianPosition:ViewModelBase
    {
        private string _header = string.Empty;
        public string Header { get { return _header; } set { _header = value;RaisePropertyChanged("Header"); } }
        private double _x = 0.0;
        public double X { get { return _x; } set { _x = value; RaisePropertyChanged("X"); } }

        private double _y = 0.0;
        public double Y { get { return _y; } set { _y = value; RaisePropertyChanged("Y"); } }

        private double _z = 0.0;
        public double Z { get { return _z; } set { _z = value; RaisePropertyChanged("Z"); } }

    }
}
