using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace miRobotEditor.GUI.KUKA.AdjustST
{
    [Localizable(false)]
    class AdjustST:ViewModelBase
    {
        private ToolItems _toolItems = new ToolItems();

        public ToolItems ToolItems { get { return _toolItems; } set { _toolItems = value; RaisePropertyChanged("ToolItems"); } }
    }
}
