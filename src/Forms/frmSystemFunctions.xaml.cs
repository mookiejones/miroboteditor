using System;

namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmSystemFunctions.xaml
    /// </summary>
    public partial class FrmSystemFunctions : IDisposable
    {
        public FrmSystemFunctions()
        {
            InitializeComponent();
        }

        public bool Structures
        {
            get { return ckStructures.IsChecked != null && (bool)ckStructures.IsChecked; }
        }
        public bool Variables
        {
            get { return ckVariables.IsChecked != null && (bool)ckVariables.IsChecked; }
        }
        public bool Programs
        {
            get { return ckPrograms.IsChecked != null && (bool)ckPrograms.IsChecked; }
        }
        public bool Functions
        {
            get { return ckFunctions.IsChecked != null && (bool)ckFunctions.IsChecked; }
        }

        void IDisposable.Dispose()
        {
            ((IDisposable) this).Dispose();
        }
    }
}
