/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 2:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
	  /// <summary>
    /// Abstract base class for an AvalonDock pane view-model.
    /// </summary>
    public abstract class AbstractPaneViewModel : ViewModelBase
    {
        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
        private bool _isVisible = true;

        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
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

                RaisePropertyChanged();
            }
        }
    }
}
