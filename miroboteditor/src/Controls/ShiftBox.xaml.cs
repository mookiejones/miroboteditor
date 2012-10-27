using System.Windows.Controls;

namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for ShiftBox.xaml
    /// </summary>
    public partial class ShiftBox : UserControl
    {
        private decimal _x, _y,_z;
        public decimal X
        {
            get { return _x; }
            set { _x = value; }
        }
        public decimal Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public decimal Z
        {
            get { return _z; }
            set { _z = value; }
        }
        public ShiftBox()
        {
            InitializeComponent();
        }
    }
}
