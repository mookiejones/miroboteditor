using System;
using System.ComponentModel;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.Practices.ServiceLocation;
using miRobotEditor.Classes;
using miRobotEditor.ViewModel;

namespace miRobotEditor.GUI.Editor
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public sealed class CodeCompletion : ICompletionData
    {
        private string _description = string.Empty;

        public CodeCompletion(IVariable variable)
        {
            Text = variable.Name;
            Image = variable.Icon;
            Description = variable.Description;
        }


        [Localizable(false)]
        public CodeCompletion(string text)
        {
            Text = text;
        }

        public ImageSource Image { get; set; }
        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return Text; }
        }


        [Localizable(false)]
        public object Description
        {
            get
            {
                return String.IsNullOrEmpty(_description)
                    ? null
                    : String.Format("Description for {0} \r\n {1}", Text, _description);
            }
            set { _description = (string) value; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            string currentWord = main.ActiveEditor.TextBox.FindWord();
            int offs = completionSegment.Offset - currentWord.Length;
            // Create New AnchorSegment 
            textArea.Document.Replace(offs, currentWord.Length, Text);
        }

        public double Priority
        {
            get { return 0; }
        }
    }

    #region DefaultClasses

    #endregion

    #region Interfaces

    #endregion
}