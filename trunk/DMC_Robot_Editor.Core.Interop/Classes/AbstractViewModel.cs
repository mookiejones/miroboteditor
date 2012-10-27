using System.ComponentModel;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Abstract base-class for a view-model.
    /// </summary>
    public abstract class AbstractViewModel:INotifyPropertyChanged          
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
