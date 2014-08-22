using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace miRobotEditor.Classes
{
    internal sealed class IndentationClass
    {
        private Block _block; // block is the current block

        private bool _blockComment;
        private Stack<Block> _blocks; // blocks contains all blocks outside of the current
        private bool _escape;
        private bool _inChar;
        private bool _inString;

        private char _lastRealChar; // last non-comment char
        private bool _lineComment;
        private bool _verbatim;
        private StringBuilder _wordBuilder;

        public void Reformat(IDocumentAccessor doc, IndentationSettings set)
        {
            Init();

            while (doc.MoveNext())
            {
                Step(doc, set);
            }
        }

        public void Init()
        {
            _wordBuilder = new StringBuilder();
            _blocks = new Stack<Block>();
            _block = new Block
            {
                InnerIndent = "",
                OuterIndent = "",
                Bracket = '{',
                Continuation = false,
                LastWord = "",
                OneLineBlock = 0,
                PreviousOneLineBlock = 0,
                StartLine = 0
            };

            _inString = false;
            _inChar = false;
            _verbatim = false;
            _escape = false;

            _lineComment = false;
            _blockComment = false;

            _lastRealChar = ' '; // last non-comment char
        }

        [Localizable(false)]
        public void Step(IDocumentAccessor doc, IndentationSettings set)
        {
            string line = doc.Text;
            if (set.LeaveEmptyLines && line.Length == 0) return; // leave empty lines empty
            line = line.TrimStart();

            var indent = new StringBuilder();
            if (line.Length == 0)
            {
                // Special treatment for empty lines:
                if (_blockComment || (_inString && _verbatim))
                    return;
                indent.Append(_block.InnerIndent);
                indent.Append(Repeat(set.IndentString, _block.OneLineBlock));
                if (_block.Continuation)
                    indent.Append(set.IndentString);
                if (doc.Text != null && doc.Text != indent.ToString())
                    doc.Text = indent.ToString();
                return;
            }

            if (TrimEnd(doc))
                line = doc.Text.TrimStart();

            Block oldBlock = _block;
            bool startInComment = _blockComment;
            bool startInString = (_inString && _verbatim);

            #region Parse char by char

            _lineComment = false;
            _inChar = false;
            _escape = false;
            if (!_verbatim) _inString = false;

            _lastRealChar = '\n';

            char c = ' ';
            char nextchar = line[0];
            for (int i = 0; i < line.Length; i++)
            {
                if (_lineComment) break; // cancel parsing current line

                char lastchar = c;
                c = nextchar;
                nextchar = i + 1 < line.Length ? line[i + 1] : '\n';

                if (_escape)
                {
                    _escape = false;
                    continue;
                }

                #region Check for comment/string chars

                switch (c)
                {
                    case '/':
                        if (_blockComment && lastchar == '*')
                            _blockComment = false;
                        if (!_inString && !_inChar)
                        {
                            if (!_blockComment && nextchar == '/')
                                _lineComment = true;
                            if (!_lineComment && nextchar == '*')
                                _blockComment = true;
                        }
                        break;
                    case '#':
                        if (!(_inChar || _blockComment || _inString))
                            _lineComment = true;
                        break;
                    case '"':
                        if (!(_inChar || _lineComment || _blockComment))
                        {
                            _inString = !_inString;
                            if (!_inString && _verbatim)
                            {
                                if (nextchar == '"')
                                {
                                    _escape = true; // skip escaped quote
                                    _inString = true;
                                }
                                else
                                {
                                    _verbatim = false;
                                }
                            }
                            else if (_inString && lastchar == '@')
                            {
                                _verbatim = true;
                            }
                        }
                        break;
                    case '\'':
                        if (!(_inString || _lineComment || _blockComment))
                        {
                            _inChar = !_inChar;
                        }
                        break;
                    case '\\':
                        if ((_inString && !_verbatim) || _inChar)
                            _escape = true; // skip next character
                        break;
                }

                #endregion

                if (_lineComment || _blockComment || _inString || _inChar)
                {
                    if (_wordBuilder.Length > 0)
                        _block.LastWord = _wordBuilder.ToString();
                    _wordBuilder.Length = 0;
                    continue;
                }

                if (!Char.IsWhiteSpace(c) && c != '[' && c != '/')
                {
                    if (_block.Bracket == '{')
                        _block.Continuation = true;
                }

                if (Char.IsLetterOrDigit(c))
                {
                    _wordBuilder.Append(c);
                }
                else
                {
                    if (_wordBuilder.Length > 0)
                        _block.LastWord = _wordBuilder.ToString();
                    _wordBuilder.Length = 0;
                }

                #region Push/Pop the blocks

                switch (c)
                {
                    case '{':
                        _block.ResetOneLineBlock();
                        _blocks.Push(_block);
                        _block.StartLine = doc.LineNumber;
                        if (_block.LastWord == "switch")
                        {
                            _block.Indent(set.IndentString + set.IndentString);
                            /* oldBlock refers to the previous line, not the previous block
                             * The block we want is not available anymore because it was never pushed.
                             * } else if (oldBlock.OneLineBlock) {
                            // Inside a one-line-block is another statement
                            // with a full block: indent the inner full block
                            // by one additional level
                            block.Indent(set, set.IndentString + set.IndentString);
                            block.OuterIndent += set.IndentString;
                            // Indent current line if it starts with the '{' character
                            if (i == 0) {
                                oldBlock.InnerIndent += set.IndentString;
                            }*/
                        }
                        else
                        {
                            _block.Indent(set);
                        }
                        _block.Bracket = '{';
                        break;
                    case '}':
                        while (_block.Bracket != '{')
                        {
                            if (_blocks.Count == 0) break;
                            _block = _blocks.Pop();
                        }
                        if (_blocks.Count == 0) break;
                        _block = _blocks.Pop();
                        _block.Continuation = false;
                        _block.ResetOneLineBlock();
                        break;
                    case '(':
                    case '[':
                        _blocks.Push(_block);
                        if (_block.StartLine == doc.LineNumber)
                            _block.InnerIndent = _block.OuterIndent;
                        else
                            _block.StartLine = doc.LineNumber;
                        _block.Indent(Repeat(set.IndentString, oldBlock.OneLineBlock) +
                                      (oldBlock.Continuation ? set.IndentString : "") +
                                      (i == line.Length - 1 ? set.IndentString : new String(' ', i + 1)));
                        _block.Bracket = c;
                        break;
                    case ')':
                        if (_blocks.Count == 0) break;
                        if (_block.Bracket == '(')
                        {
                            _block = _blocks.Pop();
                            if (IsSingleStatementKeyword(_block.LastWord))
                                _block.Continuation = false;
                        }
                        break;
                    case ']':
                        if (_blocks.Count == 0) break;
                        if (_block.Bracket == '[')
                            _block = _blocks.Pop();
                        break;
                    case ';':
                    case ',':
                        _block.Continuation = false;
                        _block.ResetOneLineBlock();
                        break;
                    case ':':
                        if (_block.LastWord == "case"
                            || line.StartsWith("case ", StringComparison.Ordinal)
                            || line.StartsWith(_block.LastWord + ":", StringComparison.Ordinal))
                        {
                            _block.Continuation = false;
                            _block.ResetOneLineBlock();
                        }
                        break;
                }

                if (!Char.IsWhiteSpace(c))
                {
                    // register this char as last char
                    _lastRealChar = c;
                }

                #endregion
            }

            #endregion

            if (_wordBuilder.Length > 0)
                _block.LastWord = _wordBuilder.ToString();
            _wordBuilder.Length = 0;

            if (startInString) return;
            if (startInComment && line[0] != '*') return;
            if (doc.Text.StartsWith("//\t", StringComparison.Ordinal) || doc.Text == "//")
                return;

            if (line[0] == '}')
            {
                indent.Append(oldBlock.OuterIndent);
                oldBlock.ResetOneLineBlock();
                oldBlock.Continuation = false;
            }
            else
            {
                indent.Append(oldBlock.InnerIndent);
            }

            if (indent.Length > 0 && oldBlock.Bracket == '(' && line[0] == ')')
            {
                indent.Remove(indent.Length - 1, 1);
            }
            else if (indent.Length > 0 && oldBlock.Bracket == '[' && line[0] == ']')
            {
                indent.Remove(indent.Length - 1, 1);
            }

            if (line[0] == ':')
            {
                oldBlock.Continuation = true;
            }
            else if (_lastRealChar == ':' && indent.Length >= set.IndentString.Length)
            {
                if (_block.LastWord == "case" || line.StartsWith("case ", StringComparison.Ordinal) ||
                    line.StartsWith(_block.LastWord + ":", StringComparison.Ordinal))
                    indent.Remove(indent.Length - set.IndentString.Length, set.IndentString.Length);
            }
            else if (_lastRealChar == ')')
            {
                if (IsSingleStatementKeyword(_block.LastWord))
                {
                    _block.OneLineBlock++;
                }
            }
            else if (_lastRealChar == 'e' && _block.LastWord == "else")
            {
                _block.OneLineBlock = Math.Max(1, _block.PreviousOneLineBlock);
                _block.Continuation = false;
                oldBlock.OneLineBlock = _block.OneLineBlock - 1;
            }

            if (doc.IsReadOnly)
            {
                // We can't change the current line, but we should accept the existing
                // indentation if possible (=if the current statement is not a multiline
                // statement).
                if (!oldBlock.Continuation && oldBlock.OneLineBlock == 0 &&
                    oldBlock.StartLine == _block.StartLine &&
                    _block.StartLine < doc.LineNumber && _lastRealChar != ':')
                {
                    // use indent StringBuilder to get the indentation of the current line
                    indent.Length = 0;
                    line = doc.Text; // get untrimmed line
                    foreach (char t in line.TakeWhile(Char.IsWhiteSpace))
                    {
                        indent.Append(t);
                    }
                    // /* */ multiline comments have an extra space - do not count it
                    // for the block's indentation.
                    if (startInComment && indent.Length > 0 && indent[indent.Length - 1] == ' ')
                    {
                        indent.Length -= 1;
                    }
                    _block.InnerIndent = indent.ToString();
                }
                return;
            }

            if (line[0] != '{')
            {
                if (line[0] != ')' && oldBlock.Continuation && oldBlock.Bracket == '{')
                    indent.Append(set.IndentString);
                indent.Append(Repeat(set.IndentString, oldBlock.OneLineBlock));
            }

            // this is only for blockcomment lines starting with *,
            // all others keep their old indentation
            if (startInComment)
                indent.Append(' ');

            if (indent.Length != (doc.Text.Length - line.Length) ||
                !doc.Text.StartsWith(indent.ToString(), StringComparison.Ordinal) ||
                Char.IsWhiteSpace(doc.Text[indent.Length]))
            {
                doc.Text = indent + line;
            }
        }

        private static string Repeat(string text, int count)
        {
            if (count == 0)
                return string.Empty;
            if (count == 1)
                return text;
            var b = new StringBuilder(text.Length*count);
            for (int i = 0; i < count; i++)
                b.Append(text);
            return b.ToString();
        }

        private static bool IsSingleStatementKeyword(string keyword)
        {
            switch (keyword)
            {
                case "if":
                case "for":
                case "while":
                case "do":
                case "foreach":
                case "using":
                case "lock":
                    return true;
                default:
                    return false;
            }
        }

        [Localizable(false)]
        private static bool TrimEnd(IDocumentAccessor doc)
        {
            string line = doc.Text;
            if (!Char.IsWhiteSpace(line[line.Length - 1])) return false;

            // one space after an empty comment is allowed
            if (line.EndsWith("// ", StringComparison.Ordinal) || line.EndsWith("* ", StringComparison.Ordinal))
                return false;

            doc.Text = line.TrimEnd();
            return true;
        }

        /// <summary>
        ///     An indentation block. Tracks the state of the indentation.
        /// </summary>
        private struct Block
        {
            /// <summary>
            ///     The type of bracket that opened this block (, [ or {
            /// </summary>
            public char Bracket;

            /// <summary>
            ///     Gets whether there's currently a line continuation going on inside this block.
            /// </summary>
            public bool Continuation;

            /// <summary>
            ///     The indentation inside the block.
            /// </summary>
            public string InnerIndent;

            /// <summary>
            ///     The last word that was seen inside this block.
            ///     Because parenthesis open a sub-block and thus don't change their parent's LastWord,
            ///     this property can be used to identify the type of block statement (if, while, switch)
            ///     at the position of the '{'.
            /// </summary>
            [Localizable(false)] public string LastWord;

            /// <summary>
            ///     Gets whether there's currently a 'one-line-block' going on. 'one-line-blocks' occur
            ///     with if statements that don't use '{}'. They are not represented by a Block instance on
            ///     the stack, but are instead handled similar to line continuations.
            ///     This property is an integer because there might be multiple nested one-line-blocks.
            ///     As soon as there is a finished statement, OneLineBlock is reset to 0.
            /// </summary>
            public int OneLineBlock;

            /// <summary>
            ///     The indentation outside of the block.
            /// </summary>
            public string OuterIndent;

            /// <summary>
            ///     The previous value of one-line-block before it was reset.
            ///     Used to restore the indentation of 'else' to the correct level.
            /// </summary>
            public int PreviousOneLineBlock;

            /// <summary>
            ///     Gets the line number where this block started.
            /// </summary>
            public int StartLine;

            public void ResetOneLineBlock()
            {
                PreviousOneLineBlock = OneLineBlock;
                OneLineBlock = 0;
            }

            public void Indent(IndentationSettings set)
            {
                Indent(set.IndentString);
            }

            public void Indent(string indentationString)
            {
                OuterIndent = InnerIndent;
                InnerIndent += indentationString;
                Continuation = false;
                ResetOneLineBlock();
                LastWord = "";
            }

            [Localizable(false)]
            public override string ToString()
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "[Block StartLine={0}, LastWord='{1}', Continuation={2}, OneLineBlock={3}, PreviousOneLineBlock={4}]",
                    StartLine, LastWord, Continuation, OneLineBlock, PreviousOneLineBlock);
            }
        }
    }

    internal sealed class IndentationSettings
    {
        [Localizable(false)] public string IndentString = "\t";

        /// <summary>Leave empty lines empty.</summary>
        public bool LeaveEmptyLines = true;
    }

    [Localizable(false)]
    public class IndentationStrategy : DefaultIndentationStrategy
    {
        private string _indentationString = "\t";

        /// <summary>
        ///     Creates a new CSharpIndentationStrategy.
        /// </summary>
        public IndentationStrategy()
        {
        }

        /// <summary>
        ///     Creates a new CSharpIndentationStrategy and initializes the settings using the text editor options.
        /// </summary>
        public IndentationStrategy(TextEditorOptions options)
        {
            IndentationString = options.IndentationString;
        }

        /// <summary>
        ///     Gets/Sets the indentation string.
        /// </summary>
        public string IndentationString
        {
            get { return _indentationString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Indentation string must not be null or empty");
                _indentationString = value;
            }
        }

        /// <summary>
        ///     Performs indentation using the specified document accessor.
        /// </summary>
        /// <param name="document">Object used for accessing the document line-by-line</param>
        /// <param name="keepEmptyLines">Specifies whether empty lines should be kept</param>
        public void Indent(IDocumentAccessor document, bool keepEmptyLines)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            var settings = new IndentationSettings {IndentString = IndentationString, LeaveEmptyLines = keepEmptyLines};
            var r = new IndentationClass();
            r.Reformat(document, settings);
        }

        public void UnIndentLine(TextDocument document, DocumentLine line)
        {
        }

        /// <inheritdoc cref="IIndentationStrategy.IndentLine" />
        public override void IndentLine(TextDocument document, DocumentLine line)
        {
            int lineNr = line.LineNumber;
            var acc = new TextDocumentAccessor(document, lineNr, lineNr);
            Indent(acc, false);

            string t = acc.Text;
            if (t.Length == 0)
            {
                // use AutoIndentation for new lines in comments / verbatim strings.
                base.IndentLine(document, line);
            }
        }

        /// <inheritdoc cref="IIndentationStrategy.IndentLines" />
        public override void IndentLines(TextDocument document, int beginLine, int endLine)
        {
            Indent(new TextDocumentAccessor(document, beginLine, endLine), true);
        }
    }
}