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
                string Text = t.ReadToEnd();


                // Dont Include Empty Values
                if (String.IsNullOrEmpty(matchstring.ToString())) return null;

                Match m = matchstring.Match(Text.ToLower());
                return m;

            }


        }

        public class ViewModel:INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
        

       

        public class VariableBase : IVariable
        {
            public BitmapImage Icon { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Path { get; set; }
            public string Value { get; set;}
            public int Offset { get; set; }
            public string Comment { get; set; }
        }

        
    }
}
