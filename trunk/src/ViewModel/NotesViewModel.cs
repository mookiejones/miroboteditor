using System;
using miRobotEditor.Classes;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
    public class NotesViewModel : ToolViewModel
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
        ///     The <see cref="Text" /> property's name.
        /// </summary>
        public const string TextPropertyName = "Text";

        private string _text = String.Empty;

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

                RaisePropertyChanging(TextPropertyName);
                _text = value;
                RaisePropertyChanged(TextPropertyName);
            }
        }

        #endregion
    }
}