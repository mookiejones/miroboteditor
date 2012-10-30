using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public class Function :ViewModelBase, IVariable
    {
        public string Scope { get; set; }
        public string Returns { get; set; }
        public BitmapImage Icon { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int Offset { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }

        public List<Function> GetFunctions(string filename)
        {

            var result = new List<Function>();
            BitmapImage icon = Utilities.LoadBitmap(Global.imgMethod);
            var m = VariableHelper.FindMatches(DummyDoc.ActiveEditor.FileLanguage.MethodRegex, filename);

            while (m.Success)
            {
                var f = new Function();
                f.Offset = m.Index;
                f.Icon = icon;
                f.Path = filename;
                f.Type = m.Groups[1].ToString();
                f.Name = m.Groups[2].ToString();
                result.Add(f);
                m = m.NextMatch();
            }

            return result;
        }

       
    }
}
