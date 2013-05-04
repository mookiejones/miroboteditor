using System.ComponentModel;
using System.Text;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Classes
{
	public class FunctionItem:ViewModelBase
    {
		
		private string _type = string.Empty;
		private string _name = string.Empty;
		private string _returns = string.Empty;
		private string _parameters = string.Empty;
		
		public bool ShowType{get{return Type.Length>0;}}
		public bool ShowName{get{return Name.Length>0;}}
		public bool ShowReturns{get{return Returns.Length>0;}}
		public bool ShowParameters{get{return Parameters.Length>0;}}
		
		
		
		public string Type { get{ return _type;}  set{_type=value;RaisePropertyChanged("Type");} }
		public string Name { get{ return _name;}  set{_name=value;RaisePropertyChanged("Name");} }
		public string Returns { get{ return _returns;}  set{_returns=value;RaisePropertyChanged("Returns");} }
		public string Parameters { get{ return _parameters;}  set{_parameters=value;RaisePropertyChanged("Parameters");} }
        
      	private int _offset = -1;
      	
      	public int Offset { get{return _offset;} set{_offset=value;RaisePropertyChanged("Offset");} }

     
        public FunctionItem()
        {
        }

      
        public override string ToString()
        {
            return Name;
        }

    }
}
