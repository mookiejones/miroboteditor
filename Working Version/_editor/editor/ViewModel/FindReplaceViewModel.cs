using System;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace miRobotEditor.ViewModel
{
    public sealed class FindReplaceViewModel : ViewModelBase
    {
        // ReSharper disable UnusedField.Compiler
        private static RelayCommand _findpreviouscommand;
        private static RelayCommand _findnextcommand;
        private static RelayCommand _replacecommand;
        private static RelayCommand _replaceallcommand;
        private static RelayCommand _highlightallcommand;
        private static FindReplaceViewModel _instance;
        private RelayCommand _findallcommand;
        private string _lookfor = string.Empty;
        private bool _matchcase;
        private bool _matchwholeword;
        private string _replacewith = string.Empty;
        private string _searchresult = string.Empty;
        private bool _useregex;

        #region FindPreviousCommand

        private RelayCommand _findPreviousCommand;

        /// <summary>
        ///     Gets the FindPreviousCommand.
        /// </summary>
        public RelayCommand FindPreviousCommand
        {
            get
            {
                return _findPreviousCommand
                       ?? (_findPreviousCommand = new RelayCommand(FindPrevious));
            }
        }

        #endregion

        #region

        private RelayCommand _findNextCommand;

        /// <summary>
        ///     Gets the FindNextCommand.
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

        #region

        private static RelayCommand _replaceCommand;

        /// <summary>
        ///     Gets the ReplaceCommand.
        /// </summary>
        public static RelayCommand ReplaceCommand
        {
            get
            {
                return _replaceCommand
                       ?? (_replaceCommand = new RelayCommand(Replace));
            }
        }

        #endregion

        #region

        private static RelayCommand _replaceAllCommand;

        /// <summary>
        ///     Gets the ReplaceAllCommand.
        /// </summary>
        public static RelayCommand ReplaceAllCommand
        {
            get
            {
                return _replaceAllCommand
                       ?? (_replaceAllCommand = new RelayCommand(ReplaceAll));
            }
        }

        #endregion

        #region

        private static RelayCommand _highlightAllCommand;

        /// <summary>
        ///     Gets the HighlightAllCommand.
        /// </summary>
        public static RelayCommand HighlightAllCommand
        {
            get
            {
                return _highlightAllCommand
                       ?? (_highlightAllCommand = new RelayCommand(
                           HighlightAll));
            }
        }

        #endregion

        #region

        private RelayCommand _findAllCommand;

        /// <summary>
        ///     Gets the FindAllCommand.
        /// </summary>
        public RelayCommand FindAllCommand
        {
            get
            {
                return _findAllCommand
                       ?? (_findAllCommand = new RelayCommand(
                           FindAll));
            }
        }

        #endregion

        // ReSharper restore UnusedField.Compiler

        public static FindReplaceViewModel Instance
        {
            get
            {
                FindReplaceViewModel arg_15_0;
                if ((arg_15_0 = _instance) == null)
                {
                    arg_15_0 = (_instance = new FindReplaceViewModel());
                }
                return arg_15_0;
            }
            set { _instance = value; }
        }

        public bool UseRegex
        {
            get { return _useregex; }
            set
            {
                _useregex = value;
                RaisePropertyChanged("UseRegex");
            }
        }

        public bool MatchCase
        {
            get { return _matchcase; }
            set
            {
                _matchcase = value;
                RaisePropertyChanged("MatchCase");
            }
        }

        public bool MatchWholeWord
        {
            get { return _matchwholeword; }
            set
            {
                _matchwholeword = value;
                RaisePropertyChanged("MatchWholeWord");
            }
        }

        public Regex RegexPattern
        {
            get
            {
                var pattern = (!UseRegex) ? Regex.Escape(LookFor) : LookFor;
                var options = MatchCase ? 0 : 1;
                return new Regex(pattern, (RegexOptions) options);
            }
        }

        public string RegexString
        {
            get { return (!UseRegex) ? Regex.Escape(LookFor) : LookFor; }
        }

        public string LookFor
        {
            get { return _lookfor; }
            set
            {
                _lookfor = value;
                RaisePropertyChanged("LookFor");
            }
        }

        public string ReplaceWith
        {
            get { return _replacewith; }
            set
            {
                _replacewith = value;
                RaisePropertyChanged("ReplaceWith");
            }
        }

        public string SearchResult
        {
            get { return _searchresult; }
            set
            {
                _searchresult = value;
                RaisePropertyChanged("SearchResult");
            }
        }

        private static void FindPrevious()
        {
            throw new NotImplementedException();
        }

        private static void FindNext()
        {
            var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
            instance.ActiveEditor.TextBox.FindText();
        }

        private static void Replace()
        {
            var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
            instance.ActiveEditor.TextBox.ReplaceText();
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
}