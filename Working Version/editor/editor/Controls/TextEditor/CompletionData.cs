using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using miRobotEditor.Controls.TextEditor.Snippets.CompletionData;
using System;
using System.Windows.Media;

namespace miRobotEditor.Controls.TextEditor
{
    public abstract class CompletionData : ICompletionData
    {
        protected double priority;
        public virtual object Content => Text;
        public virtual object Description => "Description";
        public virtual ImageSource Image => null;
        public virtual double Priority => priority;
        public abstract string Text
        {
            get;
        }
        protected string UsageName
        {
            get;
            set;
        }
        public virtual void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var textEditor = textArea.GetService(typeof(AvalonEditor)) as AvalonEditor;
            if (textEditor == null)
            {
                return;
            }
            var kukaTextEditor = textEditor as AvalonEditor;
            var text = (kukaTextEditor == null) ? textEditor.GetWordBeforeCaret() : kukaTextEditor.GetWordBeforeCaret(kukaTextEditor.GetWordParts());
            if (Text.StartsWith(text, StringComparison.InvariantCultureIgnoreCase) || Text.ToLowerInvariant().Contains(text.ToLowerInvariant()))
            {
                textEditor.Document.Replace(textEditor.CaretOffset - text.Length, text.Length, Text);
            }
            else
            {
                textEditor.Document.Insert(textEditor.CaretOffset, Text);
            }
            if (UsageName != null)
            {
                CodeCompletionDataUsageCache.IncrementUsage(UsageName);
            }
        }
    }

}
