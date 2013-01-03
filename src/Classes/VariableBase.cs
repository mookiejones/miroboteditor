using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Imaging;
using miRobotEditor.GUI;
using miRobotEditor.Languages;

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
        public BitmapImage Icon { get { return _icon; } set { _icon = value; RaisePropertyChanged("Icon"); } }
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }
        public string Comment { get { return _comment; } set { _comment = value; RaisePropertyChanged("Comment"); } }       
        public string Path { get { return _path; } set { _path = value; RaisePropertyChanged("Path"); } }
        public string Value { get { return _value; } set { _value = value; RaisePropertyChanged("Value"); } }
        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged("Type"); } }
         public string Declaration { get { return _declaration; } set { _declaration = value; RaisePropertyChanged("Declaration"); } }
        public int Offset { get { return _offset; } set { _offset = value; RaisePropertyChanged("Offset"); } }

       private static List<IVariable> _positions = new List<IVariable>();

       public static List<IVariable> Positions
       {
           get { return _positions; }
           set
           {
               _positions = value;             
           }
       }
        public static List<IVariable> Locals
        {
            get;
            set;
        }
       internal class WorkerArgs
       {
           public string Filename { get; set; }
           public AbstractLanguageClass Lang { get; set; }
           public string IconPath { get; set; }
       }


       public static void GetLocals(string filename,AbstractLanguageClass lang,string iconpath)
       {
           var localworker = new BackgroundWorker();
           localworker.DoWork += LocalworkerDoWork;
           localworker.RunWorkerCompleted += LocalworkerRunWorkerCompleted;
           localworker.RunWorkerAsync(new WorkerArgs {Filename = filename, Lang = lang, IconPath = iconpath});
       }

       static void LocalworkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
       {
          // throw new System.NotImplementedException();
       }

       static void LocalworkerDoWork(object sender, DoWorkEventArgs e)
       {
           var s = e.Argument as WorkerArgs;
           if (System.IO.File.Exists(s.Filename))
           {
               if (Locals == null)
                   Locals = new List<IVariable>();
               var lang = DummyDoc.Instance.FileLanguage;
               Locals.AddRange(GetVariables(s.Filename, lang.XYZRegex, Global.ImgXyz));
               Locals.AddRange(GetVariables(s.Filename, lang.FieldRegex, Global.ImgField));
           }

       }

       public static void GetPositions(string filename,AbstractLanguageClass lang,string iconpath)
       {
           var backgroundworker = new BackgroundWorker();
           backgroundworker.DoWork += BackgroundworkerDoWork;
           backgroundworker.RunWorkerCompleted += BackgroundworkerRunWorkerCompleted;
           backgroundworker.RunWorkerAsync(new WorkerArgs {Filename = filename, Lang =lang, IconPath = iconpath});
       }



       static  void BackgroundworkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
       {
          
       }

       static void BackgroundworkerDoWork(object sender, DoWorkEventArgs e)
       {
           var s = e.Argument as WorkerArgs;
           var icon = Utilities.LoadBitmap(s.IconPath);
           var lang = DummyDoc.Instance.FileLanguage;
           var m = VariableHelper.FindMatches(s.Lang.XYZRegex, s.Filename);
           var isxyz =
               System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath).
                   Contains("XYZ");
           while (m.Success)
           {
               var p = new Position
               {
                   Icon = icon,
                   Path = s.Filename,
                   Offset = m.Index,
                   Type = m.Groups[1].ToString(),
                   Name = m.Groups[2].ToString(),
                   Value =
                       isxyz
                           ? lang.ExtractXYZ(m.ToString())
                           : m.Groups[3].ToString(),
                   Comment = m.Groups[4].ToString()
               };
               Positions.Add(p);
               m = m.NextMatch();
           }
       }

       public static List<IVariable> GetVariables(string filename, Regex regex, string iconpath)
       {
           //GetPositions(filename, DummyDoc.Instance.FileLanguage, iconpath);
           var result = new List<IVariable>();
           var icon = Utilities.LoadBitmap(iconpath);
           var lang = DummyDoc.Instance.FileLanguage;
           var m = VariableHelper.FindMatches(regex, filename);
           var isxyz =System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath).Contains("XYZ");
           if (m==null)
           {
               var s = new StringBuilder();
               MessageViewModel.Instance.Add("Variable for " + lang.RobotType.ToString(),
                                             "Does not exist in VariableBase.GetVariables", null);
               return null;
           }
           while (m.Success)
           {
               var p = new Position
                           {
                               Icon = icon,
                               Path = filename,
                               Offset = m.Index,
                               Type = m.Groups[1].ToString(),
                               Name = m.Groups[2].ToString(),
                               Value =
                                   isxyz
                                       ? lang.ExtractXYZ(m.ToString())
                                       : m.Groups[3].ToString(),
                               Comment = m.Groups[4].ToString()
                           };
               result.Add(p);
               m = m.NextMatch();
           }


           return result;
       }

      
    }
}
