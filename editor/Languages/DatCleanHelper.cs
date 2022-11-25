using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.ViewModel;

namespace RobotEditor.Languages
{
    public sealed class DatCleanHelper : ToolViewModel
    {
        private const int NItemsSelected = 0;
        private const int NItemsTotal = 0;
        private static DatCleanHelper _instance;
        public static RelayCommand Cleandat;
        private readonly string _filename;

        private readonly ObservableCollection<string> _usedvartypes = new ObservableCollection<string>
        {
            "actual selection",
            "actual dat",
            "all Dat's"
        };

        private bool _commentdeclaration;
        private bool _deletedeclaration;
        private bool _exclusivetypes;
        private bool _ignoretypes;
        private ReadOnlyCollection<IVariable> _listItems;
        private int _progress;

        private int _selectedVarIndex;

        public DatCleanHelper()
            : base("Dat Cleaner")
        {
            Instance = this;
            DefaultPane = DefaultToolPane.Right;
            var instance = ServiceLocator.Current.GetInstance<MainViewModel>();
            _filename = instance.ActiveEditor.FilePath;
            base.Width = 619;
            base.Height = 506;
        }

        public ReadOnlyCollection<IVariable> ListItems
        {
            get
            {
                ReadOnlyCollection<IVariable> _items;
                if ((_items = _listItems) == null)
                {
                    _items =
                        (_listItems =
                            ServiceLocator.Current.GetInstance<ObjectBrowserViewModel>()
                                .GetVarForFile(KUKA.GetDatFileName(_filename)));
                }
                return _items;
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public static DatCleanHelper Instance
        {
            get
            {
                DatCleanHelper arg_15_0;
                if ((arg_15_0 = _instance) == null)
                {
                    arg_15_0 = (_instance = new DatCleanHelper());
                }
                return arg_15_0;
            }
            set { _instance = value; }
        }

        public bool IgnoreTypes
        {
            get { return _ignoretypes; }
            set
            {
                _ignoretypes = value;
                RaisePropertyChanged("IgnoreTypes");
            }
        }

        public bool ExclusiveTypes
        {
            get { return _exclusivetypes; }
            set
            {
                _exclusivetypes = value;
                RaisePropertyChanged("ExclusiveTypes");
            }
        }

        public bool DeleteDeclaration
        {
            get { return _deletedeclaration; }
            set
            {
                _deletedeclaration = value;
                RaisePropertyChanged("DeleteDeclaration");
            }
        }

        public bool CommentDeclaration
        {
            get { return _commentdeclaration; }
            set
            {
                _commentdeclaration = value;
                RaisePropertyChanged("CommentDeclaration");
            }
        }

        public int SelectedVarIndex
        {
            get { return _selectedVarIndex; }
            set
            {
                _selectedVarIndex = value;
                RaisePropertyChanged("SelectedVarIndex");
            }
        }

        public ObservableCollection<string> UsedVarTypes
        {
            get { return _usedvartypes; }
        }

        #region CleanDatCmd

        private static RelayCommand<object> _cleanDatCmd;

        /// <summary>
        ///     Gets the CleanDatCmd.
        /// </summary>
        public static RelayCommand<object> CleanDatCmd
        {
            get
            {
                return _cleanDatCmd
                       ?? (_cleanDatCmd = new RelayCommand<object>(
                           p => Instance.CleanDat()));
            }
        }

        #endregion

        #region CheckedCmd

        private static RelayCommand _checkedCmd;

        /// <summary>
        ///     Gets the CheckedCmd.
        /// </summary>
        public static RelayCommand CheckedCmd
        {
            get
            {
                return _checkedCmd
                       ?? (_checkedCmd = new RelayCommand(
                           () => Instance.Checked()));
            }
        }

        #endregion

        #region DeleteVarTypeCmd

        private RelayCommand _deleteVarTypeCmd;

        /// <summary>
        ///     Gets the DeleteVarTypeCmd.
        /// </summary>
        public RelayCommand DeleteVarTypeCmd
        {
            get
            {
                return _deleteVarTypeCmd
                       ?? (_deleteVarTypeCmd = new RelayCommand(ExecuteDeleteVarTypeCmd));
            }
        }

        private void ExecuteDeleteVarTypeCmd()
        {
            Instance.DeleteVarType();
        }

        #endregion

        #region AddVarTypeCmd

        private RelayCommand _addVarTypeCmd;

        /// <summary>
        ///     Gets the AddVarTypeCmd.
        /// </summary>
        public RelayCommand AddVarTypeCmd
        {
            get
            {
                return _addVarTypeCmd
                       ?? (_addVarTypeCmd = new RelayCommand(ExecuteAddVarTypeCmd));
            }
        }

        private void ExecuteAddVarTypeCmd()
        {
            Instance.AddVarType();
        }

        #endregion

        #region SelectAllCommand

        private RelayCommand _selectAllCommand;

        /// <summary>
        ///     Gets the SelectAllCommand.
        /// </summary>
        public RelayCommand SelectAllCommand
        {
            get
            {
                return _selectAllCommand
                       ?? (_selectAllCommand = new RelayCommand(ExecuteSelectAllCommand));
            }
        }

        private void ExecuteSelectAllCommand()
        {
            Instance.SelectAll();
        }

        #endregion

        #region InvertSelectionCommand

        private static RelayCommand _invertSelectionCommand;

        /// <summary>
        ///     Gets the InvertSelectionCommand.
        /// </summary>
        public static RelayCommand InvertSelectionCommand
        {
            get
            {
                return _invertSelectionCommand
                       ?? (_invertSelectionCommand = new RelayCommand(
                           () => Instance.InvertSelection()));
            }
        }

        #endregion

        public void CleanDat()
        {
            throw new NotImplementedException();
        }

        public void Checked()
        {
            throw new NotImplementedException();
        }

        public void DeleteVarType()
        {
            throw new NotImplementedException();
        }

        public void AddVarType()
        {
            throw new NotImplementedException();
        }

        private void SelectAll()
        {
            foreach (var current in ListItems)
            {
                current.IsSelected = true;
            }
            RaisePropertyChanged("IgnoreTypes");
        }

        private void InvertSelection()
        {
            foreach (var current in ListItems)
            {
                current.IsSelected = !current.IsSelected;
            }
            RaisePropertyChanged("IgnoreTypes");
        }

        public void Clean()
        {
        }
    }
}