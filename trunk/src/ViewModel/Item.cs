using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class Item : ViewModelBase
    {

        public Item(string type, string description)
        {
            Type = type;
            Description = description;
        }

        private int _index;
        public int Index { get { return _index; } set { _index = value;RaisePropertyChanged("Index"); } }
        private string _type = string.Empty;
        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged("Type"); } }

        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged("Description"); } }

        public override string ToString()
        {
            return String.Format("{0};{1}", Type, Description);
        }
    }
}