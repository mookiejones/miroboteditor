using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class FindReplaceViewModel:ViewModelBase
    {

        #region  Commands
        private static RelayCommand _findpreviouscommand;
        public static ICommand FindPreviousCommand
        {
            get
            {
                return _findpreviouscommand ??
                       (_findpreviouscommand = new RelayCommand(param => FindPrevious(), param => true));
            }
        }
        private static RelayCommand _findnextcommand;
        public static ICommand FindNextCommand
        {
            get { return _findnextcommand ?? (_findnextcommand = new RelayCommand(param => FindNext(), param => true)); }
        }
        private static RelayCommand _replacecommand;
        public static ICommand ReplaceCommand
        {
            get
            {
                return _replacecommand ??
                       (_replacecommand = new RelayCommand(param => Replace(), param => true));
            }
        }
        private static RelayCommand _replaceallcommand;
        public static ICommand ReplaceAllCommand
        {
            get
            {
                return _replaceallcommand ??
                       (_replaceallcommand = new RelayCommand(param => ReplaceAll(), param => true));
            }
        }
        private static RelayCommand _highlightallcommand;
        public static ICommand HighlightAllCommand
        {
            get
            {
                return _highlightallcommand ??
                       (_highlightallcommand = new RelayCommand(param => HighlightAll(), param => true));
            }
        }
        private RelayCommand _findallcommand;
        public ICommand FindAllCommand
        {
            get
            {
                return _findallcommand ??
                       (_findallcommand = new RelayCommand(param => FindAll(), param => true));
            }
        }
        #endregion

        private static FindReplaceViewModel _instance;
        public static FindReplaceViewModel Instance
        {
            get { return _instance ?? (_instance = new FindReplaceViewModel()); }
            set { _instance = value; }
        }

        #region Properties


        private bool _useregex;
        public bool UseRegex
        {
            get { return _useregex; }
            set { _useregex = value; RaisePropertyChanged(); }
        }
        private bool _matchcase;

        public bool MatchCase
        {
            get { return _matchcase; }
            set { _matchcase = value; RaisePropertyChanged(); }
        }
        private bool _matchwholeword;

        public bool MatchWholeWord
        {
            get { return _matchwholeword; }
            set { _matchwholeword = value; RaisePropertyChanged(); }
        }

        public Regex RegexPattern
        {
            get
            {
                var pattern = UseRegex == false ? Regex.Escape(LookFor) : LookFor;
                var options = MatchCase ? 0 : 1;
                return new Regex(pattern, (RegexOptions)options);
            }
        }

        public string RegexString
        {
            get
            {
                return UseRegex == false ? Regex.Escape(LookFor) : LookFor;
            }
        }

        private string _lookfor = string.Empty;

        public string LookFor
        {
            get { return _lookfor; }
            set
            {
                _lookfor = value;
                RaisePropertyChanged();
            }
        }

        private string _replacewith = string.Empty;
        public string ReplaceWith
        {
            get { return _replacewith; }
            set { _replacewith = value; RaisePropertyChanged(); }
        }

        private string _searchresult = string.Empty;
        public string SearchResult
        {
            get { return _searchresult; }
            set { _searchresult = value; RaisePropertyChanged(); }
        }
        #endregion

        private static void FindPrevious()
        {
            throw new System.NotImplementedException();
        }
        private static void FindNext()
        {
            Workspace.Instance.ActiveEditor.TextBox.FindText();
        }

        private static void Replace()
        {
            Workspace.Instance.ActiveEditor.TextBox.ReplaceText();
        }

        private static void ReplaceAll()
        {
            throw new System.NotImplementedException();
        }

        private static void HighlightAll()
        {
            throw new System.NotImplementedException();
        }

        private static void FindAll()
        {
            throw new System.NotImplementedException();
        }
    }

    public class FindandReplaceControl:Window
    {
        private static FindandReplaceControl _instance;

        public FindandReplaceControl(MainWindow instance)
        {
            throw new NotImplementedException();
        }

        private FindandReplaceControl()
        {
            throw new NotImplementedException();
        }

        public static FindandReplaceControl Instance
        {
            get { return _instance ?? (_instance = new FindandReplaceControl()); }
            set { _instance = value; }
        }
    }
}
