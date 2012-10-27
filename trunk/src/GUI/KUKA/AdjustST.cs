using System.ComponentModel;
namespace miRobotEditor.GUI.KUKA.AdjustST
{



    class AdjustST:INotifyPropertyChanged
    {
        private ToolItems _toolItems = new ToolItems();

        public ToolItems ToolItems { get { return _toolItems; } set { _toolItems = value; NotifyPropertyChanged("ToolItems"); } }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
