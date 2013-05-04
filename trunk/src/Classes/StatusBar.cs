using System;
using System.ComponentModel;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public class StatusBar:ViewModelBase
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
        public String Line { get { return "Ln" + _line; } set { _line = value; RaisePropertyChanged("Line"); } }
        public String Column { get { return "Col" + _column; } set { _column = value; RaisePropertyChanged("Column"); } }
        public String Offset { get { return "Offs" + _offset; } set { _offset = value; RaisePropertyChanged("Offset"); } }
        public String Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }
        public String Robot { get { return _robot; } set { _robot = value; RaisePropertyChanged("FileLanguage"); } }
        public String FileSave { get { return "Mod: " + _filesave; } set { _filesave = value; RaisePropertyChanged("FileSave"); } }
    }
}
