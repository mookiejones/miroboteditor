using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    public class VariableHelper
    {
        public static ICollectionView PositionView { get; set; }
        public static ICollectionView PositionCollection { get; set; }

        public static Match FindMatches(Regex matchstring, string filename)
        {
            var text = File.ReadAllText(filename);
            Match result;
            if (string.IsNullOrEmpty(matchstring.ToString()))
            {
                result = null;
            }
            else
            {
                var match = matchstring.Match(text.ToLower());
                result = match;
            }
            return result;
        }

        public class VariableBase : ViewModelBase, IVariable
        {
            private string _comment = string.Empty;
            private string _declaration = string.Empty;
            private string _description = string.Empty;
            private BitmapImage _icon;
            private string _name = string.Empty;
            private int _offset = -1;
            private string _path = string.Empty;
            private string _type = string.Empty;
            private string _value = string.Empty;
            public bool IsSelected { get; set; }

            public string Description
            {
                get { return _description; }
                set
                {
                    _description = value;
                    RaisePropertyChanged("Description");
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
                    RaisePropertyChanged("Path");
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

            public int Offset
            {
                get { return _offset; }
                set
                {
                    _offset = value;
                    RaisePropertyChanged("Offset");
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
        }
    }
}