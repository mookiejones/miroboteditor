using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CREATOR
{
    /// <summary>
    /// Interaction logic for KUKAServoReader.xaml
    /// </summary>
    public partial class KUKAServoReader : UserControl,INotifyPropertyChanged
    {
        private string _filename ;

        private ObservableCollection<ServoItems> _list;
        public ObservableCollection<ServoItems> List
        {
            get
            {
                if (_list == null)
                {
                    _list = new ObservableCollection<ServoItems>();
                }
                return _list;
            }
            set
            {
                _list = value;
                OnPropertyChanged("List");
            }
        }


        public string Value;

        public void Load(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return;
            int i = 0;
            using (var reader = new StreamReader(filename))
            {
                while ((Value = reader.ReadLine()) != null)
                {
                    Value = reader.ReadLine();
                    var item = new ServoItems();

                    item.Name = Value;
                    item.Value = Value;
                    item.Remark = Value;
                    List.Add(item);
                    i++;
                }
            }
        }

        public KUKAServoReader()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var ofd= new OpenFileDialog();
            ofd.Filter = "KUKA Servo File (*.txt)|*.txt";
            ofd.ShowDialog();
            if (System.IO.File.Exists(ofd.FileName))
            _filename = ofd.FileName;            
            Load(_filename);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ServoItems
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Remark { get; set; }

    }
}
