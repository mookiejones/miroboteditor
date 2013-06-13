using System.Windows.Input;
using miRobotEditor.Core;
using miRobotEditor.GUI;
using RelayCommand = miRobotEditor.Commands.RelayCommand;

namespace miRobotEditor.ViewModel
{
    public class GotoViewModel:ViewModelBase
    {


        #region Properties
        private Editor _editor = new Editor();
        public Editor Editor { get { return _editor; } set { _editor = value; RaisePropertyChanged(); } }

        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged(); } }

        private int _enteredText;
        public int EnteredText { get { return _enteredText; } set { _enteredText = value; RaisePropertyChanged(); } }

        private int _selectedLine;
        public int SelectedLine { get { return _selectedLine; } set { _selectedLine = value; RaisePropertyChanged(); } }
        #endregion

        #region Constructor
        public GotoViewModel(Editor editor)
        {
            Editor = editor;
        }
        public GotoViewModel() { }
        #endregion

        #region Commands
        private RelayCommand _okCommand;
        public ICommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand(param => Accept(), param => (true)));
            }
        }
        #endregion

        void Accept()
        {
            var d = Editor.Document.GetLineByNumber(EnteredText);
            Editor.CaretOffset = d.Offset;
            Editor.TextArea.Caret.BringCaretToView();
            Editor.ScrollToLine(_selectedLine);
        }
    }
}
