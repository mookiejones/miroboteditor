

namespace miRobotEditor.ViewModel
{
    public sealed class Item : ViewModelBase
    {
        #region Index



        private int _index = -1;

        /// <summary>
        ///     Sets and gets the Index property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int Index
        {
            get => _index;

            set => SetProperty(ref _index, value);

        }

        #endregion

        #region Type



        private string _type = string.Empty;

        /// <summary>
        ///     Sets and gets the Type property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Type
        {
            get => _type;

            set => SetProperty(ref _type, value);
        }

        #endregion

        #region Description



        private string _description = string.Empty;

        /// <summary>
        ///     Sets and gets the Description property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Description
        {
            get => _description;

            set => SetProperty(ref _description, value);
        }

        #endregion

        public Item(string type, string description)
        {
            Type = type;
            Description = description;
        }

        public override string ToString() => $"{Type};{Description}";
    }
}