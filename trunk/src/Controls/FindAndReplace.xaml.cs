using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using miRobotEditor.Core;
using System.Linq;
namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for FindandReplaceControl.xaml
    /// </summary>
    public partial class FindandReplaceControl
    {

       
        public FindandReplaceControl()
        {
            InitializeComponent();
        }
    }
    public class FindReplaceViewModel:ToolViewModel
    {
        #region Members

        private readonly BackgroundWorker _background = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
        #endregion

        #region DependencyProperties

        #region Include SubFolders
        public static readonly DependencyProperty IncludeSubFoldersProperty =
            DependencyProperty.Register("IncludeSubFolders", typeof (bool), typeof (FindReplaceViewModel), new PropertyMetadata(default(bool)));

        public bool IncludeSubFolders
        {
            get { return (bool) GetValue(IncludeSubFoldersProperty); }
            set { SetValue(IncludeSubFoldersProperty, value); }
        }

        #endregion

        #region Keep Modified Files Open
        public static readonly DependencyProperty KeepModifiedFilesOpenProperty =
            DependencyProperty.Register("KeepModifiedFilesOpen", typeof (bool), typeof (FindReplaceViewModel), new PropertyMetadata(true));

        public bool KeepModifiedFilesOpen
        {
            get { return (bool) GetValue(KeepModifiedFilesOpenProperty); }
            set { SetValue(KeepModifiedFilesOpenProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty InputIsValidProperty =
            DependencyProperty.Register("InputIsValid", typeof (bool), typeof (FindReplaceViewModel), new PropertyMetadata(default(bool)));


        public MatchCollection SearchResults { get; set; }

        public bool InputIsValid
        {
            get { return (bool) GetValue(InputIsValidProperty); }
            set { SetValue(InputIsValidProperty, value); }
        }

     
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof (Window), typeof (FindReplaceViewModel), new PropertyMetadata(default(Window)));

        public Window Owner
        {
            get { return (Window) GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }



        #region Filter Items


        public static readonly DependencyProperty FilterItemsProperty =
            DependencyProperty.Register("FilterItems", typeof(List<String>), typeof(FindReplaceViewModel), new PropertyMetadata(default(List<String>)));

        public List<String> FilterItems
        {
            get { return (List<String>)GetValue(FilterItemsProperty); }
            set { SetValue(FilterItemsProperty, value); }
        }


        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FindReplaceViewModel), new PropertyMetadata(default(string)));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        #endregion

        #region Find items property       

        private string _findString;
        public string FindString { get { return _findString; } set { _findString = value;RaisePropertyChanged(); } }
       

        private List<string> _findItems = new List<string>();
        public List<string> FindItems { get { return _findItems; } set { _findItems = value;RaisePropertyChanged(); } } 


        #endregion

        #region Replace items properties

        private string _replaceString;
        public string ReplaceString { get { return _replaceString; } set { _replaceString = value;RaisePropertyChanged(); } }

        public ObservableCollection<String> ReplaceItems { get; set; } 
        public static readonly DependencyProperty ReplaceEnabledProperty =
          DependencyProperty.Register("ReplaceEnabled", typeof(bool), typeof(FindReplaceViewModel), new PropertyMetadata(default(bool)));

        public bool ReplaceEnabled
        {
            get { return (bool)GetValue(ReplaceEnabledProperty); }
            set { SetValue(ReplaceEnabledProperty, value); }
        }

        #endregion 

        #region Directory Properties


        private string _directoryString;
        public string DirectoryString { get { return _directoryString; } set { _directoryString = value;RaisePropertyChanged("DirectoryString"); } }

       
        public ObservableCollection<String> DirectoryItems { get; set; }
     
       
        #endregion

        #region Match Whole Word 

        private bool _matchWholeWord;
        public bool MatchWholeWord { get { return _matchWholeWord; } set { _matchWholeWord = value;RaisePropertyChanged(); } }
        #endregion

        #region Match Case
        private bool _matchCase;
        public bool MatchCase { get { return _matchCase; } set { _matchCase = value; RaisePropertyChanged(); } }
        #endregion

        #region Use WildCards
        private bool _useWildCards;
        public bool UseWildCards { get { return _useWildCards; } set { _useWildCards = value; RaisePropertyChanged(); } }
        #endregion


        #region Progress
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof (int), typeof (FindReplaceViewModel), new PropertyMetadata(default(int)));

        public int ProgressValue
        {
            get { return (int) GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        public static readonly DependencyProperty ProgressStringProperty =
            DependencyProperty.Register("ProgressString", typeof (string), typeof (FindReplaceViewModel), new PropertyMetadata(default(string)));

        public string ProgressString
        {
            get { return (string) GetValue(ProgressStringProperty); }
            set { SetValue(ProgressStringProperty, value); }
        }

        public static readonly DependencyProperty ProgressVisibilityProperty =
            DependencyProperty.Register("ProgressVisibility", typeof (Visibility), typeof (FindReplaceViewModel), new PropertyMetadata(Visibility.Visible));

        public Visibility ProgressVisibility
        {
            get { return (Visibility) GetValue(ProgressVisibilityProperty); }
            set { SetValue(ProgressVisibilityProperty, value); }
        }
        #endregion

        #region Cancel
        public static readonly DependencyProperty CancelEnabledProperty =
            DependencyProperty.Register("CancelEnabled", typeof (bool), typeof (FindReplaceViewModel), new PropertyMetadata(default(bool)));

        public bool CancelEnabled
        {
            get { return (bool) GetValue(CancelEnabledProperty); }
            set { SetValue(CancelEnabledProperty, value); }
        }

        #endregion

        #region Status
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof (string), typeof (FindReplaceViewModel), new PropertyMetadata(default(string)));

        public string StatusText
        {
            get { return (string) GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }
        #endregion

        #endregion

        #region Constructor
        public FindReplaceViewModel():base("Search Results")
        {
            DirectoryItems=new ObservableCollection<string> {"Current Window", "Current Selection", "Files"};
            StatusText = "...";
            _background.DoWork += _backgroundWorker_DoWork;
            _background.ProgressChanged += backgroundWorker_ProgressChanged;
            _background.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

    
        #endregion

        #region Commands
        private RelayCommand _findAllCommand;
        public ICommand FindAllCommand { get { return _findAllCommand ?? (_findAllCommand = new RelayCommand(p => FindAll(), p => true)); } }

        private RelayCommand _replaceAllCommand;
        public ICommand ReplaceAllCommand { get { return _replaceAllCommand ?? (_replaceAllCommand = new RelayCommand(p => ReplaceAll(), p => true)); } }

        private RelayCommand _findCommand;
        public ICommand FindCommand { get { return _findCommand ?? (_findCommand = new RelayCommand(p => Find(), p => DirectoryString==("Current Window")|DirectoryString=="Current Selection")); } }
        private RelayCommand _replaceCommand;
        public ICommand ReplaceCommand { get { return _replaceCommand ?? (_replaceCommand = new RelayCommand(p => Replace(), p =>  DirectoryString==("Current Window")|DirectoryString=="Current Selection")); } }
        private RelayCommand _browseFoldersCommand;
        public ICommand BrowseFoldersCommand { get { return _browseFoldersCommand ?? (_browseFoldersCommand = new RelayCommand(p => BrowseForFolder(), p => true)); } }

        #endregion


        #region Methods

        void BrowseForFolder()
        {
            
        }

        private int DoFind(BackgroundWorker worker)
        {
            //Initialize the affected count variable
            var filesAffectedCount = 0;

            //Initialize the counter
            var counter = 0;


            //TODO Add FileTypes

            //Get all XML files in the directory
            var filesInDirectory = Directory.GetFiles(DirectoryString, "*.src");

            //Initialize total file count
            int totalFiles = filesInDirectory.GetLength(0);

            //Analyze each file in the directory
            foreach (string file in filesInDirectory)
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
        /// Executes the main find and replace operation.
        /// </summary>
        /// <param name="worker">The BackgroundWorker object.</param>
        /// <returns>The number of files affected by the replace operation.</returns>
        private int DoFindReplace(BackgroundWorker worker)
        {
            //Initialize the affected count variable
            var filesAffectedCount = 0;

            //Initialize the counter
            var counter = 0;


            //TODO Add FileTypes

            //Get all XML files in the directory
            var filesInDirectory = Directory.GetFiles(DirectoryString, "*.txt");

            //Initialize total file count
            int totalFiles = filesInDirectory.GetLength(0);

            //Analyze each file in the directory
            foreach (string file in filesInDirectory)
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
            //holds the content of the file

            //Create a new object to read a file
            var content = File.ReadAllText(file);

            //Get search text
            var searchText = GetSearchText(FindString);

            switch (Work)
            {
                case WorkType.Find:
                    Find(file);
                    break;
                case WorkType.FindAll:
                    FindAll(file);
                    break;
                case WorkType.Replace:
                    Replace(file);
                    break;
                case WorkType.ReplaceAll:
                    ReplaceAll(file);
                    break;
            }

            //Look for a match
            if (Regex.IsMatch(content, searchText, GetRegExOptions()))
            {
                //Replace the text
                var newText = Regex.Replace(content, searchText, ReplaceString, GetRegExOptions());

                //Create a new object to write a file
                File.WriteAllText(file,newText);

                //A match was found and replaced
                return true;
            }

            //No match found and replaced
            return false;
        }
        void Find(string file)
        {
            
        }
        void FindAll(string file)
        {
            //Create a new object to read a file
            var content = File.ReadAllText(file);

            //Get search text
            var searchText = GetSearchText(FindString);

            SearchResults = Regex.Matches(content, searchText);
            RaisePropertyChanged("SearchResults");


        }

        void Replace(string file)
        {
            
        }
        void ReplaceAll(string file)
        {
            
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
            string searchText = Regex.Escape(textToFind);

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
      
        #endregion

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

            DialogResult result;


            if (e.Error != null)
                StatusText = e.Error.Message.ToString(CultureInfo.InvariantCulture);
            else if (e.Cancelled)
                StatusText = "The operation was ended by the user";
            else
                StatusText=string.Format("{0} files were updated by the operation", e.Result);
        }

      

        private void lnkBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Select a directory to output files
            BrowseDirectory();
        }

       
        #endregion

        private enum WorkType
        {
            Find,
            FindAll,
            Replace,
            ReplaceAll
        };

       
        void UpdateComboBoxes(ICollection<string> box,string item )
        {
            if (box.Any(v => item == v))
                return;

            box.Add(item);
        }

        private void FindAll()
        {
            InitializeSearch(WorkType.FindAll);
        }

        private void ReplaceAll()
        {
            InitializeSearch(WorkType.ReplaceAll);
        }

        void Find()
        {
            InitializeSearch(WorkType.Find);
        }
        void Replace()
        {
            InitializeSearch(WorkType.Replace);
        }

        private WorkType Work;
        void InitializeSearch(WorkType worktype)
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
                    UpdateComboBoxes(FindItems, FindString);
                    break;
                case WorkType.Replace:
                case WorkType.ReplaceAll:
                    UpdateComboBoxes(ReplaceItems, ReplaceString);
                    break;
            }

            UpdateComboBoxes(DirectoryItems,DirectoryString);

            _background.RunWorkerAsync();
        }
    }

}
