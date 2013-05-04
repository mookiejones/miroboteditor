/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 2:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using miRobotEditor.Classes;
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
        private bool isVisible = true;

        /// <summary>
        /// Set to 'true' when the pane is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                if (isVisible == value)
                {
                    return;
                }

                isVisible = value;

                RaisePropertyChanged("IsVisible");
            }
        }
    }
}
