using System.ComponentModel;

namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class ViewModelBase:SerializeBase,INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
      
        protected  void RaisePropertyChanged(string propertyName)
        {
            try
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (System.Exception ex)
            {
                if (ex.Message == @"'#FF333333' is not a valid value for property 'BorderBrush'.")
                    return;
                MessageViewModel.AddError("Error on RaisePropertyChanged with " + propertyName,ex);
            }
        }
        
        protected  void RaisePropertyChanging(string propertyName)
        {
        	if (PropertyChanging!=null)
        		PropertyChanging(this,new PropertyChangingEventArgs(propertyName));
        }
 
        private string _serializefile;
        public override string SerializeFileName
        {
            get { return _serializefile; }
            set{_serializefile=value;
                RaisePropertyChanged("SerializeFileName");
            }
        }
// ReSharper disable UnusedMember.Local
       void BringToFront()
// ReSharper restore UnusedMember.Local
        {
        	
        }

        public void SendMessage(string title,string message,MsgIcon icon, bool forceshow = true)
        {
            MessageViewModel.Instance.Add(title,message,icon,forceshow);
        }


    }
}
