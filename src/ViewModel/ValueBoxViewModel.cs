using System;
using System.Windows;
using GalaSoft.MvvmLight;
using miRobotEditor.Classes;
using miRobotEditor.Enums;

namespace miRobotEditor.ViewModel
{
    public class ValueBoxViewModel : ViewModelBase
    {
        public event ItemsChangedEventHandler ItemsChanged;

        private void RaiseItemsChanged()
        {
            if (ItemsChanged != null)
                ItemsChanged(this, new EventArgs());
        }

        private void CheckVisibility()
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

        #region Properties

        #region V1

        /// <summary>
        ///     The <see cref="V1" /> property's name.
        /// </summary>
        public const string V1PropertyName = "V1";

        private double _v1;

        /// <summary>
        ///     Sets and gets the V1 property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double V1
        {
            get { return _v1; }

            set
            {
                if (_v1 == value)
                {
                    return;
                }

                RaisePropertyChanging(V1PropertyName);
                _v1 = value;
                RaisePropertyChanged(V1PropertyName);
                RaiseItemsChanged();
            }
        }

        #endregion

        #region V2

        /// <summary>
        ///     The <see cref="V2" /> property's name.
        /// </summary>
        public const string V2PropertyName = "V2";

        private double _v2;

        /// <summary>
        ///     Sets and gets the V1 property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double V2
        {
            get { return _v2; }

            set
            {
                if (_v2 == value)
                {
                    return;
                }

                RaisePropertyChanging(V2PropertyName);
                _v2 = value;
                RaisePropertyChanged(V2PropertyName);
                RaiseItemsChanged();
            }
        }

        #endregion

        #region V3

        /// <summary>
        ///     The <see cref="V3" /> property's name.
        /// </summary>
        public const string V3PropertyName = "V3";

        private double _v3;

        /// <summary>
        ///     Sets and gets the V3 property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double V3
        {
            get { return _v3; }

            set
            {
                if (_v3 == value)
                {
                    return;
                }

                RaisePropertyChanging(V3PropertyName);
                _v3 = value;
                RaisePropertyChanged(V3PropertyName);
                RaiseItemsChanged();
            }
        }

        #endregion

        #region V4

        /// <summary>
        ///     The <see cref="V4" /> property's name.
        /// </summary>
        public const string V4PropertyName = "V4";

        private double _v4;

        /// <summary>
        ///     Sets and gets the V4 property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double V4
        {
            get { return _v4; }

            set
            {
                if (_v4 == value)
                {
                    return;
                }

                RaisePropertyChanging(V4PropertyName);
                _v4 = value;
                RaisePropertyChanged(V4PropertyName);
                RaiseItemsChanged();
            }
        }

        #endregion

        #region IsReadOnly

        /// <summary>
        ///     The <see cref="IsReadOnly" /> property's name.
        /// </summary>
        public const string IsReadOnlyPropertyName = "IsReadOnly";

        private bool _isReadOnly;

        /// <summary>
        ///     Sets and gets the IsReadOnly property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }

            set
            {
                if (_isReadOnly == value)
                {
                    return;
                }

                RaisePropertyChanging(IsReadOnlyPropertyName);
                _isReadOnly = value;
                RaisePropertyChanged(IsReadOnlyPropertyName);
            }
        }

        #endregion

        #region Header

        /// <summary>
        ///     The <see cref="Header" /> property's name.
        /// </summary>
        public const string HeaderPropertyName = "Header";

        private string _header = String.Empty;

        /// <summary>
        ///     Sets and gets the Header property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Header
        {
            get { return _header; }

            set
            {
                if (_header == value)
                {
                    return;
                }

                RaisePropertyChanging(HeaderPropertyName);
                _header = value;
                RaisePropertyChanged(HeaderPropertyName);
            }
        }

        #endregion

        #region Visibility

        /// <summary>
        ///     The <see cref="BoxVisibility" /> property's name.
        /// </summary>
        public const string BoxVisibilityPropertyName = "BoxVisibility";

        private Visibility _boxVisibility = Visibility.Visible;

        /// <summary>
        ///     Sets and gets the BoxVisibility property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Visibility BoxVisibility
        {
            get { return _boxVisibility; }

            set
            {
                if (_boxVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(BoxVisibilityPropertyName);
                _boxVisibility = value;
                RaisePropertyChanged(BoxVisibilityPropertyName);
            }
        }

        #endregion

        #region CartesianItems

        private readonly CartesianItems _selectionitems = new CartesianItems();

        public CartesianItems SelectionItems => _selectionitems;

        #endregion

        private CartesianEnum _selectedItem = CartesianEnum.ABB_Quaternion;

        public CartesianEnum SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                CheckVisibility();
                RaisePropertyChanged("SelectedItem");
                RaiseItemsChanged();
            }
        }

        #endregion
    }
}