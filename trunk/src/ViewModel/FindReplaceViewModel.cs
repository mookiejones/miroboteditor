using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class FindReplaceViewModel : ViewModelBase
    {
        #region  Commands




        #region FindPreviousCommand
        private RelayCommand _findPreviousCommand;

        /// <summary>
        /// Gets the FindPreviousCommand.
        /// </summary>
        public RelayCommand FindPreviousCommand
        {
            get
            {
                return _findPreviousCommand
                    ?? (_findPreviousCommand = new RelayCommand(FindPrevious));
            }
        }

        private void ExecuteFindPreviousCommand()
        {
            
        }
        #endregion


        #region FindNextCommand
        private RelayCommand _findNextCommand;

        /// <summary>
        /// Gets the FindNextCommand.
        /// </summary>
        public RelayCommand FindNextCommand
        {
            get
            {
                return _findNextCommand
                    ?? (_findNextCommand = new RelayCommand(FindNext));
            }
        }


        #endregion


    

        #region ReplaceCommand
        private RelayCommand _replaceCommand;

        /// <summary>
        /// Gets the ReplaceCommand.
        /// </summary>
        public RelayCommand ReplaceCommand
        {
            get
            {
                return _replaceCommand
                    ?? (_replaceCommand = new RelayCommand(Replace));
            }
        }

       
        #endregion


        #region ReplaceAllCommand
        private RelayCommand _replaceAllCommand;

        /// <summary>
        /// Gets the ReplaceAllCommand.
        /// </summary>
        public RelayCommand ReplaceAllCommand
        {
            get
            {
                return _replaceAllCommand
                    ?? (_replaceAllCommand = new RelayCommand(ReplaceAll));
            }
        }


        #region HighlightAllCommand
        private RelayCommand _highlightAllCommand;

        /// <summary>
        /// Gets the HighlightAllCommand.
        /// </summary>
        public RelayCommand HighlightAllCommand
        {
            get
            {
                return _highlightAllCommand
                    ?? (_highlightAllCommand = new RelayCommand(HighlightAll));
            }
        }

     
        #endregion

        #region FindAllCommand
        private RelayCommand _findAllCommand;

        /// <summary>
        /// Gets the FindAllCommand.
        /// </summary>
        public RelayCommand FindAllCommand
        {
            get
            {
                return _findAllCommand
                    ?? (_findAllCommand = new RelayCommand(FindAll));
            }
        }

        #endregion

        #endregion

        #endregion

        private static FindReplaceViewModel _instance;

        public static FindReplaceViewModel Instance
        {
            get { return _instance ?? (_instance = new FindReplaceViewModel()); }
            set { _instance = value; }
        }

        #region Properties

        private string _lookfor = string.Empty;
        private bool _matchcase;
        private bool _matchwholeword;
        private string _replacewith = string.Empty;
        private string _searchresult = string.Empty;
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

 

        #region MatchWholeWord 
        /// <summary>
        /// The <see cref="MatchWholeWord" /> property's name.
        /// </summary>
        private const string MatchWholeWordPropertyName = "MatchWholeWord";

        private bool _matchWholeWord = false;

        /// <summary>
        /// Sets and gets the MatchWholeWord property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool MatchWholeWord
        {
            get
            {
                return _matchWholeWord;
            }

            set
            {
                if (_matchWholeWord == value)
                {
                    return;
                }

                RaisePropertyChanging(MatchWholeWordPropertyName);
                _matchWholeWord = value;
                RaisePropertyChanged(MatchWholeWordPropertyName);
            }
        }
        #endregion

        #region MatchCase
        /// <summary>
        /// The <see cref="MatchCase" /> property's name.
        /// </summary>
        private const string MatchCasePropertyName = "MatchCase";

        private bool _matchCase = false;

        /// <summary>
        /// Sets and gets the MatchCase property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool MatchCase
        {
            get
            {
                return _matchCase;
            }

            set
            {
                if (_matchCase == value)
                {
                    return;
                }

                RaisePropertyChanging(MatchCasePropertyName);
                _matchCase = value;
                RaisePropertyChanged(MatchCasePropertyName);
            }
        }
        #endregion
        public Regex RegexPattern
        {
            get
            {
                string pattern = UseRegex == false ? Regex.Escape(LookFor) : LookFor;
                int options = MatchCase ? 0 : 1;
                return new Regex(pattern, (RegexOptions) options);
            }
        }

        public string RegexString
        {
            get { return UseRegex == false ? Regex.Escape(LookFor) : LookFor; }
        }

        public string LookFor
        {
            get { return _lookfor; }
            set
            {
                _lookfor = value;
                RaisePropertyChanged(@"LookFor");
            }
        }

        public string ReplaceWith
        {
            get { return _replacewith; }
            set
            {
                _replacewith = value;
                RaisePropertyChanged(@"ReplaceWith");
            }
        }

        public string SearchResult
        {
            get { return _searchresult; }
            set
            {
                _searchresult = value;
                RaisePropertyChanged(@"SearchResult");
            }
        }

        #endregion

        private static void FindPrevious()
        {
            throw new NotImplementedException();
        }

        private static void FindNext()
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.ActiveEditor.TextBox.FindText();
        }

        private static void Replace()
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.ActiveEditor.TextBox.ReplaceText();
        }

        private static void ReplaceAll()
        {
            throw new NotImplementedException();
        }

        private static void HighlightAll()
        {
            throw new NotImplementedException();
        }

        private static void FindAll()
        {
            throw new NotImplementedException();
        }
    }

    public class FindandReplaceControl : Window
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