using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using miRobotEditor.GUI;

namespace miRobotEditor.Classes
{
   public class VariableBase:ViewModelBase,IVariable
    {
       public bool IsSelected { get; set; }
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
            var result = new List<IVariable>();
            var icon = Utilities.LoadBitmap(iconpath);
            var lang = DummyDoc.Instance.FileLanguage;
            var m = VariableHelper.FindMatches(regex, filename);
            var isxyz = System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath).Contains("XYZ");
            while (m.Success)
            {
                var p = new Position
                            {
                                Icon = icon,
                                Path = filename,
                                Offset = m.Index,
                                Type = m.Groups[1].ToString(),
                                Name = m.Groups[2].ToString(),
                                Value = isxyz?lang.ExtractXYZ(m.ToString()):m.Groups[3].ToString(),
                                Comment = m.Groups[4].ToString()
                            };
                result.Add(p);
                m = m.NextMatch();
            }

            return result;
        }
    }
}
