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


        void IDisposable.Dispose()
        {
           
            GC.SuppressFinalize(this);
        }
    }
  
}
