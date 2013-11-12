using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class NotesViewModel:ToolViewModel
    {

        public const string ToolContentId = "NotesTool";

        public NotesViewModel()
            : base("Notes")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Bottom;
        }

        private string _text = string.Empty;
        public string Text { get { return _text; } set { _text = value; RaisePropertyChanged("Text"); } }
    }
}
