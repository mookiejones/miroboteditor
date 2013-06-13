using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using miRobotEditor.Core;
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
        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged(); } }

        public BitmapImage Icon { get { return _icon; } set { _icon = value; RaisePropertyChanged(); } }
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged(); } }
        public string Comment { get { return _comment; } set { _comment = value; RaisePropertyChanged(); } }       
        public string Path { get { return _path; } set { _path = value; RaisePropertyChanged(); } }
        public string Value { get { return _value; } set { _value = value; RaisePropertyChanged(); } }
        public string Type { get { return _type; } set { _type = value; RaisePropertyChanged(); } }
         public string Declaration { get { return _declaration; } set { _declaration = value; RaisePropertyChanged(); } }
        public int Offset { get { return _offset; } set { _offset = value; RaisePropertyChanged(); } }


// ReSharper disable UnusedAutoPropertyAccessor.Local
        public static List<IVariable> Variables { get; private set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
      
       internal class WorkerArgs
       {
           public string Filename { get; set; }
           public AbstractLanguageClass Lang { get; set; }
           public string IconPath { get; set; }
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
           if (s == null) return;
           var icon = Utilities.LoadBitmap(s.IconPath);
           var lang = Workspace.Instance.ActiveEditor.FileLanguage;
           

           var m = VariableHelper.FindMatches(s.Lang.XYZRegex, s.Filename);
           var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath);
           var isxyz =fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
           while (m.Success)
           {
               var p = new Variable
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
               Variables.Add(p);
               m = m.NextMatch();
           }
       }

       public static List<IVariable> GetVariables(string filename, Regex regex, string iconpath)
       {
           var result = new List<IVariable>();
           var icon = Utilities.LoadBitmap(iconpath);

           var lang = Workspace.Instance.ActiveEditor.FileLanguage;
           var m = VariableHelper.FindMatches(regex, filename);
           var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath);
           var isxyz =fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
           if (m==null)
           {             
             
               MessageViewModel.Instance.Add("Variable for " + lang.RobotType,"Does not exist in VariableBase.GetVariables", MsgIcon.Error);
               return null;
           }

           while (m.Success)
           {
               var p = new Variable
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
