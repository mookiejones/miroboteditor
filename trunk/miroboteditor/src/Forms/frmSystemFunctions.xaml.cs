using System;
using System.Windows;

namespace miRobotEditor.Forms
{
    /// <summary>
    /// Interaction logic for frmSystemFunctions.xaml
    /// </summary>
    public partial class frmSystemFunctions : Window,IDisposable
    {
        public frmSystemFunctions()
        {
            InitializeComponent();
        }

        public bool Structures
        {
            get { return (bool)ckStructures.IsChecked; }
        }
        public bool Variables
        {
            get { return (bool)ckVariables.IsChecked; }
        }
        public bool Programs
        {
            get { return (bool)ckPrograms.IsChecked; }
        }
        public bool Functions
        {
            get { return (bool)ckFunctions.IsChecked; }
        }

        public void Dispose()
        {
            Dispose();
        }
    }
}
