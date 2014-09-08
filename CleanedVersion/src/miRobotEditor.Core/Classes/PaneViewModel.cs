using System;
using GalaSoft.MvvmLight;

namespace miRobotEditor.Core.Classes
{
    public class PaneViewModel:ViewModelBase
    {

        public PaneViewModel(string filename = "")
        {
            
        }

        #region ContentID
        /// <summary>
        /// The <see cref="ContentId" /> property's name.
        /// </summary>
        public const string ContentIdPropertyName = "ContentId";

        private string _contentID = String.Empty;

        /// <summary>
        /// Sets and gets the ContentId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ContentId
        {
            get
            {
                return _contentID;
            }

            set
            {
                if (_contentID == value)
                {
                    return;
                }

                RaisePropertyChanging(ContentIdPropertyName);
                _contentID = value;
                RaisePropertyChanged(ContentIdPropertyName);
            }
        }
        #endregion
  
        #region Title
        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        private const string TitlePropertyName = "Title";

        private string _title = string.Empty;

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
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
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }
        #endregion
    }
}
