using System.ComponentModel;

namespace miRobotEditor.Classes
{
    [Localizable(false)]
    public class ViewModelBase:INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

       public void OnPropertyChanged(string propertyName)
       {
           if (PropertyChanged!=null)
               PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
       }
    }
}
