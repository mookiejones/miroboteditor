using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace miRobotEditor.Core
{
    /// <summary>
    /// Allows creating new text editor instances and accessing the default text editor options.
    /// </summary>
    public interface IEditorControlService
    {
        ITextEditor CreateEditor(out object control);
        ITextEditorOptions GlobalOptions { get; }
        EditorControlService.ISyntaxHighlighter CreateHighlighter(IDocument document, string fileName);
    }

    /// <summary>
    /// Allows creating new text editor instances and accessing the default text editor options.
    /// </summary>
    public static class EditorControlService
    {
        private static readonly Lazy<IEditorControlService> instance = new Lazy<IEditorControlService>(true);
            

        public static IEditorControlService Instance
        {
            get { return instance.Value; }
        }

        public static ITextEditor CreateEditor(out object control)
        {
            return Instance.CreateEditor(out control);
        }

        public static ITextEditorOptions GlobalOptions
        {
            get { return Instance.GlobalOptions; }
        }

        // Fallback if AvalonEdit.AddIn is not available (e.g. some unit tests)
        sealed class DummyService : IEditorControlService, ITextEditorOptions
        {
            public ITextEditorOptions GlobalOptions
            {
                get { return this; }
            }

            public ITextEditor CreateEditor(out object control)
            {
                var avalonedit = new TextEditor();
                control = avalonedit;
                return new AvalonEditDocumentAdapter(avalonedit);
            }

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add { } remove { } }

            public string IndentationString
            {
                get { return "\t"; }
            }

            public bool AutoInsertBlockEnd
            {
                get { return true; }
            }

            public bool ConvertTabsToSpaces
            {
                get { return false; }
            }

            public int IndentationSize
            {
                get { return 4; }
            }

            public int VerticalRulerColumn
            {
                get { return 120; }
            }

            public bool UnderlineErrors
            {
                get { return true; }
            }

            public string FontFamily
            {
                get
                {
                    return "Consolas";
                }
            }

            public ISyntaxHighlighter CreateHighlighter(IDocument document, string fileName)
            {
                return null;
            }
        }
        /// <summary>
        /// Represents the syntax highlighter inside the text editor.
        /// </summary>
        public interface ISyntaxHighlighter
        {
            /// <summary>
            /// Retrieves the names of the spans that are active at the start of the specified line.
            /// Nested spans are returned in inside-out order (first element of result enumerable is the innermost span).
            /// </summary>
            IEnumerable<string> GetSpanColorNamesFromLineStart(int lineNumber);

            /// <summary>
            /// Retrieves the HighlightingColor with the specified name. Returns null if no color matching the name is found.
            /// </summary>
            HighlightingColor GetNamedColor(string name);

            /// <summary>
            /// Creates a <see cref="HighlightedInlineBuilder"/> for a specified line.
            /// </summary>
            HighlightedInlineBuilder BuildInlines(int lineNumber);

            /// <summary>
            /// Gets the default text color.
            /// </summary>
            HighlightingColor DefaultTextColor { get; }
        }

        
    }
    public static class SyntaxHighligherKnownSpanNames
    {
        public const string Comment = "Comment";
        public const string String = "String";
        public const string Char = "Char";

        public static bool IsLineStartInsideComment(this EditorControlService.ISyntaxHighlighter highligher, int lineNumber)
        {
            return highligher.GetSpanColorNamesFromLineStart(lineNumber).Contains(Comment);
        }

        public static bool IsLineStartInsideString(this EditorControlService.ISyntaxHighlighter highligher, int lineNumber)
        {
            return highligher.GetSpanColorNamesFromLineStart(lineNumber).Contains(String);
        }
    }
}
