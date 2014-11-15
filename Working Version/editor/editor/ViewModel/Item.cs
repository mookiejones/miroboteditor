using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    public sealed class Item : ViewModelBase
    {
        #region Index

        /// <summary>
        ///     The <see cref="Index" /> property's name.
        /// </summary>
        private const string IndexPropertyName = "Index";

        private int _index = -1;

        /// <summary>
        ///     Sets and gets the Index property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Index
        {
            get { return _index; }

            set
            {
                if (_index == value)
                {
                    return;
                }

                
                _index = value;
                RaisePropertyChanged(IndexPropertyName);
            }
        }

        #endregion

        #region Type

        /// <summary>
        ///     The <see cref="Type" /> property's name.
        /// </summary>
        private const string TypePropertyName = "Type";

        private string _type = string.Empty;

        /// <summary>
        ///     Sets and gets the Type property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Type
        {
            get { return _type; }

            set
            {
                if (_type == value)
                {
                    return;
                }

                
                _type = value;
                RaisePropertyChanged(TypePropertyName);
            }
        }

        #endregion

        #region Description

        /// <summary>
        ///     The <see cref="Description" /> property's name.
        /// </summary>
        private const string DescriptionPropertyName = "Description";

        private string _description = string.Empty;

        /// <summary>
        ///     Sets and gets the Description property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Description
        {
            get { return _description; }

            set
            {
                if (_description == value)
                {
                    return;
                }

                
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        #endregion

        public Item(string type, string description)
        {
            Type = type;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", Type, Description);
        }
    }
}