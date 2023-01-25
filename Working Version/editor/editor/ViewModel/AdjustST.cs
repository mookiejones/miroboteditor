

namespace miRobotEditor.ViewModel
{
    internal sealed class AdjustST : ViewModelBase
    {
        #region ToolItems

        private ToolItems _toolItems = new();

        /// <summary>
        ///     Sets and gets the ToolItems property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ToolItems ToolItems
        {
            get => _toolItems;

            set => SetProperty(ref _toolItems, value);

        }

        #endregion
    }
}