
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

        
        #region Text
        /// <summary>
        /// The <see cref="Text" /> property's name.
        /// </summary>
        public const string TextPropertyName = "Text";

        private string _text = string.Empty;
<<<<<<< .mine

        /// <summary>
        /// Sets and gets the Text property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (_text == value)
                {
                    return;
                }

                RaisePropertyChanging(TextPropertyName);
                var oldValue = _text;
                _text = value;
                RaisePropertyChanged(TextPropertyName, oldValue, value, true);
            }
        }
        #endregion

=======
        public string Text { get { return _text; } set { _text = value; RaisePropertyChanged("Text"); } }
>>>>>>> .r144
    }
}
