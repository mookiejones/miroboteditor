using System;
using System.Collections.Generic;
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

    class ToolItems:List<String>
    {
        public ToolItems()
        {
            Add("All");
            Add("1");
            Add("2");
            Add("3");
            Add("4");
            Add("5");
            Add("6");
            Add("7");
            Add("8");
            Add("9");
            Add("10");
            Add("11");
            Add("12");
            Add("13");
            Add("14");
            Add("15");
            Add("16");
            Add("17");
            Add("18");
            Add("19");
            Add("20");
            Add("21");
            Add("22");
            Add("23");
            Add("24");
            Add("25");
            Add("26");
            Add("27");
            Add("28");
            Add("29");
            Add("30");
            Add("31");
            Add("32");
        }

    }
}
