using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit.Document;
using miRobotEditor.Core.EventArgs;

namespace miRobotEditor.Core
{
    public interface ITextEditorProvider : IFileDocumentProvider
    {
        ITextEditor TextEditor
        {
            get;
        }
    }
    /// <summary>
    /// A read-only view on a (potentially mutable) text buffer.
    /// The IDocument interfaces derives from this interface.
    /// </summary>
    public interface ITextBuffer
    {
        /// <summary>
        /// Gets a version identifier for this text buffer.
        /// Returns null for unversioned text buffers.
        /// </summary>
        ITextBufferVersion Version { get; }

        /// <summary>
        /// Creates an immutable snapshot of this text buffer.
        /// Unlike all other methods in this interface, this method is thread-safe.
        /// </summary>
        ITextBuffer CreateSnapshot();

        /// <summary>
        /// Creates an immutable snapshot of a part of this text buffer.
        /// Unlike all other methods in this interface, this method is thread-safe.
        /// </summary>
        ITextBuffer CreateSnapshot(int offset, int length);

        /// <summary>
        /// Creates a new TextReader to read from this text buffer.
        /// </summary>
        TextReader CreateReader();

        /// <summary>
        /// Creates a new TextReader to read from this text buffer.
        /// </summary>
        TextReader CreateReader(int offset, int length);

        /// <summary>
        /// Gets the total text length.
        /// </summary>
        /// <returns>The length of the text, in characters.</returns>
        /// <remarks>This is the same as Text.Length, but is more efficient because
        ///  it doesn't require creating a String object.</remarks>
        int TextLength { get; }

        /// <summary>
        /// Gets the whole text as string.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Is raised when the Text property changes.
        /// </summary>
        event EventHandler TextChanged;

        /// <summary>
        /// Gets a character at the specified position in the document.
        /// </summary>
        /// <paramref name="offset">The index of the character to get.</paramref>
        /// <exception cref="ArgumentOutOfRangeException">Offset is outside the valid range (0 to TextLength-1).</exception>
        /// <returns>The character at the specified position.</returns>
        /// <remarks>This is the same as Text[offset], but is more efficient because
        ///  it doesn't require creating a String object.</remarks>
        char GetCharAt(int offset);

        /// <summary>
        /// Retrieves the text for a portion of the document.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
        /// <remarks>This is the same as Text.Substring, but is more efficient because
        ///  it doesn't require creating a String object for the whole document.</remarks>
        string GetText(int offset, int length);
    }

    /// <summary>
    /// Represents a version identifier for a text buffer.
    /// </summary>
    /// <remarks>
    /// This is SharpDevelop's equivalent to AvalonEdit ChangeTrackingCheckpoint.
    /// It is used by the ParserService to efficiently detect whether a document has changed and needs reparsing.
    /// It is a separate class from ITextBuffer to allow the GC to collect the text buffer while the version checkpoint
    /// is still in use.
    /// </remarks>
    public interface ITextBufferVersion
    {
        /// <summary>
        /// Gets whether this checkpoint belongs to the same document as the other checkpoint.
        /// </summary>
        bool BelongsToSameDocumentAs(ITextBufferVersion other);

        /// <summary>
        /// Compares the age of this checkpoint to the other checkpoint.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this version.</exception>
        /// <returns>-1 if this version is older than <paramref name="other"/>.
        /// 0 if <c>this</c> version instance represents the same version as <paramref name="other"/>.
        /// 1 if this version is newer than <paramref name="other"/>.</returns>
        int CompareAge(ITextBufferVersion other);

        /// <summary>
        /// Gets the changes from this checkpoint to the other checkpoint.
        /// If 'other' is older than this checkpoint, reverse changes are calculated.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
        IEnumerable<TextChangeEventArgs> GetChangesTo(ITextBufferVersion other);

        /// <summary>
        /// Calculates where the offset has moved in the other buffer version.
        /// </summary>
        /// <exception cref="ArgumentException">Raised if 'other' belongs to a different document than this checkpoint.</exception>
        int MoveOffsetTo(ITextBufferVersion other, int oldOffset, AnchorMovementType movement);
    }

    public sealed class StringTextBuffer : AvalonEditTextSourceAdapter
    {
        public StringTextBuffer(string text): base(new ICSharpCode.AvalonEdit.Document.StringTextSource(text))
        {
        }
    }
    public class AvalonEditTextSourceAdapter : ITextBuffer
    {
        internal readonly ITextSource textSource;

        public AvalonEditTextSourceAdapter(ITextSource textSource)
        {
            if (textSource == null)
                throw new ArgumentNullException("textSource");
            this.textSource = textSource;
        }

        public virtual ITextBufferVersion Version
        {
            get { return null; }
        }

        /// <summary>
        /// Creates an immutable snapshot of this text buffer.
        /// </summary>
        public virtual ITextBuffer CreateSnapshot()
        {
            return new AvalonEditTextSourceAdapter(textSource.CreateSnapshot());
        }

        /// <summary>
        /// Creates an immutable snapshot of a part of this text buffer.
        /// Unlike all other methods in this interface, this method is thread-safe.
        /// </summary>
        public ITextBuffer CreateSnapshot(int offset, int length)
        {
            return new AvalonEditTextSourceAdapter(textSource.CreateSnapshot(offset, length));
        }

        /// <summary>
        /// Creates a new TextReader to read from this text buffer.
        /// </summary>
        public System.IO.TextReader CreateReader()
        {
            return textSource.CreateReader();
        }

        /// <summary>
        /// Creates a new TextReader to read from this text buffer.
        /// </summary>
        public System.IO.TextReader CreateReader(int offset, int length)
        {
            return textSource.CreateSnapshot(offset, length).CreateReader();
        }

        public int TextLength
        {
            get { return textSource.TextLength; }
        }

        public string Text
        {
            get { return textSource.Text; }
        }

        /// <summary>
        /// Is raised when the Text property changes.
        /// </summary>
        public event EventHandler TextChanged
        {
            add { textSource.TextChanged += value; }
            remove { textSource.TextChanged -= value; }
        }

        public char GetCharAt(int offset)
        {
            return textSource.GetCharAt(offset);
        }

        public string GetText(int offset, int length)
        {
            return textSource.GetText(offset, length);
        }
    }

    /// <summary>
    /// Interface for text editors.
    /// </summary>
    public interface ITextEditor : IServiceProvider
    {


        /// <summary>
        /// Gets Active TextBoxControl
        /// </summary>
        ITextEditor TextBox { get; set; }
        /// <summary>
        /// Gets the primary view if split-view is active.
        /// If split-view is disabled, the current ITextEditor instance is returned.
        /// This property never returns null.
        /// </summary>
        /// <example>bool isSecondaryView = (editor != editor.PrimaryView);</example>
        ITextEditor PrimaryView { get; }

        /// <summary>
        /// Gets the document that is being edited.
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// Gets an object that represents the caret inside this text editor.
        /// This property never returns null.
        /// </summary>
        ITextEditorCaret Caret { get; }

        /// <summary>
        /// Gets the set of options used in the text editor.
        /// This property never returns null.
        /// </summary>
        ITextEditorOptions Options { get; }

        /// <summary>
        /// Gets the language binding attached to this text editor.
        /// This property never returns null.
        /// </summary>
        ILanguageBinding Language { get; }

        /// <summary>
        /// Gets the start offset of the selection.
        /// </summary>
        int SelectionStart { get; }

        /// <summary>
        /// Gets the length of the selection.
        /// </summary>
        int SelectionLength { get; }

        /// <summary>
        /// Gets/Sets the selected text.
        /// </summary>
        string SelectedText { get; set; }

        /// <summary>
        /// Sets the selection.
        /// </summary>
        /// <param name="selectionStart">Start offset of the selection</param>
        /// <param name="selectionLength">Length of the selection</param>
        void Select(int selectionStart, int selectionLength);

        /// <summary>
        /// Is raised when the selection changes.
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Is raised before a key is pressed.
        /// </summary>
        event KeyEventHandler KeyPress;

        /// <summary>
        /// Sets the caret to the specified line/column and brings the caret into view.
        /// </summary>
        void JumpTo(int line, int column);

        FileName FileName { get; }

        ICompletionListWindow ShowCompletionWindow(ICompletionItemList data);

        /// <summary>
        /// Gets the completion window that is currently open.
        /// </summary>
        ICompletionListWindow ActiveCompletionWindow { get; }

        /// <summary>
        /// Open a new insight window showing the specified insight items.
        /// </summary>
        /// <param name="items">The insight items to show in the window.
        /// If this property is null or an empty list, the insight window will not be shown.</param>
        /// <returns>The insight window; or null if no insight window was opened.</returns>
        IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items);

        /// <summary>
        /// Gets the insight window that is currently open.
        /// </summary>
        IInsightWindow ActiveInsightWindow { get; }

        /// <summary>
        /// Gets the list of available code snippets.
        /// </summary>
        IEnumerable<ICompletionItem> GetSnippets();
    }
    /// <summary>
    /// An item in the insight window.
    /// </summary>
    public interface IInsightItem
    {
        object Header { get; }
        object Content { get; }
    }
    /// <summary>
    /// Describes a set of insight items (e.g. multiple overloads of a method) to be displayed in the insight window.
    /// </summary>
    public interface IInsightWindow : ICompletionWindow
    {
        /// <summary>
        /// Gets the items to display.
        /// </summary>
        IList<IInsightItem> Items { get; }

        /// <summary>
        /// Gets/Sets the item that is currently selected.
        /// </summary>
        IInsightItem SelectedItem { get; set; }

        /// <summary>
        /// Occurs when the document is changed while the insight window is open.
        /// </summary>
        /// <remarks>
        /// Unlike directly attaching to <see cref="IDocument.TextChanged"/>, using the event does not require handlers to unsubscribe
        /// when the insight window is closed. This makes it easier to avoid memory leaks.
        /// </remarks>
        event EventHandler<TextChangeEventArgs> DocumentChanged;

        event EventHandler SelectedItemChanged;

        event EventHandler CaretPositionChanged;
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
	
    /// <summary>
    /// Represents the completion window showing a ICompletionItemList.
    /// </summary>
    public interface ICompletionListWindow : ICompletionWindow
    {
        /// <summary>
        /// Gets/Sets the currently selected item.
        /// </summary>
        ICompletionItem SelectedItem { get; set; }
    }
    public interface ITextEditorOptions : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the text used for one indentation level.
        /// </summary>
        string IndentationString { get; }

        /// <summary>
        /// Gets whether a '}' should automatically be inserted when a block is opened.
        /// </summary>
        bool AutoInsertBlockEnd { get; }

        /// <summary>
        /// Gets if tabs should be converted to spaces.
        /// </summary>
        bool ConvertTabsToSpaces { get; }

        /// <summary>
        /// Gets the size of an indentation level.
        /// </summary>
        int IndentationSize { get; }

        /// <summary>
        /// Gets the column of the vertical ruler (line that signifies the maximum line length
        /// defined by the coding style)
        /// This property returns a valid value even if the vertical ruler is set to be invisible.
        /// </summary>
        int VerticalRulerColumn { get; }

        /// <summary>
        /// Gets whether errors should be underlined.
        /// </summary>
        bool UnderlineErrors { get; }

        /// <summary>
        /// Gets the name of the currently used font.
        /// </summary>
        string FontFamily { get; }
    }

    public interface ITextEditorCaret
    {
        /// <summary>
        /// Gets/Sets the caret offset;
        /// </summary>
        int Offset { get; set; }

        /// <summary>
        /// Gets/Sets the caret line number.
        /// Line numbers are counted starting from 1.
        /// </summary>
        int Line { get; set; }

        /// <summary>
        /// Gets/Sets the caret column number.
        /// Column numbers are counted starting from 1.
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// Gets/sets the caret position.
        /// </summary>
        Location Position { get; set; }

        /// <summary>
        /// Is raised whenever the position of the caret has changed.
        ///	</summary>
        event EventHandler PositionChanged;
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
	
}
