using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class VariableHelper
    {

        public static ICollectionView PositionView { get; set; }
        public static ICollectionView PositionCollection { get; set; }
        private static List<Position> _positions = new List<Position>() ;
        public static List<Position> Positions { get { return _positions; } set { _positions = value; } }
        private static List<IVariable> _functions = new List<IVariable>();
        public static List<IVariable> Functions { get { return _functions; } set { _functions = value; } }
        private static List<IVariable> _fields= new List<IVariable>();
        public static List<IVariable> Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        //public static void GetPositions(string filename)
        //{
        //    BitmapImage icon = Utilities.LoadBitmap(Global.imgXYZ);
        //    var m = FindMatches(DummyDoc.ActiveEditor.FileLanguage.XYZRegex, filename);
        
        //    while (m.Success)
        //    {
        //        var p = new Position();
        //        p.Icon = icon;
        //        p.Path = filename;
        //        p.Offset = m.Index;
        //        p.Type = m.Groups[1].ToString();
        //        p.Name = m.Groups[2].ToString();
        //        Positions.Add(p);
        //        m = m.NextMatch();
        //    }

        //}
        //public static void GetFields(string filename)
        //{
        //    BitmapImage icon = Utilities.LoadBitmap(Global.imgXYZ);
        //    var m = FindMatches(DummyDoc.ActiveEditor.FileLanguage.FieldRegex, filename);

        //    while (m.Success)
        //    {
        //        var f = new VariableBase();
        //        f.Icon = icon;
        //        f.Path = filename;
        //        f.Type = m.Groups[1].ToString();
        //        f.Name = m.Groups[2].ToString();
        //        Fields.Add(f);
        //        m = m.NextMatch();
        //    }

        //}

        //public static void GetFunctions(string filename)
        //{
        //    BitmapImage icon = Utilities.LoadBitmap(Global.imgMethod);
        //    var m = FindMatches(DummyDoc.ActiveEditor.FileLanguage.MethodRegex, filename);

        //    while (m.Success)
        //    {
        //        var f = new Function();
        //        var s = m.ToString().Split(' ');
        //        f.Offset = m.Index;
        //        f.Icon = icon;
        //        f.Path = filename;
        //        f.Type = m.Groups[1].ToString();
        //        f.Name = m.Groups[2].ToString();
        //        Functions.Add(f);
        //        m = m.NextMatch();
        //    }
        //}

        public static Match FindMatches(Regex matchstring, string filename)
        {
            using (var t = new StreamReader(filename))
            {
                var text = t.ReadToEnd();


                // Dont Include Empty Values
                if (String.IsNullOrEmpty(matchstring.ToString())) return null;

                var m = matchstring.Match(text.ToLower());
                return m;

            }


        }


        public class VariableBase : ViewModelBase,IVariable
        {
            public bool IsSelected { get; set; }
        	private BitmapImage _icon;
        	private string _name = string.Empty;
        	private string _type = string.Empty;
        	private string _path = string.Empty;
        	private string _value = string.Empty;
        	private string _comment = string.Empty;
        	private string _declaration = string.Empty;
        	private int _offset = -1;
        	public BitmapImage Icon { get{return _icon;} set{_icon = value;RaisePropertyChanged("Icon");} }
        	public string Name { get{return _name;} set{_name=value;RaisePropertyChanged("Name");} }
            public string Type { get{return _type;} set{_type=value;RaisePropertyChanged("Type");} }
            public string Path { get{return _path;} set{_path=value;RaisePropertyChanged("Path");} }
            public string Value { get{return _value;} set{_value=value;RaisePropertyChanged("Value");} }
            public int Offset { get{return _offset;} set{_offset=value;RaisePropertyChanged("Offset");} }
            public string Comment { get{return _comment;} set{_comment=value;RaisePropertyChanged("Comment");} }
            public string Declaration { get{return _declaration;} set{_declaration=value;RaisePropertyChanged("Declaration");} }

        }

        
    }
}
