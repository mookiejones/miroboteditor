using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    /// <summary>
    /// Provides access to language specific features for single files.
    /// </summary>
    public interface ILanguageBinding
    {
        /// <summary>
        /// Provides access to the formatting strategy for this language.
        /// </summary>
        IFormattingStrategy FormattingStrategy
        {
            get;
        }

        /// <summary>
        /// Provides access to the properties for this language.
        /// </summary>
        LanguageProperties Properties
        {
            get;
        }

        /// <summary>
        /// Provides access to the bracket search logic for this language.
        /// </summary>
        IBracketSearcher BracketSearcher
        {
            get;
        }

        /// <summary>
        /// Callback function for backend bindings to add services to ITextEditor.
        /// This is called when the file name of an ITextEditor changes.
        /// </summary>
        void Attach(ITextEditor editor);

        /// <summary>
        /// Callback function for backend bindings to remove all added services from ITextEditor.
        /// This is called when the file name of an ITextEditor changes, to unload all added
        /// features properly.
        /// </summary>
        void Detach();
    }
    /// <summary>
    /// Indentation and formatting strategy.
    /// </summary>
    public interface IFormattingStrategy
    {
        /// <summary>
        /// This function formats a specific line after <code>charTyped</code> is pressed.
        /// </summary>
        void FormatLine(ITextEditor editor, char charTyped);

        /// <summary>
        /// This function sets the indentation level in a specific line
        /// </summary>
        void IndentLine(ITextEditor editor, IDocumentLine line);

        /// <summary>
        /// This function sets the indentation in a range of lines.
        /// </summary>
        void IndentLines(ITextEditor editor, int beginLine, int endLine);

        /// <summary>
        /// This function surrounds the selected text with a comment.
        /// </summary>
        void SurroundSelectionWithComment(ITextEditor editor);
    }
    /// <summary>
    /// A line inside a <see cref="IDocument"/>.
    /// </summary>
    public interface IDocumentLine
    {
        /// <summary>
        /// Gets the starting offset of the line in the document's text.
        /// </summary>
        int Offset { get; }

        /// <summary>
        /// Gets the length of this line (=the number of characters on the line).
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the ending offset of the line in the document's text (= Offset + Length).
        /// </summary>
        int EndOffset { get; }

        /// <summary>
        /// Gets the length of this line, including the line delimiter.
        /// </summary>
        int TotalLength { get; }

        /// <summary>
        /// Gets the length of the line terminator.
        /// Returns 1 or 2; or 0 at the end of the document.
        /// </summary>
        int DelimiterLength { get; }

        /// <summary>
        /// Gets the number of this line.
        /// The first line has the number 1.
        /// </summary>
        int LineNumber { get; }

        /// <summary>
        /// Gets the text on this line.
        /// </summary>
        string Text { get; }
    }
    /// <summary>
    /// Allows language specific search for matching brackets.
    /// </summary>
    public interface IBracketSearcher
    {
        /// <summary>
        /// Searches for a matching bracket from the given offset to the start of the document.
        /// </summary>
        /// <returns>A BracketSearchResult that contains the positions and lengths of the brackets. Return null if there is nothing to highlight.</returns>
        BracketSearchResult SearchBracket(IDocument document, int offset);
    }
    /// <summary>
    /// Describes a pair of matching brackets found by IBracketSearcher.
    /// </summary>
    public class BracketSearchResult
    {
        public int OpeningBracketOffset { get; private set; }

        public int OpeningBracketLength { get; private set; }

        public int ClosingBracketOffset { get; private set; }

        public int ClosingBracketLength { get; private set; }

        public BracketSearchResult(int openingBracketOffset, int openingBracketLength,
                                   int closingBracketOffset, int closingBracketLength)
        {
            this.OpeningBracketOffset = openingBracketOffset;
            this.OpeningBracketLength = openingBracketLength;
            this.ClosingBracketOffset = closingBracketOffset;
            this.ClosingBracketLength = closingBracketLength;
        }
    }
}
