using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using miRobotEditor.ViewModel;


namespace miRobotEditor.Languages
{
    /// <summary>
    /// Description of DatCleanHelper.
    /// </summary>
    public class DatCleanHelper : ToolViewModel
    {


        private readonly string _filename;


        private ReadOnlyCollection<IVariable> _listItems;
        public ReadOnlyCollection<IVariable> ListItems
        {
            get {
                return _listItems ??
                       (_listItems =
                        ObjectBrowserViewModel.Instance.GetVarForFile(KUKA.GetDatFileName(_filename)));
            }
        }



        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; RaisePropertyChanged("Progress"); }
        }

        private static DatCleanHelper _instance;
        public static DatCleanHelper Instance
        {
            get { return _instance ?? (_instance = new DatCleanHelper()); }
            set { _instance = value; }
        }

        #region Commands

        #region CleanDatCmd

        private RelayCommand _cleanDatCmdCommand;
        /// <summary>
        /// Gets the CleanDatCmd.
        /// </summary>
        public RelayCommand CleanDatCmd
        {
            get
            {
                return _cleanDatCmdCommand
                    ?? (_cleanDatCmdCommand = new RelayCommand(ExecuteCleanDatCmd));
            }
        }

        private void ExecuteCleanDatCmd()
        {
            Instance.CleanDat();
        }
        #endregion
       
        public void CleanDat() { throw new NotImplementedException(); }


        #region CheckedCmd

        private RelayCommand _checkedCmdCommand;
        /// <summary>
        /// Gets the CheckedCmd.
        /// </summary>
        public RelayCommand CheckedCmd
        {
            get
            {
                return _checkedCmdCommand
                    ?? (_checkedCmdCommand = new RelayCommand(ExecuteCheckedCmd));
            }
        }

        private void ExecuteCheckedCmd()
        {
            Instance.Checked();
        }
        #endregion

        public void Checked()
        {
            throw new NotImplementedException();
        }

        #region DeleteVarTypeCmd

        private RelayCommand _deleteVarTypeCmdCommand;
        /// <summary>
        /// Gets the DeleteVarTypeCmd.
        /// </summary>
        public RelayCommand DeleteVarTypeCmd
        {
            get
            {
                return _deleteVarTypeCmdCommand
                    ?? (_deleteVarTypeCmdCommand = new RelayCommand(ExecuteDeleteVarTypeCmd));
            }
        }

        private void ExecuteDeleteVarTypeCmd()
        {
            Instance.DeleteVarType();
        }
        #endregion

    
        public void DeleteVarType() { throw new NotImplementedException(); }


        #region AddVarTypeCmd

        private RelayCommand _addVarTypeCmdCommand;
        /// <summary>
        /// Gets the AddVarTypeCmd.
        /// </summary>
        public RelayCommand AddVarTypeCmd
        {
            get
            {
                return _addVarTypeCmdCommand
                    ?? (_addVarTypeCmdCommand = new RelayCommand(ExecuteAddVarTypeCmd));
            }
        }

        private void ExecuteAddVarTypeCmd()
        {
            Instance.AddVarType();
        }
        #endregion
    
        public void AddVarType() { throw new NotImplementedException(); }



        #region SelectAllCommand

        private RelayCommand _selectAllCommand;
        /// <summary>
        /// Gets the SelectAllCommand.
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

        private RelayCommand _invertSelectionCommand;
        /// <summary>
        /// Gets the InvertSelectionCommand.
        /// </summary>
        public RelayCommand InvertSelectionCommand
        {
            get
            {
                return _invertSelectionCommand
                    ?? (_invertSelectionCommand = new RelayCommand(ExecuteInvertSelectionCommand));
            }
        }

        private void ExecuteInvertSelectionCommand()
        {
            Instance.InvertSelection();
        }
        #endregion
     
        void SelectAll()
        {
            foreach (var v in ListItems)
                v.IsSelected = true;

            RaisePropertyChanged("IgnoreTypes");
        }
        void InvertSelection()
        {
            foreach (var v in ListItems)
                v.IsSelected = !v.IsSelected;
            RaisePropertyChanged("IgnoreTypes");

        }
        #endregion

        private bool _ignoretypes;
        private bool _exclusivetypes;
        private bool _deletedeclaration;
        private bool _commentdeclaration;
        private readonly ObservableCollection<String> _usedvartypes = new ObservableCollection<string> { "actual selection", "actual dat", "all Dat's" };
        public bool IgnoreTypes
        {
            get { return _ignoretypes; }
            set { _ignoretypes = value; RaisePropertyChanged("IgnoreTypes"); }
        }
        public bool ExclusiveTypes
        {
            get { return _exclusivetypes; }
            set { _exclusivetypes = value; RaisePropertyChanged("ExclusiveTypes"); }
        }
        public bool DeleteDeclaration
        {
            get { return _deletedeclaration; }
            set { _deletedeclaration = value; RaisePropertyChanged("DeleteDeclaration"); }
        }
        public bool CommentDeclaration
        {
            get { return _commentdeclaration; }
            set { _commentdeclaration = value; RaisePropertyChanged("CommentDeclaration"); }
        }

        private int _selectedVarIndex;
        public int SelectedVarIndex
        {
            get { return _selectedVarIndex; }
            set { _selectedVarIndex = value; RaisePropertyChanged("SelectedVarIndex"); }
        }
        public ObservableCollection<String> UsedVarTypes
        {
            get { return _usedvartypes; }
        }

    

        public DatCleanHelper():base("Dat Cleaner")
        {
            Instance = this;
            DefaultPane = DefaultToolPane.Right;
            _filename = Workspace.Instance.ActiveEditor.FilePath;
            Width = 619;
            Height = 506;
        }


        public void Clean()
        {

        }
    }
}