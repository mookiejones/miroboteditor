using System.ComponentModel;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Abstract base class for an AvalonDock pane view-model.
    /// </summary>
    public abstract class AbstractPaneViewModel : AbstractViewModel
    {
        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
        private bool _isVisible = true;

        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
        [Localizable(false)]
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;

                OnPropertyChanged("IsVisible");
            }
        }
    }
}
