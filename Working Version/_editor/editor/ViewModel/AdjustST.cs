using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
    internal sealed class AdjustST : ViewModelBase
    {
        #region ToolItems

        /// <summary>
        ///     The <see cref="ToolItems" /> property's name.
        /// </summary>
        private const string ToolItemsPropertyName = "ToolItems";

        private ToolItems _toolItems = new ToolItems();

        /// <summary>
        ///     Sets and gets the ToolItems property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ToolItems ToolItems
        {
            get { return _toolItems; }

            set
            {
                if (_toolItems == value)
                {
                    return;
                }

                
                _toolItems = value;
                RaisePropertyChanged(ToolItemsPropertyName);
            }
        }

        #endregion
    }
}