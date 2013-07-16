﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using miRobotEditor.Classes;
using miRobotEditor.Core;
using miRobotEditor.ViewModel;
using RelayCommand = miRobotEditor.Commands.RelayCommand;

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
            set { _progress = value; RaisePropertyChanged(); }
        }

        private static DatCleanHelper _instance;
        public static DatCleanHelper Instance
        {
            get { return _instance ?? (_instance = new DatCleanHelper()); }
            set { _instance = value; }
        }

        #region Commands
        public static RelayCommand Cleandat;
        public static ICommand CleanDatCmd
        {
            get { return Cleandat ?? (Cleandat = new RelayCommand(param => Instance.CleanDat(), param => true)); }
        }
        public void CleanDat() { throw new NotImplementedException(); }

        private static RelayCommand _checked;
        public static ICommand CheckedCmd
        {
            get { return _checked ?? (_checked = new RelayCommand(param => Instance.Checked(), param => true)); }
        }
        public void Checked()
        {
            throw new NotImplementedException();
        }

        private static RelayCommand _deletevartype;
        public static ICommand DeleteVarTypeCmd
        {
            get { return _deletevartype ?? (_deletevartype = new RelayCommand(param => Instance.DeleteVarType(), param => true)); }
        }
        public void DeleteVarType() { throw new NotImplementedException(); }
        private static RelayCommand _addvartype;
        public static ICommand AddVarTypeCmd
        {
            get { return _addvartype ?? (_addvartype = new RelayCommand(param => Instance.AddVarType(), param => true)); }
        }
        public void AddVarType() { throw new NotImplementedException(); }
        private static RelayCommand _selectallcommand;
        public static ICommand SelectAllCommand
        {
            get { return _selectallcommand ?? (_selectallcommand = new RelayCommand(param => Instance.SelectAll(), param => true)); }
        }
        private static RelayCommand _invertselection;
        public static ICommand InvertSelectionCommand
        {
            get
            {
                return _invertselection ??
                       (_invertselection = new RelayCommand(param => Instance.InvertSelection(), param => true));
            }
        }
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

#pragma warning disable 169
        private const int NItemsSelected = 0;
#pragma warning restore 169
#pragma warning disable 169
        private const int NItemsTotal = 0;
#pragma warning restore 169
        private bool _ignoretypes;
        private bool _exclusivetypes;
        private bool _deletedeclaration;
        private bool _commentdeclaration;
        private readonly ObservableCollection<String> _usedvartypes = new ObservableCollection<string> { "actual selection", "actual dat", "all Dat's" };
        public bool IgnoreTypes
        {
            get { return _ignoretypes; }
            set { _ignoretypes = value; RaisePropertyChanged(); }
        }
        public bool ExclusiveTypes
        {
            get { return _exclusivetypes; }
            set { _exclusivetypes = value; RaisePropertyChanged(); }
        }
        public bool DeleteDeclaration
        {
            get { return _deletedeclaration; }
            set { _deletedeclaration = value; RaisePropertyChanged(); }
        }
        public bool CommentDeclaration
        {
            get { return _commentdeclaration; }
            set { _commentdeclaration = value; RaisePropertyChanged(); }
        }

        private int _selectedVarIndex;
        public int SelectedVarIndex
        {
            get { return _selectedVarIndex; }
            set { _selectedVarIndex = value; RaisePropertyChanged(); }
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