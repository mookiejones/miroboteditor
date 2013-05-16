using System;
using System.ComponentModel;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using miRobotEditor.Interfaces;
using miRobotEditor.GUI;
namespace miRobotEditor.Languages
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class CodeCompletion : ICompletionData
    {
        [Localizable(false)]
        public CodeCompletion(string text)
        {
            Text = text;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }
        private string _text = string.Empty;
        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return Text; }
        }

        [Localizable(false)]
        public object Description
        {
            get { return "Description for " + Text; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var currentWord = DummyDoc.Instance.TextBox.FindWord();
            var offs = completionSegment.Offset - currentWord.Length;
            // Create New AnchorSegment 
            textArea.Document.Replace(offs, currentWord.Length, Text);
        }

        public double Priority
        {
            get { return 0; }
        }
    }
    /// <summary>
    /// Container class for the parameters available to the Complete function.
    /// </summary>
        public class CompletionContext
    {
        /// <summary>
        /// Gets/Sets the editor in which completion is performed.
        /// </summary>
        public ITextEditor Editor { get; set; }

        /// <summary>
        /// Gets/Sets the start offset of the completion range.
        /// </summary>
        public int StartOffset { get; set; }

        /// <summary>
        /// Gets/Sets the end offset of the completion range.
        /// </summary>
        public int EndOffset { get; set; }

        /// <summary>
        /// Gets the length between EndOffset and StartOffset.
        /// </summary>
        public int Length { get { return EndOffset - StartOffset; } }

        /// <summary>
        /// Gets/Sets the character that triggered completion.
        /// This property is '\0' when completion was triggered using the mouse.
        /// </summary>
        public char CompletionChar { get; set; }

        /// <summary>
        /// Gets/Sets whether the CompletionChar was already inserted.
        /// </summary>
        public bool CompletionCharHandled { get; set; }
    }

        public enum CompletionItemListKeyResult
        {
            /// <summary>
            /// Normal key, used to choose an entry from the completion list
            /// </summary>
            NormalKey,
            /// <summary>
            /// This key triggers insertion of the completed expression
            /// </summary>
            InsertionKey,
            /// <summary>
            /// Increment both start and end offset of completion region when inserting this
            /// key. Can be used to insert whitespace (or other characters) in front of the expression
            /// while the completion window is open.
            /// </summary>
            BeforeStartKey,
            /// <summary>
            /// This key triggers cancellation of completion. The completion window will be closed.
            /// </summary>
            Cancel
        }


        public class DefaultCompletionItem : ICompletionItem
    {
        public string Text { get; private set; }
        public virtual string Description { get; set; }
        public virtual IImage Image { get; set; }

        public virtual double Priority { get { return 0; } }

        public DefaultCompletionItem(string text)
        {
            Text = text;
        }

        public virtual void Complete(CompletionContext context)
        {
            context.Editor.Document.Replace(context.StartOffset, context.Length, Text);
            context.EndOffset = context.StartOffset + Text.Length;
        }
    }

        #region DefaultClasses


        public class DefaultCompletionItemList : ICompletionItemList
        {
            readonly List<ICompletionItem> _items = new List<ICompletionItem>();

            public List<ICompletionItem> Items
            {
                get { return _items; }
            }

            /// <inheritdoc />
            public virtual bool ContainsAllAvailableItems
            {
                get { return true; }
            }

            /// <summary>
            /// Sorts the items by their text.
            /// </summary>
            public void SortItems()	// PERF this is called twice
            {
                // the user might use method names is his language, so sort using CurrentCulture
                _items.Sort((a, b) =>
                {
                    var r = string.Compare(a.Text, b.Text, StringComparison.CurrentCultureIgnoreCase);
                    return r != 0 ? r : string.Compare(a.Text, b.Text, StringComparison.CurrentCulture);
                });
            }

            /// <inheritdoc/>
            public int PreselectionLength { get; set; }

            /// <inheritdoc/>
            public ICompletionItem SuggestedItem { get; set; }

            IEnumerable<ICompletionItem> ICompletionItemList.Items
            {
                get { return _items; }
            }

            /// <summary>
            /// Allows the insertion of a single space in front of the completed text.
            /// </summary>
            public bool InsertSpace { get; set; }

            /// <inheritdoc/>
            public virtual CompletionItemListKeyResult ProcessInput(char key)
            {
                if (key == ' ' && InsertSpace)
                {
                    InsertSpace = false; // insert space only once
                    return CompletionItemListKeyResult.BeforeStartKey;
                }
                if (char.IsLetterOrDigit(key) || key == '_')
                {
                    InsertSpace = false; // don't insert space if user types normally
                    return CompletionItemListKeyResult.NormalKey;
                }
                // do not reset insertSpace when doing an insertion!
                return CompletionItemListKeyResult.InsertionKey;
            }

            /// <inheritdoc/>
            public virtual void Complete(CompletionContext context, ICompletionItem item)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                if (item == null)
                    throw new ArgumentNullException("item");
                if (InsertSpace)
                {
                    InsertSpace = false;
                    context.Editor.Document.Insert(context.StartOffset, " ");
                    context.StartOffset++;
                    context.EndOffset++;
                }
                item.Complete(context);
            }
        }


        #endregion

        #region Interfaces
        /// <summary>
        /// Completion item that supports complex content and description.
        /// </summary>
        public interface IFancyCompletionItem : ICompletionItem
        {
            object Content { get; }
            new object Description { get; }
        }
        public interface ISnippetCompletionItem : ICompletionItem
        {
            string Keyword { get; }
        }

        public interface ICompletionItem
        {
            string Text { get; }
            string Description { get; }
            IImage Image { get; }

            /// <summary>
            /// Performs code completion for the item.
            /// </summary>
            void Complete(CompletionContext context);

            /// <summary>
            /// Gets a priority value for the completion data item.
            /// When selecting items by their start characters, the item with the highest
            /// priority is selected first.
            /// </summary>
            double Priority
            {
                get;
            }
        }
        public interface ICompletionItemList
        {
            /// <summary>
            /// Gets the items in the list.
            /// </summary>
            IEnumerable<ICompletionItem> Items { get; }

            /// <summary>
            /// Gets the suggested item.
            /// This item will be pre-selected in the completion list.
            /// </summary>
            ICompletionItem SuggestedItem { get; }

            /// <summary>
            /// Gets the length of the preselection (text in front of the completion list that
            /// should be included as completed expression).
            /// </summary>
            int PreselectionLength { get; }

            /// <summary>
            /// Processes the specified key press.
            /// </summary>
            CompletionItemListKeyResult ProcessInput(char key);

            /// <summary>
            /// True if this list contains all items that were available.
            /// False if this list could contain even more items 
            /// (e.g. by including items from all referenced projects, regardless of imports).
            /// </summary>
            bool ContainsAllAvailableItems { get; }

            /// <summary>
            /// Performs code completion for the selected item.
            /// </summary>
            void Complete(CompletionContext context, ICompletionItem item);
        }
        /// <summary>
        /// Base interface for IInsightWindow and ICompletionListWindow.
        /// </summary>
        public interface ICompletionListWindow
        {
            /// <summary>
            /// Closes the window.
            /// </summary>
            void Close();

            /// <summary>
            /// Occurs after the window was closed.
            /// </summary>
            event EventHandler Closed;

            /// <summary>
            /// Gets/Sets the width of the window.
            /// double.NaN is used to represent automatic width.
            ///
            /// For the completion list window default width is a fixed number - using automatic width
            /// will reduce performance when a large number of items is shown.
            /// </summary>
            double Width { get; set; }

            /// <summary>
            /// Gets/Sets the height of the window.
            /// double.NaN is used to represent automatic height.
            /// </summary>
            double Height { get; set; }

            /// <summary>
            /// Gets/Sets whether the window should close automatically.
            /// The default value is true.
            /// </summary>
            bool CloseAutomatically { get; set; }

            /// <summary>
            /// Gets/Sets the start of the text range in which the window stays open.
            /// Has no effect if CloseAutomatically is false.
            /// </summary>
            int StartOffset { get; set; }

            /// <summary>
            /// Gets/Sets the end of the text range in which the window stays open.
            /// Has no effect if CloseAutomatically is false.
            /// </summary>
            int EndOffset { get; set; }
        }
        /// <summary>
        /// Base interface for IInsightWindow and ICompletionListWindow.
        /// </summary>
        public interface ICompletionWindow
        {
            /// <summary>
            /// Closes the window.
            /// </summary>
            void Close();

            /// <summary>
            /// Occurs after the window was closed.
            /// </summary>
            event EventHandler Closed;

            /// <summary>
            /// Gets/Sets the width of the window.
            /// double.NaN is used to represent automatic width.
            ///
            /// For the completion list window default width is a fixed number - using automatic width
            /// will reduce performance when a large number of items is shown.
            /// </summary>
            double Width { get; set; }

            /// <summary>
            /// Gets/Sets the height of the window.
            /// double.NaN is used to represent automatic height.
            /// </summary>
            double Height { get; set; }

            /// <summary>
            /// Gets/Sets whether the window should close automatically.
            /// The default value is true.
            /// </summary>
            bool CloseAutomatically { get; set; }

            /// <summary>
            /// Gets/Sets the start of the text range in which the window stays open.
            /// Has no effect if CloseAutomatically is false.
            /// </summary>
            int StartOffset { get; set; }

            /// <summary>
            /// Gets/Sets the end of the text range in which the window stays open.
            /// Has no effect if CloseAutomatically is false.
            /// </summary>
            int EndOffset { get; set; }
        }


#endregion
 }

