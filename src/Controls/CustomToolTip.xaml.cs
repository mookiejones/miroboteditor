using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
namespace miRobotEditor.Controls
{
    /// <summary>
    /// Interaction logic for CustomToolTip.xaml
    /// </summary>
    public partial class CustomToolTip : UserControl,INotifyPropertyChanged
    {
        public CustomToolTip()
        {
            InitializeComponent();
            DataContext = this;
        }

       
        public event PropertyChangedEventHandler PropertyChanged;
        private string _title;
        private string _message;
        private string _additional;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged("Title"); } }
        public string Message { get { return _message; } set { _message = value; RaisePropertyChanged("Message"); } }
        public string Additional { get { return _additional; } set { _additional = value; RaisePropertyChanged("Additional"); } }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
