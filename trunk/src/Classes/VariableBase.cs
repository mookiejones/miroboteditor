using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Core;
using miRobotEditor.Languages;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Classes
{
    public class VariableBase : ViewModelBase, IVariable
    {
       
        public static List<IVariable> Variables { get; private set; }
        public bool IsSelected { get; set; }



        
        #region Description
        /// <summary>
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _description = String.Empty;

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }
        #endregion


        
        #region Icon
        /// <summary>
        /// The <see cref="Icon" /> property's name.
        /// </summary>
        public const string IconPropertyName = "Icon";

        private BitmapImage _icon = null;

        /// <summary>
        /// Sets and gets the Icon property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public BitmapImage Icon
        {
            get
            {
                return _icon;
            }

            set
            {
                if (_icon == value)
                {
                    return;
                }

                RaisePropertyChanging(IconPropertyName);
                _icon = value;
                RaisePropertyChanged(IconPropertyName);
            }
        }
        #endregion

        
        #region Name
        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = String.Empty;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }
        #endregion

        
        #region Comment
        /// <summary>
        /// The <see cref="Comment" /> property's name.
        /// </summary>
        public const string CommentPropertyName = "Comment";

        private string _comment = String.Empty;

        /// <summary>
        /// Sets and gets the Comment property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Comment
        {
            get
            {
                return _comment;
            }

            set
            {
                if (_comment == value)
                {
                    return;
                }

                RaisePropertyChanging(CommentPropertyName);
                _comment = value;
                RaisePropertyChanged(CommentPropertyName);
            }
        }
        #endregion

        
        #region Path
        /// <summary>
        /// The <see cref="Path" /> property's name.
        /// </summary>
        public const string PathPropertyName = "Path";

        private string _path = String.Empty;

        /// <summary>
        /// Sets and gets the Path property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                if (_path == value)
                {
                    return;
                }

                RaisePropertyChanging(PathPropertyName);
                _path = value;
                RaisePropertyChanged(PathPropertyName);
            }
        }
        #endregion
        
        #region Value
        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        private string _myvalue = String.Empty
;

        /// <summary>
        /// Sets and gets the Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Value
        {
            get
            {
                return _myvalue;
            }

            set
            {
                if (_myvalue == value)
                {
                    return;
                }

                RaisePropertyChanging(ValuePropertyName);
                _myvalue = value;
                RaisePropertyChanged(ValuePropertyName);
            }
        }
        #endregion


        
        #region Type
        /// <summary>
        /// The <see cref="Type" /> property's name.
        /// </summary>
        public const string TypePropertyName = "Type";

        private string _type = String.Empty;

        /// <summary>
        /// Sets and gets the Type property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                if (_type == value)
                {
                    return;
                }

                RaisePropertyChanging(TypePropertyName);
                _type = value;
                RaisePropertyChanged(TypePropertyName);
            }
        }
        #endregion
        
        #region Declaration
        /// <summary>
        /// The <see cref="Declaration" /> property's name.
        /// </summary>
        public const string DeclarationPropertyName = "Declaration";

        private string _declaration = String.Empty;

        /// <summary>
        /// Sets and gets the Declaration property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Declaration
        {
            get
            {
                return _declaration;
            }

            set
            {
                if (_declaration == value)
                {
                    return;
                }

                RaisePropertyChanging(DeclarationPropertyName);
                _declaration = value;
                RaisePropertyChanged(DeclarationPropertyName);
            }
        }
        #endregion

        
        #region Offset
        /// <summary>
        /// The <see cref="Offset" /> property's name.
        /// </summary>
        public const string OffsetPropertyName = "Offset";

        private int _offset = 0;

        /// <summary>
        /// Sets and gets the Offset property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                if (_offset == value)
                {
                    return;
                }

                RaisePropertyChanging(OffsetPropertyName);
                _offset = value;
                RaisePropertyChanged(OffsetPropertyName);
            }
        }
        #endregion

     

// ReSharper disable UnusedAutoPropertyAccessor.Local


        public static void GetPositions(string filename, AbstractLanguageClass lang, string iconpath)
        {
            var backgroundworker = new BackgroundWorker();
            backgroundworker.DoWork += BackgroundworkerDoWork;
            backgroundworker.RunWorkerCompleted += BackgroundworkerRunWorkerCompleted;

            backgroundworker.RunWorkerAsync(new WorkerArgs {Filename = filename, Lang = lang, IconPath = iconpath});
        }


        private static void BackgroundworkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private static void BackgroundworkerDoWork(object sender, DoWorkEventArgs e)
        {
            var s = e.Argument as WorkerArgs;
            if (s == null) return;
            BitmapImage icon = Utilities.LoadBitmap(s.IconPath);
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            AbstractLanguageClass lang = main.ActiveEditor.FileLanguage;


            Match m = VariableHelper.FindMatches(s.Lang.XYZRegex, s.Filename);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath);
            bool isxyz = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
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
            BitmapImage icon = Utilities.LoadBitmap(iconpath);
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            AbstractLanguageClass lang = main.ActiveEditor.FileLanguage;
            Match m = VariableHelper.FindMatches(regex, filename);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(icon.UriSource.AbsolutePath);
            bool isxyz = fileNameWithoutExtension != null && fileNameWithoutExtension.Contains("XYZ");
            if (m == null)
            {
                MessageViewModel.Instance.Add("Variable for " + lang.RobotType,
                    "Does not exist in VariableBase.GetVariables", MsgIcon.Error);
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

        internal class WorkerArgs
        {
            public string Filename { get; set; }
            public AbstractLanguageClass Lang { get; set; }
            public string IconPath { get; set; }
        }
    }
}