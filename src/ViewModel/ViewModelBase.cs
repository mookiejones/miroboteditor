using System.ComponentModel;
using miRobotEditor.Interfaces;
using System.Diagnostics;
using System.Windows.Input;
using miRobotEditor.Commands;
namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class ViewModelBase:SerializeBase,INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

 
        [DebuggerStepThrough]
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
                MessageViewModel.AddError(ex);
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
       void BringToFront()
        {
        	
        }

        public void SendMessage(string title,string message,MSGIcon icon, bool forceshow = true)
        {
            MessageViewModel.Instance.Add(title,message,icon,forceshow);
        }


    }

    public abstract class SerializeBase
    {

        public abstract string SerializeFileName { get; set; }

        public void Serialize() { }
        public void Deserialize() { }

    }



}
