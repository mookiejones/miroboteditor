using miRobotEditor.Enums;

namespace miRobotEditor.ViewModel
{
    public sealed class NotesViewModel : ToolViewModel
    {
        #region Text

         

        private string _text = string.Empty;

        /// <summary>
        ///     Sets and gets the Text property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Text
        {
            get { return _text; }

            set
            {
                if (_text == value)
                {
                    return;
                }

                 
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        #endregion

        public NotesViewModel()
            : base("Notes")
        {
            ContentId = "NotesTool";
            DefaultPane = DefaultToolPane.Bottom;
        }
    }
}