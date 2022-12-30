using CommunityToolkit.Mvvm.Input;
using miRobotEditor.Controls.TextEditor;
using System.Windows.Input;

namespace miRobotEditor.ViewModel
{
    public sealed class GotoViewModel : ViewModelBase
    {
        private RelayCommand _okCommand;

        #region Editor



        private AvalonEditor _editor = new AvalonEditor();

        /// <summary>
        ///     Sets and gets the Editor property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public AvalonEditor Editor
        {
            get => _editor;

            set => SetProperty(ref _editor, value);

        }

        #endregion

        #region Description



        private string _description = string.Empty;

        /// <summary>
        ///     Sets and gets the Description property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Description
        {
            get => _description;

            set
            => SetProperty(ref _description, value);
        }

        #endregion

        #region EnteredText


        private int _enteredText = -1;

        /// <summary>
        ///     Sets and gets the EnteredText property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int EnteredText
        {
            get => _enteredText;

            set
            => SetProperty(ref _enteredText, value);
        }

        #endregion

        #region SelectedLine



        private int _selectedLine = -1;

        /// <summary>
        ///     Sets and gets the SelectedLine property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int SelectedLine
        {
            get => _selectedLine;

            set
            => SetProperty(ref _selectedLine, value);
        }

        #endregion

        #region CancelCommand

        private RelayCommand _cancelCommand;

        /// <summary>
        ///     Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand => _cancelCommand
                       ?? (_cancelCommand = new RelayCommand(ExecuteCancelCommand));

        private void ExecuteCancelCommand()
        {
        }

        #endregion

        // ReSharper restore ExplicitCallerInfoArgument

        public GotoViewModel(AvalonEditor editor) => Editor = editor;

        public GotoViewModel()
        {
        }

        public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Accept));

        private void Accept()
        {
            var lineByNumber = Editor.Document.GetLineByNumber(EnteredText);
            Editor.CaretOffset = lineByNumber.Offset;
            Editor.TextArea.Caret.BringCaretToView();
            Editor.ScrollToLine(_selectedLine);
        }
    }
}