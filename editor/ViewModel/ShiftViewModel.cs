using RobotEditor.Enums;

namespace RobotEditor.ViewModel
{
    public sealed class ShiftViewModel : ToolViewModel
    {
        private static ShiftViewModel _instance;

        private CartesianPosition _diffvalues = new CartesianPosition
        {
            Header = "Difference"
        };

        private CartesianPosition _newvalues = new CartesianPosition
        {
            Header = "New Values"
        };

        private CartesianPosition _oldvalues = new CartesianPosition
        {
            Header = "Old Values"
        };

        public ShiftViewModel()
            : base("Shift Program")
        {
            DefaultPane = DefaultToolPane.Right;
        }

        public CartesianPosition OldValues
        {
            get { return _oldvalues; }
            set
            {
                _oldvalues = value;
                RaisePropertyChanged("OldValues");
            }
        }

        public CartesianPosition NewValues
        {
            get { return _newvalues; }
            set
            {
                _newvalues = value;
                RaisePropertyChanged("NewValues");
            }
        }

        public CartesianPosition DiffValues
        {
            get { return _diffvalues; }
            set
            {
                _diffvalues = value;
                RaisePropertyChanged("DiffValues");
            }
        }

        public static ShiftViewModel Instance
        {
            get { return _instance ?? new ShiftViewModel(); }
            set { _instance = value; }
        }
    }
}