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
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Core;
using miRobotEditor.Interfaces;
using miRobotEditor.ViewModel;

namespace miRobotEditor.GUI.Editor
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

        public int Count => _array.Count;

        public object SyncRoot => this;

        public bool IsSynchronized => false;
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

        public FindReplaceHistory History { get { return _history; } set { _history = value; RaisePropertyChanged("History"); } }

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

        public bool IncludeSubFolders { get { return _includeSubFolders; } set { _includeSubFolders = value; RaisePropertyChanged("IncludeSubFolders"); } }

        #endregion Include SubFolders

        #region Keep Modified Files Open

        private bool _keepModifiedPropertiesOpen;

        public bool KeepModifiedPropertiesOpen { get { return _keepModifiedPropertiesOpen; } set { _keepModifiedPropertiesOpen = value; RaisePropertyChanged("KeepModifiedPropertiesOpen"); } }

        #endregion Keep Modified Files Open

        private bool _inputIsValid;

        public bool InputIsValid
        {
            get { return _inputIsValid; }
            set
            {
                _inputIsValid = value;
                RaisePropertyChanged("InputIsValid");
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
                RaisePropertyChanged("Owner");
            }
        }

        #endregion Owner

        #region Filter Items

        private List<string> _filterItems = new List<string>();

        public List<string> FilterItems { get { return _filterItems; } set { _filterItems = value; RaisePropertyChanged("FilterItems"); } }

        private string _filter;

        public string Filter { get { return _filter; } set { _filter = value; RaisePropertyChanged("Filter"); } }

        #endregion Filter Items

        #region Find Items property

        private string _findString;

        public string FindString
        {
            get { return _findString; }
            set
            {
                _findString = value;
                RaisePropertyChanged("FindString");
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
                RaisePropertyChanged("ReplaceString");
            }
        }

        private bool _replaceEnabled;

        public bool ReplaceEnabled { get { return _replaceEnabled; } set { _replaceEnabled = value; RaisePropertyChanged("ReplaceEnabled"); } }

        #endregion Replace items properties

        #region Directory Properties

        private string _directoryString = @"C:\Users\Charles\Google Drive\Work\KUKA Robot\Backups\BSILHR01\BSILHR01\KRC\R1";

        public string DirectoryString
        {
            get { return _directoryString; }
            set
            {
                _directoryString = value;
                RaisePropertyChanged("DirectoryString");
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
                RaisePropertyChanged("MatchWholeWord");
            }
        }

        #endregion Match Whole Word

        #region Match Case

        private bool _matchCase;

        public bool MatchCase { get { return _matchCase; } set { _matchCase = value; RaisePropertyChanged("MatchCase"); } }

        #endregion Match Case

        #region Use WildCards

        private bool _useWildCards;

        public bool UseWildCards
        {
            get { return _useWildCards; }
            set { _useWildCards = value; RaisePropertyChanged("UseWildCards"); }
        }

        #endregion Use WildCards

        #region Progress

        private int _progressValue;

        public int ProgressValue { get { return _progressValue; } set { _progressValue = value; RaisePropertyChanged("ProgressValue"); } }

        private string _progressString;

        public string ProgressString { get { return _progressString; } set { _progressString = value; RaisePropertyChanged("ProgressString"); } }

        private Visibility _progressVisibility = Visibility.Collapsed;

        public Visibility ProgressVisibility { get { return _progressVisibility; } set { _progressVisibility = value; RaisePropertyChanged("ProgressVisibility"); } }

        #endregion Progress

        #region Cancel

        private bool _cancelEnabled;

        public bool CancelEnabled { get { return _cancelEnabled; } set { _cancelEnabled = value; RaisePropertyChanged("CancelEnabled"); } }

        #endregion Cancel

        #region Status

        private string _statusText;

        public string StatusText { get { return _statusText; } set { _statusText = value; RaisePropertyChanged("StatusText"); } }

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

        private void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(FindReplaceHistory));

            using (var reader = new StreamReader(HistoryPath))
            {
                History = (FindReplaceHistory)serializer.Deserialize(reader);
            }
        }

        #region Commands


        #region FindAllCommand

        private RelayCommand _findAllCommand;
        /// <summary>
        /// Gets the FindAllCommand.
        /// </summary>
        public RelayCommand FindAllCommand => _findAllCommand
                    ?? (_findAllCommand = new RelayCommand(ExecuteFindAllCommand));

        private void ExecuteFindAllCommand()
        {
            InitializeSearch(WorkType.FindAll);
        }
        #endregion


        #region ReplaceAllCommand

        private RelayCommand _replaceAllCommand;
        /// <summary>
        /// Gets the ReplaceAllCommand.
        /// </summary>
        public RelayCommand ReplaceAllCommand => _replaceAllCommand
                    ?? (_replaceAllCommand = new RelayCommand(ExecuteReplaceAllCommand));

        private void ExecuteReplaceAllCommand()
        {
            InitializeSearch(WorkType.ReplaceAll);
        }
        #endregion

        private RelayCommand _findCommand;

        /// <summary>
        /// Gets the FindCommand.
        /// </summary>
        public RelayCommand FindCommand => _findCommand ?? (_findCommand = new RelayCommand(
                    ExecuteFindCommand,
                    CanExecuteFindCommand));

        private void ExecuteFindCommand()
        {
            InitializeSearch(WorkType.Find);
        }

        private bool CanExecuteFindCommand()
        {
            return ( DirectoryString == ("Current Window") | DirectoryString == "Current Selection");
        }


        private RelayCommand _replaceCommand;

        /// <summary>
        /// Gets the ReplaceCommand.
        /// </summary>
        public RelayCommand ReplaceCommand => _replaceCommand ?? (_replaceCommand = new RelayCommand(
                    ExecuteReplaceCommand,
                    CanExecuteReplaceCommand));

        private void ExecuteReplaceCommand()
        {
            InitializeSearch(WorkType.Replace);
        }

        private bool CanExecuteReplaceCommand()
        {
            return ( DirectoryString == ("Current Window") | DirectoryString == "Current Selection");
        }

        private RelayCommand _browseFoldersCommand;

        /// <summary>
        /// Gets the BrowseFoldersCommand.
        /// </summary>
        public RelayCommand BrowseFoldersCommand => _browseFoldersCommand ?? (_browseFoldersCommand = new RelayCommand(
                    ExecuteBrowseFoldersCommand,
                    CanExecuteBrowseFoldersCommand));

        private void ExecuteBrowseFoldersCommand()
        {
            BrowseForFolder();
        }

        private bool CanExecuteBrowseFoldersCommand()
        {
            return true;
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
            switch (_work)
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
            WorkspaceViewModel.Instance.ActiveEditor.TextBox.FindText(FindString);
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

            if (WorkspaceViewModel.Instance.Tools.Any(i => i.Name == "Search Results"))
            {
                return;
            }
            WorkspaceViewModel.Instance.AddTool(this);
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

        private WorkType _work;

        private void InitializeSearch(WorkType worktype)
        {
            _work = worktype;
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


        #region FindPreviousCommand

        private RelayCommand _findPreviousCommand;
        /// <summary>
        /// Gets the FindPreviousCommand.
        /// </summary>
        public RelayCommand FindPreviousCommand => _findPreviousCommand
                    ?? (_findPreviousCommand = new RelayCommand(ExecuteFindPreviousCommand));

        private void ExecuteFindPreviousCommand()
        {
            FindPrevious();
        }
        #endregion

        #region FindNextCommand

        private RelayCommand _findNextCommand;
        /// <summary>
        /// Gets the FindNextCommand.
        /// </summary>
        public RelayCommand FindNextCommand => _findNextCommand
                    ?? (_findNextCommand = new RelayCommand(ExecuteFindNextCommand));

        private void ExecuteFindNextCommand()
        {
            FindNext();
        }
        #endregion


        #region HighlightAllCommand

        private RelayCommand _highlightAllCommand;
        /// <summary>
        /// Gets the HighlightAllCommand.
        /// </summary>
        public RelayCommand HighlightAllCommand => _highlightAllCommand
                    ?? (_highlightAllCommand = new RelayCommand(ExecuteHighlightAllCommand));

        private void ExecuteHighlightAllCommand()
        {
            HighlightAll();
        }
        #endregion

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
                RaisePropertyChanged("UseRegex");
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

        public string RegexString => UseRegex == false ? Regex.Escape(LookFor) : LookFor;

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
            set
            {
                _replacewith = value;
                RaisePropertyChanged("ReplaceWith");
            }
        }

        private string _searchresult = string.Empty;

        public string SearchResult
        {
            get { return _searchresult; }
            set
            {
                _searchresult = value;
                RaisePropertyChanged("SearchResult");
            }
        }

        #endregion Properties

        private static void FindPrevious()
        {
            throw new NotImplementedException();
        }

        private static void FindNext()
        {
            WorkspaceViewModel.Instance.ActiveEditor.TextBox.FindText();
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