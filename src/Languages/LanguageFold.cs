using miRobotEditor.Controls;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Languages
{
    public class LanguageFold : NewFolding
    {
    	
    	#region Properties
    	public string StartFold{get;private set;}
    	public string EndFold{get;private set;}
        public string Text { get; private set; }
        public int Start{get;private set;}
        public int End{get;private set;}
        public ToolTipViewModel ToolTip { get; private set; }

    	#endregion
        public LanguageFold()
        {
        }

        
        public LanguageFold(int start,int end, string text, string startfold, string endfold,bool closed):base(start,end)
        {
            Name = System.String.Format("{0}æ{1}", startfold, endfold);
        	StartFold = startfold;
        	EndFold = endfold;
        	DefaultClosed=closed;
        	Start = start;
        	End = end;
        	Text = text;
            string title = text;
        	
        	var p = title.IndexOf("\r\n");
        	var n = title.IndexOf('%');
        	
        	if (n>-1)
        		title = title.Substring(0,n);
        	else if (p> -1)
        		title = title.Substring(0,p);
        	
        	ToolTip = new ToolTipViewModel{Title = title, Message = text};        	        
        }        
    }


}
