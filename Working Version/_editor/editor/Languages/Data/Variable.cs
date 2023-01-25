using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    public sealed class Variable : ViewModelBase, IVariable
    {
        private string _comment = string.Empty;
        private string _declaration = string.Empty;
        private string _description = string.Empty;
        private BitmapImage _icon;
        private bool _isSelected;
        private string _name = string.Empty;
        private int _offset;
        private string _path = string.Empty;
        private string _type = string.Empty;
        private string _value = string.Empty;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public BitmapImage Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged("Icon");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged("Path");
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        public string Declaration
        {
            get { return _declaration; }
            set
            {
                _declaration = value;
                RaisePropertyChanged("Declaration");
            }
        }

        public int Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                RaisePropertyChanged("Offset");
            }
        }

        #region Returns

        /// <summary>
        ///     The <see cref="Returns" /> property's name.
        /// </summary>
        private const string ReturnsPropertyName = "Returns";

        private string _returns = String.Empty;

        /// <summary>
        ///     Sets and gets the Returns property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Returns
        {
            get { return _returns; }

            set
            {
                if (_returns == value)
                {
                    return;
                }

                
                _returns = value;
                RaisePropertyChanged(ReturnsPropertyName);
            }
        }

        #endregion

        #region ShowDeclaration

        /// <summary>
        ///     The <see cref="ShowDeclaration" /> property's name.
        /// </summary>
        private const string ShowDeclarationPropertyName = "ShowDeclaration";

        private bool _showDeclaration;

        /// <summary>
        ///     Sets and gets the ShowDeclaration property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowDeclaration
        {
            get { return _showDeclaration; }

            set
            {
                if (_showDeclaration == value)
                {
                    return;
                }

                
                _showDeclaration = value;
                RaisePropertyChanged(ShowDeclarationPropertyName);
            }
        }

        #endregion

        #region ShowReturns

        /// <summary>
        ///     The <see cref="ShowReturns" /> property's name.
        /// </summary>
        private const string ShowReturnsPropertyName = "ShowReturns";

        private bool _showReturns;

        /// <summary>
        ///     Sets and gets the ShowReturns property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowReturns
        {
            get { return _showReturns; }

            set
            {
                if (_showReturns == value)
                {
                    return;
                }

                
                _showReturns = value;
                RaisePropertyChanged(ShowReturnsPropertyName);
            }
        }

        #endregion

        #region ShowOffset

        /// <summary>
        ///     The <see cref="ShowOffset" /> property's name.
        /// </summary>
        private const string ShowOffsetPropertyName = "ShowOffset";

        private string _showOffset = String.Empty;

        /// <summary>
        ///     Sets and gets the ShowOffset property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string ShowOffset
        {
            get { return _showOffset; }

            set
            {
                if (_showOffset == value)
                {
                    return;
                }

                
                _showOffset = value;
                RaisePropertyChanged(ShowOffsetPropertyName);
            }
        }

        #endregion
    }
}