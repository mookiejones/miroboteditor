using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
   public class VariableBase:ViewModelBase,IVariable
    {
        private BitmapImage _icon;
        private string _name;
        private string _path;
        private string _value;
        private string _type;
        private string _declaration;
        private int _offset;
        private string _comment;
        public BitmapImage Icon { get { return _icon; } set { _icon = value; OnPropertyChanged("Icon"); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public string Comment { get { return _comment; } set { _comment = value; OnPropertyChanged("Comment"); } }       
        public string Path { get { return _path; } set { _path = value; OnPropertyChanged("Path"); } }
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); } }
         public string Declaration { get { return _declaration; } set { _declaration = value; OnPropertyChanged("Declaration"); } }
        public int Offset { get { return _offset; } set { _offset = value; OnPropertyChanged("Offset"); } }

        public static List<IVariable> GetVariables(string filename,Regex regex, string iconpath)
        {
            var _result = new List<IVariable>();
            BitmapImage icon = Utilities.LoadBitmap(iconpath);
            var m = VariableHelper.FindMatches(regex, filename);
            while (m.Success)
            {
                var p = new Position();
                p.Icon = icon;
                p.Path = filename;
                p.Offset = m.Index;
                p.Type = m.Groups[1].ToString();
                p.Name = m.Groups[2].ToString();
                p.Value = m.Groups[3].ToString();
                p.Comment = m.Groups[4].ToString();
                _result.Add(p);
                m = m.NextMatch();
            }

            return _result;
        }
    }
}
