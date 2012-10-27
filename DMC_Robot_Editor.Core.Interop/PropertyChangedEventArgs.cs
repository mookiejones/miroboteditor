using System;

namespace miRobotEditor.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class PropertyChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        Services.Properties properties;
        string key;
        object newValue;
        object oldValue;

        /// <returns>
        /// returns the changed property object
        /// </returns>
        public Services.Properties Properties
        {
            get
            {
                return properties;
            }
        }

        /// <returns>
        /// The key of the changed property
        /// </returns>
        public string Key
        {
            get
            {
                return key;
            }
        }

        /// <returns>
        /// The new value of the property
        /// </returns>
        public object NewValue
        {
            get
            {
                return newValue;
            }
        }

        /// <returns>
        /// The new value of the property
        /// </returns>
        public object OldValue
        {
            get
            {
                return oldValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="key"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public PropertyChangedEventArgs(Services.Properties properties, string key, object oldValue, object newValue)
        {
            this.properties = properties;
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
