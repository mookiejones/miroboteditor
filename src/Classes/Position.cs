using System.Collections.Generic;
using System.Windows.Media.Imaging;
using miRobotEditor.GUI;

namespace miRobotEditor.Classes
{
    public class Position : ViewModelBase,IVariable
    {
        private BitmapImage _icon;
        private string _name;
        private string _path;
        private string _value;
        private string _comment;
        private string _declaration;

        private string _type;
        private int _offset;
        public BitmapImage Icon { get { return _icon; } set { _icon = value; OnPropertyChanged("Icon"); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public string Path { get { return _path; } set { _path = value; OnPropertyChanged("Path"); } }
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); } }
        public int Offset { get { return _offset; } set { _offset = value; OnPropertyChanged("Offset"); } }
        public string Comment { get { return _comment; } set { _comment = value; OnPropertyChanged("Comment"); } }
        public string Declaration { get { return _declaration; } set { _declaration = value; OnPropertyChanged("Declaration"); } }
        public List<IVariable> GetPositions(string filename)
        {
            var result = new List<IVariable>();
            BitmapImage icon = Utilities.LoadBitmap(Global.ImgXyz);
            var m = VariableHelper.FindMatches(DummyDoc.Instance.FileLanguage.XYZRegex, filename);

            while (m.Success)
            {
                var p = new Position
                            {
                                Icon = icon,
                                Path = filename,
                                Offset = m.Index,
                                Type = m.Groups[1].ToString(),
                                Name = m.Groups[2].ToString()
                            };
                result.Add(p);
                m = m.NextMatch();
            }
            return result;
        }
     
    }

   
}
