using System.Text.RegularExpressions;
using System.Timers;
using System;
using System.Collections.ObjectModel;
using miRobotEditor.Classes;
namespace miRobotEditor.ViewModel
{
	public class FunctionWindowViewModel:AbstractPaneViewModel
    {
    	
		public FunctionWindowViewModel(Regex _match, string text)
		{
			Instance = this;
			MatchString = _match;
			Text = _text;
		}
		
		
		public static FunctionWindowViewModel Instance{get;set;}
		
    	 /// <summary>
        /// View-model for main window.
        /// </summary>
        private MainViewModel ParentViewModel{get;set;}

        public FunctionWindowViewModel()
        {
        	Instance = this;	
        }
    	
    	public FunctionWindowViewModel(MainViewModel parentviewmodel) 
    	{
    		   if (parentviewmodel == null)
            {
                throw new ArgumentNullException("mainWindowViewModel");
            }

            this.ParentViewModel = parentviewmodel;
            this.ParentViewModel.ActiveDocumentChanged += new EventHandler<EventArgs>(ParentViewModel_ActiveDocumentChanged);
   
    	}
    	
    	 /// <summary>
        /// Event raised when the active document in the main window's view model has changed.
        /// </summary>
        private void ParentViewModel_ActiveDocumentChanged(object sender, EventArgs e)
        {         
 	        RaisePropertyChanged("Text");
            RaisePropertyChanged("Items");
            RaisePropertyChanged("MatchString");
        }
    	
    	public bool FormattingEnabled { get; set; }
        
        private Regex _matchstring = new Regex(string.Empty);
        public Regex  MatchString { get { return _matchstring; } set { _matchstring = value;RaisePropertyChanged("MatchString"); } }

         private string _text = string.Empty;
         public string Text { get { return _text; } set { _text = value; GetMatches(); RaisePropertyChanged("Text"); } }
        
      private ObservableCollection<FunctionItem> _items = new ObservableCollection<FunctionItem>();
      public ObservableCollection<FunctionItem>Items{get{return _items;}set{_items=value;RaisePropertyChanged("Items");}}
        
        
       void GetMatches()
     {
     	if (string.IsNullOrEmpty(Text))return;
        	
        	Items = new ObservableCollection<FunctionItem>();        
        	
        for (var match = MatchString.Match(Text); match.Success; match = match.NextMatch())
        {
            string type = match.Groups[1].ToString();
            string returns = match.Groups[3].ToString();
            string name = match.Groups[2].ToString();
            string parameters = match.Groups[5].ToString();           
//            Items.Add(new FunctionItem(text, name, returns, parameters, match.Index));
            
            var  fi = new FunctionItem();
            fi.Name=name;
            fi.Offset=match.Index;
            fi.Type=type;
            fi.Parameters=parameters;
            fi.Returns=returns;
            
            Items.Add(fi);
        }
        }
        	
        
    }
}