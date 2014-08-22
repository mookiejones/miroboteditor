using System.ComponentModel;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace miRobotEditor.Classes
{
    public class Variable : ViewModelBase, IVariable
    {
        private string _comment = string.Empty;
        private string _declaration = string.Empty;
        private string _description = string.Empty;
        private BitmapImage _icon;
        private bool _isSelected;
        private string _name = string.Empty;
        private int _offset;
        private string _path = string.Empty;
        private string _type = string.Empty;
        private string _value = string.Empty;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public BitmapImage Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged("Icon");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(Path);
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        public string Declaration
        {
            get { return _declaration; }
            set
            {
                _declaration = value;
                RaisePropertyChanged("Declaration");
            }
        }

        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                RaisePropertyChanged("Offset");
            }
        }
    }

    public interface IVariable
    {
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