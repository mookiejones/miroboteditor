using miRobotEditor.Core;
using miRobotEditor.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;

namespace miRobotEditor.GUI
{
    /// <summary>
    /// Interaction logic for FindandReplaceControl.xaml
    /// </summary>
    public partial class FindandReplaceControl
    {
        private static FindandReplaceControl _instance;

        public static FindandReplaceControl Instance { get { return _instance ?? (_instance = new FindandReplaceControl()); } set { _instance = value; } }

        public FindandReplaceControl()
        {
            Instance = this;
            InitializeComponent();
        }

        public FindandReplaceControl(Window owner)
        {
            Instance = this;
            InitializeComponent();
        }
    }

    [Serializable]
    public class Results : ICollection
    {
        public Results()
        {
        }

        public Results this[int index]
        {
            get { return (Results)_array[index]; }
        }

        private readonly ArrayList _array = new ArrayList();

        public IEnumerator GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _array.CopyTo(array, index);
        }

        public void Add(FindReplaceResult item)
        {
            _array.Add(item);
        }

        public int Count { get { return _array.Count; } }

        public object SyncRoot { get { return this; } }

        public bool IsSynchronized { get { return false; } }
    }

    [Localizable(false)]
    [Serializable]
    public class FindReplaceHistory
    {
        private List<string> _findItems = new List<string>(10);

        public List<string> FindItems { get { return _findItems; } set { _findItems = value; } }

        private List<string> _replaceItems = new List<string>(10);

        public List<string> ReplaceItems { get { return _replaceItems; } set { _replaceItems = value; } }

        private List<string> _directoryItems = new List<string>(10) { "Current Window", "Current Selection", "Files" };

        public List<string> DirectoryItems { get { return _directoryItems; } set { _directoryItems = value; } }
    }

    [Serializable]
    public class FindReplaceViewModel : ToolViewModel
    {
        private const string HistoryPath = "D:\\History.xml";

        ~FindReplaceViewModel()
        {
            var obj = History;
            var serializer = new XmlSerializer(typeof(FindReplaceHistory));
            TextWriter streamWriter = new StreamWriter(HistoryPath);
            serializer.Serialize(streamWriter, History);
        }

        private FindReplaceHistory _history = new FindReplaceHistory();

        public FindReplaceHistory History { get { return _history; } set { _history = value; RaisePropertyChanged(); } }

        #region Members

        private readonly BackgroundWorker _background = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

        #endregion Members

        #region DependencyProperties

        public Results FindReplaceResults { get; set; }

        #region Include SubFolders

        private bool _includeSubFolders;

        public bool IncludeSubFolders { get { return _includeSubFolders; } set { _includeSubFolders = value; RaisePropertyChanged(); } }

        #endregion Include SubFolders

        #region Keep Modified Files Open

        private bool _keepModifiedPropertiesOpen;

        public bool KeepModifiedPropertiesOpen { get { return _keepModifiedPropertiesOpen; } set { _keepModifiedPropertiesOpen = value; RaisePropertyChanged(); } }

        #endregion Keep Modified Files Open

        private bool _inputIsValid;

        public bool InputIsValid
        {
            get { return _inputIsValid; }
            set
            {
                _inputIsValid = value;
                RaisePropertyChanged();
            }
        }

        #region Owner

        private Window _owner;

        public Window Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                RaisePropertyChanged();
            }
        }

        #endregion Owner

        #region Filter Items

        private List<string> _filterItems = new List<string>();

        public List<string> FilterItems { get { return _filterItems; } set { _filterItems = value; RaisePropertyChanged(); } }

        private string _filter;

        public string Filter { get { return _filter; } set { _filter = value; RaisePropertyChanged(); } }

        #endregion Filter Items

        #region Find Items property

        private string _findString;

        public string FindString
        {
            get { return _findString; }
            set
            {
                _findString = value;
                RaisePropertyChanged();
            }
        }

        #endregion Find Items property

        #region Replace items properties

        private string _replaceString;

        public string ReplaceString
        {
            get { return _replaceString; }
            set
            {
                _replaceString = value;
                RaisePropertyChanged();
            }
        }

        private bool _replaceEnabled;

        public bool ReplaceEnabled { get { return _replaceEnabled; } set { _replaceEnabled = value; RaisePropertyChanged(); } }

        #endregion Replace items properties

        #region Directory Properties

        private string _directoryString = @"C:\Users\Charles\Google Drive\Work\KUKA Robot\Backups\BSILHR01\BSILHR01\KRC\R1";

        public string DirectoryString
        {
            get { return _directoryString; }
            set
            {
                _directoryString = value;
                RaisePropertyChanged();
            }
        }

        #endregion Directory Properties

        #region Match Whole Word

        private bool _matchWholeWord;

        public bool MatchWholeWord
        {
            get { return _matchWholeWord; }
            set
            {
                _matchWholeWord = value;
                RaisePropertyChanged();
            }
        }

        #endregion Match Whole Word

        #region Match Case

        private bool _matchCase;

        public bool MatchCase { get { return _matchCase; } set { _matchCase = value; RaisePropertyChanged(); } }

        #endregion Match Case

        #region Use WildCards

        private bool _useWildCards;

        public bool UseWildCards
        {
            get { return _useWildCards; }
            set { _useWildCards = value; RaisePropertyChanged(); }
        }

        #endregion Use WildCards

        #region Progress

        private int _progressValue;

        public int ProgressValue { get { return _progressValue; } set { _progressValue = value; RaisePropertyChanged(); } }

        private string _progressString;

        public string ProgressString { get { return _progressString; } set { _progressString = value; RaisePropertyChanged(); } }

        private Visibility _progressVisibility = Visibility.Collapsed;

        public Visibility ProgressVisibility { get { return _progressVisibility; } set { _progressVisibility = value; RaisePropertyChanged(); } }

        #endregion Progress

        #region Cancel

        private bool _cancelEnabled;

        public bool CancelEnabled { get { return _cancelEnabled; } set { _cancelEnabled = value; RaisePropertyChanged(); } }

        #endregion Cancel

        #region Status

        private string _statusText;

        public string StatusText { get { return _statusText; } set { _statusText = value; RaisePropertyChanged(); } }

        #endregion Status

        #endregion DependencyProperties

        #region Constructor

        public FindReplaceViewModel()
            : base("Search Results")
        {
            Deserialize();

            DefaultPane = DefaultToolPane.Bottom;
            StatusText = "...";
            _background.DoWork += _backgroundWorker_DoWork;
            _background.ProgressChanged += backgroundWorker_ProgressChanged;
            _background.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        #endregion Constructor

        private new void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(FindReplaceHistory));

            using (var reader = new StreamReader(HistoryPath))
            {
                History = (FindReplaceHistory)serializer.Deserialize(reader);
            }
        }

        #region Commands

        [NonSerialized]
        private RelayCommand _findAllCommand;

        public ICommand FindAllCommand
        {
            get { return _findAllCommand ?? (_findAllCommand = new RelayCommand(p => InitializeSearch(WorkType.FindAll), p => true)); }
        }

        [NonSerialized]
        private RelayCommand _replaceAllCommand;

        public ICommand ReplaceAllCommand
        {
            get { return _replaceAllCommand ?? (_replaceAllCommand = new RelayCommand(p => InitializeSearch(WorkType.ReplaceAll), p => true)); }
        }

        [NonSerialized]
        private RelayCommand _findCommand;

        public ICommand FindCommand
        {
            get
            {
                return _findCommand ??
                       (_findCommand =
                        new RelayCommand(p => InitializeSearch(WorkType.Find), p => DirectoryString == ("Current Window") | DirectoryString == "Current Selection"));
            }
        }

        [NonSerialized]
        private RelayCommand _replaceCommand;

        public ICommand ReplaceCommand
        {
            get
            {
                return _replaceCommand ??
                       (_replaceCommand =
                        new RelayCommand(p => InitializeSearch(WorkType.Replace),
                                         p =>
                                         DirectoryString == ("Current Window") | DirectoryString == "Current Selection"));
            }
        }

        [NonSerialized]
        private RelayCommand _browseFoldersCommand;

        public ICommand BrowseFoldersCommand
        {
            get
            {
                return _browseFoldersCommand ??
                       (_browseFoldersCommand = new RelayCommand(p => BrowseForFolder(), p => true));
            }
        }

        #endregion Commands

        #region Methods

        private void BrowseForFolder()
        {
        }

        private int DoFind(BackgroundWorker worker)
        {
            //Initialize the affected count variable
            var filesAffectedCount = 0;

            //Initialize the counter
            var counter = 0;
            //TODO Add FileTypes

            // Create new items found collection
            FindReplaceResults = new Results();
            //Get all XML files in the directory
            var filesInDirectory = Directory.GetFiles(DirectoryString, "*.src");

            //Initialize total file count
            var totalFiles = filesInDirectory.GetLength(0);

            //Analyze each file in the directory
            foreach (var file in filesInDirectory)
            {
                //Perform find and replace operation
                if (FindAndReplace(file))
                {
                    //The file was changed so increment variable
                    filesAffectedCount++;
                }

                //Increment the counter
                counter++;

                //Report progress
                worker.ReportProgress((int)(counter / totalFiles * 100.00));
            }

            //Return the total number of files changed
            return filesAffectedCount;
        }

        /// <summary>
        /// Performs the find and replace operation on a file.
        /// </summary>
        /// <param name="file">The path of the file to operate on.</param>
        /// <returns>A value indicating if the file has changed.</returns>
        private bool FindAndReplace(string file)
        {
            switch (Work)
            {
                case WorkType.Find:
                    Find();
                    break;

                case WorkType.FindAll:
                    return FindAll(file);

                case WorkType.Replace:
                    Replace(file);
                    break;

                case WorkType.ReplaceAll:
                    ReplaceAll(file);
                    break;
            }

            //No match found and replaced
            return false;
        }

        private void Find()
        {
            Workspace.Instance.ActiveEditor.TextBox.FindText(FindString);
        }

        private bool FindAll(string filePath)
        {
            if (!History.DirectoryItems.Contains(DirectoryString))
                History.DirectoryItems.Add(DirectoryString);

            if (!History.FindItems.Contains(FindString))
                History.FindItems.Add(FindString);

            var filesFound = 0;
            //Create a new object to read a file
            var temp = File.ReadLines(filePath);

            //Get search text

            var searchText = GetSearchText(FindString);

            var matchstring = GetRegexSearch(FindString);

            foreach (var i in temp)
            {
                var m = matchstring.Match(i);
                while (m.Success)
                {
                    filesFound++;
                    FindReplaceResults.Add(new FindReplaceResult
                    {
                        File = filePath,
                        Groups = m.Groups,
                        Line = i,
                        RegexString = matchstring,
                        SearchString = searchText
                    });
                    m = m.NextMatch();
                }
            }
            RaisePropertyChanged("FindReplaceResults");
            return filesFound > 0;
        }

        private void Replace(string file)
        {
        }

        private void ReplaceAll(string file)
        {
        }

        private Regex GetRegexSearch(string textToFind)
        {
            return new Regex(GetSearchText(textToFind), GetRegExOptions());
        }

        /// <summary>
        /// Adds options to the expression.
        /// </summary>
        /// <returns>A Regex options object.</returns>
        private RegexOptions GetRegExOptions()
        {
            //Create a new option
            var options = new RegexOptions();

            //Is the match case check box checked
            if (MatchCase == false)
                options |= RegexOptions.IgnoreCase;

            //Return the options
            return options;
        }

        /// <summary>
        /// Gets the text to find based on the selected options.
        /// </summary>
        /// <param name="textToFind">The text to find in the file.</param>
        /// <returns>The text to search for.</returns>
        private string GetSearchText(string textToFind)
        {
            //Copy the text to find into the search text variable
            //Make the text regex safe
            var searchText = Regex.Escape(textToFind);

            //Is the match whole word box checked
            if (MatchWholeWord)
            {
                //Update the search string to find whole words only
                searchText = string.Format("{0}{1}{0}", @"\b", textToFind);
            }

            //Is the wildcard box checked
            if (UseWildCards)
            {
                searchText = searchText.Replace(@"\*", ".*").Replace(@"\?", ".");
            }

            return searchText;
        }

        /// <summary>
        /// Sets the properties of the controls after the download has completed.
        /// </summary>
        private void DeinitializeProcess()
        {
            //Set properties for controls affected when operating
            ProgressString = "Ready";
            ProgressVisibility = Visibility.Collapsed;
            ReplaceEnabled = true;
            CancelEnabled = false;
        }

        /// <summary>
        /// Displays the directory browser dialog.
        /// </summary>
        private void BrowseDirectory()
        {
            //Create a new folder browser object
            var browser = new FolderBrowserDialog();

            //Show the dialog
            if (browser.ShowDialog() == DialogResult.OK)
            {
                //Set the selected path
                DirectoryString = browser.SelectedPath;
            }
        }

        /// <summary>
        /// Validates that the user input is complete.
        /// </summary>
        /// <returns>A value indicating if the user input is complete.</returns>

        #endregion Methods

        #region Events

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Create a work object and initialize
            var worker = sender as BackgroundWorker;

            //Run the find and replace operation and store total files affected in the result property
            e.Result = (int)DoFind(worker);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Update the prog bar

            ProgressValue = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //The background operation is done
            DeinitializeProcess();

            if (e.Error != null)
                StatusText = e.Error.Message.ToString(CultureInfo.InvariantCulture);
            else if (e.Cancelled)
                StatusText = "The operation was ended by the user";
            else
                StatusText = string.Format("{0} files were updated by the operation", e.Result);

            if (Workspace.Instance.Tools.Any(i => i.Name == "Search Results"))
            {
                return;
            }
            Workspace.Instance.AddTool(this);
        }

        private void lnkBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Select a directory to output files
            BrowseDirectory();
        }

        #endregion Events

        private enum WorkType
        {
            Find,
            FindAll,
            Replace,
            ReplaceAll
        };

        private void UpdateComboBoxes(ICollection<string> box, string item)
        {
            if (box.Any(v => item == v))
                return;

            box.Add(item);
        }

        private WorkType Work;

        private void InitializeSearch(WorkType worktype)
        {
            Work = worktype;
            ProgressString = "Working...";
            ProgressValue = 0;
            ProgressVisibility = Visibility.Visible;
            ReplaceEnabled = false;

            switch (worktype)
            {
                case WorkType.Find:
                case WorkType.FindAll:
                    UpdateComboBoxes(History.FindItems, FindString);
                    break;

                case WorkType.Replace:
                case WorkType.ReplaceAll:
                    UpdateComboBoxes(History.ReplaceItems, ReplaceString);
                    break;
            }

            UpdateComboBoxes(History.DirectoryItems, DirectoryString);

            _background.RunWorkerAsync();
        }

        /* Implemented from old control
         */

        #region Commands

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
        private static RelayCommand _highlightallcommand;

        public static ICommand HighlightAllCommand
        {
            get
            {
                return _highlightallcommand ??
                       (_highlightallcommand = new RelayCommand(param => HighlightAll(), param => true));
            }
        }

        #endregion Commands

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
            set
            {
                _useregex = value;
                RaisePropertyChanged();
            }
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
            get { return UseRegex == false ? Regex.Escape(LookFor) : LookFor; }
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
            set
            {
                _replacewith = value;
                RaisePropertyChanged();
            }
        }

        private string _searchresult = string.Empty;

        public string SearchResult
        {
            get { return _searchresult; }
            set
            {
                _searchresult = value;
                RaisePropertyChanged();
            }
        }

        #endregion Properties

        private static void FindPrevious()
        {
            throw new NotImplementedException();
        }

        private static void FindNext()
        {
            Workspace.Instance.ActiveEditor.TextBox.FindText();
        }

        private static void Replace(IDocument document)
        {
            document.TextBox.ReplaceText();
        }

        private static void HighlightAll()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class FindReplaceResult
    {
        public FindReplaceResult()
        {
        }

        /// <summary>
        /// Group result from regex search
        /// </summary>
        public GroupCollection Groups { get; set; }

        /// <summary>
        /// Name of File Origin
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Full Line of text from file
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// String Searched for
        /// </summary>
        public Regex RegexString { get; set; }

        public String SearchString { get; set; }
    }
}