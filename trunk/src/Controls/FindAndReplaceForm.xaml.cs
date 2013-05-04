using System.Text.RegularExpressions;
using System.Windows.Input;
using miRobotEditor.Commands;
using miRobotEditor.GUI;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for FindAndREplaceForm.xaml
    /// </summary>
    public partial class FindAndReplaceForm
    {
        private static FindAndReplaceForm _instance;
        public static FindAndReplaceForm Instance
        {
            get { return _instance ?? (_instance = new FindAndReplaceForm()); }
            set { _instance = value; }
        }

      
        

    }

    public class FindReplaceViewModel : ViewModelBase
    {

        #region  Commands
        private static RelayCommand _findpreviouscommand;
        public static ICommand FindPreviousCommand
        {
            get {
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
            get {
                return _replacecommand ??
                       (_replacecommand = new RelayCommand(param => Instance.Replace(), param => true));
            }
        }
        private static RelayCommand _replaceallcommand;
        public static ICommand ReplaceAllCommand
        {
            get {
                return _replaceallcommand ??
                       (_replaceallcommand = new RelayCommand(param => Instance.ReplaceAll(), param => true));
            }
        }
        private static RelayCommand _highlightallcommand;
        public static ICommand HighlightAllCommand
        {
            get {
                return _highlightallcommand ??
                       (_highlightallcommand = new RelayCommand(param => Instance.HighlightAll(), param => true));
            }
        }
        private static RelayCommand _findallcommand;
        public static ICommand FindAllCommand
        {
            get {
                return _findallcommand ??
                       (_findallcommand = new RelayCommand(param => Instance.FindAll(), param => true));
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
            set { _useregex = value;RaisePropertyChanged("UseRegex"); }
        }
        private bool _matchcase;

        public bool MatchCase
        {
            get { return _matchcase; }
            set { _matchcase = value; RaisePropertyChanged("MatchCase"); }
        }
        private bool _matchwholeword;

        public bool MatchWholeWord
        {
            get { return _matchwholeword; }
            set { _matchwholeword = value; RaisePropertyChanged("MatchWholeWord"); }
        }

        public Regex RegexPattern
        {
            get
            {
                var pattern = UseRegex == false ? Regex.Escape(LookFor) : LookFor;
                int options = MatchCase ? 0 : 1;
                return new Regex(pattern, (RegexOptions)options);
            }
        }

        public string RegexString
        {
            get
            {
                if (UseRegex == false)
                    return Regex.Escape(LookFor);
                return LookFor;
            }
        }

        private string _lookfor = string.Empty;

        public string LookFor
        {
            get { return _lookfor; }
            set
            {
                _lookfor = value;
                RaisePropertyChanged("LookFor");
            }
        }

        private string _replacewith = string.Empty;
        public string ReplaceWith
        {
            get { return _replacewith; }
            set { _replacewith = value;RaisePropertyChanged("ReplaceWith"); }
        }

        private string _searchresult = string.Empty;
        public string SearchResult
        {
            get { return _searchresult; }
            set { _searchresult = value;RaisePropertyChanged("SearchResult"); }
        }
        #endregion

        private static void FindPrevious()
        {
            throw new System.NotImplementedException();
        }
        private static void FindNext()
        {
            DummyDoc.Instance.TextBox.FindText();
        }

        private void Replace()
        {
            DummyDoc.Instance.TextBox.ReplaceText();
        }

        private void ReplaceAll()
        {
            throw new System.NotImplementedException();
        }

        private void HighlightAll()
        {
            throw new System.NotImplementedException();
        }

        private void FindAll()
        {
            throw new System.NotImplementedException();
        }

    }
}
