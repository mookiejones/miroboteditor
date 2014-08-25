/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/11/2013
 * Time: 6:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using miRobotEditor.Commands;

namespace miRobotEditor.ViewModel
{
    // ReSharper disable UnusedMember.Local

    /// <summary>
    ///     Description of KUKAObjectBrowserViewModel.
    /// </summary>
    public class KUKAObjectBrowserViewModel : ViewModelBase
    {
        // ReSharper restore UnusedMember.Local


        private readonly ObservableCollection<EnumClass> _enumItems = new ObservableCollection<EnumClass>();
        private readonly ObservableCollection<FunctionClass> _functionItems = new ObservableCollection<FunctionClass>();
        private readonly ReadOnlyObservableCollection<EnumClass> _readonlyEnumItems = null;
        private readonly ReadOnlyObservableCollection<FunctionClass> _readonlyFunctionItems = null;
        private readonly ReadOnlyObservableCollection<StructureClass> _readonlyStructureItems = null;

        private readonly ReadOnlyObservableCollection<VariableClass> _readonlyVariableItems = null;

        private readonly ObservableCollection<StructureClass> _structureItems =
            new ObservableCollection<StructureClass>();

        private readonly ObservableCollection<VariableClass> _variableItems = new ObservableCollection<VariableClass>();
        private RelayCommand _clearFilterCommand;

        private string _variableitems = "0";

        public KUKAObjectBrowserViewModel()
        {
            Instance = this;
        }

        public ReadOnlyObservableCollection<FunctionClass> FunctionItems
        {
            get { return _readonlyFunctionItems ?? new ReadOnlyObservableCollection<FunctionClass>(_functionItems); }
        }

        public ReadOnlyObservableCollection<VariableClass> VariableItems
        {
            get { return _readonlyVariableItems ?? new ReadOnlyObservableCollection<VariableClass>(_variableItems); }
        }


        public ReadOnlyObservableCollection<EnumClass> EnumItems
        {
            get { return _readonlyEnumItems ?? new ReadOnlyObservableCollection<EnumClass>(_enumItems); }
        }


        public ReadOnlyObservableCollection<StructureClass> StructureItems
        {
            get { return _readonlyStructureItems ?? new ReadOnlyObservableCollection<StructureClass>(_structureItems); }
        }

        public static KUKAObjectBrowserViewModel Instance { get; set; }

        public ICommand ClearFilterCommand
        {
            get
            {
                return _clearFilterCommand ??
                       (_clearFilterCommand =
                           new RelayCommand(param => FilterText = String.Empty,
                               param => (!String.IsNullOrEmpty(FilterText))));
            }
        }


        
        #region SelectedItem
        /// <summary>
        /// The <see cref="SelectedItem" /> property's name.
        /// </summary>
        public const string SelectedItemPropertyName = "SelectedItem";

        private ListViewItem _selectedItem = new ListViewItem();

        /// <summary>
        /// Sets and gets the SelectedItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ListViewItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (_selectedItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedItemPropertyName);
                _selectedItem = value;
                RaisePropertyChanged(SelectedItemPropertyName);
            }
        }
        #endregion

        
        #region FilterText
        /// <summary>
        /// The <see cref="FilterText" /> property's name.
        /// </summary>
        public const string FilterTextPropertyName = "FilterText";

        private string _filterText = String.Empty;

        /// <summary>
        /// Sets and gets the FilterText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FilterText
        {
            get
            {
                return _filterText;
            }

            set
            {
                if (_filterText == value)
                {
                    return;
                }

                RaisePropertyChanging(FilterTextPropertyName);
                _filterText = value;
                RaisePropertyChanged(FilterTextPropertyName);
            }
        }
        #endregion



        
        #region Functions
        /// <summary>
        /// The <see cref="Functions" /> property's name.
        /// </summary>
        public const string FunctionsPropertyName = "Functions";

        private string _functions = "2";

        /// <summary>
        /// Sets and gets the Functions property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Functions
        {
            get
            {
                return _functions;
            }

            set
            {
                if (_functions == value)
                {
                    return;
                }

                RaisePropertyChanging(FunctionsPropertyName);
                _functions = value;
                RaisePropertyChanged(FunctionsPropertyName);
            }
        }
        #endregion

     


        public string VariablesItems
        {
            get { return _variableitems; }
            set
            {
                _variableitems = value;
                RaisePropertyChanged("VariableItems");
            }
        }

        public class EnumClass
        {
            private string Name { get; set; }
            private string Type { get; set; }
            private string Path { get; set; }
            private string IsGlobal { get; set; }
            private string Info { get; set; }
        }

        public class FunctionClass
        {
            private string Name { get; set; }
            private string Type { get; set; }
            private string Path { get; set; }
            private string IsGlobal { get; set; }
            private string Info { get; set; }
        }

        public class StructureClass
        {
            private string Name { get; set; }
            private string Type { get; set; }
            private string Path { get; set; }
            private string IsGlobal { get; set; }
            private string Info { get; set; }
        }

        public class VariableClass
        {
            private string Name { get; set; }
            private string Type { get; set; }
            private string Path { get; set; }
            private string IsGlobal { get; set; }
            private string Info { get; set; }
        }
    }
}