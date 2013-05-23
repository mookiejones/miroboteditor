/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 8:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace miRobotEditor.ViewModel
{
    public enum DefaultToolPane { Left, Right, Bottom, None };

	/// <summary>
	/// Description of ToolViewModel.
	/// </summary>
	public class ToolViewModel:PaneViewModel
	{


     public ToolViewModel(string name)
        {
            Name = name;
            Title = name;
        }

     public DefaultToolPane DefaultPane = DefaultToolPane.None;
    
        // Sizes for External Tools
     public int Height { get; set; }
     public int Width { get; set; }


        public string Name
        {
            get;
            private set;
        }

        #region IsVisible

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        #endregion

	}
}
