using System;
using System.ComponentModel;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public class StatusBar:INotifyPropertyChanged
    {
        private static StatusBar _instance;
        public static StatusBar Instance
        {
            get { return _instance ?? (_instance = new StatusBar()); }
            set { _instance = value; }
        }

        private string _line;
        private string _column;
        private string _name;
        private string _offset;
        private string _robot;
        private string _filesave;
        public String Line { get { return "Ln" + _line; } set { _line = value; OnPropertyChanged("Line"); } }
        public String Column { get { return "Col" + _column; } set { _column = value; OnPropertyChanged("Column"); } }
        public String Offset { get { return "Offs" + _offset; } set { _offset = value; OnPropertyChanged("Offset"); } }
        public String Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public String Robot { get { return _robot; } set { _robot = value; OnPropertyChanged("FileLanguage"); } }
        public String FileSave { get { return "Mod: " + _filesave; } set { _filesave = value; OnPropertyChanged("FileSave"); } }



      
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
