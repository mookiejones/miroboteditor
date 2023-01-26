using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using CommonServiceLocator;

using miRobotEditor.Abstract;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Messages;
using miRobotEditor.ViewModel;
using miRobotEditor.Utilities;

namespace miRobotEditor.Classes
{
    public class VariableBase : ViewModelBase, IVariable
    {
        private string _comment;
        private string _declaration;
        private string _description = string.Empty;
        private BitmapImage _icon;
        private string _name;
        private int _offset;
        private string _path;
        private string _type;
        private string _value;
        public static List<IVariable> Variables { get; private set; }
        public bool IsSelected { get; set; }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public BitmapImage Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged(nameof(Icon));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChanged(nameof(Comment));
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(nameof(Type));
            }
        }

        public string Declaration
        {
            get { return _declaration; }
            set
            {
                _declaration = value;
                RaisePropertyChanged(nameof(Declaration));
            }
        }

        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                RaisePropertyChanged(nameof(Offset));
            }
        }

        public static void GetPositions(string filename, AbstractLanguageClass lang, string iconpath)
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundworkerDoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundworkerRunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(new WorkerArgs
            {
                Filename = filename,
                Lang = lang,
                IconPath = iconpath
            });
        }

        private static void BackgroundworkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private static void BackgroundworkerDoWork(object sender, DoWorkEventArgs e)
        {
            var workerArgs = e.Argument as WorkerArgs;
            if (workerArgs != null)
            {
                var bitmapImage = ImageHelper.LoadBitmap(workerArgs.IconPath);
                var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
                var fileLanguage = instance.ActiveEditor.FileLanguage;
                var match = VariableHelper.FindMatches(workerArgs.Lang.XYZRegex, workerArgs.Filename);
                var fileNameWithoutExtension =
                    System.IO.Path.GetFileNameWithoutExtension(bitmapImage.UriSource.AbsolutePath);
                var flag = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
                while (match.Success)
                {
                    var item = new Variable
                    {
                        Icon = bitmapImage,
                        Path = workerArgs.Filename,
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = flag ? fileLanguage.ExtractXYZ(match.ToString()) : match.Groups[3].ToString(),
                        Comment = match.Groups[4].ToString()
                    };
                    Variables.Add(item);
                    match = match.NextMatch();
                }
            }
        }

        public static List<IVariable> GetVariables(string filename, Regex regex, string iconpath)
        {
            var list = new List<IVariable>();
            var bitmapImage = ImageHelper.LoadBitmap(iconpath);
            var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
            var fileLanguage = instance.ActiveEditor.FileLanguage;
            var match = VariableHelper.FindMatches(regex, filename);
            var fileNameWithoutExtension =
                System.IO.Path.GetFileNameWithoutExtension(bitmapImage.UriSource.AbsolutePath);
            var flag = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
            List<IVariable> result;
            if (match == null)
            {
                var msg = new ErrorMessage("Variable for " + fileLanguage.RobotType,
                    "Does not exist in VariableBase.GetVariables", MessageType.Error);
                Messenger.Default.Send<IMessage>(msg);
                result = null;
            }
            else
            {
                while (match.Success)
                {
                    var item = new Variable
                    {
                        Icon = bitmapImage,
                        Path = filename,
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = flag ? fileLanguage.ExtractXYZ(match.ToString()) : match.Groups[3].ToString(),
                        Comment = match.Groups[4].ToString()
                    };
                    list.Add(item);
                    match = match.NextMatch();
                }
                result = list;
            }
            return result;
        }

        internal class WorkerArgs
        {
            public string Filename { get; set; }
            public AbstractLanguageClass Lang { get; set; }
            public string IconPath { get; set; }
        }
    }
}