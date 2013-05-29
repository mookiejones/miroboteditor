namespace miRobotEditor.ViewModel
{
    public class ShiftViewModel:ToolViewModel
    {
        public ShiftViewModel() : base("Shift Program")
        {
            DefaultPane = DefaultToolPane.Right;
        }

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
}
