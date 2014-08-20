using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    /// <summary>
    /// Cartesian Position to be used with the shift
    /// </summary>
    public class CartesianPosition:ViewModelBase
    {
        private string _header = string.Empty;
        public string Header { get { return _header; } set { _header = value;RaisePropertyChanged(); } }
        private double _x;
        public double X { get { return _x; } set { _x = value; RaisePropertyChanged(); } }

        private double _y;
        public double Y { get { return _y; } set { _y = value; RaisePropertyChanged(); } }

        private double _z;
        public double Z { get { return _z; } set { _z = value; RaisePropertyChanged(); } }

    }
}