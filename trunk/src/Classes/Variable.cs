using System.Windows.Media.Imaging;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Classes
{
    public class Variable:ViewModelBase,IVariable
    {
        private bool _isSelected = false;
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; RaisePropertyChanged("IsSelected"); } }
        private BitmapImage _icon = null;
        public BitmapImage Icon { get { return _icon; } set { _icon = value; RaisePropertyChanged("Icon"); } }

        private string _name=string.Empty;
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }

        private string _type = string.Empty;
        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged("Type"); } }

        private string _path = string.Empty;
        public string Path { get { return _path; } set { _path = value; RaisePropertyChanged("Path"); } }

        private string _value = string.Empty;
        public string Value { get { return _value; } set { _value = value; RaisePropertyChanged("Value"); } }

        private string _comment = string.Empty;
        public string Comment { get { return _comment; } set { _comment = value; RaisePropertyChanged("Comment"); } }

        private string _declaration = string.Empty;
        public string Declaration { get { return _declaration; } set { _declaration = value; RaisePropertyChanged("Declaration"); } }

        private int _offset = 0;
        public int Offset { get { return _offset; } set { _offset = value; RaisePropertyChanged("Offset"); } }
    }


    public class MethodVariable : Variable
    {
    }

}
