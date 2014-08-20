using System;
using System.Windows;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class ValueBoxViewModel :ViewModelBase
    {
        public event ItemsChangedEventHandler ItemsChanged;

        #region Properties
        private double _v1;
        private double _v2;
        private double _v3;
        private double _v4;
        public double V1 { get { return _v1; } set { _v1 = value; RaisePropertyChanged(); RaiseItemsChanged(); } }
        public double V2 { get { return _v2; } set { _v2 = value; RaisePropertyChanged(); RaiseItemsChanged(); } }
        public double V3 { get { return _v3; } set { _v3 = value; RaisePropertyChanged(); RaiseItemsChanged(); } }
        public double V4 { get { return _v4; } set { _v4 = value; RaisePropertyChanged(); RaiseItemsChanged(); } }

        private bool _isReadOnly;
        public bool IsReadOnly { get { return _isReadOnly; } set { _isReadOnly = value; RaisePropertyChanged(); } }

        private string _header = String.Empty;
        public string Header { get { return _header; } set { _header = value; RaisePropertyChanged(); } }

        private Visibility _boxVisibility = Visibility.Visible;
        public Visibility BoxVisibility { get { return _boxVisibility; } set { _boxVisibility = value; RaisePropertyChanged(); } }





        private readonly CartesianItems _selectionitems = new CartesianItems();
        public CartesianItems SelectionItems { get { return _selectionitems; } }


        private CartesianEnum _selectedItem = CartesianEnum.ABB_Quaternion;
        public CartesianEnum SelectedItem { get { return _selectedItem; } set { _selectedItem = value; CheckVisibility(); RaisePropertyChanged(); RaiseItemsChanged(); } }
        #endregion

        void RaiseItemsChanged()
        {
            if (ItemsChanged != null)
                ItemsChanged(this, new EventArgs());
        }
        void CheckVisibility()
        {
            switch (_selectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                case CartesianEnum.Axis_Angle:
                    BoxVisibility = Visibility.Visible;
                    break;
                default:
                    BoxVisibility = Visibility.Collapsed;
                    break;
            }

        }
    }
}