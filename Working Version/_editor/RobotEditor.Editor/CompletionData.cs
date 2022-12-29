using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using RobotEditor.Editor.Snippets.CompletionData;

namespace RobotEditor.Editor
{
    public abstract class CompletionData : ICompletionData
    {
        protected double priority;
        public virtual object Content
        {
            get
            {
                return Text;
            }
        }
        public virtual object Description
        {
            get
            {
                return "Description";
            }
        }
        public virtual ImageSource Image
        {
            get
            {
                return null;
            }
        }
        public virtual double Priority
        {
            get
            {
                return priority;
            }
        }
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
            var textEditor = textArea.GetService(typeof(Editor)) as Editor;
            if (textEditor == null)
            {
                return;
            }
            var kukaTextEditor = textEditor as Editor;
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
