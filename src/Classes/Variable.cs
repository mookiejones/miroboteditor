using System;
using System.Windows.Forms;
using miRobotEditor.Core;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class Variable : ViewModelBase, IVariable
    {

        private bool _isFiltered = false;
        public bool IsFiltered{get { return _isFiltered; }set { _isFiltered = value; RaisePropertyChanged("IsFiltered");}}

        public bool Contains(string value)
        {
            var result=false;
            value = value.ToLower();
            if (Description.ToLower().Contains(value)|Name.ToLower().Contains(value) |Type.ToLower().Contains(value) |Path.Contains(value)| Declaration.ToLower().Contains(value))
            {
                result = true;
            }
            IsFiltered = result;
            return IsFiltered;

        }
        private bool _isSelected;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; RaisePropertyChanged(); } }

        private BitmapImage _icon;

        public BitmapImage Icon { get { return _icon; } set { _icon = value; RaisePropertyChanged(); } }

        private string _description = string.Empty;

        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged(); } }

        private string _name = string.Empty;

        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged(); } }

        private string _type = string.Empty;

        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged(); } }

        private string _path = string.Empty;

        public string Path { get { return _path; } set { _path = value; RaisePropertyChanged(); } }

        private string _value = string.Empty;

        public string Value { get { return _value; } set { _value = value; RaisePropertyChanged(); } }

        private string _comment = string.Empty;

        public string Comment { get { return _comment; } set { _comment = value; RaisePropertyChanged(); } }

        private string _declaration = string.Empty;

        public string Declaration { get { return _declaration; } set { _declaration = value; RaisePropertyChanged(); } }

        private int _offset;

        public int Offset { get { return _offset; } set { _offset = value; RaisePropertyChanged(); } }
    }

    public interface IVariable
    {

        bool Contains(string value);
        bool IsSelected { get; set; }

        BitmapImage Icon { get; set; }

        string Name { get; set; }

        [Localizable(false)]
        string Type { get; set; }

        string Path { get; set; }

        string Value { get; set; }

        string Comment { get; set; }

        string Declaration { get; set; }

        string Description { get; set; }

        int Offset { get; set; }
    }
}