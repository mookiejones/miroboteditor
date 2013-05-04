using System.ComponentModel;

namespace MookiesEditor.Core
{
    public class ViewModelBase:INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public  void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
