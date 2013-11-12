/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 7:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.ComponentModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace miRobotEditor.ViewModel
{
	/// <summary>
	/// Description of PaneViewModel.
	/// </summary>
	[Localizable(false)]
	public class PaneViewModel:ViewModelBase
	{
	    #region Title

        
        #region
        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _title = string.Empty;

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                RaisePropertyChanging(TitlePropertyName);
                var oldValue = _title;
                _title = value;
                RaisePropertyChanged(TitlePropertyName, oldValue, value, true);
            }
        }
        #endregion

        #endregion

        public ImageSource IconSource
        {
            get;
            set;
        }

        #region ContentId

        private string _contentId;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId == value) return;
                _contentId = value;
                RaisePropertyChanged("ContentId");
            }
        }

        #endregion

        #region IsSelected

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        #endregion

        #region IsActive

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        #endregion

	}
    public enum DefaultToolPane { Left, Right, Bottom, None };
}
