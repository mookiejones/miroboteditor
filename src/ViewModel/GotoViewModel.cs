using System.Windows.Input;
using GalaSoft.MvvmLight;
using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.Commands;
using miRobotEditor.GUI.Editor;

namespace miRobotEditor.ViewModel
{
    public class GotoViewModel : ViewModelBase
    {
        #region Properties

        private string _description = string.Empty;
        private AvalonEditor _editor = new AvalonEditor();
        private int _enteredText;
        private int _selectedLine;

        public AvalonEditor Editor
        {
            get { return _editor; }
            set
            {
                _editor = value;
                RaisePropertyChanged("Editor");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public int EnteredText
        {
            get { return _enteredText; }
            set
            {
                _enteredText = value;
                RaisePropertyChanged("EnteredText");
            }
        }

        public int SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
                RaisePropertyChanged("SelectedLine");
            }
        }

        #endregion

        #region Constructor

        public GotoViewModel(AvalonEditor editor)
        {
            Editor = editor;
        }

        public GotoViewModel()
        {
        }

        #endregion

        #region Commands

        private RelayCommand _okCommand;

        public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(param => Accept(), param => (true)));

        #endregion

        private void Accept()
        {
            DocumentLine d = Editor.Document.GetLineByNumber(EnteredText);
            Editor.CaretOffset = d.Offset;
            Editor.TextArea.Caret.BringCaretToView();
            Editor.ScrollToLine(_selectedLine);
        }
    }
}