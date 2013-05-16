using System;
using System.Windows;


namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmShift.xaml
    /// </summary>
    public partial class FrmShift : Window
    {
        private static FrmShift _instance;
        public static FrmShift Instance
        {
            get { return _instance ?? (_instance = new FrmShift()); }
            set { _instance = value; }
        }
        public FrmShift()
        {
            InitializeComponent();           
        }
    }

}
