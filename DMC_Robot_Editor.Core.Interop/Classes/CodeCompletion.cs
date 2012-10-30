using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace miRobotEditor.Classes
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class CodeCompletion : ICompletionData
    {
        public CodeCompletion(string text)
        {
            Text = text;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return Text; }
        }

        public object Description
        {
            get { return "Description for " + Text; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,  EventArgs insertionRequestEventArgs)
        {
            var currentWord = DummyDoc.ActiveEditor.TextBox.FindWord();
            var offs = completionSegment.Offset - currentWord.Length;
            // Create New AnchorSegment 
            textArea.Document.Replace(offs,currentWord.Length,Text);
        }

        public double Priority
        {
            get { return 0; }
        }
    }
}
