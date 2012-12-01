using miRobotEditor.Classes;
namespace miRobotEditor.GUI.KUKA.AdjustST
{
    class AdjustST:ViewModelBase
    {
        private ToolItems _toolItems = new ToolItems();

        public ToolItems ToolItems { get { return _toolItems; } set { _toolItems = value; OnPropertyChanged("ToolItems"); } }
    }
}
