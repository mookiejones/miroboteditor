using miRobotEditor.Controls;
using ICSharpCode.AvalonEdit.Folding;

namespace miRobotEditor.Languages
{
    public class LanguageFold : NewFolding
    {
        public LanguageFold()
        {
        }

        public LanguageFold(int start,int end, string message):base(start,end)
        {
            Message = message;
           
        }

        public LanguageFold(int start, int end, string text, string message)
            : base(start, end)
        {
            Message = message;
            ToolTip = new Classes.ToolTipViewModel { Title = text, Message = text };
            Text = Text;
        }

        public LanguageFold(string message)
        {
            Message = message;
            ToolTip = new Classes.ToolTipViewModel();
        }

        public string Text { get; private set; }
        public string Message { get; private set; }
        public Classes.ToolTipViewModel ToolTip { get; private set; }
       
    }


}
