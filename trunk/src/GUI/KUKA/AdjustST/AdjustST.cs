using miRobotEditor.Core;

namespace miRobotEditor.GUI.KUKA.AdjustST
{
    sealed class AdjustST:ViewModelBase
    {
        /// <summary>
        /// The <see cref="ToolItems" /> property's name.
        /// </summary>
        private const string ToolItemsPropertyName = "ToolItems";

        private ToolItems _toolItems = new ToolItems();

        /// <summary>
        /// Sets and gets the ToolItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
// ReSharper disable once UnusedMember.Global
        public ToolItems ToolItems
        {
            get
            {
                return _toolItems;
            }

            set
            {
                if (_toolItems == value)
                {
                    return;
                }

                RaisePropertyChanging(ToolItemsPropertyName);
                _toolItems = value;
                RaisePropertyChanged(ToolItemsPropertyName);
            }
        }

    }
}
