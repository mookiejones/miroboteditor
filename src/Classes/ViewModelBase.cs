using System.ComponentModel;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public class ViewModelBase:SerializeBase,INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

       public void RaisePropertyChanged(string propertyName)
       {
           if (PropertyChanged!=null)
               PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
       }

       

        private string _serializefile;
        public override string SerializeFileName
        {
            get { return _serializefile; }
            set{_serializefile=value;
                RaisePropertyChanged("SerializeFileName");
            }
        }
      
    }
}
