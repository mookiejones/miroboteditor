using System.ComponentModel;
using System.Windows.Controls;

namespace CREATOR
{
    /// <summary>
    /// Interaction logic for Position.xaml
    /// </summary>
    public partial class Position : UserControl,INotifyPropertyChanged
    {

        
        private string _xValue = "0.0";
        private string _yValue = "0.0";
        private string _zValue = "0.0";
        private string _aValue = "0.0";
        private string _bValue = "0.0";
        private string _cValue = "0.0";

       
        public string X
        {
            get { return _xValue; }
            set
            {
                _xValue = value;
            OnPropertyChanged("X");}
        }
        public string Y
        {
            get { return _yValue; }
            set
            {
                _yValue = value;
                OnPropertyChanged("Y");
            }
        }
        public string Z
        {
            get { return _zValue; }
            set
            {
                _zValue = value;
                OnPropertyChanged("Z");
            }
        }
        public string A
        {
            get { return _aValue; }
            set
            {
                _aValue = value;
                OnPropertyChanged("A");
            }
        }
        public string B
        {
            get { return _bValue; }
            set
            {
                _bValue = value;
                OnPropertyChanged("B");
            }
        }
        public string C
        {
            get { return _cValue; }
            set
            {
                _cValue = value;
                OnPropertyChanged("C");
            }
        }

        public Position()
        {
            InitializeComponent();
            DataContext = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
