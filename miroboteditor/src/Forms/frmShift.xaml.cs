using System;
using System.Windows;

namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmShift.xaml
    /// </summary>
    public partial class FrmShift : Window
    {

        public FrmShift()
        {
            InitializeComponent();
        }

        public Decimal OldX { get; private set; }
        public Decimal OldY { get; private set; }
        public Decimal OldZ { get; private set; }
        public Decimal NewX { get; private set; }
        public Decimal NewY { get; private set; }
        public Decimal NewZ { get; private set; }
        public Decimal DiffX { get; private set; }
        public Decimal DiffY { get; private set; }
        public Decimal DiffZ { get; private set; }

       
    }
}
