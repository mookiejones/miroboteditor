using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class GotoViewModel:ViewModelBase
    {


        #region Properties
        private GUI.EditorClass _editor = new GUI.EditorClass();
        public GUI.EditorClass Editor { get { return _editor; } set { _editor = value; RaisePropertyChanged("Editor"); } }

        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; RaisePropertyChanged("Description"); } }

        private int _enteredText;
        public int EnteredText { get { return _enteredText; } set { _enteredText = value; RaisePropertyChanged("EnteredText"); } }

        private int _selectedLine;
        public int SelectedLine { get { return _selectedLine; } set { _selectedLine = value; RaisePropertyChanged("SelectedLine"); } }
        #endregion

        #region Constructor
        public GotoViewModel(GUI.EditorClass editor)
        {
            Editor = editor;
        }
        public GotoViewModel() { }
        #endregion

        #region Commands

        #region OKCommand

        private RelayCommand _okCommand;
        /// <summary>
        /// Gets the OKCommand.
        /// </summary>
        public RelayCommand OKCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand(ExecuteOKCommand));
            }
        }

        private void ExecuteOKCommand()
        {
            Accept();
        }
        #endregion
     
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
