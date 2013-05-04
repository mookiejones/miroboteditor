using System.Collections.ObjectModel;
using FanucTools;
using MookiesEditor.Core;

namespace TestApplication
{
    public class FanucREPCDViewModel:ViewModelBase
    {
      

        public FanucREPCDViewModel()
        {
            Instance = this;
        }
        public static FanucREPCDViewModel Instance { get; set; }
        private string _stringvalue = string.Empty;
        public string StringValue { get { return _stringvalue; } set { _stringvalue = value;RaisePropertyChanged("StringValue");
        } }
        private ObservableCollection<REPTCDItem> _reptcditems = new ObservableCollection<REPTCDItem>();
        public ObservableCollection<REPTCDItem> REPTCDItems { get { return _reptcditems; } set { _reptcditems = value;RaisePropertyChanged("REPTCDItems"); } } 

    }
}
